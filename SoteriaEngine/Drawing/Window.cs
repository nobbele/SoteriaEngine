using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSDL;
using static SDL2.SDL;
using static SoteriaEngine.Game.Input;
using static SoteriaEngine.Game;

namespace SoteriaEngine.Drawing
{
    public class Window {
        private bool destroyed;
        internal SDLWindow window;
        internal SDLRenderer renderer;
        public string Title { get; }
        public int Width { get; }
        public int Height { get; }
        public bool Running { get; set; } = false;
        internal Window(string title, int width, int height) {
            Helper.Init();
            Title = title;
            Width = width;
            Height = height;
            window = new SDLWindow(title, width, height);
            renderer = new SDLRenderer(window);
        }
        internal void Destroy() {
            if (!destroyed) {
                window.Destroy();
                renderer.Destroy();
                destroyed = true;
            }
        }
        ~Window() {
            Destroy();
        }
        public void Draw(Drawable drawable) {
            drawable.Draw(this);
        }
        public void DrawScreen() {
            renderer.DrawScreen(window);
        }
        public void Clear() {
            renderer.ClearScreen(window);
        }
        private Key? GetKey(SDL_Keycode key) {
            if (key >= SDL_Keycode.SDLK_a && key <= SDL_Keycode.SDLK_z)
                return Key.a + (key - SDL_Keycode.SDLK_a);
            if (key >= SDL_Keycode.SDLK_0 && key <= SDL_Keycode.SDLK_9)
                return Key.ZERO + (key - SDL_Keycode.SDLK_0);
            switch(key) {
                case SDL_Keycode.SDLK_LSHIFT:
                case SDL_Keycode.SDLK_RSHIFT:
                    return Key.SHIFT;
                case SDL_Keycode.SDLK_LCTRL:
                case SDL_Keycode.SDLK_RCTRL:
                    return Key.CTRL;
                case SDL_Keycode.SDLK_SPACE:
                    return Key.SPACE;
                default:
                    return null;
            }
        }
        public void Poll() {
            SDLEvent e;
            while (Helper.PollEvent(out e) > 0) {
                switch (e.GetEventType()) {
                    case SDL_EventType.SDL_QUIT:
                        Running = false;
                        break;
                    case SDL_EventType.SDL_WINDOWEVENT:
                        switch (e.GetEvent().window.windowEvent) {
                            case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN:
                                Console.WriteLine("Window shown");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN:
                                Console.WriteLine("Window hidden");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED:
                                Console.WriteLine("Window exposed");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED:
                                Console.WriteLine("Window %d moved");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                                Console.WriteLine("Window %d resized");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                                Console.WriteLine("Window size changed");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED:
                                Console.WriteLine("Window minimized");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED:
                                Console.WriteLine("Window maximized");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED:
                                Console.WriteLine("Window restored");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_ENTER:
                                Console.WriteLine("Mouse entered window");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE:
                                Console.WriteLine("Mouse left window");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED:
                                Console.WriteLine("Window gained keyboard focus");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST:
                                Console.WriteLine("Window lost keyboard focus");
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE:
                                Console.WriteLine("Window closed");
                                break;
                        }
                        break;
                    case SDL_EventType.SDL_KEYDOWN:
                        Key? nulldownkey = GetKey(e.GetEvent().key.keysym.sym);
                        Key downkey;
                        if (nulldownkey == null) {
                            break;
                        } else {
                            downkey = nulldownkey.Value;
                        }
                        Input.keys[downkey] = Input.keys[downkey] == KeyState.PRESSED ? KeyState.DOWN : KeyState.PRESSED;
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        Key? nullupkey = GetKey(e.GetEvent().key.keysym.sym);
                        Key upkey;
                        if(nullupkey == null) {
                            break;
                        } else {
                            upkey = nullupkey.Value;
                        }
                        Input.keys[upkey] = KeyState.UP;
                        break;
                    case SDL_EventType.SDL_TEXTINPUT:
                        break;
                    case SDL_EventType.SDL_MOUSEMOTION:
                        //TODO
                        break;
                    default:
                        Console.WriteLine(e.GetEvent().type.ToString());
                        break;
                }
            }
        }
    }
}
