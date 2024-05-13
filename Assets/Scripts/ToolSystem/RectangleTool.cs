using System.Collections.Generic;
using System.Linq;
using MyTiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyToolSystem
{
    public class RectangleTool : Tool
    {
        private Vector3Int _startCoordinate, _endCoordinate;
        private Dictionary<Vector3Int, TileInfo> _extraData;

        public RectangleTool(CurrentState currentEditorState) : base(currentEditorState)
        {
            _extraData = new Dictionary<Vector3Int, TileInfo>();
        }

        public override void OnToolMouseDown(Vector3Int coordinate)
        {
            _startCoordinate = coordinate;
            _extraData = new Dictionary<Vector3Int, TileInfo>();
        }

        public override void OnToolMouseDrag(Vector3Int coordinate)
        {
            if (_endCoordinate == coordinate)
                return;

            _endCoordinate = coordinate;
            PlaceRectangleTiles();
            EraseRedundantTiles();
        }

        public override void OnToolMouseUp(Vector3Int coordinate)
        {
            RecordRectangleTiles();
        }

        public override void OnToolMouseUnClicked(Vector3Int coordinate)
        {
            Preview(coordinate);
        }

        public override void Preview(Vector3Int coordinate)
        {
            base.Preview(coordinate);
            PreviewTile(coordinate, CurrentState.CurrentTileBaseType);
        }

        public override void UnPreview()
        {
            base.UnPreview();
            DestroyTile(PreviewObj);
        }

        private void PlaceRectangleTiles()
        {
            for (int x = Mathf.Min(_startCoordinate.x, _endCoordinate.x);
                 x <= Mathf.Max(_startCoordinate.x, _endCoordinate.x);
                 x++)
            {
                for (int y = Mathf.Min(_startCoordinate.y, _endCoordinate.y);
                     y <= Mathf.Max(_startCoordinate.y, _endCoordinate.y);
                     y++)
                {
                    var sortingOrder = CurrentState.CurrentTileMap.GetComponent<TilemapRenderer>().sortingOrder;
                    Vector3Int coordinate = new Vector3Int(x, y, _startCoordinate.z);
                    var tempData = CurrentState.CurrentTileMapDict.Concat(_extraData).GroupBy(pair => pair.Key)
                        .ToDictionary(group => group.Key,
                            group => group.Last().Value); // 额外的数据与地图数据结合，相同Vector3Int的只保留额外的数据
                    var tileBase = PlaceTile(coordinate, CurrentState.CurrentTileBaseType, sortingOrder,
                        tempData);

                    if (tileBase != null)
                    {
                        _extraData[coordinate] = new TileInfo(tileBase: tileBase);
                    }
                }
            }
        }


        private void EraseRedundantTiles()
        {
            List<Vector3Int> coordinatesToRemove = new List<Vector3Int>();
            foreach (GridTileBase tileBase in _extraData.Values.Select(tileInfo => tileInfo.TileBase))
            {
                var coordinate = tileBase.GetTileCoordinate();
                bool isXBetween = coordinate.x >= Mathf.Min(_startCoordinate.x, _endCoordinate.x) &&
                                  coordinate.x <= Mathf.Max(_startCoordinate.x, _endCoordinate.x);
                bool isYBetween = coordinate.y >= Mathf.Min(_startCoordinate.y, _endCoordinate.y) &&
                                  coordinate.y <= Mathf.Max(_startCoordinate.y, _endCoordinate.y);
                if (isXBetween && isYBetween)
                    continue;
                DestroyTile(tileBase);
                coordinatesToRemove.Add(coordinate);
            }

            foreach (var removeCoordinate in coordinatesToRemove)
            {
                _extraData.Remove(removeCoordinate);
            }
        }


        private void RecordRectangleTiles()
        {
            foreach (var pair in _extraData)
            {
                if (CurrentState.CurrentTileMapDict.TryGetValue(pair.Key, out TileInfo previousTileInfo))
                {
                    DestroyTile(previousTileInfo.TileBase);
                    CurrentState.CurrentTileMapDict.Remove(pair.Key);
                }

                CurrentState.CurrentTileMapDict[pair.Key] = pair.Value;
            }
        }
    }
}