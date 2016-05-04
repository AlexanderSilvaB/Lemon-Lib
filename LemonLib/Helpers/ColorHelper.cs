using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

using LemonLib.Extensions;

namespace LemonLib.Helpers
{
    public class ColorHelper
    {
        public static Color ToColor(string hexColor)
        {
            return hexColor.ToColor();
        }

        public static string ToHex(Color color)
        {
            return color.ToHex();
        }
    }
}
