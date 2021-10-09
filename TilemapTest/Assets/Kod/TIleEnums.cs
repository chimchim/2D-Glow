using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map
{
    [Flags]
    public enum TileNeighbours
    {
        NotSet = 1,
        Top = 2,
        Bot = 4,
        Right = 8,
        Left = 16,
        TopLeft = 32,
        TopRight = 64,
        BotLeft = 128,
        BotRight = 256,
        Bounds = 512
    }

    public enum TileColor
    {
        Red,
        Blue,
        Green,
        Yellow
    }

    public enum TileType
    {
        Air,
        Outline,
        Block
    }
}