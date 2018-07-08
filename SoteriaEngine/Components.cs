using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoteriaEngine
{
    public static class Components
    {
        public class Collision : EventArgs
        {
            public Rect Rect;
            public CollisionComponent other;
            public GameObject GameObject;
        }
        public class CollisionComponent : Component
        {
            public EventHandler<Collision> CollisionHandler;
            public Rect MyCollision {
                get {
                    Rect rect = new Rect();
                    rect.x = Parent.Position.x;
                    rect.y = Parent.Position.y;
                    rect.w = Parent.Size.x;
                    rect.h = Parent.Size.y;
                    return rect;
                }
            }
            public CollisionComponent(GameObject parent) : base(parent) {
            }

            public override void Update() {
                foreach(var obj in GameObject.Root) {
                    CollisionComponent other = obj.GetComponent<CollisionComponent>();
                    if (other != null && other != this) {
                        SDL2.SDL.SDL_Rect result;
                        if (CSDL.Helper.IntersectRect(MyCollision, other.MyCollision, out result)) {
                            Collision args = new Collision();
                            args.GameObject = other.Parent;
                            args.other = other;
                            args.Rect = result;
                            CollisionHandler.Invoke(this, args);
                        }
                    }
                }
            }
        }
    }
}
