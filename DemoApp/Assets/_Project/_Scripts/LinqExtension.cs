using System;
using System.Collections.Generic;

public static class LinqExtension
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (var el in collection)
            action.Invoke(el);
    }
}