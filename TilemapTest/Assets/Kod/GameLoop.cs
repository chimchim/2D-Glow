using System.Collections;
using System.Collections.Generic;
using Assets.Kod.Misc;
using Gameloop.Systems;
using Map;
using ScriptableObjects;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameLoop : MonoBehaviour
{
    public bool UsePlayer;
    public Tiles Tiles;
    public MapGeneratorSettings MGS;
    private MapGenerator MapGenerator;
    private LevelGenerator LevelGenerator;

    public float ZoomScale = 4;
    private GameManager Game;
    private UpdateMapTiles UpdateMapTiles;

    public Transform Player;
    public Transform Grid;
    private Tilemap Tilemap;
    public float Speed;
    void Start()
    {
        Tilemap = Grid.GetComponentInChildren<Tilemap>();
        MapGenerator = new MapGenerator(Tiles, MGS);
        MapGenerator.GenerateMap();
        Game = new GameManager(MapGenerator, MGS, Tiles);
        UpdateMapTiles = new UpdateMapTiles(Tilemap);
        UpdateMapTiles.Initiate(Game, new Vector2Int(70, 60));
        if (UsePlayer)
            Camera.main.GetComponent<CameraFollow>().SetplayerClient(Player, Game);

        Player.transform.position = new Vector3(70, 60, 0);
    }

    public void Zoom()
    {
        var f = Input.mouseScrollDelta.y * ZoomScale;
        if (f != 0)
        {
            Camera.main.orthographicSize -= f * Time.deltaTime;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        float yTranslate = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        float xTranslate = Input.GetAxis("Horizontal") * Speed * Time.deltaTime;
        Player.transform.position = new Vector3(xTranslate, yTranslate, 0) + Player.transform.position;
        UpdateMapTiles.GraphicMap.Position = Player.transform.position;
        UpdateMapTiles.ColliderMaps[0].Position = Player.transform.position;
        UpdateMapTiles.ColliderMaps[1].Position = Player.transform.position + new Vector3(3,3);
        UpdateMapTiles.Update(Game);
        Zoom();
        
        var tm = MapGenerator.TileCreator.Tilemap;
        //var map = LevelGenerator.TheMap;
        return;
        for (int y = 0; y < MapGenerator.FullHeight; y++)
        {
            for (int x = 0; x < MapGenerator.FullWidth; x++)
            {
                var tile = tm[x, y];
                if(tile != null && tile.Type == TileType.Outline)
                {
                    Color col = Color.blue;
                    if(tile.Type == TileType.Outline)
                        col = Color.green;
                    Vector2 leftBot = new Vector2(x - 0.5f, y - 0.5f);
                    Vector2 leftTop = new Vector2(x - 0.5f, y + 0.5f);
                    Vector2 rightTop = new Vector2(x + 0.5f, y + 0.5f);
                    Vector2 rightBot = new Vector2(x + 0.5f, y - 0.5f);
                    Debug.DrawLine(leftBot, leftTop, col);
                    Debug.DrawLine(leftTop, rightTop, col);
                    Debug.DrawLine(rightTop, rightBot, col);
                    Debug.DrawLine(rightBot, leftBot, col);
                }
            }
        }
    }
}
