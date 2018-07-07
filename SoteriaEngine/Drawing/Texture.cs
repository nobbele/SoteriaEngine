using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoteriaEngine.Drawing
{
    public class Texture
    {
        CSDL.SDLTexture texture;
        string file;
        int w, h;
        bool loaded;
        public Texture(string file, int w, int h) {
            this.file = file;
            this.w = w;
            this.h = h;
        }
        private void Load(Window window) {
            texture = window.renderer.GetTexture(file, w, h);
            if (texture == IntPtr.Zero)
                Console.WriteLine(SDL2.SDL.SDL_GetError());
            loaded = true;
        }
        public void Use(Window window, SDL2.SDL.SDL_Rect target) {
            if (!loaded)
                Load(window);
            window.renderer.DrawTexture(texture, target);
        }
    }
}
