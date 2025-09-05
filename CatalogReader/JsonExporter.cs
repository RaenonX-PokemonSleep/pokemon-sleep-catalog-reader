using System.Text.Encodings.Web;
using System.Text.Json;
using AddressablesTools.Catalog;

namespace CatalogReader;

internal static class JsonExporter {
    public static void Export(ContentCatalogData ccd, string? outputPath, bool pretty) {
        var jsonOptions = new JsonWriterOptions {
            Indented = pretty,
            SkipValidation = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        Stream outputStream;
        var disposeStream = false;

        if (!string.IsNullOrWhiteSpace(outputPath)) {
            outputStream = File.Create(outputPath);
            disposeStream = true;
        } else {
            outputStream = Console.OpenStandardOutput();
        }

        try {
            using var writer = new Utf8JsonWriter(outputStream, jsonOptions);
            writer.WriteStartArray();

            foreach (var loc in ccd.Resources.Select(kv => kv.Value).SelectMany(list => list)) {
                if (loc == null) {
                    continue;
                }

                writer.WriteStartObject();
                writer.WriteString("path", loc.PrimaryKey ?? string.Empty);
                writer.WriteString("hash", loc.InternalId ?? string.Empty);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.Flush();
        } finally {
            if (disposeStream) {
                outputStream.Dispose();
            }
        }
    }
}