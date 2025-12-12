using Manifold.Core.Runtime;
using OpenTK.Windowing.Desktop;

namespace Manifold.Internal;

public static class DPI
{
    public static float X;
    public static float Y;
    static DPI()
    {
        X = Application.Instance.OpenTKWindow.CurrentMonitor.HorizontalScale;
        Y = Application.Instance.OpenTKWindow.CurrentMonitor.VerticalScale;
    }
}