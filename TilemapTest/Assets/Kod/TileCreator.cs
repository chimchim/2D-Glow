using System.Collections.Generic;

using ScriptableObjects;
using UnityEngine;

namespace Map
{
    public class TileCreator
    {
        public TileCreator(Tiles prefabsTiles, MapGeneratorSettings mgs)
        {
            PrefabTiles = prefabsTiles;
            Msg = mgs;
            Parent = new GameObject();
            Parent.name = "Tileparent";
            OutlinePool = new Stack<Transform>();
            BlockPool = new Stack<Transform>();
            for (int i = 0; i < 3000; i++)
                BlockPool.Push(CreateTileForPool(TileType.Block));
            for (int i = 0; i < 3000; i++)
                BlockPool.Push(CreateTileForPool(TileType.Outline));

        }

        public GameObject Parent;
        public MapGeneratorSettings Msg;
        public Tiles PrefabTiles;
        public List<GameObject> TileStack;
        public Stack<Transform> BlockPool;
        public Stack<Transform> OutlinePool;
        public TileData[,] Tilemap;

        public void CreateMapFromTiledata(TileData[,] perlinMap)
        {
            Tilemap = perlinMap;
        }

        public Transform PopBlock(TileType type)
        {
            switch (type)
            {
                case TileType.Block:
                    if (BlockPool.Count > 0)
                        return BlockPool.Pop();
                    return CreateTileForPool(TileType.Block);
                case TileType.Outline:
                    if (OutlinePool.Count > 0)
                        return OutlinePool.Pop();
                    return CreateTileForPool(TileType.Outline);
            }

            Debug.LogError("POP ERROR");
            return null;
        }

        public TileInUse NewTileInUse(TileData tile, TileCreator tc)
        {
            var type = tile.Type;
            Transform transform = null;
            if (type == TileType.Block)
                transform = tc.PopBlock(TileType.Block);
            if (type == TileType.Outline)
                transform = tc.PopBlock(TileType.Outline);

            var position = new Vector2(tile.Index.x, tile.Index.y);
            var block = TileInUse.Make(transform, tile, position);
            return block;
        }

        private Transform CreateTileForPool(TileType type)
        {
            var go = GameObject.Instantiate(PrefabTiles.TestTile, new Vector2(-10, -10), Quaternion.identity);
            go.transform.parent = Parent.transform;
            return go.transform;
        }

        private GameObject CreateTile(int x, int y, TileData[,] tiledata)
        {
            var td = tiledata[x, y];
            if (td != null)
            {
                var go = GameObject.Instantiate(PrefabTiles.TestTile, new Vector2(x, y),
                    Quaternion.identity);
                go.transform.parent = Parent.transform;
                TileStack.Add(go);
                return go;
            }

            return null;

        }
    }
}
