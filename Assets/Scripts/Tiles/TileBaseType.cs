using UnityEngine;

namespace MyTiles
{
    // Please config the enum value you want to use in your game,
    // this is just a reference, you can delete or add more
    public enum TileBaseType
    {
        NotSet, // 特殊的值，表示未设置
        Wall,
        Cursor,
        Player,
        Enemy,
    }
}