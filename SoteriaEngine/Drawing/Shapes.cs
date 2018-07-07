using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoteriaEngine.Drawing
{
    public class Rectangle : Drawable
    {
        public int width = 25, height = 25;
        public bool filled = true;

       internal override void Draw(Window window) {
            base.Draw(window);
            if (filled) window.renderer.DrawFilledRect(this);
            else window.renderer.DrawRect(this);
        }
        public static implicit operator SDL2.SDL.SDL_Rect(Rectangle me) => new SDL2.SDL.SDL_Rect() { x = (int)me.x, y = (int)me.y, w = me.width, h = me.height };
    }
    public class DrawableTexture : Drawable
    {
        public int width, height;
        public Texture texture;
        public DrawableTexture(Texture texture, int width = 25, int height = 25, int x = 0, int y = 0) {
            this.x = x; this.y = x;
            this.width = width; this.height = height;
            color = Color.Transparent;
            this.texture = texture;
        }
        public DrawableTexture(string texturepath, int width = 25, int height = 25, int x = 0, int y = 0) : this(new Texture(texturepath, width, height), width, height, x, y) {}

        internal override void Draw(Window window) {
            base.Draw(window);
            texture?.Use(window, this);
        }
        public static implicit operator SDL2.SDL.SDL_Rect(DrawableTexture me) {
            SDL2.SDL.SDL_Rect rect;
            rect.x = (int)me.x;
            rect.y = (int)me.y;
            rect.w = me.width;
            rect.h = me.height;
            return rect;
        }
    }
    public class Text : Drawable
    {
        public int size;
        public string text;
    }
}
