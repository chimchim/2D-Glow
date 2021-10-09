using System;
using System.Collections.Generic;
using Assets.Kod.Misc;
using Gameloop;
using Gameloop.Systems;
using JetBrains.Annotations;
using Map;
using Misc;
using UnityEngine;

namespace Gameloop.Systems
{
    public class UpdateMap
    {
        public List<Lane> ToAdd = new List<Lane>();
        public List<Lane> ToRemove = new List<Lane>();
        public Vector2Int LatestPosIndex;
        public SquareCorners LatestBuildBox;
        public Vector2Int Size;
        public Vector2 Position; // spelarens poss
        public int DrawHeight => Size.y + 1;

        public UpdateMap(Vector2Int firstPos, Vector2Int size)
        {
            LatestPosIndex = firstPos;
            Size = size;
            Position = firstPos;

        }
        public void InitiateMap(GameManager game)
        {
            var posAsCorner = GetPosIndex(Position);
            BuildFirst(game);
        }
        public bool UpdateTiles(GameManager game)
        {
            bool gotUpdated = false;
            var posAsCorner = GetPosIndex(Position);
            var tc = game.MapGenerator.TileCreator;
            var buildSquare = GetBuildSquare(posAsCorner, game.MapGenerator);

            if (!posAsCorner.Equals(LatestPosIndex))
            {
                float xDiffLeft = LatestBuildBox.BotLeft.x - buildSquare.BotLeft.x;
                float xDiffRight = LatestBuildBox.BotRight.x - buildSquare.BotRight.x;
                float yDiffTop = LatestBuildBox.TopLeft.y - buildSquare.TopLeft.y;
                float yDiffBot = LatestBuildBox.BotRight.y - buildSquare.BotRight.y;

                int addTop = yDiffTop < 0 ? Math.Abs((int)yDiffTop) : 0;
                int addBot = yDiffBot > 0 ? (int)yDiffBot : 0;
                int addRight = xDiffRight < 0 ? Math.Abs((int)xDiffRight) : 0;
                int addLeft = xDiffLeft > 0 ? (int)xDiffLeft : 0;

                int removeTop = yDiffTop > 0 ? (int)yDiffTop : 0;
                int removeBot = yDiffBot < 0 ? Math.Abs((int)yDiffBot) : 0;
                int removeRight = xDiffRight > 0 ? (int)xDiffRight : 0;
                int removeLeft = xDiffLeft < 0 ? Math.Abs((int)xDiffLeft) : 0;
                RemoveTiles(removeBot, removeTop, removeLeft, removeRight, LatestPosIndex);
                AddTiles(addBot, addTop, addLeft, addRight, tc, buildSquare, posAsCorner);
                gotUpdated = true;
                LatestPosIndex = posAsCorner;
                LatestBuildBox = buildSquare;
            }
            
            return gotUpdated;
        }
        public void RemoveTiles(int removeBot, int removeTop, int removeLeft, int removeRight, Vector2Int pos)
        {
            for (int y = 0; y < removeTop; y++)
                ToRemove.Add(new Lane(LatestBuildBox.TopLeft - new Vector2Int(0, y), LatestBuildBox.TopRight - new Vector2Int(0, y)));
            for (int y = 0; y < removeBot; y++)
                ToRemove.Add(new Lane(LatestBuildBox.BotLeft + new Vector2Int(0, y), LatestBuildBox.BotRight + new Vector2Int(0, y)));
            for (int x = 0; x < removeLeft; x++)
                ToRemove.Add(new Lane(LatestBuildBox.BotLeft + new Vector2Int(x, 0), LatestBuildBox.TopLeft + new Vector2Int(x, 0)));
            for (int x = 0; x < removeRight; x++)
                ToRemove.Add(new Lane(LatestBuildBox.BotRight - new Vector2Int(x, 0), LatestBuildBox.TopRight - new Vector2Int(x, 0)));
        }
        public void AddTiles(int addBot, int addTop, int addLeft, int addRight, TileCreator tc, SquareCorners build, Vector2Int pos)
        {
            for (int y = 0; y < addBot; y++)
                ToAdd.Add(new Lane(build.BotLeft + new Vector2Int(0, y), build.BotRight + new Vector2Int(0, y)));
            for (int y = 0; y < addTop; y++)
                ToAdd.Add(new Lane(build.TopLeft - new Vector2Int(0, y), build.TopRight - new Vector2Int(0, y)));
            for (int x = 0; x < addLeft; x++)
                ToAdd.Add(new Lane(build.BotLeft + new Vector2Int(x, 0), build.TopLeft + new Vector2Int(x, 0)));
            for (int x = 0; x < addRight; x++)
                ToAdd.Add(new Lane(build.BotRight - new Vector2Int(x, 0), build.TopRight - new Vector2Int(x, 0)));
        }


        public void BuildFirst(GameManager game)
        {
            LatestPosIndex = GetPosIndex(Position);
            LatestBuildBox = GetBuildSquare(LatestPosIndex, game.MapGenerator);

            for (int y = 0; y < DrawHeight; y++)
            {
                var offset = new Vector2Int(0, y);
                var lane = new Lane(LatestBuildBox.BotLeft + offset, LatestBuildBox.BotRight + offset);
                ToAdd.Add(lane);
            }

        }

        public SquareCorners GetBuildSquare(Vector2Int pos, MapGenerator map)
        {
            var fullBuild = TileMapHelper.GetFullBuild(pos, Size);

            int rightClampedTest = Mathf.Clamp(fullBuild.BotRight.x, Size.x, map.FullWidth - 1); // OK
            int leftClampedTest = Mathf.Clamp(fullBuild.BotLeft.x, 0, map.FullWidth - Size.x - 1); // OK
            int botClampedTest = Mathf.Clamp(fullBuild.BotLeft.y, 0, map.FullHeight - Size.y - 1);
            int topClampedTest = Mathf.Clamp(fullBuild.TopLeft.y, Size.y, map.FullHeight - 1);

            var botleft = new Vector2Int(leftClampedTest, botClampedTest);
            var topLeft = new Vector2Int(leftClampedTest, topClampedTest);
            var botRight = new Vector2Int(rightClampedTest, botClampedTest);
            var topRight = new Vector2Int(rightClampedTest, topClampedTest);

            return new SquareCorners(botleft, topLeft, topRight, botRight);
        }

    
        
        private Vector2Int GetPosIndex(Vector2 pos)
        {
            return new Vector2Int((int)(pos.x), (int)(pos.y));
        }
        public void Draw(SquareCorners build)
        {
            TileMapHelper.DrawSquareCorners(build, Color.red);
        }

       
    }
}