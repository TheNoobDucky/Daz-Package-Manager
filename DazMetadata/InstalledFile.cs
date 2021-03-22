using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Collections.Generic;

using Helpers;

namespace DazPackage
{
    public interface IContenType
    {
        public static bool ContentTypeMatches(string sourceContentType) => throw new NotImplementedException();
    }

    public class InstalledFile : IContenType, INotifyPropertyChanged
    {
        public InstalledFile(InstalledPackage package, AssetMetadata asset)
        {
            Package = package;
            ProductName = Package.ProductName;
            ContentType = asset.ContentType;
            AssetType = MatchContentType(asset.ContentType);
            if ((AssetType & AssetTypes.Generation) != AssetTypes.None)
            {
                foreach (var compatibility in asset.Compatibilities)
                {
                    Generations |= GetGeneration(compatibility);
                }
                // Unset Unknown flag if any other generation is flagged. 
                if ((Generations ^ Generation.Unknown) != Generation.None)
                {
                    Generations ^= Generation.Unknown;
                }
                package.Generations |= Generations;
            }
        }

        public InstalledFile() { }

        public string Image { get; set; }
        public string Path { get; set; }
        public string ProductName { get; set; }

        public AssetTypes AssetType { get; set; } = AssetTypes.Unknown;

        public string ContentType { get; set; }

        public Generation Generations { get; set; } = Generation.Unknown;
        public Gender Genders { get; set; } = Gender.Unknown;

        [JsonIgnore] public bool Selected { get { return Package.Selected; } set { Package.Selected = value; OnPropertyChanged(); } }
        private InstalledPackage package = new InstalledPackage();
        public InstalledPackage Package
        {
            get { return package; }
            set
            {
                package = value;
                package.PropertyChanged += Package_PropertyChanged;
            }
        }

        public static bool ContentTypeMatches(string sourceContentType) => false;



        /// <summary>
        /// Pass PropertyChanged event from package back to view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Package_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected static Generation GetGeneration(string compatibility)
        {
            return compatibility switch
            {
                string s when s.StartsWith("/Genesis 8/") || s.StartsWith("/Genesis 8.1/") => Generation.Genesis_8,
                string s when s.StartsWith("/Genesis 3/") => Generation.Genesis_3,
                string s when s.StartsWith("/Genesis 2/") => Generation.Genesis_2,
                "/Genesis" => Generation.Genesis_1,
                string s when s.StartsWith("/Generation4") => Generation.Gen4,
                _ => Generation.Unknown,
            };
        }

