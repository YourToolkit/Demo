using UnityEngine;

namespace MyToolSystem
{
    public class EraserTool : Tool
    {
        public override void OnToolMouseDown(Vector3Int coordinate)
        {
        }

        public override void OnToolMouseDrag(Vector3Int coordinate)
        {
            if (CurrentState.CurrentTileMapDict.ContainsKey(coordinate))
            {
                EraseTile(coordinate);
            }
        }

        public override void OnToolMouseUp(Vector3Int coordinate)
        {
        }

        public override void OnToolMouseUnClicked(Vector3Int coordinate)
        {
        }


        public EraserTool(CurrentState currentState) : base(currentState)
        {
        }

        private void EraseTile(Vector3Int coordinate)
        {
            DestroyTile(CurrentState.CurrentTileMapDict[coordinate].TileBase);
            CurrentState.CurrentTileMapDict.Remove(coordinate);
        }
    }
}