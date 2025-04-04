namespace ReconRico.Extensions;

public static class IsSubset
{
    public static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> target)
    {
        return source.All(target.Contains);
    }
}