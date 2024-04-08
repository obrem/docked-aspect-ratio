namespace AspectRatioChanger.Pocos;

public class CoreDescription
{
    public string CoreName { get; set; }

    public string CurrentAspectRatio { get; set; }

    public string? DockedAspectRatio { get; set; }
    public int DockedPercentageAspectRatio { get; set; }

    public bool Flipped { get; set; }
}