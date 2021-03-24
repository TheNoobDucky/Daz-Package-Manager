using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DazPackage
{
    /// <summary>
    /// Represent a Daz product zip file.
    /// </summary>
    public class DazProductZip
    {
        public string ProductName { get; }
        public bool HasSupplementFile { get; } = false;
        public bool MissingMetadata { get; } = true;
        public bool RequireReview { get; }
        public DazProductZip(FileInfo file)
        {
            this.file = file;

            using var archive = System.IO.Compression.ZipFile.OpenRead(file.ToString());

            // Check Manifest.dsx file
            var manifestFile = archive.Entries.First(x => x.FullName == "Manifest.dsx");
            new PackageManifestFile(manifestFile);

            /// Check for metadata file
            var metadataFileEntries = archive.Entries.Where(
            x =>
            (x.FullName.StartsWith("Content/Runtime/Support/") || x.FullName.StartsWith("Content/runtime/Support/"))
            && x.FullName.EndsWith(".dsx"));

            var numberOfMetadataFiles = metadataFileEntries.Count();
            MissingMetadata = numberOfMetadataFiles == 0;
            RequireReview = numberOfMetadataFiles != 1;

            if (!MissingMetadata && !RequireReview)
            {
                var metadataFileEntry = metadataFileEntries.First();
                packageMetadata = new PackageMetadata(metadataFileEntry);
                ProductName = packageMetadata.ProductName;
            }

            // Check for Supplemen.dsx file.
            try
            {
                var supplementFileEntry = archive.Entries.First(x => x.FullName == "Supplement.dsx");
                var supplement = new SupplementFile(supplementFileEntry);
                ProductName = supplement.ProductName;
                HasSupplementFile = true;
            }
            catch (InvalidOperationException)
            {
            }
        }

        //public IEnumerable<XElement> Assets { get { return packageMetadata.Assets; } }
        public string GeneratePackageName()
        {
            var storeID = "IM";
            var productId = packageMetadata.ProductToken;
            if (productId.StartsWith("CGB")) // Special case for CGB store
            {
                productId = productId[5..];
                storeID = "CGB";
            }
            return storeID + UInt32.Parse(productId).ToString("D8") + "-01_"
                + StripInvalidCharacter(packageMetadata.ProductName) + ".zip";
        }

        public static string StripInvalidCharacter(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c) && !Char.IsPunctuation(c))
                .ToArray());
        }

        private readonly FileInfo file;
        private readonly PackageMetadata packageMetadata;
    }

    public class DazPackageNameValidator
    {
        public static bool IsValid(string filename)
        {
            return regex.Match(filename).Success;
        }
        static readonly Regex regex = new Regex(@"^([A-Z][0-9A-Z]{0,6})(?=\d{8})(\d{8})(-(\d{2}))?_([0-9A-Za-z]+)\.zip$");
    }
}
