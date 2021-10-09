using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "Setting/MapGeneratorSettings", order = 1)]
    public class MapGeneratorSettings : ScriptableObject
    {
        public int MapHeight = 50;
        public int MapWidth = 90;
        public int HeightBound = 8;
        public int WidthBound = 8;
        public int TopBoundOffset = 4;
        public float PerlinZoom = 0.1f;
        public Vector2 PerlinShift = Vector2.zero;

        [Header("Tiling")]
        public Vector2Int ClientSize;
        public Vector2Int ColliderSize;

    }
}