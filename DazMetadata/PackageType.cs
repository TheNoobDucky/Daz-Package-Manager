using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Data;

namespace DazPackage
{
    [Flags]
    public enum Generation
    {
        None = 0,
        Unknown = 1 << 0,
        Gen4 = 1 << 4,
        Genesis_1 = 1 << 5,
        Genesis_2 = 1 << 6,
        Genesis_3 = 1 << 7,
        Genesis_8 = 1 << 8,

        All = ~None
    }

    public static class GenerationExtension
    {
        public static string PrettyString(this Generation generation)
        {
            return generation.ToString().Replace('_', ' ');
        }
    }

    [Flags]
    public enum Gender
    {
        None = 0,
        Female = 1 << 0,
        Male = 1 << 1,
        Both = Female | Male,

        Unknown = 1 << 30,
        All = ~None
    }

    [Flags]
    public enum AssetTypes
    {
        None = 0,
        Character = 1 << 0,
        Morph = 1 << 1,
        Expression = 1 << 2,
        Pose = 1 << 3,
        Clothing = 1 << 4,
        Material = 1 << 5,
        Shape = 1 << 6,

        Unknown = 1 << 30,
        All = ~None
    }


    public class PackageType
    {
        public CharacterContentFlags CharacterContent { get; set; } = new CharacterContentFlags();
        public OtherContentFlags OtherContents { get; set; } = new OtherContentFlags();
        public GenerationFlag Generation { get; set; } = new GenerationFlag();
        public GenderFlag Gender { get; set; } = new GenderFlag();
        public bool ImpactLoadSpeed { get; set; } = false;
        public bool IsPeople { get; set; } = false;
        public bool MissingDirectory { get; set; } = true;
    }

    public class CharacterContentFlags
    {
        public bool Character { get; set; } = false;
        public bool Expressions { get; set; } = false;
        public bool Poses { get; set; } = false;
        public bool Shapes { get; set; } = false;
        public bool Hair { get; set; } = false;
        public bool Clothing { get; set; } = false;
        public bool Accessories { get; set; } = false;
        public bool Anatomy { get; set; } = false;
        public bool Props { get; set; } = false;
        public bool Materials { get; set; } = false;
        public bool Morphs { get; set; } = false;
        public bool Unknown { get; set; } = false;
    }

    public class OtherContentFlags
    {
        public bool Props { get; set; } = false;
        public bool Animals { get; set; } = false;
        public bool Animations { get; set; } = false;
        public bool Environments { get; set; } = false;
        public bool Vehicles { get; set; } = false;
        public bool Materials { get; set; } = false;
        public bool Lights { get; set; } = false;
        public bool Scripts { get; set; } = false;
        public bool Posers { get; set; } = false;
        public bool Scenes { get; set; } = false;
        public bool General { get; set; } = false;
        public bool Cameras { get; set; } = false;
    }

    public class GenerationFlag
    {
        public bool Genesis1 { get; set; } = false;
        public bool Genesis2 { get; set; } = false;
        public bool Genesis3 { get; set; } = false;
        public bool Genesis8 { get; set; } = false;
    }

    public class GenderFlag
    {
        public bool Female { get; set; } = false;
        public bool Male { get; set; } = false;
        public bool Both { get; set; } = false;
    }
}
