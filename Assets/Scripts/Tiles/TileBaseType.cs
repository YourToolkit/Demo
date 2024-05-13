using UnityEngine;

namespace MyTiles
{
    public enum TileBaseType
    {
        NotSet, // 特殊的值，表示未设置

        // Isometric Reference
        Grass,
        Ground,
        Cursor,
        Floor,
        Player,

        // Rectangle
        Mosaic,
        Test
    }
}