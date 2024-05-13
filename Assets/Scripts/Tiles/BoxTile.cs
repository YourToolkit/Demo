using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MyTiles
{
    public class BoxTile : GridTileBase
    {
        [SerializeField] private BoxData _boxData;

        public override void Init(Vector3Int coordinate, BaseTileData tileData, int sortingOrder)
        {
            base.Init(coordinate, tileData, sortingOrder);
            TileData = tileData;
        }

        public override void Init(Vector3Int coordinate, TileBaseType tileBaseType, int sortingOrder)
        {
            base.Init(coordinate, tileBaseType, sortingOrder);
            // 设置sortingOrder
            SpriteRenderer.sortingOrder = sortingOrder;
        }

        public override BaseTileData TileData
        {
            get => _boxData;
            protected set => _boxData = (BoxData)value;
        }

        protected override void CreateTileData(TileBaseType tileBaseType)
        {
            _boxData = new BoxData(tileBaseType);
        }
    }
}