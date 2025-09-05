using AddressablesTools;
using AddressablesTools.Catalog;
using AssetsTools.NET;

namespace CatalogReader;

internal static class CatalogLoader {
    public static ContentCatalogData Load(string path) {
        // Try bundle first (UnityFS header)
        var isBundle = false;

        try {
            using var fileReader = new AssetsFileReader(path);
            const string unityFs = "UnityFS";

            if (fileReader.BaseStream.Length >= unityFs.Length) {
                isBundle = fileReader.ReadStringLength(unityFs.Length) == unityFs;
            }
        } catch {
            // ignore, not a bundle
        }

        if (isBundle) {
            return AddressablesCatalogFileParser.FromBundle(path);
        }

        CatalogFileType fileType;

        using (var fs = File.OpenRead(path)) {
            fileType = AddressablesCatalogFileParser.GetCatalogFileType(fs);
        }

        switch (fileType) {
            case CatalogFileType.Json:
                return AddressablesCatalogFileParser.FromJsonString(File.ReadAllText(path));
            case CatalogFileType.Binary:
                return AddressablesCatalogFileParser.FromBinaryData(File.ReadAllBytes(path));
            case CatalogFileType.None:
            default:
                throw new InvalidOperationException("Not a valid catalog file.");
        }
    }
}