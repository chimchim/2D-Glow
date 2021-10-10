using System;
using System.Collections.Generic;
using Assets.Kod.Misc;
using JetBrains.Annotations;
using Map;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = Map.TileData;

namespace Gameloop.Systems
{
    public class UpdateMapTiles 
    {
        public Dictionary<int, SquareCorners> GraphicBuilds = new Dictionary<int, SquareCorners>(4);
        public Dictionary<int, SquareCorners> ColliderBuilds = new Dictionary<int, SquareCorners>(4);
        public bool[,] ActiveGrahics;
        public bool[,] ActiveColliders;
        public Transform[,] ActiveCollidersTransforms;
        List<Vector2Int> ToRemoveGraphics = new List<Vector2Int>();
        List<Vector2Int> ToRemoveColliders = new List<Vector2Int>();
        List<Vector2Int> ToAddGraphics = new List<Vector2Int>();
        List<Vector2Int> ToAddColliders = new List<Vector2Int>();
        public List<UpdateMap> ColliderMaps = new List<UpdateMap>(4);
        public UpdateMap GraphicMap;
        //public UpdateMap ColliderMap;
        private Tilemap Tilemap;
        public UpdateMapTiles(Tilemap tilemap)
        {
            Tilemap = tilemap;
        }
        public void Initiate(GameManager game, Vector2Int startpos)
        {

            var mg = game.MapGenerator;

            #region Init Graphic/Collider maps

            ActiveGrahics = new bool[mg.FullWidth, mg.FullHeight];
            ActiveColliders = new bool[mg.FullWidth, mg.FullHeight];
            ActiveCollidersTransforms = new Transform[mg.FullWidth, mg.FullHeight];
            GraphicMap = new UpdateMap(startpos, game.MapGeneratorSettings.ClientSize);
            var ColliderMap = new UpdateMap(startpos, game.MapGeneratorSettings.ColliderSize);
            ColliderMap.InitiateMap(game);
            var ColliderMap2 = new UpdateMap(startpos, game.MapGeneratorSettings.ColliderSize);
            ColliderMap2.InitiateMap(game);

            GraphicMap.InitiateMap(game);
            GraphicBuilds.Add(0, GraphicMap.LatestBuildBox);
            ColliderBuilds.Add(0, ColliderMap.LatestBuildBox);
            ColliderMaps.Add(ColliderMap);
            ColliderBuilds.Add(1, ColliderMap2.LatestBuildBox);
            ColliderMaps.Add(ColliderMap2);
            #endregion
            AddAll(game);
        }
        public void Update(GameManager game)
        {
            for(int i = 0; i < ColliderMaps.Count; i++)
            {
                var map = ColliderMaps[i];
                map.UpdateTiles(game);
                ColliderBuilds[i] = map.LatestBuildBox;
                GraphicMap.Draw(map.LatestBuildBox);

            }
            GraphicMap.UpdateTiles(game);
            GraphicBuilds[0] = GraphicMap.LatestBuildBox; 

            RemoveAll(game);
            AddAll(game);
        }
        public void AddAll(GameManager game)
        {
            ToAddColliders.Clear();
            ToAddGraphics.Clear();
            Add(game.Tilemap, GraphicMap, ToAddGraphics, ActiveGrahics, false);
            AddGraphicsTile(game);

            for (int i = 0; i < ColliderMaps.Count; i++)
            {
                var map = ColliderMaps[i];
                Add(game.Tilemap, map, ToAddColliders, ActiveColliders, true);
            }
            AddColliderTiles(game.MapGenerator.TileCreator);
        }
        public void RemoveAll(GameManager game)
        {
            ToRemoveColliders.Clear();
            ToRemoveGraphics.Clear();
            Remove(GraphicMap, ToRemoveGraphics, GraphicBuilds, ActiveGrahics);

            for (int i = 0; i < ColliderMaps.Count; i++)
            {
                var map = ColliderMaps[i];
                Remove(map, ToRemoveColliders, ColliderBuilds, ActiveColliders);

            }
            RemoveGraphicTiles();
            RemoveColliderTiles(game.MapGenerator.TileCreator);
        }
        public void RemoveColliderTiles(TileCreator tc)
        {
            for (int i = 0; i < ToRemoveColliders.Count; i++)
            {
                var removeAt = ToRemoveColliders[i];
                var t = ActiveCollidersTransforms[removeAt.x, removeAt.y];
                if(t == null)
                    continue;
                ActiveCollidersTransforms[removeAt.x, removeAt.y] = null;
                tc.RecycleCollider(t);
                ActiveColliders[removeAt.x, removeAt.y] = false;
            }
        }
        public void RemoveGraphicTiles()
        {
            for (int i = 0; i < ToRemoveGraphics.Count; i++)
            {
                var removeAt = ToRemoveGraphics[i];
                Tilemap.SetTile(new Vector3Int(removeAt.x, removeAt.y, 0), null);
                ActiveGrahics[removeAt.x, removeAt.y] = false;
            }
        }
        public void AddColliderTiles(TileCreator tc)
        {
            for (int i = 0; i < ToAddColliders.Count; i++)
            {
                var toAdd = ToAddColliders[i];
                ActiveCollidersTransforms[toAdd.x, toAdd.y] = tc.PopTransform(toAdd);
                ActiveColliders[toAdd.x, toAdd.y] = true;
            }
        }

