using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoteriaEngine;
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
    abstract class GravitationalGameObject : SoteriaEngine.GameObject
    {
        public GravitationalGameObject(Drawable drawable, string name = "") : base(drawable, name) { }

        public const float Gravity = 4.2f;
        protected float GravMult = 1;

        protected override void Update() {
            Move(0, Gravity * GravMult);
        }
    }
    class Player : SoteriaEngine.GameObject
    {
        public Player(Drawable drawable, string name = "Player") : base(drawable, name) {
            AddComponent<Components.CollisionComponent>();
            GetComponent<Components.CollisionComponent>().CollisionHandler += (object sender, Components.Collision coll) => {
                Debug.Log(Name + " collided with " + coll.GameObject.Name + " at " + Position);
            };
        }

        protected int speed = 2;

        protected Input.Key up, down, left, right;

        protected override void Start() {
            up = Input.Key.w;
            down = Input.Key.s;
            left = Input.Key.a;
            right = Input.Key.d;
        }

        protected override void Update() {
            base.Update();
            if (Input.KeyDown(up)) {
                Move(0, -speed);
            }
            if (Input.KeyDown(right)) {
                Move(speed, 0);
            }
            if (Input.KeyDown(left)) {
                Move(-speed, 0);
            }
            if (Input.KeyDown(down)) {
                Move(0, speed);
            }
        }
    }
    class Player2 : Player
    {
        public Player2(Drawable drawable, string name = "") : base(drawable, name) {}

        protected override void Start() {
            up = Input.Key.i;
            down = Input.Key.k;
            left = Input.Key.j;
            right = Input.Key.l;
        }
    }
    class Game : SoteriaEngine.Game
    {
        protected override void Load() {
            Add(new Player(new DrawableTexture("test.png"), "Player1")).Position = new Vector2(50, 50);
            Add(new Player2(new DrawableTexture("test.png"), "Player2"));
            Background = new DrawableTexture(new Texture("Track1A.png", Width, Height), Width, Height);
        }
        protected override void Draw(Window window) {
        }
        protected override void Update() {
            
        }
    }
}
