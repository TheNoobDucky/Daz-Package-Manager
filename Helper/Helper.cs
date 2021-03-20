using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Media;
using System.IO.Compression;
using System.Text.Json;

namespace Helpers
{
    public class Helper
    {
        public static void DeleteEmptyFolder(string folder)
        {
            if (!Directory.EnumerateFileSystemEntries(folder).Any())
            {
                try
                {
                    Output.Write("Deleting empty folder: " + folder, Brushes.Blue, 60.0);
                    Directory.Delete(folder);
                }
                catch (UnauthorizedAccessException) { }
                catch (DirectoryNotFoundException) { }
                catch (IOException)
                { //block not empty?
                }
            }
        }

        public static void SafeMove(FileInfo source, string destination)
        {
            if (!File.Exists(destination))
            {
                source.MoveTo(destination, false);
            }
            else
            {
                var sourceHash = CalculateMD5Hash(source);
                var destinationHash = CalculateMD5Hash(new FileInfo(destination));
                if (sourceHash.SequenceEqual(destinationHash))
                {
                    Output.Write("Identical file, deleting: " + source.Name, Brushes.Blue, 20.0);
                    try
                    {
                        source.Delete();
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
                else
                {
                    Output.Write("Non Identical file, skipping: " + source.Name, Brushes.Red, 0.0);
                }
            }
        }

        public static Byte[] CalculateMD5Hash(FileInfo file)
        {
            using var sourceStream = file.OpenRead();
            using var md5 = System.Security.Cryptography.MD5.Create();
            return md5.ComputeHash(sourceStream);
        }

        public static JsonDocument ReadJsonFromGZfile (FileInfo file)
        {
            using var sceneStream = file.OpenRead();
            using var scene = new GZipStream(sceneStream, CompressionMode.Decompress);
            return JsonDocument.Parse(scene);
        }
    }
}
