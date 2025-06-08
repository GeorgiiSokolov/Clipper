using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System;

namespace Clipper;

[Transaction(TransactionMode.Manual)]
public class ClipAllByNormal : IExternalCommand
{

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        // throw new NotImplementedException("The command is under construction.");
        

        UIDocument uidoc = commandData.Application.ActiveUIDocument;
        Document doc = uidoc.Document;
        View view = doc.ActiveView;

        Core core = new Core(commandData);

        XYZ p, n;
        BoundingBoxXYZ bbox;
        BoundingBoxXYZ bbox_new;

        try
        {
            
            (p, n) = core.GetPointAndNormalOnSurf();
            //Get view direction
            View3D view3D = doc.ActiveView as View3D;
            XYZ view_z = view3D.ViewDirection;
            //Get clipping surface normal from view and angle
            XYZ cn = core.GetClippingNormalFromNormalAndViewZ(n, view_z);

            bbox = core.MakeBBoxForAllElemInAV();

            bbox_new = core.ClipSpaceByPointAndNormal(bbox, p, cn);
            TrContext.Run(doc, tx =>
            {
                (view as View3D).SetSectionBox(bbox_new);
            }, "Clipping View Section Box.");

        }
        catch (Autodesk.Revit.Exceptions.OperationCanceledException)
        {
            // User cancelled — return cancelled cleanly
            return Result.Cancelled;
        }
        catch (Exception ex)
        {
            // Show error in Revit
            TaskDialog.Show("Error", ex.Message);
            return Result.Failed;
        }

        return Result.Succeeded;
    }
}