using Bloxx.Source.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.NPCs
{
    public class Item : NPC
    {
        Tile block;
        public Item(World world, Tile _block) : base(world, _block.id.ToString())
        {
            block = _block;
            transform.size = new Vector2(Tile.size / 2);
        }

        protected override Texture2D GetTexture()
        {
            return block.Data.texture;
        }

        public override void Update(GameTime gameTime)
        {
            transform.Gravitate();

            if (transform.position.Y > world.Bottom)
            {
                Finished = true;
            }

            base.Update(gameTime);
        }

        public override NPCType GetNPCType() => NPCType.ITEM;
    }
}
