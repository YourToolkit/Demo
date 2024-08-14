using System;
using System.Collections.Generic;
using System.Linq;
using MyGridSystem;
using MyMapManager;
using MyToolSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
using MyTiles;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

[Serializable]
[ShowOdinSerializedPropertiesInInspector]
[CreateAssetMenu(fileName = "TempMapData", menuName = "Map Data", order = 0)]
public class CurrentState : ScriptableObject
{
    /// <summary>
    /// 用于存储所有的Tilemap以及对应的字典
    /// </summary>
    public List<Dictionary<Vector3Int, TileInfo>> TileDictList = new List<Dictionary<Vector3Int, TileInfo>>();

    public List<Tilemap> TilemapList = new List<Tilemap>();
    public int CurrentZPosition;

    /// <summary>
    /// 当前的 Tilemap以及对应的字典currentTiles
    /// </summary>
    public Tilemap CurrentTileMap;

    public Dictionary<Vector3Int, TileInfo> CurrentTileMapDict;

    public TileBaseType CurrentTileBaseType;
    public MyToolSystem.Tool CurrentTool;
    public GridTileBase[] TilePrefabs;
    public GridTileBase Tile;
    public GameMode GameMode;
    public bool IsDragging;

    public MapManager MapManager; 

    public void Reset()
    { 
        TileDictList = new List<Dictionary<Vector3Int, TileInfo>>();
        TilemapList = new List<Tilemap>();
        CurrentZPosition = 0;
        CurrentTileMap = null;
        CurrentTileMapDict = null;
        CurrentTool = new BrushTool(this);
        IsDragging = false;
        if (TilePrefabs == null || TilePrefabs.Length == 0)
        {
            Debug.LogWarning("Please config your grid tile bases first");
            return;
        }

        CurrentTileBaseType = TilePrefabs.First().TileData.TileBaseType;
        Tile = TilePrefabs.First();
    }

    public void OnValidate()
    {
        if (TilePrefabs == null || TilePrefabs.Length == 0)
        {
            Debug.LogWarning("Please config your grid tile bases first");
            return;
        }

        if (MapManager == null)
        {
            Reset();
            Debug.LogWarning("Please create assign the map manager to the current state or load map data");
            return;
        }

        Tile = TilePrefabs?.First(c => c.TileData.TileBaseType == CurrentTileBaseType);
    }


    private void OnEnable()
    {
        CurrentTool = new BrushTool(this);
    }

    private void OnDisable()
    {
        Reset();
    }

    public GridTileBase FindTileBase(Vector3Int coordinate)
    {
        if (CurrentTileMapDict.TryGetValue(coordinate, out TileInfo tileInfo))
        {
            return tileInfo.TileBase;
        }

        //Debug.LogWarning("Cannot find any tile base");
        return null;
    }
}