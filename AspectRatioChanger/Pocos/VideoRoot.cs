using System.Text.Json;
using System.Text.Json.Serialization;

namespace AspectRatioChanger.Pocos;


[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Root))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
public class Root
{
    public Video video { get; set; }
}

public class Video
{
    public string magic { get; set; }
    public List<VideoRoot> scaler_modes { get; set; }
    public List<DisplayMode> display_modes { get; set; }
}

public class VideoRoot
{
    public int width { get; set; }
    public int height { get; set; }
    public int aspect_w { get; set; }
    public int aspect_h { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? dock_aspect_w { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? dock_aspect_h { get; set; }
    public int rotation { get; set; }
    public int mirror { get; set; }
}

public class DisplayMode
{
    public string id { get; set; }
}


