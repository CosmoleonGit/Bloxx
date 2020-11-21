using Bloxx.Source.Map;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoExt;
using MonoNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.NPCs
{
    public abstract class NPC : Component
    {
        protected World world;

        string tag;
        string lastTag;

        public string Tag { get => tag; set 
            {
                if (tag == value) return;
                lastTag = tag;
                tag = value;
                if (!Networking.IsServer) TagChanged();
            } }

        public virtual void UpdateData(NetIncomingMessage msg) { }

        public NPC(World _world, string _tag) 
        { 
            world = _world; 
            transform = new Transform(_world);
            tag = _tag;
        }

        public virtual void TagChanged() { }

        public Transform transform;

        protected Color colour = Color.White;
        protected SpriteEffects spriteEffects;

        protected abstract Texture2D GetTexture();

        public bool Finished { get; protected set; }

        public virtual void ClientUpdate(GameTime gameTime) { }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(GetTexture(), transform.GetRectangle(), null, colour, 0f, Vector2.Zero, spriteEffects, 0f);

            Texture2D tex = GetTexture();

            spriteBatch.Draw(tex,
                             transform.position,
                             null,
                             Color.White,
                             0f,
                             Vector2.Zero,
                             Vector2.One / new Vector2(tex.Width, tex.Height),
                             spriteEffects,
                             0f);
        }

        public virtual void ReportData(int id)
        {
            //Networking.SendMessage($"P/{id},{transform.position.X},{transform.position.Y}", NetDeliveryMethod.UnreliableSequenced);

            if (transform.position != transform.LastPosition)
            {
                var msg = Networking.CreateMessage();

                msg.Write((byte)GameScreen.ServerHeaders.POSITION);
                msg.Write(id);
                msg.Write(transform.position.X);
                msg.Write(transform.position.Y);

                Networking.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);
            }

            transform.PushPosition();
            
            if (tag != lastTag)
            {
                var msg = Networking.CreateMessage();

                msg.Write((byte)GameScreen.ServerHeaders.UPDATE_TAG);
                msg.Write(id);
                msg.Write(tag);

                Networking.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);

                
            }

            tag = lastTag;
        }

        

        public enum NPCType
        {
            UNKNOWN,
            FALLING_BLOCK,
            ITEM
        }

        public virtual NPCType GetNPCType() => NPCType.UNKNOWN;

        public static NPC FromNPCType(World world, NPCType type, string tag = "")
        {
            switch (type)
            {
                case NPCType.FALLING_BLOCK:
                    return new FallingBlock(world, new Tile(int.Parse(tag)));
                case NPCType.ITEM:
                    return new Item(world, new Tile(int.Parse(tag)));
                default:
                    return null;
            }
        }
    }
}
