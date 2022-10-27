using ModEngine.Core;

namespace ModEngine.Merge;

public static class MergeExtensions
{
    public static List<TMod> GetMods<TMod>(this IEnumerable<MergeComponent<TMod>> mergeComponents)
        where TMod : Mod {
        return mergeComponents
            .OrderBy(p => p.Priority)
            .SelectMany(p => p.Mods)
            .ToList();
    }

    public static Dictionary<string, string> GetParameters<TMod>(this IEnumerable<MergeComponent<TMod>> mergeComponents)
        where TMod : Mod {
        return mergeComponents
            .OrderByDescending(p => p.Priority)
            .Select(p => p.Parameters)
            .Aggregate(new Dictionary<string, string>(), 
                (total, next) => total.MergeLeft(next)
            );
    }

    private static T MergeLeft<T,TKey,TValue>(this T me, params IDictionary<TKey,TValue>[] others)
        where T : IDictionary<TKey,TValue>, new()
    {
        var newMap = new T();
        foreach (var src in
                 (new List<IDictionary<TKey,TValue>> { me }).Concat(others)) {
            // ^-- echk. Not quite there type-system.
            foreach (var p in src) {
                newMap[p.Key] = p.Value;
            }
        }
        return newMap;
    }
}