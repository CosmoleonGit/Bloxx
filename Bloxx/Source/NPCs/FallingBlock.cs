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
    public class FallingBlock : NPC
    {
        readonly Tile block;
        public FallingBlock(World world, Tile _block) : base(world, _block.id.ToString())
        {
            block = _block;
            transform.size = Vector2.One;
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

            if (transform.grounded)
            {
                int x = (int)transform.position.X;
                int y = (int)transform.position.Y;

                world.SetTile(block, x, y, true, true);

                Finished = true;
            }

            base.Update(gameTime);
        }

        public override NPCType GetNPCType()
        {
            return NPCType.FALLING_BLOCK;
        }
    }
}
