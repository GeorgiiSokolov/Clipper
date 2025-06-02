using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System;

namespace Clipper;

[Transaction(TransactionMode.Manual)]
public class ClipAllByPlane : IExternalCommand
{

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
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
            bbox = core.MakeBBoxForAllElemInAV();
            bbox_new = core.ClipSpaceByPointAndNormal(bbox, p, n);
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