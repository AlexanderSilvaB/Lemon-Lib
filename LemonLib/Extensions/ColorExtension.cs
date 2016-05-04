using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace LemonLib.Extensions
{
    public static class ColorExtension
    {
        public static Color ToColor(this string hexColor)
        {
            string tempHexColor = hexColor;
            if (tempHexColor.Length == 7)
                tempHexColor = "#FF" + hexColor.Substring(1, 6);
            if (tempHexColor.Length != 9)
                tempHexColor = "#00000000";
            return Color.FromArgb(
               Convert.ToByte(tempHexColor.Substring(1, 2), 16),
               Convert.ToByte(tempHexColor.Substring(3, 2), 16),
               Convert.ToByte(tempHexColor.Substring(5, 2), 16),
               Convert.ToByte(tempHexColor.Substring(7, 2), 16));
        }

        public static string ToHex(this Color color)
        {
            return "#" + Convert.ToString(color.A, 16) +
                Convert.ToString(color.R, 16) +
                Convert.ToString(color.G, 16) +
                Convert.ToString(color.B, 16);
        }
    }
}
