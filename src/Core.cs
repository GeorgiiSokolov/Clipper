using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Linq;

namespace Clipper;

class Core
{
	// Context
	private Document doc;
	private UIDocument uidoc;
	private View view;
	private ExternalCommandData commandData;

	public Core(ExternalCommandData commandData)
	{
		this.commandData = commandData;
		uidoc = commandData.Application.ActiveUIDocument;
		doc = uidoc.Document;
		view = doc.ActiveView;
	}

	// Function to select a face and get the point and normal
	public (XYZ, XYZ) GetPointAndNormalOnSurf()
	{
		// Let the user pick a face
		Reference objRef = uidoc.Selection.PickObject(ObjectType.Face, "Select a face");
		XYZ p = objRef.GlobalPoint;

		// Get the picked element
		Element element = doc.GetElement(objRef.ElementId);

		// Retrieve the geometry object from the reference
		Face face = element.GetGeometryObjectFromReference(objRef) as Face
			?? throw new ApplicationException("Selected geometry is not a face.");

		UV uv = face.Project(p).UVPoint;
		XYZ n = face.ComputeNormal(uv);

		return (p, n);
	}

	// Find box through all elements in view (Revit)
/* 	public BoundingBoxXYZ MakeBBoxForAllElemInAV_old()
	{
		XYZ bbmin = new(float.MaxValue, float.MaxValue, float.MaxValue);
		XYZ bbmax = new(float.MinValue, float.MinValue, float.MinValue);

		BuiltInCategory[] skipCategories =
		[
			BuiltInCategory.OST_SectionBox,
			BuiltInCategory.OST_Views,
			BuiltInCategory.OST_Levels,
			BuiltInCategory.OST_Cameras,
			BuiltInCategory.OST_SharedBasePoint,
			BuiltInCategory.OST_ProjectBasePoint,
			BuiltInCategory.OST_CoordinateSystem
		];

		foreach (Element element in new FilteredElementCollector(doc).WhereElementIsNotElementType())
		{
			try
			{
				BoundingBoxXYZ ebbox = element.get_BoundingBox(view);
				if (ebbox != null) // Check if not None
				{
					if (skipCategories.Contains(element.Category.BuiltInCategory)) { continue; }
					// TODO: Geting the most outstanding elements - if ebbox.Min.Y < bbmin.Y: most = element

					bbmin = new XYZ(
						Math.Min(bbmin.X, ebbox.Min.X),
						Math.Min(bbmin.Y, ebbox.Min.Y),
						Math.Min(bbmin.Z, ebbox.Min.Z)
					);

					bbmax = new XYZ(
						Math.Max(bbmax.X, ebbox.Max.X),
						Math.Max(bbmax.Y, ebbox.Max.Y),
						Math.Max(bbmax.Z, ebbox.Max.Z)
					);

				}
			}
			catch
			{

			

			}
		}

		BoundingBoxXYZ bbox = new() { Min = bbmin, Max = bbmax };
		return bbox;
	} */

	public BoundingBoxXYZ MakeBBoxForAllElemInAV()
	{
		BoundingBoxXYZ bbox = null;
		View activeView = doc.ActiveView;

		if (activeView is View3D view3D)
		{
			TrContext.Run(doc, tx =>
			{
				view3D.IsSectionBoxActive = false;
			}, "View section box off.");

			TrContext.Run(doc, tx =>
			{
				view3D.IsSectionBoxActive = true;			
			}, "View section box on.");

			bbox = view3D.GetSectionBox();
			XYZ old_origin = bbox.Transform.Origin;
			bbox.Transform.Origin = XYZ.Zero;
			bbox.Max += old_origin;
			bbox.Min += old_origin;
		}

		return bbox;
	}

	public BoundingBoxXYZ ClipSpaceByPointAndNormal(BoundingBoxXYZ bbox, XYZ p, XYZ n)
	{
		BoundingBoxXYZ bbox_new = new BoundingBoxXYZ();

		if (n.IsAlmostEqualTo(new XYZ(0, 0, 1)))
		{
			bbox_new.Max = new XYZ(bbox.Max.X, bbox.Max.Y, p.Z);
			bbox_new.Min = bbox.Min;
		}
		else if (n.IsAlmostEqualTo(new XYZ(0, 0, -1)))
		{
			bbox_new.Min = new XYZ(bbox.Min.X, bbox.Min.Y, p.Z);
			bbox_new.Max = bbox.Max;
		}
		else
		{
			// Generate rotated bbox_all, cut by point

			// 4 points of bbox in world space
			XYZ p0 = new XYZ(bbox.Min.X, bbox.Min.Y, 0);
			XYZ p1 = new XYZ(bbox.Min.X, bbox.Max.Y, 0);
			XYZ p2 = new XYZ(bbox.Max.X, bbox.Max.Y, 0);
			XYZ p3 = new XYZ(bbox.Max.X, bbox.Min.Y, 0);

			// Projection of normal onto XY plane
			XYZ n_proj = new XYZ(n.X, n.Y, 0).Normalize();

			// Build transformation based on projected normal
			Transform transform_n = Transform.Identity;
			transform_n.BasisX = new XYZ(-n_proj.Y, n_proj.X, 0);
			transform_n.BasisY = new XYZ(-n_proj.X, -n_proj.Y, 0);
			transform_n.BasisZ = new XYZ(0, 0, 1);

			// Inverse: transform to new local space
			Transform transform_nspace = transform_n.Inverse;

			// Transform input point and bbox corners into local space
			XYZ p_ns = transform_nspace.OfPoint(p);
			XYZ[] bpts_ns =
			[
				transform_nspace.OfPoint(p0),
				transform_nspace.OfPoint(p1),
				transform_nspace.OfPoint(p2),
				transform_nspace.OfPoint(p3)
			];

			double[] xs = bpts_ns.Select(pt => pt.X).ToArray();
			double[] ys = bpts_ns.Select(pt => pt.Y).ToArray();


			bbox_new.Min = new XYZ(xs.Min(), p_ns.Y, bbox.Min.Z);
			bbox_new.Max = new XYZ(xs.Max(), ys.Max(), bbox.Max.Z);
			bbox_new.Transform = transform_n;


		}
		bbox = bbox_new;
		return bbox;
	}

	public XYZ GetClippingNormalFromNormalAndViewZ(XYZ normal, XYZ view_z)
	{
		double tolerance = 1e-9;
		if (Math.Abs(normal.X) < tolerance && Math.Abs(normal.Y) < tolerance) { return normal; }

		// Project both on xy
		XYZ n_xy = new XYZ(normal.X, normal.Y, 0).Normalize();
		XYZ vz_xy = new XYZ(view_z.X, view_z.Y, 0).Normalize();

		// Determine direction using 2D cross product (Z component)
		double crossZ = n_xy.X * vz_xy.Y - n_xy.Y * vz_xy.X;

		if (crossZ >= 0)
		{
			// Rotate +90°: counter-clockwise
			return new XYZ(-n_xy.Y, n_xy.X, 0);
		}
		else
		{
			// Rotate -90°: clockwise
			return new XYZ(n_xy.Y, -n_xy.X, 0);
		}
	}
}