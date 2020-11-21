using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Map
{
    public struct Tile
    {
        public enum TileEnum
        {
            AIR,
            STONE,
            GRASS,
            DIRT
        }

        public Tile(int _id)
        {
            id = _id;
            //data = TileData.tileData[id];
        }

        public int id;
        public TileData Data => TileData.tileData[id];

        public const int size = 16;

        public void Draw(SpriteBatch spriteBatch, int x, int y, float l = 1f)
        {
            Texture2D tex = Data.texture;

            if (tex != null)
            {
                spriteBatch.Draw(tex,
                             new Vector2(x, y),
                             null,
                             Color.White,
                             0f,
                             Vector2.Zero,
                             Vector2.One / 8,
                             SpriteEffects.None,
                             0f);
            }
                
                //spriteBatch.Draw(tex, new Rectangle(x * size, y * size, size, size), Color.Lerp(Color.Black, Color.White, l));
        }
    }
}
