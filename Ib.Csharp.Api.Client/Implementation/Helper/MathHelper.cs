using System;

namespace Ib.Csharp.Api.Client.Implementation.Helper;

public static class MathHelper
{
    public static double RoundToNearest(double value, int interval = 100)
    {
        if (interval == 0) return value;
        return Math.Round(value / interval, MidpointRounding.AwayFromZero) * interval;
    }

    public static bool DoubleEquals(double a, double b, double tolerance = 1e-9)
    {
        return Math.Abs(a - b) < tolerance;
    }

    public static double NormalPdf(double x)
    {
        return Math.Exp(-0.5 * x * x) / Math.Sqrt(2 * Math.PI);
    }
}