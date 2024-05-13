using MyGridSystem;
using MyTiles;
using UnityEngine;

public interface IPreview
{
    GridTileBase PreviewObj { get; set; }
    void Preview(Vector3Int coordinate);
    void UnPreview();
    
}