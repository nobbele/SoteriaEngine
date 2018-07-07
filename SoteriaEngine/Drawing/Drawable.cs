using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoteriaEngine.Drawing
{
    
    public abstract class Drawable
    {
        public float x = 0, y = 0;
        public Color color = Color.Transparent;
        internal virtual void Draw(Window window) {
            window.renderer.SetColor(color);
        }
    }
}
