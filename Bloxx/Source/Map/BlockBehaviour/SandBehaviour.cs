using Bloxx.Source.NPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Map.BlockBehaviour
{
    class SandBehaviour : TileBehaviour
    {
        public override void BlockUpdate(World world, int x, int y)
        {
            if (world.InBounds(x, y + 1) && world.GetTile(x, y + 1).id == 0)
            {
                var fB = new FallingBlock(world, world.GetTile(x, y));
                fB.transform.position = new Vector2(x, y);

                //world.NPCs.Add(fB);
                world.SpawnEntity(fB);

                world.SetTile(new Tile(0), x, y, true, true);

                //world.UpdateOthers(x, y);
            }

            
        }
    }
}
