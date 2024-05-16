#if UNITY_EDITOR
using MyTiles;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.Tilemaps;

[CreateAssetMenu(fileName = "Prefab Brush", menuName = "Brushes/My Prefab brush")]
[CustomGridBrush(false, true, false, "My Prefab Brush")]
public class MyPrefabBrush : GridBrush
{
    public CurrentState CurrentState; //全局数据的引用

    // 在Tilemap上画出预制体
    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (!CurrentState.TilemapList.Contains(brushTarget.GetComponent<Tilemap>()))
        {
            Debug.LogWarning("Please click the correct tile map or your Current State is empty");
            return;
        }

        var index = CurrentState.TilemapList.IndexOf(brushTarget.GetComponent<Tilemap>());
        var data = CurrentState.TileDictList[index];

        if (data.TryGetValue(position, out TileInfo previousTileInfo) &&
            previousTileInfo.TileData.TileBaseType == CurrentState.Tile.TileData.TileBaseType)
        {
            return;
        }

        // 在指定的位置上实例化预制体
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(CurrentState.Tile.gameObject);

        if (instance != null)
        {
            instance.GetComponent<GridTileBase>().Init(position, CurrentState.Tile.TileData.TileBaseType,
                CurrentState.CurrentTileMap.GetComponent<TilemapRenderer>().sortingOrder);
            instance.name =
                $"{instance.GetComponent<GridTileBase>().TileData.TileBaseType} ({position.x}, {position.y}, {position.z})";
            instance.transform.SetParent(brushTarget.transform);
            instance.transform.position =
                grid.LocalToWorld(grid.CellToLocalInterpolated(position));
            data[position] = new TileInfo(tileBase: instance.GetComponent<GridTileBase>());
        }
    }

    // 清除Tilemap上的预制体
    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        // 找到指定位置的预制体并删除它
        Transform erased = GetTransformInCell(grid, brushTarget.transform, position);
        if (erased != null)
        {
            Debug.Log("Erasing");
            CurrentState.CurrentTileMapDict.Remove(position);
            Undo.DestroyObjectImmediate(erased.gameObject);
        }
    }

    private static Transform GetTransformInCell(GridLayout grid, Transform parent, Vector3Int position)
    {
        int childCount = parent.childCount;
        Vector3 cellPosition = grid.LocalToWorld(grid.CellToLocalInterpolated(position));

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (Mathf.Approximately(child.position.x, cellPosition.x) &&
                Mathf.Approximately(child.position.y, cellPosition.y) &&
                Mathf.Approximately(child.position.z, cellPosition.z))
            {
                return child;
            }
        }

        return null;
    }
}


#endif