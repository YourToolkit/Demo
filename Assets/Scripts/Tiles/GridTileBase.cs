using System;
using System.Collections.Generic;
using MyGridSystem;
using UnityEngine;

namespace MyTiles
{
    //所有用于editor的prefab都需要带有的基类，如果你想要用map editor放置你的prefab，必须创建一个新的class继承该类，再将新的class放置到你的prefab里面
    public abstract class GridTileBase : MonoBehaviour
    {
        [SerializeField] protected Vector3Int Coordinate;
        [SerializeField] protected SpriteRenderer SpriteRenderer;
        public CurrentState CurrentState;

        //用于存储Data到Json，如果prefab中有特殊的data，你需要重写这个property里面的内容，来get到你想要的数据
        public virtual BaseTileData TileData { get; protected set; }

        protected virtual void OnValidate()
        {
            if (CurrentState == null || CurrentState.CurrentTileMapDict == null)
            {
                return;
            }

            if (CurrentState.CurrentTileMapDict.TryGetValue(Coordinate, out TileInfo tileInfo))
            {
                if (tileInfo.TileBase == this)
                {
                    TileData = tileInfo.TileData;
                }
            }
        }


        //这是用于未有原先数据生成新的prefab的，所以我们并没有先前的TileData，所以我们只能去用TileBaseType去初始化
        public virtual void Init(Vector3Int coordinate, TileBaseType tileBaseType, int sortingOrder)
        {
            CreateTileData(tileBaseType);
            TileData.TileBaseType = tileBaseType;
            Coordinate = coordinate;
            SetSpriteWorldPosition();
        }

        //用于我们有先前的TileData，直接用TileData生成
        public virtual void Init(Vector3Int coordinate, BaseTileData tileData, int sortingOrder)
        {
            Coordinate = coordinate;
            SetSpriteWorldPosition();
        }

        private void UpdateRelativeData(Vector3Int coordinate)
        {
            // 只用于PlayMode，当你确定位置一定改变的时候用这个来改变current state的值
            if (coordinate != Coordinate)
            {
                var tileInfo = CurrentState.CurrentTileMapDict[Coordinate]; // 保留先前数据的value，即tile info
                CurrentState.CurrentTileMapDict.Remove(Coordinate); // 删除原先的数据
                CurrentState.CurrentTileMapDict[coordinate] = tileInfo; // 生成新的数据，更新新的coordinate
            }
        }


        private void SetSpriteWorldPosition()
        {
            var spriteTransform = SpriteRenderer.transform;
            var localPosition = new Vector3(0, (.26f) * Coordinate.z, Coordinate.z);
            spriteTransform.localPosition = localPosition;
        }

        protected abstract void CreateTileData(TileBaseType tileBaseType);


        public static Vector3 GetTileWorldPosition(Vector3Int coordinate, CurrentState currentState)
        {
            //这是由于tilemap他的z轴是0，所以需要把z轴设置为-10，这样才能正确在上面生成tile,最后再把z轴设置为原来的z轴，这样才能正确的设置tile的depth
            Vector3Int temp = coordinate;
            temp.z = -10;
            var location = currentState.CurrentTileMap.CellToWorld(temp);

            location.z = coordinate.z;

            return location;
        }

        public Vector3Int GetTileCoordinate()
        {
            return Coordinate;
        }

        public void SetTileCoordinate(Vector3Int coordinate)
        {
            UpdateRelativeData(coordinate);
            Coordinate = coordinate;
        }
    }
}