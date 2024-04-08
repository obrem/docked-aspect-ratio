using AspectRatioChanger.Pocos;

namespace AspectRatioChanger;

public class RatioHandler
{
    private const double MaxAspectRatioWidth = 16.0 / 9.0;

    public List<VideoRoot> AddDockedModes(List<VideoRoot> scalerModes, double increaseRate, bool reset = false)
    {
        foreach (var mode in scalerModes)
        {
            if (reset)
            {
                mode.dock_aspect_w = null;
                mode.dock_aspect_h = null;
                continue;
            }

            var isVerticalMode = mode.rotation == 90 || mode.rotation == 270;
            if (isVerticalMode)
            {
                mode.dock_aspect_h = (int)(mode.aspect_h * 10 * increaseRate);
                mode.dock_aspect_w = mode.aspect_w * 10;
            }
            else
            {
                // Add check if very large numbers divided by 10, then trim trailing zeros
                var isWidescreenAlready = CheckCurrentAspectRatio(mode.aspect_w, mode.aspect_h);
                if (isWidescreenAlready) continue;

                mode.dock_aspect_w = (int)(mode.aspect_w * 10 * increaseRate);
                mode.dock_aspect_h = mode.aspect_h * 10;

                var isOverStretched = MaxAspectRatioWidth < (double)mode.dock_aspect_w / (double)mode.dock_aspect_h;
                if (isOverStretched)
                {
                    mode.dock_aspect_w = 16;
                    mode.dock_aspect_h = 9;
                }
            }
        }

        return scalerModes;
    }

    private bool CheckCurrentAspectRatio(int aspect_w, int aspect_h)
    {
        double width = aspect_w;
        double height = aspect_h;

        var currentAspect = width / height;

        if (currentAspect >= MaxAspectRatioWidth)
            return true;

        return false;
    }
}