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
            Colliders = new Stack<Transform>();
            for (int i = 0; i < 300; i++)
                Colliders.Push(CreateTileForPool());
        }

        public GameObject Parent;
        public MapGeneratorSettings Msg;
        public Tiles PrefabTiles;
        public List<GameObject> TileStack;
        public Stack<Transform> Colliders;
        public TileData[,] Tilemap;

        public void CreateMapFromTiledata(TileData[,] perlinMap)
        {
            Tilemap = perlinMap;
        }


        public Transform PopTransform(Vector2Int position)
        {
            if(Colliders.Count == 0)
                Colliders.Push(CreateTileForPool());
            var t = Colliders.Pop();
            t.position = new Vector3(position.x, position.y);
            t.name = $"x:{position.x}, y:{position.y}";
            return t;
        }
        public void RecycleCollider(Transform t)
        {
            t.position = new Vector3(-10, -10);
            Colliders.Push(t);
        }
        private Transform CreateTileForPool()
        {
            var go = GameObject.Instantiate(PrefabTiles.Collider, new Vector2(-10, -10), Quaternion.identity);
            go.transform.parent = Parent.transform;
            return go.transform;
        }

        private GameObject CreateTile(int x, int y, TileData[,] tiledata)
        {
            var td = tiledata[x, y];
            if (td != null)
            {
                var go = GameObject.Instantiate(PrefabTiles.Collider, new Vector2(x, y),
                    Quaternion.identity);
                go.transform.parent = Parent.transform;
                TileStack.Add(go);
                return go;
            }

            return null;

        }
    }
}
