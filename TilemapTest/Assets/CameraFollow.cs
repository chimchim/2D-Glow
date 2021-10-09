using System.Collections;
using System.Collections.Generic;
using Assets.Kod.Misc;
using Gameloop;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //[HideInInspector]
    public Transform Player;
    public bool Initiated;
    private GameManager Game;
    void Start()
    {

    }

    public void SetplayerClient(Transform player, GameManager game)
    {
        Player = player;
        Initiated = true;
        Game = game;
    }

    // Update is called once per frame
    void Update()
    {
        if (Initiated)
        {
            
                float sizeX = (float)(Game.MapGeneratorSettings.ClientSize.x - 6);
                float sizeY = (float)(Game.MapGeneratorSettings.ClientSize.y - 3);
                float mapHeight = (float)(Game.MapGeneratorSettings.MapHeight) ;
                float mapWidth = (float)(Game.MapGeneratorSettings.MapWidth);
                var clampedX = Mathf.Clamp(Player.position.x, sizeX / 2, mapWidth - sizeX / 2);
                var clampedY = Mathf.Clamp(Player.position.y, sizeY / 2, mapHeight - sizeY / 2);
                transform.position = new Vector3(Player.position.x, Player.position.y, -1);
        }
    }
}