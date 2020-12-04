using Bloxx.Source.Map.Biomes;
using Bloxx.Source.NPCs;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoCamera2D;
using MonoExt;
using MonoNet;
using NoiseTest;
using SharpHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Map
{
    public partial class World : Component
    {
        public int Left => 0;
        public int Right => width * Tile.size;
        public int Top => 0;
        public int Bottom => height * Tile.size;

        public int width, height;

        readonly int cW, cH;

        public World(int w, int h)
        {
            cW = w; cH = h;
            chunks = new Chunk[cW, cH];

            width = w * Tile.size; height = h * Tile.size;

            if (Networking.IsServer)
            {
                NPCs = new List<NPC>()
                {
                    new Player(this, new PlayerKeys(Keys.D, Keys.A, Keys.W, Keys.Space), true, Color.Red)
                };

                
            } else
            {
                NPCs = new List<NPC>()
                {
                    new Player(this, null, false, Color.Red)
                };
            }
            
        }

        readonly List<Biome> biomeList = new List<Biome>()
        {
            new Plains(),
            new Desert(),
            new Snow()
        };

        internal OpenSimplexNoise noise = new OpenSimplexNoise();

        internal static readonly Random rnd = new Random();

        public void Generate()
        {
            // Initialise chunks

            for (int x = 0; x < cW; x++)
            {
                for (int y = 0; y < cH; y++)
                {
                    chunks[x, y] = new Chunk(x, y);
                }
            }

            // Add biomes

            biomeList.Shuffle(Main.random);

            for (int x = 0; x < width; x++)
            {
                int b = (int)Math.Floor((float)x / width * biomeList.Count);
                biomeList[b].GenerateX(this, x);
            }

            for (int x = 0; x < width; x++)
            {
                int b = (int)Math.Floor((float)x / width * biomeList.Count);
                biomeList[b].LateGenerateX(this, x);
            }

            #region Ore Generation

            PlaceOres(29, 150, 250, 12);    // Amethyst
            PlaceOres(30, 350, 150, 24);    // Copper
            PlaceOres(31, 50, 350, 8);      // Diamond
            PlaceOres(32, 150, 250, 12);    // Emerald
            PlaceOres(33, 50, 300, 10);     // Gold
            PlaceOres(34, 150, 200, 12);    // Ruby
            PlaceOres(35, 150, 200, 12);    // Sapphire
            PlaceOres(36, 200, 150, 18);    // Silver

            #endregion

            #region Structure Generation

            bool stoneFunc(Tile tile) => tile.Data.category == TileData.Category.STONE;

            // J Shrine
            PlaceStructInArea(new Structure("JShrine"), height - 80, height - 20, stoneFunc);

            // Dungeons
            var dungeon = new Structure("Dungeon");

            for (int i = 0; i < 35; i++)
                PlaceStructInArea(dungeon, 200, height - 25, stoneFunc);

            #endregion
        }

        public void PlaceOres(int tileID, int amount, int level, int maxLength)
        {
            for (int i = 0; i < amount; i++)
            {
                int eX, eY;

                while (true)
                {
                    eX = rnd.Next(width);
                    eY = rnd.Next(level, height);

                    if (GetTile(eX, eY).id != 1)
                    {
                        continue;
                    }

                    break;
                }

                int maxLen = rnd.Next(1, maxLength);

                var points = new Stack<Point>();

                bool neighbourStone(int sX, int sY)
                {
                    if (TileHasID(1, sX, sY - 1)) return true;
                    if (TileHasID(1, sX + 1, sY)) return true;
                    if (TileHasID(1, sX, sY + 1)) return true;
                    if (TileHasID(1, sX - 1, sY)) return true;

                    return false;
                }

                for (int j = 0; j < maxLen; j++)
                {
                    SetTile(new Tile(tileID), eX, eY, false);
                    points.Push(new Point(eX, eY));

                    while (!neighbourStone(eX, eY) && points.Count != 0)
                    {
                        Point p = points.Pop();

                        eX = p.X; eY = p.Y;
                    }

                    if (points.Count == 0) break;

                    List<Point> nextPos = new List<Point>()
                    {
                        new Point(eX, eY - 1),
                        new Point(eX + 1, eY),
                        new Point(eX, eY + 1),
                        new Point(eX - 1, eY)
                    };

                    nextPos.Shuffle(Main.random);

                    foreach (Point p in nextPos)
                    {
                        if (TileHasID(1, p.X, p.Y))
                        {
                            eX = p.X; eY = p.Y;
                            break;
                        }
                    }

                    if (points.Count == 0) continue;
                }
            }
        }

        public void PlaceCalmOres(int tileID, int amount, int level, int maxLength)
        {
            for (int i = 0; i < amount; i++)
            {
                int eX, eY;

                while (true)
                {
                    eX = rnd.Next(width);
                    eY = rnd.Next(level, height);

                    if (GetTile(eX, eY).id != 1)
                    {
                        continue;
                    }

                    break;
                }

                int maxLen = rnd.Next(1, maxLength);

                var points = new List<Point>();

                int neighboursStone(int sX, int sY)
                {
                    int num = 0;

                    if (TileHasID(1, sX, sY - 1)) num++;
                    if (TileHasID(1, sX + 1, sY)) num++;
                    if (TileHasID(1, sX, sY + 1)) num++;
                    if (TileHasID(1, sX - 1, sY)) num++;

                    return num;
                }

                for (int j = 0; j < maxLen; j++)
                {
                    SetTile(new Tile(tileID), eX, eY, false);
                    points.Add(new Point(eX, eY));

                    bool once = false;

                    while (!once || neighboursStone(eX, eY) == 0 && points.Count != 0)
                    {
                        int r = rnd.Next(points.Count);
                        Point p = points[r];

                        points.RemoveAt(r);

                        eX = p.X; eY = p.Y;

                        once = true;
                    } 

                    if (neighboursStone(eX, eY) == 0 && points.Count == 0) continue;

                    List<Point> nextPos = new List<Point>()
                    {
                        new Point(eX, eY - 1),
                        new Point(eX + 1, eY),
                        new Point(eX, eY + 1),
                        new Point(eX - 1, eY)
                    };

                    nextPos.Shuffle(Main.random);

                    Point p1 = new Point(0);
                    int greatest = 0;
                    
                    foreach (Point p2 in nextPos)
                    {
                        int k;
                        if ((k = neighboursStone(p2.X, p2.Y)) > greatest)
                        {
                            p1 = p2;
                            greatest = k;
                        }
                    }

                    eX = p1.X; eY = p1.Y;

                    if (points.Count == 0) continue;
                }
            }
        }


        bool TileHasID(int id, int x, int y) =>
            InBounds(x, y) && GetTile(x, y).id == id;

        void PlaceStructInArea(Structure s, int minHeight, int maxHeight, Func<Tile, bool> func)
        {
            beginning:

            int x = Main.random.Next(5, width - s.Width - 5),
                //y = Main.random.Next(height - 80, height - 20);
                y = Main.random.Next(minHeight, maxHeight);
            
            for (int j = x - 1; j < x + s.Width + 2; j++)
            {
                for (int k = y - 1; k < y + s.Height + 2; k++)
                {
                    //if (GetTile(j, k).Data.category == TileData.Category.NONE) goto beginning;
                    if (!func(GetTile(j, k))) goto beginning;
                }
            }

            s.Place(this, x, y);
        }

        

        public bool InBounds(int x, int y) =>
            x >= 0 && x < width && y >= 0 && y < height;

        public Chunk[,] chunks;

        public List<Point> toUpdate = new List<Point>();

        public void SpawnEntity(NPC npc)
        {
            var msg = Networking.CreateMessage();
            msg.Write((byte)GameScreen.ServerHeaders.SPAWN_ENTITY);
            
            msg.Write((int)npc.GetNPCType());
            msg.Write(npc.transform.position.X);
            msg.Write(npc.transform.position.Y);
            msg.Write(npc.Tag);

            Networking.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);

            NPCs.Add(npc);
        }

        public void KillEntity(int id)
        {
            var msg = Networking.CreateMessage();

            msg.Write((byte)GameScreen.ServerHeaders.KILL_ENTITY);

            msg.Write(id);
            Networking.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 1);

            NPCs.RemoveAt(id);
        }

        public ref Tile GetTile(int x, int y)
        {
            return ref chunks[x / Chunk.size, y / Chunk.size]
                     .GetTile(x % Chunk.size, y % Chunk.size);
        }

        public void SetTile(Tile tile, int x, int y, bool report = true, bool update = false)
        {
            if (!InBounds(x, y)) return;

            var chunk = chunks[x / Chunk.size, y / Chunk.size];

            if (chunk == null) return;


            /*
            Tile thisTile = GetTile(x, y); 
            
            if (Networking.IsServer && report && tile.id == 0 && thisTile.id != 0)
            {
                var item = new Item(this, thisTile);

                item.transform.position = new Vector2((x + 0.25f) * Tile.size, (y + 0.25f) * Tile.size);

                SpawnEntity(item);
            }
            */
            

            chunk.SetTile(tile, x % Chunk.size, y % Chunk.size);

            if (Networking.IsServer && update)
            {
                toUpdate.Add(new Point(x, y));
                UpdateOthers(x, y);
            }
            
            
            if (report)
            {
                //Networking.SendMessage($"ST/{x},{y},{tile.id}");
                var msg = Networking.CreateMessage();

                msg.Write((byte)GameScreen.ServerHeaders.SET_TILE);
                msg.Write(x);
                msg.Write(y);
                msg.Write(tile.id);

                Networking.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }

        public void UpdateOthers(int x, int y)
        {
            void UpdateBlock(int j, int k)
            {
                if (InBounds(j, k)) toUpdate.Add(new Point(j, k));
            }

            UpdateBlock(x + 1, y);
            UpdateBlock(x, y + 1);
            UpdateBlock(x - 1, y);
            UpdateBlock(x, y - 1);
        }

        public bool TileIsType(int x, int y, TileData.Solidity solidity)
        {
            return InBounds(x, y) && GetTile(x, y).Data.solidity == solidity;
        }

        public Camera2D camera = new Camera2D(Main.screenWidth, Main.screenHeight);

        public List<NPC> NPCs;

        public static readonly Matrix matrix = Matrix.CreateScale(Tile.size);

        public override void Update(GameTime gameTime)
        {
            // Block updates
            for (int i = toUpdate.Count - 1; i >= 0; i--)
            {
                Point p = toUpdate[i];
                //Console.WriteLine($"{p.X}, {p.Y}");

                GetTile(p.X, p.Y).Data.behaviour?.BlockUpdate(this, p.X, p.Y);
                toUpdate.RemoveAt(i);
            }

            // Update NPCs
            for (int i = NPCs.Count - 1; i >= 0; i--)
            {
                NPC npc = NPCs[i];

                if (Networking.IsServer)
                {
                    npc.Update(gameTime);
                    npc.ReportData(i);
                    if (npc.Finished) KillEntity(i);
                } else
                {
                    npc.ClientUpdate(gameTime);
                }
            }
            
            // Camera
            if (Input.GetScrollDelta() > 0)
            {
                camera.Zoom += 0.1f;
            } else if (Input.GetScrollDelta() < 0)
            {
                camera.Zoom -= 0.1f;
            }

            if (camera.Zoom > 2) camera.Zoom = 2;
            else if (camera.Zoom < 1) camera.Zoom = 1f;

            BoundCamera(camera);
        }

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix:matrix * camera.GetProjection());

            // Draw tiles
            int minX = (int)Math.Max(0, (camera.Left) / Tile.size);
            int maxX = (int)Math.Min((camera.Right) / Tile.size, (width * Tile.size) / Tile.size - 1);

            int minY = (int)Math.Max(0, (camera.Top) / Tile.size);
            int maxY = (int)Math.Min((camera.Bottom) / Tile.size, (height * Tile.size) / Tile.size - 1);


            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    int cX = x / Chunk.size;
                    int cY = y / Chunk.size;

                    if (chunks[cX, cY] != null)
                    {
                        GetTile(x, y).Draw(spriteBatch, x, y);
                    }
                    else
                    {
                        Chunk.RequestChunk(cX, cY);
                    }
                }
            }

            // Draw all entities
            foreach (NPC npc in NPCs)
            {
                if (npc.transform.InCameraBounds(camera))
                    npc.Show(gameTime, spriteBatch);

            }
            //NPCs.ForEach(x => x.Show(gameTime, spriteBatch));

            spriteBatch.End();
        }

        public void BoundCamera(Camera2D camera)
        {
            Vector2 newPos = camera.position;

            //newPos.X = Math.Max(newPos.X, Left + camera.RelativeSize.X / 2);
            //newPos.X = Math.Min(newPos.X, Right - camera.RelativeSize.X / 2);

            //newPos.Y = Math.Max(newPos.Y, Top + camera.RelativeSize.Y / 2);
            //newPos.Y = Math.Min(newPos.Y, Bottom - camera.RelativeSize.Y / 2);

            newPos.X = newPos.X.Constrain(Left + camera.RelativeSize.X / 2, Right - camera.RelativeSize.X / 2);
            newPos.Y = newPos.Y.Constrain(Top + camera.RelativeSize.Y / 2, Bottom - camera.RelativeSize.Y / 2);

            camera.position = newPos;
        }

        public void BoundEntity(NPC entity)
        {
            ref Vector2 newPos = ref entity.transform.position;

            //newPos.X = Math.Max(newPos.X, Left);
            //newPos.X = Math.Min(newPos.X, Right - entity.transform.size.X);

            //newPos.Y = Math.Max(newPos.Y, Top);
            //newPos.Y = Math.Min(newPos.Y, Bottom - entity.transform.size.Y);

            newPos.X = newPos.X.Constrain(Left, Right - entity.transform.size.X);
            newPos.Y = newPos.Y.Constrain(Top, Bottom - entity.transform.size.Y);
        }
    }
}
