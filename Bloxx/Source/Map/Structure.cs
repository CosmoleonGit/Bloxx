using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Map
{
    public class Structure
    {
        public Structure(int w, int h)
        {
            tiles = new Tile?[w, h];
        }

        public Structure(string path)
        {
            using (var reader = new StreamReader(path))
            {
                int w = int.Parse(reader.ReadLine());
                int h = int.Parse(reader.ReadLine());

                tiles = new Tile?[w, h];

                for (int j = 0; j < w; j++)
                {
                    for (int k = 0; k < h; k++)
                    {
                        int id = int.Parse(reader.ReadLine());

                        if (id != -1)
                            SetTile(new Tile(id), j, k);
                    }
                }
            }
        }

        public int Width => tiles.GetUpperBound(0) + 1;
        public int Height => tiles.GetUpperBound(1) + 1;

        readonly Tile?[,] tiles;

        public ref Tile? GetTile(int x, int y) => ref tiles[x, y];
        public void SetTile(Tile set, int x, int y) => tiles[x, y] = set;

        public void Place(World world, int x, int y)
        {
            for (int j = 0; j < Width; j++)
            {
                for (int k = 0; k < Height; k++)
                {
                    Tile? tile = tiles[j, k];

                    if (tile.HasValue)
                        world.SetTile(tile.Value, x + j, y + k, false);
                }
            }
        }
    }
}
