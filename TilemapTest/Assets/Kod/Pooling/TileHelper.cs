using UnityEngine;

public static class TileMapHelper
{
    public static void DrawSquareCorners(SquareCorners sc, Color color)
    {
        Debug.DrawLine(new Vector2(sc.BotLeft.x, sc.BotLeft.y), new Vector2(sc.TopLeft.x, sc.TopLeft.y), color);
        Debug.DrawLine(new Vector2(sc.TopLeft.x, sc.TopLeft.y), new Vector2(sc.TopRight.x, sc.TopRight.y), color);
        Debug.DrawLine(new Vector2(sc.TopRight.x, sc.TopRight.y), new Vector2(sc.BotRight.x, sc.BotRight.y), color);
        Debug.DrawLine(new Vector2(sc.BotRight.x, sc.BotRight.y), new Vector2(sc.BotLeft.x, sc.BotLeft.y), color);
    }

    public static SquareCorners GetFullBuild(Vector2Int pos, Vector2Int size)
    {
        int height = size.y;
        int width = size.x;
        var botleft = pos + new Vector2Int(-width / 2, -height / 2);
        var topLeft = pos + new Vector2Int(-width / 2, height / 2);
        var topRight = pos + new Vector2Int(width / 2, height / 2);
        var botRight = pos + new Vector2Int(width / 2, -height / 2);
        botleft = new Vector2Int(Mathf.CeilToInt(botleft.x), Mathf.CeilToInt(botleft.y));
        topLeft = new Vector2Int(Mathf.CeilToInt(topLeft.x), Mathf.CeilToInt(topLeft.y));
        topRight = new Vector2Int(Mathf.CeilToInt(topRight.x), Mathf.CeilToInt(topRight.y));
        botRight = new Vector2Int(Mathf.CeilToInt(botRight.x), Mathf.CeilToInt(botRight.y));
        return new SquareCorners(botleft, topLeft, topRight, botRight);
    }
}