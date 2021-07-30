using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public bool MultipleMetadataFiles { get; }
        public DazProductZip(FileInfo file)
        {
            this.file = file;
            try
            {
                using var archive = System.IO.Compression.ZipFile.OpenRead(file.ToString());

                // Check Manifest.dsx file
                try
                {
                    var manifestFile = archive.GetEntry("Manifest.dsx");

                    if (manifestFile == null)
                    {
                        HasSupplementFile = false;
                        return;
                    }

                    var packageXML = PackageManifestFile.GetXML(manifestFile);

                    /// Check for metadata file
                    var files = PackageManifestFile.GetFiles(packageXML);
                    metadataFiles = PackageManifestFile.FindMetadataFile(files);

                    var numberOfMetadataFiles = metadataFiles.Count;
                    MissingMetadata = numberOfMetadataFiles == 0;
                    MultipleMetadataFiles = numberOfMetadataFiles != 1;

                    if (!MissingMetadata && !MultipleMetadataFiles)
                    {
                        var metadataFileEntry = "Content/" + metadataFiles.First();

                        var metadataFile = archive.GetEntry(metadataFileEntry);

                        if (metadataFile == null)
                        {
                            MissingMetadata = true;
                            return;
                        }

                        packageMetadata = new PackageMetadata(metadataFile);
                        ProductName = packageMetadata.ProductName;

                        if (packageMetadata == null)
                        {
                            Output.Write("Debug", Output.Level.Error);

                        }
                    }

                    // Check for Supplemen.dsx file.
                    try
                    {
                        var supplementFileEntry = archive.GetEntry("Supplement.dsx");
                        if (supplementFileEntry == null)
                        {
                            HasSupplementFile = false;
                            return;
                        }
                        var supplement = new SupplementFile(supplementFileEntry);
                        ProductName = supplement.ProductName;
                        HasSupplementFile = true;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
                catch (InvalidOperationException e)
                {
                    Output.Write($"{e.Message} {file.FullName}", Output.Level.Error);
                }
            }
            catch (InvalidDataException e)
            {
                Output.Write($"{e.Message} {file.FullName}", Output.Level.Error);
            }
        }


        public string ProductId
        {
            get
            {
                var storeID = "IM";
                var productId = packageMetadata.ProductToken;
                if (productId.StartsWith("CGB")) // Special case for CGB store
                {
                    productId = productId[5..];
                    storeID = "CGB";
                }
                return storeID + UInt32.Parse(productId).ToString("D8");

            }
        }


        private readonly FileInfo file;
        private readonly PackageMetadata packageMetadata;
        private readonly List<string> metadataFiles = null;
    }
}
