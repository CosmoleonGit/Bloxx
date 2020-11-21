using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Map.BlockBehaviour
{
    class VineBehaviour : TileBehaviour
    {
        public override void BlockUpdate(World world, int x, int y)
        {
            if (world.InBounds(x, y - 1) && world.GetTile(x, y - 1).id == 0)
            {
                world.SetTile(new Tile(0), x, y, true, true);
            }


        }
    }
}
