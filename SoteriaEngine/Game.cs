using SoteriaEngine.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static SDL2.SDL;

namespace SoteriaEngine
{
    public abstract class Shape { }
    public class Rect : Shape
    {
        public float x, y, w, h;

        public Rect(SDL2.SDL.SDL_Rect rect) {
            x = rect.x; y = rect.y;
            w = rect.w; h = rect.h;
        }
        public Rect(float x, float y, float w, float h) {
            this.x = x; this.y = y;
            this.w = w; this.h = h;
        }
        public Rect() { }

        public static implicit operator SDL2.SDL.SDL_Rect(Rect me) {
            return new SDL_Rect() { x = (int)Math.Round(me.x), y = (int)Math.Round(me.y), w = (int)Math.Round(me.w), h = (int)Math.Round(me.h) };
        }
        public static implicit operator Rect(SDL2.SDL.SDL_Rect rect) {
            return new Rect(rect);
        }
        public override string ToString() {
            return string.Format("[x: {2}, y: {3}, width: {0}, height: {1}", w, h, x, y);
        }
    }
    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) {
            this.x = x; this.y = y;
        }
        public override string ToString() {
            return string.Format("[x: {0}, y: {1}]", x, y);
        }
    }
    public abstract class Component
    {
        public GameObject Parent { get; }
        public virtual void Start() { }
        public virtual void Update() { }
        public Component(GameObject parent) {
            Parent = parent;
        }
    }
    public abstract class GameObject
    {
        internal List<Component> Components = new List<Component>();
        public void AddComponent<T>() where T : Component {
            Components.Add((Component)Activator.CreateInstance(typeof(T), this));
        }
        public T GetComponent<T>() where T : Component {
            foreach(var component in Components) {
                if (component.GetType() == typeof(T))
                    return (T)component;
            }
            return null;
        }
        public static List<GameObject> Root {
            get {
                return Game.GameObjects;
            }
        }
        public string Name { get; set; }
        public Drawable Drawable { get; set; }
        public Vector2 Position {
            get {
                return new Vector2(Drawable.x, Drawable.y);
            }
            set {
                Drawable.y = value.y;
                Drawable.x = value.x;
            }
        }
        public Vector2 Size { get; set; } = new Vector2();
        public GameObject(Drawable drawable, string name = "") {
            Name = name;
            Drawable = drawable;
            if(Drawable.GetType() == typeof(DrawableTexture)) {
                DrawableTexture tex = Drawable as DrawableTexture;
                Size = new Vector2(tex.width, tex.height);
            }
        }

        internal protected virtual void Start() {}
        internal protected virtual void Update() {}

        internal void Draw(Window window) {
            window.Draw(Drawable);
        }
        public void Scale(float w, float h) {
            Size = new Vector2(Size.x + w, Size.y + h);
        }
        public void Move(float x, float y) {
            Position = new Vector2(Position.x + x, Position.y + y);
        }
    }
    public abstract class Game
    {
        internal static List<GameObject> GameObjects { get; set; } = new List<GameObject>();
        protected GameObject Add(GameObject obj) {
            GC.KeepAlive(obj);
            GameObjects.Add(obj);
            return obj;
        }

        public string Title { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Window Window { get; private set; }
        protected DrawableTexture Background { get; set; }
        public void Start(string title, int width, int height) {
            Title = title;
            Width = width;
            Height = height;
            Window = new Window(Title, Width, Height);
            Window.Running = true;
            _load();
            Load();
            _start();
            int timer = DateTime.Now.Second;
            int fpscounter = 0;
            while (Window.Running) {
                uint start = CSDL.Helper.GetTicks();
                fpscounter++;
                {
                    if(DateTime.Now.Second != timer) {
                        timer = DateTime.Now.Second;
                        Time.FPS = fpscounter;
                        fpscounter = 0;
                    }
                    Window.Poll();
                    Window.Clear();
                    Background.Draw(Window);
                    Draw(Window);
                    _draw(Window);
                    Window.DrawScreen();
                    Update();
                    _update();
                }
                Time.DeltaTime = ((float)(CSDL.Helper.GetTicks() - start)) / Time.FPS;
                int delay = (int)(Time.DeltaTime * Time.FPS);
                if (delay > 1000 / Time.DesiredFPS)
                    delay = 1000 / Time.DesiredFPS;
                Time.Sleep((1000 / Time.DesiredFPS) - delay);
            }
            Window.Running = false;
        }
        protected abstract void Load();
        internal void _load() {
            Input.Load();
        }
        internal void _start() {
            foreach (GameObject obj in GameObjects) {
                obj.Start();
                foreach (Component comp in obj.Components) {
                    comp.Start();
                }
            }
        }
        protected virtual void Draw(Window window) {}
        internal void _draw(Window window) {
            foreach (GameObject obj in GameObjects) {
                obj.Draw(window);
            }
        }
        protected abstract void Update();
        internal void _update() {
            Input.Update();
            foreach (GameObject obj in GameObjects) {
                obj.Update();
                foreach (Component comp in obj.Components) {
                    comp.Update();
                }
            }
        }

        #region HelperMethods

        public static class Time
        {
            public static float DeltaTime = 0;
            public const int DesiredFPS = 150;
            public static int FPS = DesiredFPS;
            public static void Sleep(int ms) {
                Thread.Sleep(ms);
            }
        }

        public static class Input
        {
            internal enum KeyState
            {
                PRESSED, DOWN, UP, NONE
            }
            public enum Key
            {
                ZERO, ONE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE,
                a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z,
                SHIFT, SPACE, CTRL
            }
            internal static Dictionary<Key, KeyState> keys = new Dictionary<Key, KeyState>();
            internal static void Load() {
                foreach(Key key in Enum.GetValues(typeof(Key))) {
                    keys.Add(key, KeyState.NONE);
                }
            }
            internal static void Update() {
                var newkeys = new Dictionary<Key, KeyState>();
                foreach (KeyValuePair<Key, KeyState> pair in keys) {
                    if(pair.Value == KeyState.PRESSED) {
                        newkeys[pair.Key] = KeyState.DOWN;
                    } else if (pair.Value == KeyState.UP) {
                        newkeys[pair.Key] = KeyState.NONE;
                    } else {
                        newkeys[pair.Key] = pair.Value;
                    }
                }
                keys = newkeys;
            }
            public static bool KeyPressed(Key key) {
                return keys[key] == KeyState.PRESSED;
            }
            public static bool KeyDown(Key key) {
                return keys[key] == KeyState.DOWN;
            }
            public static bool KeyUp(Key key) {
                return keys[key] == KeyState.UP;
            }
        }

        #endregion
    }
}
