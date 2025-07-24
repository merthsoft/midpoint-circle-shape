using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Merthsoft.DesignatorShapes.MidpointCircle;

public static class MidpointCircle
{
    public static IEnumerable<IntVec3> Line(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        var ret = new HashSet<IntVec3> {
            new(x1, y1, z1),
            new(x2, y2, z2)
        };

        var deltaX = Math.Abs(x1 - x2);
        var deltaZ = Math.Abs(z1 - z2);
        var stepX = x2 < x1 ? 1 : -1;
        var stepZ = z2 < z1 ? 1 : -1;

        var err = deltaX - deltaZ;

        while (true)
        {
            ret.Add(new(x2, y1, z2));
            if (x2 == x1 && z2 == z1)
                break;
            var e2 = 2 * err;

            if (e2 > -deltaZ)
            {
                err -= deltaZ;
                x2 += stepX;
            }

            if (x2 == x1 && z2 == z1)
                break;
            ret.Add(new IntVec3(x2, y1, z2));

            if (e2 < deltaX)
            {
                err += deltaX;
                z2 += stepZ;
            }
        }

        return ret;
    }

    /// <summary>
    /// Draws a filled ellipse to the sprite.
    /// </summary>
    /// <remarks>Taken from http://enchantia.com/graphapp/doc/tech/ellipses.html.</remarks>
    /// <param name="x">The center point X coordinate.</param>
    /// <param name="z">The center point Z coordinate.</param>
    /// <param name="xRadius">The x radius.</param>
    /// <param name="zRadius">The z radius.</param>
    /// <param name="fill">True to fill the ellipse.</param>
    public static IEnumerable<IntVec3> RadialEllipse(int x, int y, int z, int xRadius, int zRadius, bool fill)
    {
        var ret = new HashSet<IntVec3>();

        var plotX = 0;
        var plotZ = zRadius;

        var xRadiusSquared = xRadius * xRadius;
        var zRadiusSquared = zRadius * zRadius;

        var crit1 = -(xRadiusSquared / 4 + xRadius % 2 + zRadiusSquared);
        var crit2 = -(zRadiusSquared / 4 + zRadius % 2 + xRadiusSquared);
        var crit3 = -(zRadiusSquared / 4 + zRadius % 2);

        var t = -xRadiusSquared * plotZ;
        var dxt = 2 * zRadiusSquared * plotX;
        var dzt = -2 * xRadiusSquared * plotZ;
        var d2xt = 2 * zRadiusSquared;
        var d2zt = 2 * xRadiusSquared;

        while (plotZ >= 0 && plotX <= xRadius)
        {
            circlePlot(x, y, z, ret, plotX, plotZ, fill);

            if (t + zRadiusSquared * plotX <= crit1 || t + xRadiusSquared * plotZ <= crit3)
                incrementX(ref plotX, ref dxt, ref d2xt, ref t);
            else if (t - xRadiusSquared * plotZ > crit2)
                incrementY(ref plotZ, ref dzt, ref d2zt, ref t);
            else
            {
                incrementX(ref plotX, ref dxt, ref d2xt, ref t);
                circlePlot(x, y, z, ret, plotX, plotZ, fill);
                incrementY(ref plotZ, ref dzt, ref d2zt, ref t);
            }
        }

        return ret;

        static void incrementX(ref int x, ref int dxt, ref int d2xt, ref int t)
        {
            x++;
            dxt += d2xt;
            t += dxt;
        }

        static void incrementY(ref int y, ref int dyt, ref int d2yt, ref int t)
        {
            y--;
            dyt += d2yt;
            t += dyt;
        }

        static void circlePlot(int x, int y, int z, HashSet<IntVec3> ret, int plotX, int plotZ, bool fill)
        {
            var center = new IntVec3(x, y, z);
            ret.AddRange(plotOrLine(center, new IntVec3(x + plotX, 0, z + plotZ), fill));
            if (plotX != 0 || plotZ != 0)
                ret.AddRange(plotOrLine(center, new IntVec3(x - plotX, 0, z - plotZ), fill));

            if (plotX != 0 && plotZ != 0)
            {
                ret.AddRange(plotOrLine(center, new IntVec3(x + plotX, 0, z - plotZ), fill));
                ret.AddRange(plotOrLine(center, new IntVec3(x - plotX, 0, z + plotZ), fill));
            }
        }

        static IEnumerable<IntVec3> plotOrLine(IntVec3 point1, IntVec3 point2, bool line)
        => line ? Line(point1.x, point1.y, point1.z, point2.x, point2.y, point2.z)
                : [new IntVec3(point2.x, point2.y, point2.z)];
    }

    public static float getRadius(IntVec3 vert1, IntVec3 vert2)
    {
        var dx = Math.Abs(vert2.x - vert1.x);
        var dz = Math.Abs(vert2.z - vert1.z);
        return Math.Max(dx, dz);
    }

    public static IEnumerable<IntVec3> Filled(IntVec3 vert1, IntVec3 vert2)
    {
        var radius = (int)getRadius(vert1, vert2);
        return RadialEllipse(vert1.x, vert1.y, vert1.z, radius, radius, true);
    }

    public static IEnumerable<IntVec3> Outline(IntVec3 vert1, IntVec3 vert2)
    {
        var radius = (int)getRadius(vert1, vert2);
        return RadialEllipse(vert1.x, vert1.y, vert1.z, radius, radius, false);
    }
}
