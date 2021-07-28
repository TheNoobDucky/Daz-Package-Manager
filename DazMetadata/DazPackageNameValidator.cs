using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DazPackage
{
    public class DazPackageNameValidator
    {
        public static bool IsValid(string filename)
        {
            return regex.Match(filename).Success;
        }
        static readonly Regex regex = new(@"^([A-Z][0-9A-Z]{0,6})(?=\d{8})(\d{8})(-(\d{2}))?_([0-9A-Za-z]+)\.zip$");

        public static string StripInvalidCharacter(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c) && !Char.IsPunctuation(c))
                .ToArray());
        }
        public static string GeneratePackageName(string productID, string itemNumber, string productName)
        {
            return $"{productID}-{itemNumber} {StripInvalidCharacter(productName)}.zip";
        }
    }
}
