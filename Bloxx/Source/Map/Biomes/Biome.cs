using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bloxx.Source.Map.Biomes
{
    public abstract class Biome
    {
        protected static Structure tree;
        protected int treeDelay = 0;

        public virtual void GenerateX(World world, int x)
        {
            if (tree == null)
                tree = new Structure("C:/Users/Jason/Documents/Debug3.txt");

            int vine = 0;
            bool flowering = false;

            for (int y = 0; y < world.height; y++)
            {
                // Generate caves
                if (world.noise.Evaluate((double)x / 15, (double)y / 15) > 0.3f)
                {
                    world.SetTile(new Tile(0), x, y, false);
                }
                
                if (y != 0)
                {
                    // Grass
                    if (world.GetTile(x, y).id == 3 && world.GetTile(x, y - 1).id == 0)
                    {
                        PlaceGrass(world, x, y);
                        if (World.rnd.Next(10) < 1)
                            world.SetTile(new Tile(25), x, y - 1, false);
                    }

                    // Vines
                    if (world.GetTile(x, y - 1).id == 1 || world.GetTile(x, y - 1).id == 3)
                    {
                        vine = Math.Max(0, Main.random.Next(-3, 5));
                        if (world.GetTile(x, y - 1).id == 3) flowering = true;
                    }

                    if (world.GetTile(x, y).id != 0)
                    {
                        vine = 0;
                        flowering = false;
                    }
                    else if (vine > 0)
                    {
                        vine--;

                        if (flowering)
                        {
                            int r = World.rnd.Next(10);
                            if (r == 1)
                            {
                                world.SetTile(new Tile(38), x, y, false);
                            }
                            else if (r == 2)
                            {
                                world.SetTile(new Tile(39), x, y, false);
                            }
                            else if (r == 3)
                            {
                                world.SetTile(new Tile(40), x, y, false);

                            } else
                            {
                                world.SetTile(new Tile(19), x, y, false);
                            }
                        } else
                        {
                            world.SetTile(new Tile(19), x, y, false);
                        }
                        
                    }

                    // Pebbles
                    if (world.GetTile(x, y).id == 1 && world.GetTile(x, y - 1).id == 0 && World.rnd.Next(10) < 1)
                    {
                        world.SetTile(new Tile(10), x, y - 1, false);
                    }
                }
            }

            for (int y = world.height - 3 + (int)(world.noise.Evaluate((double)x / 15, (double)x / 15) * 3); y < world.height; y++)
            {
                world.SetTile(new Tile(23), x, y, false);
            }
        }

        public virtual void LateGenerateX(World world, int x) { }
        
        protected virtual void PlaceGrass(World world, int x, int y)
        {
            world.SetTile(new Tile(2), x, y, false);
        }
    }
}
