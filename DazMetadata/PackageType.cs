using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

        All = ~None,
    }
    public class GenerationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Generation generation)
            {
                var list = new List<string>();

                if (generation.HasFlag(Generation.Genesis_8))
                {
                    list.Add("Genesis 8");
                }

                if (generation.HasFlag(Generation.Genesis_3))
                {
                    list.Add("Genesis 3");
                }

                if (generation.HasFlag(Generation.Genesis_2))
                {
                    list.Add("Genesis 2");
                }

                if (generation.HasFlag(Generation.Genesis_1))
                {
                    list.Add("Genesis");
                }

                if (generation.HasFlag(Generation.Gen4))
                {
                    list.Add("Gen 4");
                }

                if (list.Count == 0)
                {
                    list.Add("Unknown");
                }
                return list;
            }
            return Binding.DoNothing;

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
            // (kudos Scott Chamberlain), if you do not support a conversion 
            // back you should return a Binding.DoNothing or a 
            // DependencyProperty.UnsetValue
            return Binding.DoNothing;
            // Original code:
            // throw new NotImplementedException();
        }

        public static int GroupNumber(string str)
        {
            return str switch
            {
                "Genesis 8" => 0,
                "Genesis 3" => 1,
                "Genesis 2" => 2,
                "Genesis" => 3,
                "Gen 4" => 4,
                "Unknwon" => 50,
                _ => 100
            };
        }
    }

    public class GenerationGroupCompare : IComparer
    {
        public int Compare(object x, object y)
        {

            if (x is CollectionViewGroup xg && y is CollectionViewGroup yg)
            {
                if (xg.Name is string xs && yg.Name is string ys)
                {
                    // higher group number have lower priority
                    return GenerationToStringConverter.GroupNumber(xs) - GenerationToStringConverter.GroupNumber(ys);
                }
            }
            throw new NotImplementedException();
        }
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
        Unknown = 1 << 0,
        Female = 1 << 1,
        Male = 1 << 2,
        Both = Female | Male,

        All = ~None,
    }
    public class GenderToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Gender gender)
            {
                var list = new List<string>();

                if (gender.HasFlag(Gender.Female))
                {
                    list.Add("Female");
                }

                if (gender.HasFlag(Gender.Male))
                {
                    list.Add("Male");
                }

                if (list.Count == 0)
                {
                    list.Add("Unknown");
                }
                return list;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
            // (kudos Scott Chamberlain), if you do not support a conversion 
            // back you should return a Binding.DoNothing or a 
            // DependencyProperty.UnsetValue
            return Binding.DoNothing;
            // Original code:
            // throw new NotImplementedException();
        }
    }

    [Flags]
    public enum AssetTypes
    {
        None = 0,
        Unknown = 1 << 0,
        Accessory = 1 << 1,
        Animation = 1 << 2,
        Attachment = 1 << 3,
        Camera = 1 << 4,
        Character = 1 << 5,
        Clothing = 1 << 6,
        Expression = 1 << 7,
        //Eyebrow = 1 << 8,
        Hair = 1 << 9,
        Light = 1 << 10,
        Material = 1 << 11,
        Morph = 1 << 12,
        Pose = 1 << 13,
        Prop = 1 << 14,
        Scene = 1 << 15,
        Script = 1 << 16,
        Shape = 1 << 17,
        Support = 1 << 18,
        //Tear = 1 << 19,



        Animal = 1 << 20,
        Environment = 1 << 21,
        Vehicle = 1 << 22,
        Poser = 1 << 23, // TODO CHECK WHAT THIS IS USED FOR.
        General = 1 << 24,
        Anatomy = 1 << 25,

        Missing = 1 << 30,
        Skipped = 1 << 31,
        TODO = 1 << 29,
        HIDDEN_ASSET_GROUP = 1 << 28, // used to hide the combination flags below by making it impossible to get in actual use.

        Shown = Accessory | Attachment | Character | Clothing | Hair | Morph | Prop | Pose | TODO | HIDDEN_ASSET_GROUP,
        Other = Expression | Script | Scene | Light | Shape | Animation | HIDDEN_ASSET_GROUP, // Eyebrow | Tear
        Handled = Shown | Other | HIDDEN_ASSET_GROUP,
        NotProcessed = Material | Skipped | Support | Missing | HIDDEN_ASSET_GROUP,
        Generation = Accessory | Attachment | Character | Clothing | Hair | Morph | Pose | Prop | HIDDEN_ASSET_GROUP,
        Categoriesed = Accessory | Attachment | Character | Clothing | Hair | Morph | Pose | Prop | HIDDEN_ASSET_GROUP | 1 << 27,
        Private = Animal | Environment | Vehicle | Poser | General | Anatomy,

        All = ~None,
    }
    public class AssetToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AssetTypes asset)
            {
                return asset.ToString().Split(", ");
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // According to https://msdn.microsoft.com/en-us/library/system.windows.data.ivalueconverter.convertback(v=vs.110).aspx#Anchor_1
            // (kudos Scott Chamberlain), if you do not support a conversion 
            // back you should return a Binding.DoNothing or a 
            // DependencyProperty.UnsetValue
            return Binding.DoNothing;
            // Original code:
            // throw new NotImplementedException();
        }
    }

    public enum BodyLocation
    {
        None,
        Unknown,
        Arm,
        Head,
        Leg,
        Neck,
        Waist,
        Torso,
    }

    public enum ClothingType
    {
        None,
        Unknown,
        Dress,
        Outfit,
        Footware,
        Glove,
        HeadWear,
        Outerwear,
        Pant,
        Shirt,
        Shoe,
        Skirt,
        Sock,
        Underwear,
    }

    public class PackageType
    {
        public AssetTypes CharacterContent { get; set; } = AssetTypes.None;
        public AssetTypes OtherContents { get; set; } = AssetTypes.None;
        public Generation Generation { get; set; } = Generation.None;
        public Gender Gender { get; set; } = Gender.None;
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


}
