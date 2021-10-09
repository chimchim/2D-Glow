using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map;
using ScriptableObjects;

namespace Assets.Kod.Misc
{
    public class GameManager
    {
        public GameManager(MapGenerator mg, MapGeneratorSettings mgs, Tiles tiles)
        {
            Tiles = tiles;
            MapGenerator = mg;
            MapGeneratorSettings = mgs;
        }

        public Tiles Tiles;
        public MapGeneratorSettings MapGeneratorSettings;
        public MapGenerator MapGenerator;
        public TileData[,] Tilemap => MapGenerator.TileCreator.Tilemap;
    }
}
