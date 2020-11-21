using Bloxx.Source.Map;
using Bloxx.Source.Menus;
using Bloxx.Source.NPCs;
using Lidgren.Network;
using Microsoft.Win32;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoExt;
using MonoNet;
using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source
{
    public class GameScreen : Component, ICanReceive
    {
        public GameScreen()
        {
            world = new World(64, 32);
            if (Networking.IsServer)
            {
                world.Generate();
            }

            Networking.OnConnect = () =>
            {
                Console.WriteLine("Connected!");

                if (Networking.IsServer)
                {
                    world.NPCs.Insert(1, new Player(world, new OtherInput(), false, Color.Blue));
                } else
                {
                    world.NPCs.Insert(1, new Player(world, new PlayerKeys(Keys.D, Keys.A, Keys.W, Keys.Space), true, Color.Blue));
                }
                
            };

            Networking.OnDisconnect = (string s) =>
            {
                Console.WriteLine("Disconnected...");
                Networking.Stop();
                Main.main.screen = new MainMenu();
            };
        }

        readonly World world;

        int tileID = 1;

        public override void Update(GameTime gameTime)
        {
            if (Input.InRectBounds(Main.GetRectangle))
            {
                if (Input.LeftMouseDown())
                {
                    Point mousePos = Vector2.Transform(Input.MousePosition.ToVector2(),
                                                   Matrix.Invert(World.matrix * world.camera.GetProjection())).ToPoint();

                    if (world.InBounds(mousePos.X, mousePos.Y))
                    {
                        world.SetTile(new Tile(tileID), mousePos.X, mousePos.Y, true, true);
                    }
                }
                else if (Input.RightMouseDown())
                {
                    Point mousePos = Vector2.Transform(Input.MousePosition.ToVector2(),
                                                   Matrix.Invert(World.matrix * world.camera.GetProjection())).ToPoint();

                    if (world.InBounds(mousePos.X, mousePos.Y))
                    {
                        world.SetTile(new Tile(0), mousePos.X, mousePos.Y, true, true);
                    }
                }
            }

            if (Input.KeyPressed(Keys.L) && tileID < TileData.tileData.Count - 1)
            {
                tileID++;
            } else if (Input.KeyPressed(Keys.K) && tileID > 1)
            {
                tileID--;
            }

            if (Input.KeyPressed(Keys.U))
            {
                Point mousePos = Vector2.Transform(Input.MousePosition.ToVector2(),
                                                   Matrix.Invert(World.matrix * world.camera.GetProjection())).ToPoint();

                DebugSaveTiles(0, 0, mousePos.X + 1, mousePos.Y + 1);
            }

            //if (tileID < 1) tileID = 1;
            //if (tileID > TileData.tileData.Count) tileID++;

            world.Update(gameTime);
        }

        void DebugSaveTiles(int dX, int dY, int dW, int dH)
        {
            using (var writer = new StreamWriter("C:/Users/Jason/Documents/Debug3.txt"))
            {
                writer.WriteLine(dW);
                writer.WriteLine(dH);

                for (int x = dX; x < dX + dW; x++)
                {
                    for (int y = dY; y < dY + dH; y++)
                    {
                        writer.WriteLine(world.GetTile(x, y).id);
                    }
                }
            }
        }

        static readonly Rectangle selectedTileRect = new Rectangle(Main.screenWidth - Tile.size, 0, Tile.size, Tile.size);

        public override void Show(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Lerp(Color.Cyan, Color.Black, ((world.camera.position.Y - 300) / (world.height * Tile.size / 4))));
            world.Show(gameTime, spriteBatch);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            //spriteBatch.DrawString(Main.MediumFont, $"{world.NPCs[0].transform.position/ Tile.size}", new Vector2(20, 20), Color.White);
            spriteBatch.Draw(TileData.tileData[tileID].texture, selectedTileRect, Color.White);
            spriteBatch.End();
        }

        public enum ServerHeaders
        {
            SET_TILE,
            POSITION,
            SEND_CHUNK,
            SPAWN_ENTITY,
            KILL_ENTITY,
            UPDATE_TAG
        }

        public enum ClientHeaders
        {
            SET_TILE,
            INPUT,
            CHUNK_REQUEST
        }

        public void ReceiveMessage(NetIncomingMessage msg)
        {
            if (Networking.IsServer)
            {
                ServerReceive(msg);
            }
            else
            {
                ClientReceive(msg);
            }

        }

        void ServerReceive(NetIncomingMessage msg)
        {
            ClientHeaders header = (ClientHeaders)msg.ReadByte();

            switch (header)
            {
                case ClientHeaders.SET_TILE:
                    int sX = msg.ReadInt32();
                    int sY = msg.ReadInt32();
                    int sSet = msg.ReadInt32();

                    world.SetTile(new Tile(sSet), sX, sY, false, true);

                    break;
                case ClientHeaders.INPUT:
                    int hor = msg.ReadInt32();
                    bool jump = msg.ReadBoolean();

                    OtherInput.hor = hor;
                    OtherInput.PushJump(jump);

                    break;
                case ClientHeaders.CHUNK_REQUEST:
                    int cX = msg.ReadInt32();
                    int cY = msg.ReadInt32();

                    var newMsg = Networking.CreateMessage();

                    newMsg.Write((byte)ServerHeaders.SEND_CHUNK);
                    newMsg.Write(cX);
                    newMsg.Write(cY);
                    newMsg.Write(world.chunks[cX, cY].ToBytes());

                    Networking.SendMessage(newMsg, NetDeliveryMethod.ReliableOrdered, 2);

                    break;
            }
        }

        void ClientReceive(NetIncomingMessage msg)
        {
            ServerHeaders header = (ServerHeaders)msg.ReadByte();

            switch (header)
            {
                case ServerHeaders.POSITION:
                    int id = msg.ReadInt32();

                    if (id > world.NPCs.Count - 1) return;

                    float pX = msg.ReadFloat();
                    float pY = msg.ReadFloat();

                    //string eTag = msg.ReadString();

                    //int pX = msg.ReadInt32();
                    //int pY = msg.ReadInt32();

                    world.NPCs[id].transform.position = new Vector2(pX, pY);
                    //world.NPCs[id].Tag = eTag;

                    break;
                case ServerHeaders.UPDATE_TAG:
                    int tID = msg.ReadInt32();
                    //string eTag = msg.ReadString();

                    //world.NPCs[tID].Tag = eTag;
                    world.NPCs[tID].UpdateData(msg);

                    break;
                case ServerHeaders.SET_TILE:
                    int sX = msg.ReadInt32();
                    int sY = msg.ReadInt32();
                    int sSet = msg.ReadInt32();

                    world.SetTile(new Tile(sSet), sX, sY, false);

                    break;
                case ServerHeaders.SEND_CHUNK:
                    int cX = msg.ReadInt32();
                    int cY = msg.ReadInt32();

                    world.chunks[cX, cY] = Chunk.BytesToChunk(msg.ReadBytes(Chunk.size * Chunk.size), cX, cY);

                    break;
                case ServerHeaders.SPAWN_ENTITY:
                    int sE = msg.ReadInt32();
                    float eX = msg.ReadFloat();
                    float eY = msg.ReadFloat();
                    string tag = msg.ReadString();

                    NPC npc = NPC.FromNPCType(world, (NPC.NPCType)sE, tag);
                    npc.transform.position = new Vector2(eX, eY);

                    world.NPCs.Add(npc);

                    break;
                case ServerHeaders.KILL_ENTITY:
                    int npcID = msg.ReadInt32();

                    world.NPCs.RemoveAt(npcID);
                    break;
            }
        }
    }
}
