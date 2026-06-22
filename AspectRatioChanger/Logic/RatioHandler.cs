namespace AspectRatioChanger.Logic;

public class RatioHandler
{
    private const double MaxAspectRatioWidth = 16.0 / 9.0;

    public List<VideoRoot> AddDockedModes(List<VideoRoot> videoRoots, double increaseRate, bool reset = false)
    {
        foreach (var mode in videoRoots)
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
                mode.dock_aspect_h = (int)Math.Round(mode.aspect_h * 10 * increaseRate, MidpointRounding.AwayFromZero);
                mode.dock_aspect_w = mode.aspect_w * 10;
            }
            else
            {
                // Add check if very large numbers divided by 10, then trim trailing zeros

                var isWidescreen = CheckCurrentAspectRatio(mode.aspect_w, mode.aspect_h);
                if (isWidescreen) continue;

                mode.dock_aspect_w = (int)Math.Round(mode.aspect_w * 10 * increaseRate, MidpointRounding.AwayFromZero);
                mode.dock_aspect_h = mode.aspect_h * 10;

                if (mode.dock_aspect_w % 10 == 0 && mode.dock_aspect_h % 10 == 0)
                {
                    mode.dock_aspect_w /= 10;
                    mode.dock_aspect_h /= 10;
                }

                var isOverStretched = MaxAspectRatioWidth < (double)mode.dock_aspect_w! / (double)mode.dock_aspect_h!;
                if (isOverStretched)
                {
                    mode.dock_aspect_w = 16;
                    mode.dock_aspect_h = 9;
                }
            }
        }

        return videoRoots;
    }

    public int GetScaledPercentage(VideoRoot mode)
    {
        var scalingPercentage = 0;

        if (mode.dock_aspect_w != null && mode.dock_aspect_h != null)
        {
            decimal normalAr;
            decimal dockedAr;

            if (mode.rotation == 90 || mode.rotation == 270)
            {
                // For vertical modes, aspect ratio is height/width
                normalAr = (decimal)mode.aspect_h / mode.aspect_w;
                dockedAr = (decimal)mode.dock_aspect_h.Value / mode.dock_aspect_w.Value;
            }
            else
            {
                // For horizontal modes, aspect ratio is width/height
                normalAr = (decimal)mode.aspect_w / mode.aspect_h;
                dockedAr = (decimal)mode.dock_aspect_w.Value / mode.dock_aspect_h.Value;
            }

            // Calculate the scaling percentage
            var scalingRatio = dockedAr / normalAr;
            scalingPercentage = (int)Math.Round(scalingRatio * 100, MidpointRounding.AwayFromZero);
        }

        return scalingPercentage;
    }

    private bool CheckCurrentAspectRatio(int aspectW, int aspectH)
    {
        double width = aspectW;
        double height = aspectH;

        var currentAspect = width / height;

        if (currentAspect >= MaxAspectRatioWidth)
            return true;

        return false;
    }
}