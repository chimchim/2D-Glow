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




    public void Recycle(TileCreator tc)
    {

        try
        {
            switch (TileData.Type)
            {
                case TileType.Block:
                    Transform.position = new Vector2(-10, -10);
                    tc.BlockPool.Push(Transform);
                    break;
                case TileType.Outline:
                    Transform.position = new Vector2(-10, -10);
                    tc.OutlinePool.Push(Transform);
                    break;
            }

            Transform = null;
            TileData = null;
            _pool.Recycle(this);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }
    public void Recycle()
    {
    }

}