using System;
using System.Collections.Generic;
using Assets.Kod.Misc;
using JetBrains.Annotations;
using Map;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gameloop.Systems
{
    public class UpdateMapTiles 
    {
        public Dictionary<int, SquareCorners> GraphicBuild = new Dictionary<int, SquareCorners>(4);
        public Dictionary<int, SquareCorners> ColliderBuild = new Dictionary<int, SquareCorners>(4);
        public bool[,] Tiles;
        List<Vector2Int> ToRemove = new List<Vector2Int>();
        List<Vector2Int> ToRemoveColliders = new List<Vector2Int>();
        public List<UpdateMap> GraphicMaps = new List<UpdateMap>(4);
        public List<UpdateMap> ColliderMaps = new List<UpdateMap>(4);
        public UpdateMap GraphicMap;
        public UpdateMap ColliderMap;
        private Tilemap Tilemap;
        public UpdateMapTiles(Tilemap tilemap)
        {
            Tilemap = tilemap;
        }
        public void Initiate(GameManager game, Vector2Int startpos)
        {

            var mg = game.MapGenerator;

            #region Init Graphic/Collider maps

            Tiles = new bool[mg.FullWidth, mg.FullHeight];
            GraphicMap = new UpdateMap(startpos, game.MapGeneratorSettings.ClientSize);
            ColliderMap = new UpdateMap(startpos, game.MapGeneratorSettings.ColliderSize);
            ColliderMap.InitiateMap(game);
            GraphicMap.InitiateMap(game);
            GraphicBuild.Add(0, GraphicMap.LatestBuildBox);
            ColliderBuild.Add(0, ColliderMap.LatestBuildBox);
            GraphicMaps.Add(GraphicMap);
            ColliderMaps.Add(ColliderMap);
            #endregion
            AddAll(game);
        }
        public void Update(GameManager game)
        {
            ColliderMap.UpdateTiles(game);
            GraphicMap.UpdateTiles(game);
            GraphicBuild[0] = GraphicMap.LatestBuildBox;
            ColliderBuild[0] = ColliderMap.LatestBuildBox;

            RemoveAll();
            AddAll(game);
            GraphicMap.Draw(ColliderMap.LatestBuildBox);
        }
        public void AddAll(GameManager game)
        {
            Add(game);
        }
        public void RemoveAll()
        {
            ToRemove.Clear();
            Remove();

            for (int i = 0; i < ToRemove.Count; i++)
            {
                var removeAt = ToRemove[i];
                Tiles[removeAt.x, removeAt.y] = false;
                Tilemap.SetTile(new Vector3Int(removeAt.x, removeAt.y, 0), null);
                //tile.Recycle(game.MapGenerator.TileCreator); //TODO RECYCLE
            }
        }
        public void Remove()
        {
            for (int i = 0; i < GraphicMap.ToRemove.Count; i++)
            {
                var toRemove = GraphicMap.ToRemove[i];
                if (toRemove.Horizontal)
                {
                    for (int x = 0; x < toRemove.Diff + 1; x++)
                    {
                        var tile2 = Tiles[toRemove.First.x + x, toRemove.Second.y];
                        CheckTile(tile2, new Vector2Int(toRemove.First.x + x, toRemove.Second.y));
                    }
                }
                else
                {
                    for (int y = 0; y < toRemove.Diff + 1; y++)
                    {
                        var tile = Tiles[toRemove.First.x, toRemove.First.y + y];
                        CheckTile(tile, new Vector2Int(toRemove.First.x, toRemove.First.y + y));
                    }
                }
                void CheckTile(bool tile, Vector2Int index)
                {
                    if (tile)
                    {
                        bool add = false;
                        foreach (var buildPair in GraphicBuild)
                        {
                            add = CheckContains(buildPair.Value, index) || add;
                        }
                        if (!add)
                            ToRemove.Add(index);
                    }
                }
            }
            GraphicMap.ToRemove.Clear();
        }
 
        public void Add(GameManager game)
        {
            for (int i = 0; i < GraphicMap.ToAdd.Count; i++)
            {
                var toAdd = GraphicMap.ToAdd[i];
                if (toAdd.Horizontal)
                {
                    for (int x = 0; x < toAdd.Diff + 1; x++)
                    {
                        var ci = new Vector2Int(toAdd.First.x + x, toAdd.Second.y);
                        Add(ci);
                    }
                }
                else
                {
                    for (int y = 0; y < toAdd.Diff + 1; y++)
                    {
                        var ci = new Vector2Int(toAdd.First.x, toAdd.First.y + y);
                        Add(ci);
                    }
                }

                void Add(Vector2Int ci)
                {
                    var fromPerlin = game.Tilemap[ci.x, ci.y];
                    if (fromPerlin == null)
                        return;
                    Tilemap.SetTile(new Vector3Int(ci.x, ci.y, 0), game.Tiles.RuleTile);
                    Tiles[ci.x, ci.y] = true;
                }
            }
            GraphicMap.ToAdd.Clear();
        }


        public bool CheckContains(SquareCorners otherBuild, Vector2Int ci)
        {
            var x = ci.x >= otherBuild.BotLeft.x && ci.x <= otherBuild.BotRight.x;
            var y = ci.y >= otherBuild.BotLeft.y && ci.y <= otherBuild.TopRight.y;
            return x && y;
        }



    }
}