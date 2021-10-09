using UnityEngine;

public struct SquareCorners
{
    public SquareCorners(Vector2Int bl, Vector2Int tl, Vector2Int tr, Vector2Int br)
    {
        BotLeft = bl;
        TopLeft = tl;
        TopRight = tr;
        BotRight = br;
    }

    public Vector2Int BotLeft;
    public Vector2Int TopLeft;
    public Vector2Int TopRight;
    public Vector2Int BotRight;
}