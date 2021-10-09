using UnityEngine;

public struct Lane
{
    public Lane(Vector2Int first, Vector2Int second)
    {
        First = first;
        Second = second;
    }

    public Vector2Int First;
    public Vector2Int Second;
    public int Diff => Horizontal ? Second.x - First.x : Second.y - First.y;
    public bool Horizontal => First.y == Second.y;
}