        public static AssetTypes MatchContentType(string sourceContentType) => sourceContentType switch
        {
            null => AssetTypes.Missing,
            "" => AssetTypes.Missing,
            // From http://docs.daz3d.com/doku.php/public/dson_spec/format_description/metadata/content_types/start
            #region GAINT LOOKUP TABLE
            "Actor" => AssetTypes.Unknown,
            "Actor/Character" => AssetTypes.Character,
            "Camera" => AssetTypes.Unknown,
            "Follower" => AssetTypes.Unknown,

            "Follower/Accessory" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Left" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Left/Hand" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Left/Lower" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Left/Upper" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Left/Wrist" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Right" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Right/Hand" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Right/Lower" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Right/Upper" => AssetTypes.Accessory,
            "Follower/Accessory/Arm/Right/Wrist" => AssetTypes.Accessory,
            "Follower/Accessory/Arms" => AssetTypes.Accessory,
            "Follower/Accessory/Arms/Hands" => AssetTypes.Accessory,
            "Follower/Accessory/Arms/Lower" => AssetTypes.Accessory,
            "Follower/Accessory/Arms/Upper" => AssetTypes.Accessory,
            "Follower/Accessory/Arms/Wrist" => AssetTypes.Accessory,
            "Follower/Accessory/Head" => AssetTypes.Accessory,
            "Follower/Accessory/Head/Ear/Left" => AssetTypes.Accessory,
            "Follower/Accessory/Head/Ear/Right" => AssetTypes.Accessory,
            "Follower/Accessory/Head/Ears" => AssetTypes.Accessory,
            "Follower/Accessory/Head/Eye/Left" => AssetTypes.Accessory,
            "Follower/Accessory/Head/Eye/Right" => AssetTypes.Accessory,
            "Follower/Accessory/Head/Eyes" => AssetTypes.Accessory,
            "Follower/Accessory/Head/Mouth" => AssetTypes.Accessory,
            "Follower/Accessory/Head/Nose" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Left" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Left/Ankle" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Left/Foot" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Left/Lower" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Left/Upper" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Right" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Right/Ankle" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Right/Foot" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Right/Lower" => AssetTypes.Accessory,
            "Follower/Accessory/Leg/Right/Upper" => AssetTypes.Accessory,
            "Follower/Accessory/Legs" => AssetTypes.Accessory,
            "Follower/Accessory/Legs/Ankles" => AssetTypes.Accessory,
            "Follower/Accessory/Legs/Feet" => AssetTypes.Accessory,
            "Follower/Accessory/Legs/Lower" => AssetTypes.Accessory,
            "Follower/Accessory/Legs/Upper" => AssetTypes.Accessory,
            "Follower/Accessory/Neck" => AssetTypes.Accessory,
            "Follower/Accessory/Waist" => AssetTypes.Accessory,
            "Follower/Accessory/Torso" => AssetTypes.Accessory,

            "Follower/Attachment" => AssetTypes.Attachment,
            "Follower/Attachment/Head" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Ear/Left" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Ear/Right" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Ears" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Face" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Face/Eye/Left" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Face/Eye/Right" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Face/Eyes" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Face/Mouth" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Face/Nose" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Forehead" => AssetTypes.Attachment,

            "Follower/Attachment/Head/Neck" => AssetTypes.Attachment,
            "Follower/Attachment/Head/Skull" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Hip" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Hip/Back" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Hip/Front" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Left" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Left/Ankle" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Left/Foot" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Left/Lower" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Left/Upper" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Right" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Right/Ankle" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Right/Foot" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Right/Lower" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Leg/Right/Upper" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Legs" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Legs/Ankles" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Legs/Feet" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Legs/Lower" => AssetTypes.Attachment,
            "Follower/Attachment/Lower-Body/Legs/Upper" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Left" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Left/Hand" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Left/Lower" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Left/Upper" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Left/Wrist" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Right" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Right/Hand" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Right/Lower" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Right/Upper" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arm/Right/Wrist" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arms" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arms/Hands" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arms/Lower" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arms/Upper" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Arms/Wrist" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Torso" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Torso/Back" => AssetTypes.Attachment,
            "Follower/Attachment/Upper-Body/Torso/Front" => AssetTypes.Attachment,
            "Follower/Hair" => AssetTypes.Hair,

            "Follower/Wardrobe" => AssetTypes.Clothing,
            "Follower/Wardrobe/Dress" => AssetTypes.Clothing,
            "Follower/Wardrobe/Full-Body" => AssetTypes.Clothing,
            "Follower/Wardrobe/Footwear" => AssetTypes.Clothing,
            "Follower/Wardrobe/Footwear/Left" => AssetTypes.Clothing,
            "Follower/Wardrobe/Footwear/Right" => AssetTypes.Clothing,
            "Follower/Wardrobe/Glove" => AssetTypes.Clothing,
            "Follower/Wardrobe/Glove/Left" => AssetTypes.Clothing,
            "Follower/Wardrobe/Glove/Right" => AssetTypes.Clothing,
            "Follower/Wardrobe/Gloves" => AssetTypes.Clothing,
            "Follower/Wardrobe/Headwear" => AssetTypes.Clothing,
            "Follower/Wardrobe/Outerwear" => AssetTypes.Clothing,
            "Follower/Wardrobe/Outerwear/Bottom" => AssetTypes.Clothing,
            "Follower/Wardrobe/Outerwear/Top" => AssetTypes.Clothing,
            "Follower/Wardrobe/Pant" => AssetTypes.Clothing,
            "Follower/Wardrobe/Shirt" => AssetTypes.Clothing,
            "Follower/Wardrobe/Shoe" => AssetTypes.Clothing,
            "Follower/Wardrobe/Shoe/Left" => AssetTypes.Clothing,
            "Follower/Wardrobe/Shoe/Right" => AssetTypes.Clothing,
            "Follower/Wardrobe/Shoes" => AssetTypes.Clothing,
            "Follower/Wardrobe/Skirt" => AssetTypes.Clothing,
            "Follower/Wardrobe/Sock" => AssetTypes.Clothing,
            "Follower/Wardrobe/Sock/Left" => AssetTypes.Clothing,
            "Follower/Wardrobe/Sock/Right" => AssetTypes.Clothing,
            "Follower/Wardrobe/Socks" => AssetTypes.Clothing,
            "Follower/Wardrobe/Underwear" => AssetTypes.Clothing,
            "Follower/Wardrobe/Underwear/Bottom" => AssetTypes.Clothing,
            "Follower/Wardrobe/Underwear/Top" => AssetTypes.Clothing,

            "Light" => AssetTypes.Unknown,
            "Modifier" => AssetTypes.Unknown,
            "Modifier/Clone" => AssetTypes.Unknown,
            "Modifier/Collision" => AssetTypes.Unknown,
            "Modifier/Corrective" => AssetTypes.Unknown,
            "Modifier/Pose" => AssetTypes.Unknown,
            "Modifier/Pose/Generated" => AssetTypes.Unknown,
            "Modifier/Shape" => AssetTypes.Unknown,
            "Modifier/Shape/Generated" => AssetTypes.Unknown,
            "Modifier/Smoothing" => AssetTypes.Unknown,
            "Preset" => AssetTypes.Unknown,
            "Preset/Character" => AssetTypes.Character,
            "Preset/Animation" => AssetTypes.Unknown,
            "Preset/Animation/aniBlock" => AssetTypes.Unknown,
            "Preset/Animation/BVH" => AssetTypes.Unknown,
            "Preset/Camera" => AssetTypes.Unknown,
            "Preset/Camera/Position" => AssetTypes.Unknown,
            "Preset/Deformer" => AssetTypes.Unknown,
            "Preset/Deformer/Injection" => AssetTypes.Unknown,
            "Preset/Fabric" => AssetTypes.Material,
            "Preset/Garment" => AssetTypes.Clothing,
            "Preset/Layered-Image" => AssetTypes.Unknown,
            "Preset/Light" => AssetTypes.Unknown,
            "Preset/Light/Position" => AssetTypes.Unknown,
            "Preset/Materials" => AssetTypes.Material,
            "Preset/Materials/LXM" => AssetTypes.Material,
            "Preset/Materials/MC6" => AssetTypes.Material,
            "Preset/Materials/MDL" => AssetTypes.Material,
            "Preset/Materials/OSL" => AssetTypes.Material,
            "Preset/Materials/RSL" => AssetTypes.Material,
            "Preset/Materials/Hierarchical" => AssetTypes.Material,
            "Preset/Materials/Hierarchical/LXM" => AssetTypes.Material,
            "Preset/Materials/Hierarchical/MC6" => AssetTypes.Material,
            "Preset/Materials/Hierarchical/MDL" => AssetTypes.Material,
            "Preset/Materials/Hierarchical/OSL" => AssetTypes.Material,
            "Preset/Materials/Hierarchical/RSL" => AssetTypes.Material,
            "Preset/Morph" => AssetTypes.Morph,
            "Preset/Morph/Apply" => AssetTypes.Morph,
            "Preset/Morph/Apply/Body" => AssetTypes.Morph,
            "Preset/Morph/Apply/Head" => AssetTypes.Morph,
            "Preset/Morph/Apply/Head/Expression" => AssetTypes.Morph,
            "Preset/Morph/Injection" => AssetTypes.Morph,
            "Preset/Morph/Injection/Body" => AssetTypes.Morph,
            "Preset/Morph/Injection/Head" => AssetTypes.Morph,
            "Preset/Morph/Injection/Head/Expression" => AssetTypes.Morph,
            "Preset/Morph/Remove" => AssetTypes.Morph,
            "Preset/Morph/Remove/Body" => AssetTypes.Morph,
            "Preset/Morph/Remove/Head" => AssetTypes.Morph,
            "Preset/Morph/Remove/Head/Expression" => AssetTypes.Morph,
            "Preset/Pose" => AssetTypes.Pose,
            "Preset/Pose/Hand" => AssetTypes.Pose,
            "Preset/Pose/Hierarchical" => AssetTypes.Pose,
            "Preset/Properties" => AssetTypes.Unknown,
            "Preset/Property" => AssetTypes.Unknown,
            "Link/Injection" => AssetTypes.Unknown,
            "Preset/Puppeteer" => AssetTypes.Unknown,
            "Preset/Render-Settings" => AssetTypes.Unknown,
            "Preset/Shader" => AssetTypes.Material,
            "Preset/Shader/LXM" => AssetTypes.Material,
            "Preset/Shader/MDL" => AssetTypes.Material,
            "Preset/Shader/MT5" => AssetTypes.Material,
            "Preset/Shader/OSL" => AssetTypes.Material,
            "Preset/Shader/RSL" => AssetTypes.Material,
            "Preset/UV" => AssetTypes.Material,
            "Preset/Visibility" => AssetTypes.Unknown,
            "Preset/Wearables" => AssetTypes.Clothing,
            "Prop" => AssetTypes.Prop,
            "Prop/Arm/Left" => AssetTypes.Prop,
            "Prop/Arm/Left/Hand" => AssetTypes.Prop,
            "Prop/Arm/Left/Lower" => AssetTypes.Prop,
            "Prop/Arm/Left/Upper" => AssetTypes.Prop,
            "Prop/Arm/Left/Wrist" => AssetTypes.Prop,
            "Prop/Arm/Right" => AssetTypes.Prop,
            "Prop/Arm/Right/Hand" => AssetTypes.Prop,
            "Prop/Arm/Right/Lower" => AssetTypes.Prop,
            "Prop/Arm/Right/Upper" => AssetTypes.Prop,
            "Prop/Arm/Right/Wrist" => AssetTypes.Prop,
            "Prop/Arms" => AssetTypes.Prop,
            "Prop/Arms/Hands" => AssetTypes.Prop,
            "Prop/Arms/Lower" => AssetTypes.Prop,
            "Prop/Arms/Upper" => AssetTypes.Prop,
            "Prop/Arms/Wrist" => AssetTypes.Prop,
            "Prop/Hair" => AssetTypes.Prop,
            "Prop/Head" => AssetTypes.Prop,
            "Prop/Head/Ear/Left" => AssetTypes.Prop,
            "Prop/Head/Ear/Right" => AssetTypes.Prop,
            "Prop/Head/Ears" => AssetTypes.Prop,
            "Prop/Head/Eye/Left" => AssetTypes.Prop,
            "Prop/Head/Eye/Right" => AssetTypes.Prop,
            "Prop/Head/Eyes" => AssetTypes.Prop,
            "Prop/Head/Mouth" => AssetTypes.Prop,
            "Prop/Head/Nose" => AssetTypes.Prop,
            "Prop/Leg/Left" => AssetTypes.Prop,
            "Prop/Leg/Left/Ankle" => AssetTypes.Prop,
            "Prop/Leg/Left/Foot" => AssetTypes.Prop,
            "Prop/Leg/Left/Lower" => AssetTypes.Prop,
            "Prop/Leg/Left/Upper" => AssetTypes.Prop,
            "Prop/Leg/Right" => AssetTypes.Prop,
            "Prop/Leg/Right/Ankle" => AssetTypes.Prop,
            "Prop/Leg/Right/Foot" => AssetTypes.Prop,
            "Prop/Leg/Right/Lower" => AssetTypes.Prop,
            "Prop/Leg/Right/Upper" => AssetTypes.Prop,
            "Prop/Legs" => AssetTypes.Prop,
            "Prop/Legs/Ankles" => AssetTypes.Prop,
            "Prop/Legs/Feet" => AssetTypes.Prop,
            "Prop/Legs/Lower" => AssetTypes.Prop,
            "Prop/Legs/Upper" => AssetTypes.Prop,
            "Prop/Neck" => AssetTypes.Prop,
            "Prop/Torso" => AssetTypes.Prop,
            "Prop/Waist" => AssetTypes.Prop,
            "Scene" => AssetTypes.Unknown,
            "Script" => AssetTypes.Unknown,
            "Script/Documentation" => AssetTypes.Unknown,
            "Script/Documentation/Lesson-Strip" => AssetTypes.Unknown,
            "Script/Tool" => AssetTypes.Unknown,
            "Script/Utility" => AssetTypes.Unknown,
            "Set" => AssetTypes.Unknown,
            "Support" => AssetTypes.Unknown,

            "Follower/Attachment/Head/Forehead/Eyebrows" => AssetTypes.Eyebrow,
            "Follower/Attachment/Head/Face/Eyelashes" => AssetTypes.Eyebrow,
            "Follower/Attachment/Head/Face/Tears" => AssetTypes.Tear,

            _ => AssetTypes.None
            #endregion
        };
    }
}
