using UnityEngine;

namespace Map
{
    public class TileData
    {
        public TileData(Vector2Int index)
        {
            Index = index;
        }

        public Vector2Int Index;
        public TileNeighbours Neighbours;
        public TileType Type;
    }
}