        public void AddGraphicsTile(GameManager game)
        {
            for (int i = 0; i < ToAddGraphics.Count; i++)
            {
                var toAdd = ToAddGraphics[i];
                var type = game.Tilemap[toAdd.x, toAdd.y].Type;
                if(type == TileType.Block)
                    Tilemap.SetTile(new Vector3Int(toAdd.x, toAdd.y, 0), game.Tiles.Brown);
                else if(type == TileType.Outline)
                    Tilemap.SetTile(new Vector3Int(toAdd.x, toAdd.y, 0), game.Tiles.Green);
                ActiveGrahics[toAdd.x, toAdd.y] = true;
            }
        }
        
        public void Remove(UpdateMap map, List<Vector2Int> toRemoveSaved, Dictionary<int, SquareCorners> otherbuilds, bool[,] activeMap)
        {
            for (int i = 0; i < map.ToRemove.Count; i++)
            {
                var toRemove = map.ToRemove[i];
                if (toRemove.Horizontal)
                {
                    for (int x = 0; x < toRemove.Diff + 1; x++)
                    {
                        var tile2 = activeMap[toRemove.First.x + x, toRemove.Second.y];
                        CheckTile(tile2, new Vector2Int(toRemove.First.x + x, toRemove.Second.y));
                    }
                }
                else
                {
                    for (int y = 0; y < toRemove.Diff + 1; y++)
                    {
                        var tile = activeMap[toRemove.First.x, toRemove.First.y + y];
                        CheckTile(tile, new Vector2Int(toRemove.First.x, toRemove.First.y + y));
                    }
                }
                void CheckTile(bool tile, Vector2Int index)
                {
                    if (tile)
                    {
                        bool add = false;
                        foreach (var buildPair in otherbuilds)
                        {
                            add = CheckContains(buildPair.Value, index) || add;
                        }
                        if (!add)
                            toRemoveSaved.Add(index);
                    }
                }
                bool CheckContains(SquareCorners otherBuild, Vector2Int ci)
                {
                    var x = ci.x >= otherBuild.BotLeft.x && ci.x <= otherBuild.BotRight.x;
                    var y = ci.y >= otherBuild.BotLeft.y && ci.y <= otherBuild.TopRight.y;
                    return x && y;
                }
            }
            map.ToRemove.Clear();
        }
 
        public void Add(TileData[,] tiledata, UpdateMap map, List<Vector2Int> toAddSaved, bool[,] activeMap, bool outlineOnly)
        {
            for (int i = 0; i < map.ToAdd.Count; i++)
            {
                var toAdd = map.ToAdd[i];
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
                    var fromPerlin = tiledata[ci.x, ci.y];
                    if (fromPerlin == null || activeMap[ci.x, ci.y] || (outlineOnly && fromPerlin.Type != TileType.Outline))
                        return;
                    
                    toAddSaved.Add(ci);
                    activeMap[ci.x, ci.y] = true;
                }
            }
            map.ToAdd.Clear();
        }
    }
}