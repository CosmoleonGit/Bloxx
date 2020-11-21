using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Map.Biomes
{
    public class Snow : Biome
    {
        public override void GenerateX(World world, int x)
        {
            for (int y = 31 + (int)(world.noise.Evaluate((double)x / 15, (double)x / 15) * 5) - 5; y < 120; y++)
            {
                world.SetTile(new Tile(3), x, y);
            }

            for (int y = 120 + (int)(world.noise.Evaluate((double)x / 15, (double)x / 15) * 3) - 1; y < world.height; y++)
            {
                world.SetTile(new Tile(1), x, y, false);
            }

            base.GenerateX(world, x);
        }

        protected override void PlaceGrass(World world, int x, int y)
        {
            world.SetTile(new Tile(26), x, y, false);
            if (World.rnd.Next(4) > 1)
                world.SetTile(new Tile(16), x, y - 1, false);
        }

        

        public override void LateGenerateX(World world, int x)
        {
            if (treeDelay == 0 && World.rnd.Next(10) < 1 && x > 10 && x < world.width - 10)
            {
                treeDelay = 7;

                for (int y = 0; y < world.height; y++)
                {
                    if (world.GetTile(x, y).id == 26)
                    {
                        tree.Place(world, x - 2, y - 8);

                        break;
                    }
                }

            }
            else if (treeDelay > 0)
            {
                treeDelay--;
            }
        }
    }
}
