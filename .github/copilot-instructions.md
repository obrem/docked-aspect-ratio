# AspectRatioChanger project instructions

## Project summary

AspectRatioChanger is a .NET console tool for modifying Analogue Pocket core `video.json` files. It searches an Analogue Pocket SD card or selected root folder for core configuration files, lists detected cores and their docked scaling state, and can add or reset docked aspect ratio values for scaler modes.

## Example video.json

```json
{
  "video": {
    "magic": "APF_VER_1",
    "scaler_modes": [
      {
        "width": 512,
        "height": 224,
        "aspect_w": 64,
        "aspect_h": 49,
        "dock_aspect_w": 716,
        "dock_aspect_h": 490,
        "rotation": 0,
        "mirror": 0
      },
      {
        "width": 512,
        "height": 224,
        "aspect_w": 8,
        "aspect_h": 7,
        "dock_aspect_w": 89,
        "dock_aspect_h": 70,
        "rotation": 0,
        "mirror": 0
      },
      {
        "width": 512,
        "height": 240,
        "aspect_w": 128,
        "aspect_h": 105,
        "dock_aspect_w": 1433,
        "dock_aspect_h": 1050,
        "rotation": 0,
        "mirror": 0
      },
      {
        "width": 512,
        "height": 240,
        "aspect_w": 16,
        "aspect_h": 15,
        "dock_aspect_w": 179,
        "dock_aspect_h": 150,
        "rotation": 0,
        "mirror": 0
      }
    ],
    "display_modes": [
      {
        "id": "0x10"
      },
      {
        "id": "0x20"
      },
      {
        "id": "0x30"
      },
      {
        "id": "0x40"
      },
      {
        "id": "0xE0"
      },
      {
        "id": "0xE1"
      }
    ]
  }
}
```

## Main behavior

- Entry point: `AspectRatioChanger/Program.cs`.
- The app uses Spectre.Console for interactive prompts and tables.
- `IoHandler` finds the Analogue Pocket root/Cores folder, recursively reads `video.json` files, deserializes them, lists core details, and writes modified JSON back to disk.
- `RatioHandler` contains the aspect-ratio calculation logic:
  - Adds `dock_aspect_w` and `dock_aspect_h` based on a user-selected stretch percentage.
  - Handles vertical modes differently when rotation is 90 or 270 degrees.
  - Skips modes already at or above 16:9.
  - Caps over-stretched horizontal modes to 16:9.
  - Can reset docked aspect values by writing nulls.
  - Computes scaled docked percentage for display.
- POCOs in `AspectRatioChanger/Pocos` mirror the expected `video.json` schema and use System.Text.Json source generation.

## Project structure

- `AspectRatioChanger/` - console application.
- `AspectRatioChanger/Logic/` - I/O, presentation, and aspect-ratio business logic.
- `AspectRatioChanger/Pocos/` - JSON model classes.
- `AspectRatioChanger.Test/` - xUnit test project.

## Technical context

- Target framework: `net10.0`.
- C# version: project/default .NET 10 context.
- Nullable reference types and implicit usings are enabled.
- Main app publishes as a Windows x64 Native AOT executable with size optimizations.
- Primary packages: Spectre.Console, Spectre.Console.Cli, PublishAotCompressed.
- Tests use xUnit with Microsoft.NET.Test.Sdk and coverlet.

## Coding guidance for this repo

- Keep changes small and consistent with the current simple architecture.
- Prefer updating existing classes over adding new abstractions unless they are clearly needed.
- Preserve the `video.json` schema property names, including current snake_case JSON-facing model properties.
- Be careful with Native AOT compatibility and System.Text.Json source generation when changing serialization models.
- For aspect-ratio changes, add or update xUnit tests in `AspectRatioChanger.Test` where possible.
- Validate with `dotnet build` and relevant tests after code changes.
