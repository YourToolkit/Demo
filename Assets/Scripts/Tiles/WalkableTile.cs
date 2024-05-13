using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyTiles
{
    public class WalkableTile : GridTileBase
    {
        [SerializeField] private WalkableTileData _walkableTileData;

        public override BaseTileData TileData
        {
            get => _walkableTileData;
            protected set => _walkableTileData = (WalkableTileData)value;
        }

        protected override void CreateTileData(TileBaseType tileBaseType)
        {
            _walkableTileData = new WalkableTileData(tileBaseType);
        }

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

        private static readonly List<Vector2Int> Dirs = new List<Vector2Int>()
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        private void Awake()
        {
            if (CurrentState == null)
            {
                Debug.LogWarning("CurrentState has not been set in the Inspector");
            }
        }

        protected virtual void OnMouseDown()
        {
            if (!_walkableTileData.Walkable) return;
            OnHoverTile?.Invoke(this);
        }

        public void OnHoverTileInvoker()
        {
            if (!_walkableTileData.Walkable) return;
            OnHoverTile?.Invoke(this);
        }

        #region pathfinding

        private void OnOnHoverTile(WalkableTile selected) => _selected = selected == this;

        private void OnEnable() => OnHoverTile += OnOnHoverTile;


        private bool _selected;
        public List<WalkableTile> Neighbors { get; protected set; }
        public WalkableTile Connection { get; private set; }
        public int G { get; private set; }
        public int H { get; private set; }
        public int F => G + H;
        public static event Action<WalkableTile> OnHoverTile;
        private void OnDisable() => OnHoverTile -= OnOnHoverTile;

        public bool IsSelected()
        {
            return _selected;
        }


        public void CacheNeighbors()
        {
            Neighbors = new List<WalkableTile>();
            foreach (var dir in Dirs)
            {
                var tileBase = FindHighestTileBase(this.Coordinate + new Vector3Int(dir.x, dir.y, 0));
                var tile = tileBase;
                if (tile != null) Neighbors.Add(tile);
            }
        }


        public void SetG(int g) => G = g;

        public void SetH(int h) => H = h;

        public void SetConnection(WalkableTile connection) => Connection = connection;

        //这个method适用于非GridBase的游戏，你可以通过去搜寻neighbor的方向来调整是否为gridBase，如果否，则增加对角的方向
        public int GetDistance(WalkableTile other)
        {
            var dist = new Vector2Int(Mathf.Abs(this.Coordinate.x - other.Coordinate.x),
                Mathf.Abs(this.Coordinate.y - other.Coordinate.y));
            var lowest = Mathf.Min(dist.x, dist.y);
            var highest = Mathf.Max(dist.x, dist.y);

            var horizontalMovesRequired = highest - lowest; // 需要额外走的水平或垂直方向的距离

            return lowest * 14 + horizontalMovesRequired * 10;
        }

        public void ResetCost()
        {
            G = int.MaxValue;
            Connection = null;
        }

        #endregion

        public WalkableTile FindHighestTileBase(Vector3Int coordinate)
        {
            WalkableTile highestTileBase = null;
            int highestZ = int.MinValue;

            foreach (var tileData in CurrentState.TileDictList)
            {
                foreach (var tileEntry in tileData)
                {
                    if (tileEntry.Key.x == coordinate.x && tileEntry.Key.y == coordinate.y &&
                        tileEntry.Key.z > highestZ)
                    {
                        var tileBase = tileEntry.Value.TileBase as WalkableTile;
                        highestZ = tileEntry.Key.z;
                        if (tileBase)
                            highestTileBase = tileBase;
                    }
                }
            }

            return highestTileBase;
        }

        public void SetColor(Color color) => SpriteRenderer.color = color;
    }
}