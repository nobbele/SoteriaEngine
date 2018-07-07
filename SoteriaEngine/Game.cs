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
    public struct Vector2
    {
        public float x, y;
        public Vector2(float x, float y) {
            this.x = x; this.y = y;
        }
    }
    public abstract class GameObject
    {
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
        public GameObject(Drawable drawable, string name = "") {
            Name = name;
            Drawable = drawable;
        }

        internal protected virtual void Start() {}
        internal protected virtual void Update() {}

        internal void Draw(Window window) {
            window.Draw(Drawable);
        }
        public void Move(int x, int y) {
            Position = new Vector2(Position.x + x, Position.y + y);
        }
    }
    public abstract class Game
    {
        private List<GameObject> GameObjects { get; set; } = new List<GameObject>();
        protected void Add(GameObject obj) {
            GC.KeepAlive(obj);
            GameObjects.Add(obj);
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
