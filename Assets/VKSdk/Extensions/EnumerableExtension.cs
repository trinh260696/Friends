using System;
using System.Collections.Generic;

public static class EnumerableExtension
{
    public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> source, int chunkSize)
    {
        if (chunkSize < 1)
            throw new ArgumentException("Chunk size must be greater than zero.");

        IEnumerator<T> enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return getChunk(enumerator, chunkSize);
        }
    }

    private static IEnumerable<T> getChunk<T>(IEnumerator<T> enumerator, int chunkSize)
    {
        int count = 0;
        do
        {
            yield return enumerator.Current;
        } while (++count < chunkSize && enumerator.MoveNext());           
    }
}