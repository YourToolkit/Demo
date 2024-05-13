using System;
using ScriptsOfBilly.Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyTiles
{
    [System.Serializable]
    public class BaseTileData
    {
        public TileBaseType TileBaseType;
        public string Id;

        private void SetUniqueId()
        {
            Id = Guid.NewGuid().ToString();
        }


        public BaseTileData(TileBaseType tileBaseType)
        {
            TileBaseType = tileBaseType;
            SetUniqueId();
        }
    }
}