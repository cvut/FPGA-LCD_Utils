using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSPtools
{
    // Using-statements:
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Static class to assist with looking up known, named colors, by name.
    /// </summary>
    public static class TBColorLookupTable
    {
        /// <summary>
        /// Stores the lookup cache of RGB colors to known names.
        /// </summary>
        private static Dictionary<int, string> rgbLookupCache;

        /// <summary>
        /// Initializes static members of the <see cref="NamedColorStaticCache"/> class.
        /// </summary>
        static TBColorLookupTable()
        {
            rgbLookupCache = new Dictionary<int, string>();
            Type colorType = typeof(Color);
            PropertyInfo[] knownColorProperties = colorType
              .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
              .Where(t => t.PropertyType == typeof(Color))
              .ToArray();

            // Avoid treating "transparent" as "white".
            AddToCache(Color.White, "White");

            foreach (PropertyInfo pi in knownColorProperties)
            {
                Color asColor = (Color)pi.GetValue(null);
                AddToCache(asColor, pi.Name);
            }
        }

        /// <summary>
        /// Converts the specified <see cref="Color"/> to a 24-bitstring on an int, of 00000000RRRRRRRRGGGGGGGGBBBBBBBB.
        /// </summary>
        /// <param name="toRGB">The color to convert to rgb as a 24 bitstring. Ignores Alpha.</param>
        /// <returns>an integer representation of the specified <see cref="Color"/>.</returns>
        public static int ToRGB(byte R, byte G, byte B)
        {
            return
              (R << 16) | (G << 8) | B;
        }

        public static int ToRGB(Color toRGB)
        {
            return
              (toRGB.R << 16) |
              (toRGB.G << 8) |
              toRGB.B;
        }

        /// <summary>
        /// Looks up the name of the specified <see cref="Color"/>.
        /// </summary>
        /// <param name="toLookup">The <see cref="Color"/> to lookup a name for.</param>
        /// <returns>A string of the associated name of the specified <see cref="Color"/>.</returns>
        public static string LookupName(byte R, byte G, byte B)
        {
            int rgb = ToRGB(R, G, B);
            if (rgbLookupCache.ContainsKey(rgb))
            {
                return rgbLookupCache[rgb];
            }

            return string.Empty;
        }

        /// <summary>
        /// Takes the specified input <see cref="Color"/>, and translates it to its nearest counterpart, using root square sum.
        /// </summary>
        /// <param name="toNearest">The <see cref="Color"/> to look up to the nearest named color.</param>
        /// <returns>A tuple structure of name, color, error.</returns>
        public static string ToNearestNamedColor(int R, int G, int B, out int foundRGB)
        {
            string foundName = string.Empty; foundRGB = -1;
            int error = int.MaxValue;
            if (rgbLookupCache == null) return String.Empty;
            foreach (KeyValuePair<int, string> pair in rgbLookupCache)
            {
                int rgb = pair.Key;
                int r = (rgb >> 16)&0xFF;
                int g = (rgb>>8)&0xFF;
                int b = rgb & 0xFF;
                int dr = r - R;
                int dg = g - G;
                int db = b - B;
                int newError= (r+R)<256 ? 2*dr*dr + 4*dg*dg + 3*db*db : 3 * dr * dr + 4 * dg * dg + 2 * db * db; 
                if (newError < error)
                {
                    foundName = pair.Value;
                    foundRGB = rgb;
                    error = newError;
                }
            }
            return foundName;

        }

        /// <summary>
        /// Takes the specified input <see cref="Color"/>, and translates it to its nearest counterpart, using root square sum.
        /// </summary>
        /// <param name="toNearest">The <see cref="Color"/> to look up to the nearest named color.</param>
        /// <returns>A tuple structure of name, color, error.</returns>
        public static Tuple<string, Color, double> ToNearestNamedColor(this Color toNearest)
        {
            string foundName = string.Empty;
            Color foundColor = Color.Black;
            double error = double.MaxValue;

            int toNearestRGB = ToRGB(toNearest);
            if (rgbLookupCache.ContainsKey(toNearestRGB))
            {
                foundName = rgbLookupCache[toNearestRGB];
                foundColor = toNearest;
                error = 0;
            }
            else
            {
                foreach (KeyValuePair<int, string> pair in rgbLookupCache)
                {
                    int rgb = pair.Key;
                    byte r = (byte)(rgb >> 16);
                    byte g = (byte)(rgb << 16 >> 24);
                    byte b = (byte)(rgb << 24 >> 24);
                    int dr = r - toNearest.R;
                    int dg = g - toNearest.G;
                    int db = b - toNearest.B;
                    double newError =
                      Math.Sqrt(
                        (dr * dr) +
                        (dg * dg) +
                        (db * db));

                    if (newError < error)
                    {
                        foundName = pair.Value;
                        foundColor = Color.FromArgb(r, g, b);
                        error = newError;
                    }

                    if (newError <= .0005)
                    {
                        break;
                    }
                }
            }

            return Tuple.Create(foundName, foundColor, error);
        }


        /// <summary>
        /// Adds the specified <see cref="Color"/> to a lookup cache of named colors.
        /// </summary>
        /// <param name="toAdd">The <see cref="Color"/> to add to the lookup cache.</param>
        /// <param name="name">The name of the <see cref="Color"/> to add to the lookup cache.</param>
        /// <returns>True if adding successful, else, false (the color was already in the cache).</returns>
        public static bool AddToCache(Color toAdd, string name)
        {
            int rgb = ToRGB(toAdd);
            if (rgbLookupCache.ContainsKey(rgb))
            {
                return false;
            }

            rgbLookupCache.Add(rgb, name);
            return true;
        }
    }
}

