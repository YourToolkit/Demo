using UnityEngine;

namespace MyTiles
{
    public class NormalTile : GridTileBase
    {
        [SerializeField] private NormalData _normalData;

        public override void Init(Vector3Int coordinate, BaseTileData tileData, int sortingOrder)
        {
            base.Init(coordinate, tileData, sortingOrder);
            Debug.Log(tileData.TileBaseType);
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
            get => _normalData;
            protected set => _normalData = (NormalData)value;
        }


        protected override void CreateTileData(TileBaseType tileBaseType)
        {
            _normalData = new NormalData(tileBaseType);
        }
    }
}