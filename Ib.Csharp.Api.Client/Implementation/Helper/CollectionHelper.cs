using System;
using System.Collections.Generic;
using System.Linq;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public static class CollectionHelper
{
    public static double GetMedian(HashSet<double> set)
    {
        if (set == null || set.Count == 0) throw new InvalidOperationException("Set is empty");

        var sorted = set.OrderBy(x => x).ToList();
        var n = sorted.Count;
    
        if (n % 2 == 1)
            return sorted[n / 2];  // Middle element for odd count
    
        // Average of two middle elements for even count
        return (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
    }
    
    public static double GetClosest(IEnumerable<double> values, double target)
    {
        ArgumentNullException.ThrowIfNull(values);

        var collection = values as IReadOnlyCollection<double> 
                         ?? values.ToList();

        if (collection.Count == 0)
            throw new InvalidOperationException("The collection is empty");

        double closest = double.NaN;
        double minDiff = double.PositiveInfinity;

        foreach (var value in collection)
        {
            double diff = Math.Abs(value - target);
            if (diff < minDiff)
            {
                minDiff = diff;
                closest = value;
            }
        }

        return closest;
    }
}