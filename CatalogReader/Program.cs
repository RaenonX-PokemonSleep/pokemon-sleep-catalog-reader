using CatalogReader;

var (parsed, exitCode) = ArgsReader.Read(args);

if (parsed == null) {
    return exitCode ?? 1;
}

try {
    var catalog = CatalogLoader.Load(parsed.InputPath);
    JsonExporter.Export(catalog, parsed.OutputPath, parsed.Pretty);
    return 0;
} catch (InvalidOperationException ex) {
    // Keep original message for invalid catalog file
    Console.Error.WriteLine(ex.Message);
    return 2;
} catch (Exception ex) {
    Console.Error.WriteLine($"Unhandled error: {ex.Message}");
    return 2;
}