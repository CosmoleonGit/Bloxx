using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Map.Biomes
{
    public class Desert : Biome
    {
        public override void GenerateX(World world, int x)
        {
            for (int y = 31 + (int)(world.noise.Evaluate((double)x / 15, (double)x / 15) * 5) - 5; y < 120; y++)
            {
                world.SetTile(new Tile(13), x, y, false);
            }

            for (int y = 120 + (int)(world.noise.Evaluate((double)x / 15, (double)x / 15) * 3) - 1; y < world.height; y++)
            {
                world.SetTile(new Tile(1), x, y, false);
            }

            base.GenerateX(world, x);

            int r = World.rnd.Next(1, 20);

            if (r <= 3)
            {
                int sY = 0;
                for (int y = 0; y < world.height; y++)
                {
                    int id = world.GetTile(x, y).id;

                    if (id == 13)
                    {
                        sY = y;
                        break;
                    }
                }

                for (int i = 0; i < r; i++)
                {
                    world.SetTile(new Tile(5), x, sY - i - 1, false);
                }
            }


        }
    }
}
