using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LinqExtensions
{
    /// <summary>
    /// SoftMax関数
    /// </summary>
    public static IEnumerable<float> SoftMax(this IEnumerable<float> source)
    {
        var exp = source.Select(Mathf.Exp).ToArray();
        var sum = exp.Sum();
        return exp.Select(x => x / sum);
    }
}
