using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace Map
{
	public class MapGenerator
	{
		// Start is called before the first frame update
		public MapGenerator(Tiles tiles, MapGeneratorSettings mgs)
		{
			Mgs = mgs;
			TileCreator = new TileCreator(tiles, mgs);
		}

		public List<TileData> Outlines = new List<TileData>();
		public TileCreator TileCreator;
		private MapGeneratorSettings Mgs;
		public int FullHeight => Mgs.MapHeight + (Mgs.HeightBound * 2) + +Mgs.TopBoundOffset;
		public int FullWidth => Mgs.MapWidth + (Mgs.WidthBound * 2);
		public void GenerateMap()
		{
			var perlinmap = GeneratePerlinMap();
			TileCreator.CreateMapFromTiledata(perlinmap);
		}

		private TileData[,] GeneratePerlinMap()
		{
			Debug.Log($"GenerateMap seed: {UnityEngine.Random.state}");
			TileData[,] tiles = new TileData[FullWidth, FullHeight];

			float lowest = 100;
			float highest = 0;
			for (int x = 0; x < FullWidth; x++)
			{
				for (int y = 0; y < FullHeight; y++)
				{

					int rightLayerBound = Mgs.MapWidth + Mgs.WidthBound - 1;
					int topLayerBound = Mgs.MapHeight + Mgs.HeightBound + Mgs.TopBoundOffset - 1;
					if ((x < Mgs.WidthBound || x > rightLayerBound) || (y < Mgs.HeightBound || y > topLayerBound))
					{
						tiles[x, y] = new TileData(new Vector2Int(x, y));
						tiles[x, y].Type = TileType.Outline;
						Outlines.Add(tiles[x, y]);
					}

				}
			}

			Vector2 shift = Mgs.PerlinShift;
			float zoom = Mgs.PerlinZoom;
			for (int x = 0; x < Mgs.MapWidth; x++)
			{
				for (int y = 0; y < Mgs.MapHeight; y++)
				{
					Vector2 pos = zoom * (new Vector2(x, y)) + shift;
					float noise = Mathf.PerlinNoise(pos.x, pos.y);

					int posX = x + Mgs.WidthBound;
					int posY = y + Mgs.HeightBound;

					if (noise < 0.16f)
					{
						tiles[posX, posY] = new TileData(new Vector2Int(posX, posY));
					}
					else if (noise < 0.5f)
					{
					}
					else if (noise < 0.9f && noise > 0.5f)
					{
						tiles[posX, posY] = new TileData(new Vector2Int(posX, posY));
					}
					else
					{
						tiles[posX, posY] = new TileData(new Vector2Int(posX, posY));
					}
					if (tiles[posX, posY] != null)
						tiles[posX, posY].Type = TileType.Block;
                    else
                    {
                        //Debug.Log("Is NUll");
                    }
				}
			}
			SetNeighbours(FullWidth, FullHeight, tiles);
			SetTileType(FullWidth, FullHeight, tiles);
			return tiles;
		}
		public void SetTileType(int fullWidth, int fullHeight, TileData[,] tiles)
		{
			for (int x = 1; x < fullWidth - 1; x++)
			{
				for (int y = 1; y < fullHeight - 1; y++)
				{
					var tile = tiles[x, y];
					if (tile == null)
						continue;
					if (tile.Neighbours.HasFlag(TileNeighbours.Top) || tile.Neighbours.HasFlag(TileNeighbours.Bot)
																	|| tile.Neighbours.HasFlag(TileNeighbours.Left) ||
																	tile.Neighbours.HasFlag(TileNeighbours.Right))
					{
						Outlines.Add(tile);
						tile.Type = TileType.Outline;
					}
				}
			}
		}
		public void SetNeighbours(int fullWidth, int fullHeight, TileData[,] tiles)
		{
			for (int x = 1; x < fullWidth - 1; x++)
			{
				for (int y = 1; y < fullHeight - 1; y++)
				{
					var tile = tiles[x, y];
					if (tile == null)
						continue;

					var tileTop = tiles[x, y + 1];
					var tileBot = tiles[x, y - 1];
					var tileLeft = tiles[x - 1, y];
					var tileRight = tiles[x + 1, y];

					var tileTopLeft = tiles[x - 1, y + 1];
					var tileTopRight = tiles[x + 1, y + 1];
					var tileBotLeft = tiles[x - 1, y - 1];
					var tileBotRight = tiles[x + 1, y - 1];


					if (tileTop == null)
						tile.Neighbours |= TileNeighbours.Top;
					if (tileBot == null)
						tile.Neighbours |= TileNeighbours.Bot;
					if (tileLeft == null)
						tile.Neighbours |= TileNeighbours.Left;
					if (tileRight == null)
						tile.Neighbours |= TileNeighbours.Right;
					if (tileTopLeft == null)
						tile.Neighbours |= TileNeighbours.TopLeft;
					if (tileTopRight == null)
						tile.Neighbours |= TileNeighbours.TopRight;
					if (tileBotLeft == null)
						tile.Neighbours |= TileNeighbours.BotLeft;
					if (tileBotRight == null)
						tile.Neighbours |= TileNeighbours.BotRight;

					if (tile.Neighbours.HasFlag(TileNeighbours.NotSet))
						tile.Neighbours &= ~TileNeighbours.NotSet;

				}
			}
		}
	}
}