using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoteriaEngine.Drawing
{
    public struct Color
    {
        public byte r, g, b, a;
        public static Color Black => new Color() { r = 0, g = 0, b = 0, a = 255 };
        public static Color Red => new Color() { r = 255, g = 0, b = 0, a = 255 };
        public static Color Transparent => new Color() { r = 0, g = 0, b = 0, a = 0 };
    }
}
