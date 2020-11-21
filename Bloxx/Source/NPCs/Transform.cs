using Bloxx.Source.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoCamera2D;
using MonoExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.NPCs
{
    public class Transform
    {
        readonly World world;
        public Transform(World _world)
        {
            world = _world;
        }

        public Vector2 position;
        public Vector2 LastPosition { get; private set; }

        public void PushPosition() => LastPosition = position;

        public Vector2 size = Vector2.One;

        public Vector2 velocity;

        public const float gravity = 0.375f;
        public const float terminalVel = 10f;

        public float gravityScale = 1f;

        public float Left => position.X;
        public float Right => position.X + size.X;
        public float Top => position.Y;
        public float Bottom => position.Y + size.Y;
        public Vector2 Centre => position + size / 2;

        public bool grounded;

        public RectangleF GetRectangle()
        {
            return new RectangleF(position, size);
        }

        public bool InCameraBounds(Camera2D camera)
        {
            return Left < camera.Right / Tile.size &&
                   Right > camera.Left / Tile.size &&
                   Top < camera.Bottom / Tile.size &&
                   Bottom > camera.Top / Tile.size;
        }

        public void Gravitate()
        {
            grounded = false;

            velocity.Y += gravity * gravityScale;
            velocity.Y = Math.Min(velocity.Y, terminalVel);

            position.Y += velocity.Y / Tile.size;
            CollisionY();
        }
        public void CollisionY()
        {
            if (OverlappingGround())
            {
                if (velocity.Y > 0)
                {
                    while (OverlappingGround())
                    {
                        position.Y -= 0.01f;
                    }

                    //transform.position.Y = (int)Math.Ceiling(transform.position.Y);
                    grounded = true;
                }
                else if (velocity.Y < 0)
                {
                    while (OverlappingGround())
                    {
                        position.Y += 0.01f;
                    }

                    //transform.position.Y = (int)Math.Floor(transform.position.Y);
                }

                velocity.Y = 0;
            }
        }

        public void CollisionX()
        {
            if (OverlappingGround())
            {
                if (velocity.X > 0)
                {
                    while (OverlappingGround())
                    {
                        position.X -= 0.01f;
                    }
                    //transform.position.X = (int)(Math.Floor(transform.position.X / Tile.size) * Tile.size);
                }
                else if (velocity.X < 0)
                {
                    while (OverlappingGround())
                    {
                        position.X += 0.01f;
                    }
                    //transform.position.X = (int)(Math.Ceiling(transform.position.X / Tile.size) * Tile.size) - 1;
                }

                velocity.X = 0;
            }
        }

        bool OverlappingGround()
        {
            
            int xMin = (int)Math.Min(Math.Floor(Left), Math.Ceiling((Right - 0.01f)));
            int xMax = (int)Math.Max(Math.Floor(Left), Math.Ceiling((Right - 0.01f)));

            int yMin = (int)Math.Min(Math.Floor(Top), Math.Floor((Bottom - 0.01f)));
            int yMax = (int)Math.Max(Math.Floor(Top), Math.Floor((Bottom - 0.01f)));
            

            for (int x = xMin; x < xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    if (world.TileIsType(x, y, TileData.Solidity.SOLID))
                    {
                        return world.GetTile(x, y).Data.solidity == TileData.Solidity.SOLID;
                    }

                }
            }

            return false;
        }

    }
}
