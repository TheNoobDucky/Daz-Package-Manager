using SD.Tools.Algorithmia.GeneralDataStructures;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DazPackage
{
    public class AssetCache
    {
        public Dictionary<AssetTypes, GenerationCache> cache { get; set; } = new Dictionary<AssetTypes, GenerationCache>();

        public IEnumerable<InstalledFile> GetAssets(AssetTypes assetType, Generation generation = Generation.All, Gender gender = Gender.All)
        {
            return cache.Where(x => x.Key.CheckFlag(assetType)).SelectMany(x => x.Value.GetAssets(generation, gender));
        }

        public void AddAsset(InstalledFile file, AssetTypes assetType, Generation generation, Gender gender)
        {
            if (!cache.TryGetValue(assetType, out GenerationCache gen))
            {
                gen = new GenerationCache();
                cache.Add(assetType, gen);
            }
            gen.AddAsset(file, generation, gender);
        }

        public void Merge(AssetCache other)
        {
            foreach (var (key, value) in other.cache)
            {
                cache.GetValueOrDefault(key).Merge(value);
            }
        }

        public static AssetCache MergeAllBags(ConcurrentBag<AssetCache> items)
        {
            var result = new AssetCache();
            foreach (var item in items)
            {
                result.Merge(item);
            }
            return result;
        }
    }

    public class GenerationCache
    {
        public Dictionary<Generation, GenderCache> cache { get; set; } = new Dictionary<Generation, GenderCache>();

        public IEnumerable<InstalledFile> GetAssets(Generation generation, Gender gender = Gender.All)
        {
            return cache.Where(x => x.Key.CheckFlag(generation)).SelectMany(x => x.Value.GetAssets(gender));
        }
        public void AddAsset(InstalledFile file, Generation generation, Gender gender)
        {
            if (!cache.TryGetValue(generation, out GenderCache gen))
            {
                gen = new GenderCache();
                cache.Add(generation, gen);
            }
            gen.AddAsset(file, gender);
        }

        public void Merge(GenerationCache other)
        {
            foreach (var (key, value) in other.cache)
            {
                cache.GetValueOrDefault(key).Merge(value);
            }
        }
    }

    public class GenderCache
    {
        public MultiValueDictionary<Gender, InstalledFile> cache { get; set; } = new MultiValueDictionary<Gender, InstalledFile>();

        public IEnumerable<InstalledFile> GetAssets(Gender gender)
        {
            return cache.Where(x => x.Key.CheckFlag(gender)).SelectMany(x => x.Value);
        }

        public void AddAsset(InstalledFile file, Gender gender)
        {
            cache.Add(gender, file);
        }

        public void Merge(GenderCache other)
        {
            cache.Merge(other.cache);
        }
    }
    public static class CacheSelect
    {
        public static bool CheckFlag(this Enum item, Enum list)
        {
            return list.HasFlag(item);
        }
    }
}
