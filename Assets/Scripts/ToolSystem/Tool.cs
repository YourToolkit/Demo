using System.Collections.Generic;
using System.Linq;
using MyGridSystem;
using MyTiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyToolSystem
{
    public abstract class Tool : IPreview
    {
        protected readonly CurrentState CurrentState;
        public GridTileBase PreviewObj { get; set; }

        protected Tool(CurrentState currentState)
        {
            CurrentState = currentState;
        }

        public abstract void OnToolMouseDown(Vector3Int coordinate);
        public abstract void OnToolMouseDrag(Vector3Int coordinate);
        public abstract void OnToolMouseUp(Vector3Int coordinate);
        public abstract void OnToolMouseUnClicked(Vector3Int coordinate);

        protected void RecordTile(Vector3Int cellPosition, GridTileBase tileBase)
        {
            if (CurrentState.CurrentTileMapDict.ContainsKey(cellPosition))
                DestroyTile(CurrentState.CurrentTileMapDict[cellPosition].TileBase);
            CurrentState.CurrentTileMapDict[cellPosition] = new TileInfo(tileBase: tileBase);
        }


        protected void DestroyTile(GridTileBase tileBase)
        {
            TilePoolManager.ReturnTileToPool(tileBase);
        }

        protected GridTileBase PlaceTile(Vector3Int coordinate, TileBaseType tileBaseType, int sortingOrder,
            Dictionary<Vector3Int, TileInfo> data)
        {
            if (data.TryGetValue(coordinate, out TileInfo previousTileInfo) &&
                previousTileInfo.TileData.TileBaseType == tileBaseType)
            {
                return null;
            }

            return InstantiateTile(coordinate, tileBaseType, sortingOrder);
        }

        private GridTileBase InstantiateTile(Vector3Int coordinate, TileBaseType tileBaseType, int sortingOrder)
        {
            Vector3 tilePosition = GridTileBase.GetTileWorldPosition(coordinate, CurrentState);
            var tileBase = CurrentState.TilePrefabs.First(c => c.TileData.TileBaseType == tileBaseType)
                ;
            var spawned = TilePoolManager.SpawnTile(tileBase, tilePosition, Quaternion.identity,
                CurrentState.CurrentTileMap.transform, tileBaseType);
            spawned.Init(coordinate, tileBaseType, sortingOrder);
            spawned.name =
                $"{spawned.TileData.TileBaseType} ({coordinate.x}, {coordinate.y}, {coordinate.z})";
            return spawned;
        }

        protected void PreviewTile(Vector3Int coordinate, TileBaseType tileBaseType)
        {
            var location = GridTileBase.GetTileWorldPosition(coordinate, CurrentState);
            var sortingOrder = CurrentState.CurrentTileMap.GetComponent<TilemapRenderer>().sortingOrder;
            if (PreviewObj == null)
            {
                // In previewObj, we don't need to pass the data to check if the tile is already there, because it's just a preview, hence we just pass an empty dictionary
                PreviewObj = PlaceTile(coordinate, tileBaseType, sortingOrder, new Dictionary<Vector3Int, TileInfo>());
            }

            if (PreviewObj.TileData.TileBaseType != tileBaseType ||
                PreviewObj.GetTileCoordinate().z != CurrentState.CurrentZPosition)
            {
                DestroyTile(PreviewObj);
                PreviewObj = PlaceTile(coordinate, tileBaseType, sortingOrder, CurrentState.CurrentTileMapDict);
            }

            PreviewObj.transform.position = location;
        }


        public virtual void Preview(Vector3Int coordinate)
        {
        }

        public virtual void UnPreview()
        {
        }
    }
}