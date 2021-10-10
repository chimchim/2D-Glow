using System;
using Map;
using Misc;
using UnityEngine;

public class TileInUse
{
    private static ObjectPool<TileInUse> _pool = new ObjectPool<TileInUse>(1500);


    public Transform Transform;
    public TileData TileData;
    public static TileInUse Make(Transform t, TileData td, Vector2 pos)
    {   //TODO gör position här
        TileInUse tiu = _pool.GetNext();
        tiu.Transform = t;
        tiu.TileData = td;
        t.position = pos;
        t.name = $"X:{td.Index.x}, Y;{td.Index.y}";
        return tiu;
    }


    public void Recycle()
    {
    }

}