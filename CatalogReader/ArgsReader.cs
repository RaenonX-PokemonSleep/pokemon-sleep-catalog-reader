using System.CommandLine;

namespace CatalogReader;

internal sealed class ArgsResult {
    public required string InputPath { get; init; }

    public string? OutputPath { get; init; }

    public bool Pretty { get; init; }
}

internal static class ArgsReader {
    public static (ArgsResult? result, int? exitCode) Read(string[] args) {
        var inputArg = new Argument<string>("input") {
            Description = "Path to catalog file (.json, .bin, or bundle)",
            Arity = ArgumentArity.ZeroOrOne,
        };

        var inputOpt = new Option<string>(
            ["--input", "-i"],
            () => string.Empty,
            "Path to catalog file (.json, .bin, or bundle)"
        );
        var outputOpt = new Option<string>(
            ["--output", "-o"],
            () => string.Empty,
            "Output JSON file path (defaults to stdout)"
        );
        var prettyOpt = new Option<bool>(
            "--pretty",
            description: "Pretty-print JSON output"
        );

        var root = new RootCommand("Reads an Addressables catalog and exports resources to JSON") {
            inputArg,
            inputOpt,
            outputOpt,
            prettyOpt
        };

        // Show help if requested explicitly
        if (Array.IndexOf(args, "--help") >= 0 || Array.IndexOf(args, "-h") >= 0 || Array.IndexOf(args, "/?") >= 0) {
            // Return exit code from invoking help
            var helpCode = root.Invoke("-h");
            return (null, helpCode);
        }

        var parseResult = root.Parse(args);
        var inputFromOpt = parseResult.GetValueForOption(inputOpt);
        var inputFromArg = parseResult.GetValueForArgument(inputArg);
        var output = parseResult.GetValueForOption(outputOpt);
        var pretty = parseResult.GetValueForOption(prettyOpt);

        var path = !string.IsNullOrWhiteSpace(inputFromOpt) ? inputFromOpt : inputFromArg;

        if (!string.IsNullOrWhiteSpace(path)) {
            return (
                new ArgsResult {
                    InputPath = path,
                    OutputPath = string.IsNullOrWhiteSpace(output) ? null : output,
                    Pretty = pretty,
                },
                null
            );
        }

        Console.Error.WriteLine("Error: input is required. Provide --input/-i or positional <input>.");
        root.Invoke("-h");
        return (null, 1);
    }
}