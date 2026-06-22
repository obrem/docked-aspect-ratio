namespace AspectRatioChanger.Pocos;

public class CoreDescription
{
    public string CoreName { get; set; } = string.Empty;

    public string CurrentAspectRatio { get; set; } = string.Empty;

    public string? DockedAspectRatio { get; set; }
    public int DockedPercentageAspectRatio { get; set; }

    public bool Flipped { get; set; }
}