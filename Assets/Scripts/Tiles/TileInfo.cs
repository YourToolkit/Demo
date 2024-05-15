using System;
using Unity.VisualScripting;
using UnityEngine;

namespace MyTiles
{
    public class TileInfo
    {
        public readonly GridTileBase TileBase;
        public readonly BaseTileData TileData;


        public TileInfo(TileBaseType tileBaseType = TileBaseType.NotSet, GridTileBase tileBase = null)
        {
            TileBase = tileBase;
            if (TileBase != null)
            {
                TileData = TileBase.TileData;
                TileData.TileBaseType = TileBase.TileData.TileBaseType;
            }
        }

        public TileInfo(BaseTileData tileData)
        {
            TileData = tileData;
        }
    }
}