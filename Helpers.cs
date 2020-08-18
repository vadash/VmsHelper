using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmsHelper
{
    public static class Helpers
    {
        public static double DistanceSquared(SharpDX.Vector2 v1, SharpDX.Vector2 v2)
        {
            return Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2);
        }
    }
}
