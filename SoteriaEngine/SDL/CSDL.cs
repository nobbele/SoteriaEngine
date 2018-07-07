using SoteriaEngine.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
using static SDL2.SDL_image;

namespace CSDL
{
    internal class SDLEvent
    {
        private SDL_Event e;
        public SDLEvent(SDL_Event e) {
            this.e = e;
        }
        public static implicit operator SDL_Event(SDLEvent me) {
            return me.e;
        }
        public static implicit operator SDLEvent(SDL_Event e) {
            return new SDLEvent(e);
        }
        public SDL_EventType GetEventType() {
            return e.type;
        }
        public SDL_Event GetEvent() {
            return this;
        }
    }
    internal class SDLSurface
    {
        private IntPtr surface;
        public SDLSurface(IntPtr surface) {
            this.surface = surface;
        }
        public void ScaleTo(SDLSurface target, int width, int height) {
            SDL_Rect rect = new SDL_Rect() { x = 0, y = 0, w = width, h = height };
            SDL_BlitScaled(surface, IntPtr.Zero, target, ref rect);
        }
        public SDLSurface(int width, int height) {
            surface = SDL_CreateRGBSurface(0, width, width, 32, 
                                        0x00FF0000,
                                        0x0000FF00,
                                        0x000000FF,
                                        0xFF000000);
        }
        public static implicit operator IntPtr(SDLSurface me) => me.surface;
        public static implicit operator SDLSurface(IntPtr me) => new SDLSurface(me);
        ~SDLSurface() {
            SDL_FreeSurface(surface);
        }
    }
    internal class SDLTexture
    {
        private IntPtr texture;
        public SDLTexture(SDLRenderer renderer, SDLSurface surface, int width, int height) {
            SDLSurface target = new SDLSurface(width, height);
            surface.ScaleTo(target, width, height);
            texture = SDL_CreateTextureFromSurface(renderer, target);
        }
        public void SetAlpha(byte alpha) {
            SDL_SetTextureAlphaMod(this, alpha);
        }
        public static implicit operator IntPtr(SDLTexture me) => me.texture;
        ~SDLTexture() {
            SDL_DestroyTexture(texture);
        }
    }
    internal static class Helper
    {
        static bool inited = false;
        public static void Init() {
            if (!inited) {
                SDL_Init(SDL_INIT_VIDEO);
                IMG_Init(IMG_InitFlags.IMG_INIT_PNG);
                inited = true;
            }
        }
        public static int PollEvent(out SDLEvent e) {
            SDL_Event _event;
            int ret = SDL_PollEvent(out _event);
            e = _event;
            return ret;
        }
        public static uint GetTicks() {
            return SDL_GetTicks();
        }
    }
    internal class SDLWindow
    {
        private IntPtr window;
        public static implicit operator IntPtr(SDLWindow me) {
            return me.window;
        }
        public string Title { get; }
        public int Width { get; }
        public int Height { get; }
        public SDLWindow(string title, int width, int height) {
            Title = title;
            Width = width;
            Height = height;
            window = SDL_CreateWindow(title, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, width, height, SDL_WindowFlags.SDL_WINDOW_SHOWN);
        }
        public void Destroy() {
            SDL_DestroyWindow(this);
        }
    }
    internal class SDLRenderer
    {
        private IntPtr renderer;
        public static implicit operator IntPtr(SDLRenderer me) {
            return me.renderer;
        }
        public SDLRenderer(SDLWindow window) {
            renderer = SDL_CreateRenderer(window, 0, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
        }
        public void SetColor(Color color) {
            SDL_SetRenderDrawColor(this, color.r, color.g, color.b, color.a);
        }
        public void DrawRect(SDL_Rect rect) {
            SDL_RenderDrawRect(this, ref rect);
        }
        public void DrawFilledRect(SDL_Rect rect) {
            SDL_RenderFillRect(this, ref rect);
        } 
        public void DrawTexture(SDLTexture texture, SDL_Rect rect) {
            SDL_RenderCopy(this, texture, IntPtr.Zero, ref rect);
        }
        public SDLTexture GetTexture(string file, int w, int h) {
            SDLSurface surface = IMG_Load(file);
            return new SDLTexture(this, surface, w, h);
        }
        public void Destroy() {
            SDL_DestroyRenderer(this);
        }
        public void DrawScreen(SDLWindow window) {
            SDL_RenderPresent(renderer);
        }
        public void ClearScreen(SDLWindow window) {
            byte r, g, b, a;
            SDL_GetRenderDrawColor(this, out r, out g, out b, out a);
            SDL_SetRenderDrawColor(this, 0, 0, 0, 255);
            SDL_RenderClear(this);
            SDL_SetRenderDrawColor(this, r, g, b, a);
        }
    }
}
