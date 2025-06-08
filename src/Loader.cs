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

        // Create a PushButton for ClipAllByPlane
        PushButtonData buttonData = new PushButtonData(
            "ClipAllByPlane",              // Button internal name
            "Clip all\nby plane",    // Button text on ribbon
            assemblyPath,           // Path to this DLL
            "Clipper.ClipAllByPlane"    // Full class name (namespace + class)
        );

        PushButton button = panel.AddItem(buttonData) as PushButton;

        button.LargeImage = LoadIcon("plane_32.png");  // 32px icon
        button.Image = LoadIcon("plane_16.png"); // 16px icon

        
        // Create a PushButton for ClipAllByPlane
        buttonData = new PushButtonData(
            "ClipAllByPerpToPlane",              // Button internal name
            "Clip all\nby normal",    // Button text on ribbon
            assemblyPath,           // Path to this DLL
            "Clipper.ClipAllByNormal"    // Full class name (namespace + class)
        );

        button = panel.AddItem(buttonData) as PushButton;

        button.LargeImage = LoadIcon("norm_32.png");  // 32px icon
        button.Image = LoadIcon("norm_16.png"); // 16px icon

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
            // TODO: Exception if not found.
        }

        return null; // Return null if resource not found
    }
}