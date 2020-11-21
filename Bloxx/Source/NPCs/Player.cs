using Bloxx.Source.Map;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoExt;
using MonoNet;
using System;

namespace Bloxx.Source.NPCs
{
    public class Player : NPC
    {
        protected const float jumpHeight = -8f;
        protected const float speed = 5f;

        readonly PlayerInput input;

        readonly bool me;

        bool jumped;

        Animation WalkingAnim;

        public Player(World _world, PlayerInput _input, bool _me, Color c) : base(_world, "")
        {
            me = _me;

            input = _input;
            //colour = c;
            transform.size = Vector2.One;

            WalkingAnim = new Animation(WalkingSprites, 5);
        }

        public override void Update(GameTime gameTime)
        {
            if (transform.grounded)
            {
                jumped = false;
            }

            if (input.JumpDown() && transform.grounded)
            {
                transform.velocity.Y = jumpHeight;
                jumped = true;
            }

            transform.Gravitate();

            if (transform.position.Y > world.Bottom)
            {
                transform.position.Y = 0;
            }

            transform.velocity.X = input.Horizontal() * speed;

            if (transform.velocity.X > 0)
            {
                spriteEffects = SpriteEffects.None;
            } else if (transform.velocity.X < 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            

            transform.position.X += transform.velocity.X / Tile.size;
            transform.CollisionX();

            Tag = $"{spriteEffects == SpriteEffects.FlipHorizontally}/{jumped}/";

            if (Math.Abs(transform.velocity.X) > 0 && transform.grounded)
            {
                WalkingAnim.Increment(1);
                Tag += WalkingAnim.CurID;
            }
            else
            {
                WalkingAnim.CurID = 0;
                Tag += -1;
            }

            //colour = OverlappingGround() ? Color.Red : Color.Yellow;

            world.BoundEntity(this);

            if (me) world.camera.Follow(transform.Centre * Tile.size, 0.1f);

            base.Update(gameTime);
        }

        bool walking;
        protected override Texture2D GetTexture()
        {
            if (walking || (Math.Abs(transform.velocity.X) > 0 && transform.grounded))
            {
                return WalkingAnim.GetTexture;
            } else
            {
                return jumped ? JumpSprite : IdleSprite;
            }
        }

        public override void ClientUpdate(GameTime gameTime)
        {
            if (me)
            {
                var msg = Networking.CreateMessage();

                msg.Write((byte)GameScreen.ClientHeaders.INPUT);
                msg.Write(input.Horizontal());
                msg.Write(input.Jump());

                Networking.SendMessage(msg, NetDeliveryMethod.ReliableSequenced, 1);

                world.camera.position = transform.Centre;
                //world.camera.Follow(transform.Centre, 1f);
            }
        }

        public override void UpdateData(NetIncomingMessage msg)
        {
            bool flip = msg.ReadBoolean();
            bool jump = msg.ReadBoolean();
            int walkID = msg.ReadInt32();

            spriteEffects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            jumped = jump;

            if (walkID != -1)
            {
                WalkingAnim.CurID = walkID;
                walking = true;
            }
            else
            {
                WalkingAnim.CurID = 0;
                walking = false;
            }
        }

        public override void TagChanged()
        {
            string[] parts = Tag.Split('/');

            bool flip = bool.Parse(parts[0]);
            bool jump = bool.Parse(parts[1]);
            int walkID = int.Parse(parts[2]);

            spriteEffects = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            jumped = jump;

            if (walkID != -1)
            {
                WalkingAnim.CurID = walkID;
                walking = true;
            }
            else
            {
                WalkingAnim.CurID = 0;
                walking = false;
            }
        }

        static Texture2D IdleSprite;
        static Texture2D JumpSprite;
        static Texture2D[] WalkingSprites;
        

        public static void LoadSprites(ContentManager content)
        {
            IdleSprite = content.Load<Texture2D>("Entities/Player/idle");
            JumpSprite = content.Load<Texture2D>("Entities/Player/jump");

            WalkingSprites = new Texture2D[] 
            { 
                content.Load<Texture2D>("Entities/Player/walk"),
                content.Load<Texture2D>("Entities/Player/walk2"),
                content.Load<Texture2D>("Entities/Player/walk3")
            };
        }
    }
}
