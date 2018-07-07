using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoteriaEngine.Drawing;
using static SoteriaEngine.Game;

namespace SampleGame
{
    class Program
    {
        static void Main(string[] args) {
            Game game = new Game();
            game.Start(title: "Hello World", width: 640, height: 480);
        }
    }
    class Player : SoteriaEngine.GameObject
    {
        public Player(Drawable drawable, string name = "") : base(drawable, name) {}

        protected override void Start() {
        }

        protected override void Update() {
            int speed = 5;
            if (Input.KeyDown(Input.Key.w)) {
                Move(0, -speed);
            }
            if (Input.KeyDown(Input.Key.d)) {
                Move(speed, 0);
            }
            if (Input.KeyDown(Input.Key.a)) {
                Move(-speed, 0);
            }
            if (Input.KeyDown(Input.Key.s)) {
                Move(0, speed);
            }
        }
    }
    class Game : SoteriaEngine.Game
    {
        protected override void Load() {
            Add(new Player(new DrawableTexture("test.png")));
            Background = new DrawableTexture(new Texture("Track1A.png", Width, Height), Width, Height);
        }
        protected override void Draw(Window window) {
        }
        protected override void Update() {
            
        }
    }
}
