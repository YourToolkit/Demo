using System.Collections.Generic;
using UnityEngine;
using MyTiles;

[System.Serializable]
public class TilemapInfo
{
    public int TilemapIndex;
    public List<Vector3Int> TileCellPositions;
    public List<BaseTileData> TileDataset;
}