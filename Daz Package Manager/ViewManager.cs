using DazPackage;
using Helpers;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Daz_Package_Manager
{
    internal class ViewManager
    {
        private readonly Backend model;

        public ViewManager(Backend model)
        {
            this.model = model;
            BuildViews();
            model.Packages.PropertyChanged += ModelChangedHandler;
            model.Settings.PropertyChanged += GuiSettingChangedHandler;
        }

        #region Product Views
        public void UpdateSelections()
        {
            Packages.Source = model.Packages.Packages.Where(x => x.Generations.CheckFlag(model.Settings.ToggleGeneration) && x.Genders.CheckFlag(model.Settings.ToggleGender));
            Accessories.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.Accessory, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
            Attachments.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.Attachment, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
            Characters.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.Character, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
            Clothings.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.Clothing, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
            Hairs.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.Hair, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
            Morphs.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.Morph, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
            Props.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.Prop, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
            Poses.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.Pose, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
            Others.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.Other, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
            TODO.Source = model.Packages.ItemsCache.GetAssets(AssetTypes.TODO, model.Settings.ToggleGeneration, model.Settings.ToggleGender);
        }

        private void ModelChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Packages")
            {
                model.ManifestScanner.SaveCache();
                UpdateSelections();
            }
        }
        private void GuiSettingChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "ToggleGeneration" or "ToggleGender")
            {
                UpdateSelections();
            }
        }

        private void BuildViews()
        {
            Packages.GroupDescriptions.Add(itemAssetTypeGrouping);
            Packages.GroupDescriptions.Add(generationGrouping);
            Packages.GroupDescriptions.Add(genderGrouping);
            Packages.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Accessories.GroupDescriptions.Add(generationGrouping);
            Accessories.GroupDescriptions.Add(genderGrouping);
            Accessories.GroupDescriptions.Add(itemCategoriesGrouping);
            Accessories.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Attachments.GroupDescriptions.Add(generationGrouping);
            Attachments.GroupDescriptions.Add(genderGrouping);
            Attachments.GroupDescriptions.Add(itemCategoriesGrouping);
            Attachments.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Characters.GroupDescriptions.Add(generationGrouping);
            Characters.GroupDescriptions.Add(genderGrouping);
            Characters.GroupDescriptions.Add(itemCategoriesGrouping);
            Characters.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Clothings.GroupDescriptions.Add(generationGrouping);
            Clothings.GroupDescriptions.Add(genderGrouping);
            Clothings.GroupDescriptions.Add(itemCategoriesGrouping);
            Clothings.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Hairs.GroupDescriptions.Add(generationGrouping);
            Hairs.GroupDescriptions.Add(genderGrouping);
            Hairs.GroupDescriptions.Add(itemCategoriesGrouping);
            Hairs.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Morphs.GroupDescriptions.Add(generationGrouping);
            Morphs.GroupDescriptions.Add(genderGrouping);
            Morphs.GroupDescriptions.Add(itemCategoriesGrouping);
            Morphs.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Others.GroupDescriptions.Add(itemContentGrouping);
            Others.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Props.GroupDescriptions.Add(itemCategoriesGrouping);
            Props.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            Poses.GroupDescriptions.Add(generationGrouping);
            Poses.GroupDescriptions.Add(genderGrouping);
            Poses.GroupDescriptions.Add(itemCategoriesGrouping);
            Poses.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            TODO.GroupDescriptions.Add(itemContentGrouping);
            TODO.SortDescriptions.Add(new SortDescription("ProductName", ListSortDirection.Ascending));

            ThirdPartyView.Source = model.ThirdParty.Files;

        }

        public CollectionViewSource Packages { get; set; } = new();
        public CollectionViewSource Accessories { get; set; } = new();
        public CollectionViewSource Attachments { get; set; } = new();
        public CollectionViewSource Characters { get; set; } = new();
        public CollectionViewSource Clothings { get; set; } = new();
        public CollectionViewSource Hairs { get; set; } = new();
        public CollectionViewSource Morphs { get; set; } = new();
        public CollectionViewSource Props { get; set; } = new();
        public CollectionViewSource Poses { get; set; } = new();
        public CollectionViewSource Others { get; set; } = new();
        public CollectionViewSource TODO { get; set; } = new();

        private static readonly GenerationToStringConverter generationToStringConverter = new();
        private static readonly GenerationGroupCompare generationGroupCompare = new();
        private static readonly PropertyGroupDescription generationGrouping = new("Generations", generationToStringConverter)
        {
            CustomSort = generationGroupCompare
        };

        private static readonly StringCompareHelper stringCompare = new();
        private static readonly AssetToStringConverter installedPackageAssetTypeConverter = new();
        private static readonly PropertyGroupDescription itemAssetTypeGrouping = new("AssetTypes", installedPackageAssetTypeConverter)
        {
            CustomSort = stringCompare
        };

        private static readonly GenderToStringConverter genderToStringConverter = new();
        private static readonly PropertyGroupDescription genderGrouping = new("Genders", genderToStringConverter)
        {
            CustomSort = stringCompare
        };

        private static readonly ContentTypeToDisplayConverter installedItemContentTypeConverter = new();
        private static readonly PropertyGroupDescription itemContentGrouping = new("ContentType", installedItemContentTypeConverter)
        {
            CustomSort = stringCompare
        };

        private static readonly CategoriesConverter installedItemCategoriesConverter = new();
        private static readonly PropertyGroupDescription itemCategoriesGrouping = new("Categories", installedItemCategoriesConverter)
        {
            CustomSort = stringCompare
        };
        public CollectionViewSource ThirdPartyView { get; set; } = new();

        #endregion
    }
}
