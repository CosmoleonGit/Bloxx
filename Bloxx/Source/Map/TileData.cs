using Bloxx.Source.Map.BlockBehaviour;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Map
{
    public class TileData
    {
        public TileData(string _name, Solidity _solidity, int _id, ContentManager content, TileBehaviour _behaviour = null, Category _category = Category.NONE)
        {
            name = _name;
            id = _id;
            
            if (File.Exists("Content/Tiles/" + name + ".xnb"))
            {
                texture = content.Load<Texture2D>("Tiles/" + _name);
            }

            solidity = _solidity;

            behaviour = _behaviour;

            category = _category;
        }

        public static List<TileData> tileData;

        public readonly string name;
        public readonly Texture2D texture;
        public readonly Solidity solidity;
        public readonly int id;

        public readonly TileBehaviour behaviour;

        public readonly Category category;
        
        public enum Category
        {
            NONE,
            STONE
        }

        public enum Solidity
        {
            AIR,
            SOLID,
            SEMISOLID
        }

        public static void LoadTiles(ContentManager content)
        {
            tileData = new List<TileData>()
            {
                new TileData("air", Solidity.AIR, 0, content),
                new TileData("stone_block", Solidity.SOLID, 1, content, null, Category.STONE),
                new TileData("grass_block", Solidity.SOLID, 2, content),
                new TileData("dirt_block", Solidity.SOLID, 3, content),
                new TileData("bricks", Solidity.SOLID, 4, content),
                new TileData("cactus", Solidity.AIR, 5, content, new BaseBehaviour()),
                new TileData("cloud_block", Solidity.SOLID, 6, content),
                new TileData("fence", Solidity.AIR, 7, content),
                new TileData("glass", Solidity.SOLID, 8, content),
                new TileData("leaves", Solidity.AIR, 9, content),
                new TileData("pebble", Solidity.AIR, 10, content, new BaseBehaviour()),
                new TileData("planks", Solidity.SOLID, 11, content),
                new TileData("platform", Solidity.SEMISOLID, 12, content),
                new TileData("sand", Solidity.SOLID, 13, content, new SandBehaviour()),
                new TileData("sandstone", Solidity.SOLID, 14, content),
                new TileData("snow_block", Solidity.SOLID, 15, content),
                new TileData("snow_layer", Solidity.AIR, 16, content, new BaseBehaviour()),
                new TileData("stone_bricks", Solidity.SOLID, 17, content),
                new TileData("wood", Solidity.AIR, 18, content),
                new TileData("vines", Solidity.AIR, 19, content, new VineBehaviour()),
                new TileData("gravel", Solidity.SOLID, 20, content, new SandBehaviour()),
                new TileData("ice", Solidity.SOLID, 21, content),
                new TileData("ice_bricks", Solidity.SOLID, 22, content),
                new TileData("abysstone", Solidity.SOLID, 23, content),
                new TileData("pulsing_abysstone", Solidity.SOLID, 24, content),
                new TileData("grass", Solidity.AIR, 25, content, new BaseBehaviour()),
                new TileData("snowy_grass_block", Solidity.SOLID, 26, content),
                new TileData("gravel", Solidity.SOLID, 27, content),
                new TileData("j", Solidity.SOLID, 28, content),
                new TileData("amethyst_ore", Solidity.SOLID, 29, content, null, Category.STONE),
                new TileData("copper_ore", Solidity.SOLID, 30, content, null, Category.STONE),
                new TileData("diamond_ore", Solidity.SOLID, 31, content, null, Category.STONE),
                new TileData("emerald_ore", Solidity.SOLID, 32, content, null, Category.STONE),
                new TileData("gold_ore", Solidity.SOLID, 33, content, null, Category.STONE),
                new TileData("ruby_ore", Solidity.SOLID, 34, content, null, Category.STONE),
                new TileData("sapphire_ore", Solidity.SOLID, 35, content, null, Category.STONE),
                new TileData("silver_ore", Solidity.SOLID, 36, content, null, Category.STONE),
                new TileData("cobblestone", Solidity.SOLID, 37, content),
                new TileData("blue_flower_vines", Solidity.AIR, 38, content, new VineBehaviour()),
                new TileData("pink_flower_vines", Solidity.AIR, 39, content, new VineBehaviour()),
                new TileData("purple_flower_vines", Solidity.AIR, 40, content, new VineBehaviour())
            };
        }
    }
}
