using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.IO;

namespace Clipper;

[Transaction(TransactionMode.Manual)]
public class Loader : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        // Create a Ribbon Panel
        RibbonPanel panel = application.CreateRibbonPanel("Clipper");

        // Path to this DLL
        string assemblyPath = Assembly.GetExecutingAssembly().Location;

        // Create a PushButton
        PushButtonData buttonData = new PushButtonData(
            "ClipAllByPlane",              // Button internal name
            "Clip all by plane",    // Button text on ribbon
            assemblyPath,           // Path to this DLL
            "Clipper.ClipAllByPlane"    // Full class name (namespace + class)
        );

        PushButton button = panel.AddItem(buttonData) as PushButton;

        button.LargeImage = LoadIcon("icon_32.png");  // 32px icon
        button.Image = LoadIcon("icon_16.png"); // 16px icon

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        return Result.Succeeded;
    }

    private BitmapImage LoadIcon(string iconName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Clipper.Resources.{iconName}";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream != null)
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                return bitmapImage;
            }
            // TODO: Exeption if not found.
        }

        return null; // Return null if resource not found
    }
}