using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet;
using System;
using System.Collections.Generic;

namespace Bloxx.Source.Map
{
    public class Chunk : IConvertable
    {
        public Chunk(int x, int y)
        {
            requested.Remove(new Point(x, y));
        }

        internal const int size = 16;
        readonly Tile[,] tiles = new Tile[size, size];

        public byte[] ToBytes()
        {
            byte[] byteArr = new byte[size * size];

            int i = 0;


            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++, i++)
                {
                    byteArr[i] = (byte)tiles[x, y].id;
                }
            }

            return byteArr;
        }

        public static Chunk BytesToChunk(byte[] bytes, int x, int y)
        {
            Chunk newChunk = new Chunk(x, y);

            int i = 0;
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++, i++)
                {
                    newChunk.tiles[j, k] = new Tile(bytes[i]);
                }
            }

            return newChunk;
        }

        public ref Tile GetTile(int x, int y) => ref tiles[x, y];
        public void SetTile(Tile tile, int x, int y) => tiles[x, y] = tile;

        static readonly List<Point> requested = new List<Point>();

        public static void RequestChunk(int x, int y)
        {
            if (!Networking.Connected) return;

            Point p = new Point(x, y);

            if (requested.Contains(p)) return;

            requested.Add(p);

            var requestMsg = Networking.CreateMessage();

            requestMsg.Write((byte)GameScreen.ClientHeaders.CHUNK_REQUEST);
            requestMsg.Write(x);
            requestMsg.Write(y);

            Networking.SendMessage(requestMsg, NetDeliveryMethod.ReliableOrdered, 2);
        }



        public void Draw(SpriteBatch spriteBatch, int x, int y)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    tiles[j, k].Draw(spriteBatch, x, y);
                    //tiles[j, k].Draw(spriteBatch, x, y, lightValues[j, k]);
                }
            }
        }

        float[,] lightValues = new float[size, size];

        public void CalculateLight(World world)
        {

        }
    }
}
