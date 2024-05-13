using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyToolSystem
{
    public class BrushTool : Tool
    {
        public override void OnToolMouseDown(Vector3Int coordinate)
        {
        }

        public override void OnToolMouseDrag(Vector3Int coordinate)
        {
            var sortingOrder = CurrentState.CurrentTileMap.GetComponent<TilemapRenderer>().sortingOrder;
            var tileBase = PlaceTile(coordinate, CurrentState.CurrentTileBaseType, sortingOrder,
                CurrentState.CurrentTileMapDict);
            if (tileBase != null)
            {
                RecordTile(coordinate, tileBase);
            }
        }

        public override void OnToolMouseUp(Vector3Int coordinate)
        {
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

        public BrushTool(CurrentState currentState) : base(currentState)
        {
        }
    }
}