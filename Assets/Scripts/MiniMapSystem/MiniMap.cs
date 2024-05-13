using System;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEditor;
using UnityEngine.Serialization;

namespace MiniMapSystem
{
    public class MiniMap : MonoBehaviour
    {
        // [FormerlySerializedAs("_currentEditorState")] [SerializeField]
        // private CurrentState _currentState;
        //
        // private Texture2D _miniMap;
        //
        // private void Awake()
        // {
        //     _miniMap = new Texture2D(50, 50);
        // }
        //
        // private void Update()
        // {
        //     if (Input.GetKeyDown(KeyCode.M))
        //     {
        //         GenerateMiniMap();
        //     }
        // }
        //
        // public void GenerateMiniMap()
        // {
        //     var currentInfo = _currentState.CurrentTileMapDict;
        //     foreach (var tileInfo in currentInfo)
        //     {
        //         var tilePos = new Vector2Int(tileInfo.Key.x, tileInfo.Key.y);
        //         var tileName = tileInfo.Value;
        //         var pixColor = _currentState.GridConfigDatabase.Prefabs
        //             .First(c => c.TileData.TileBaseType == _currentState.CurrentTileBaseType)
        //             .TextureMapColor;
        //         _miniMap.SetPixel(tilePos.x, tilePos.y, pixColor);
        //         _miniMap.Apply();
        //     }
        //
        //     byte[] bytes = _miniMap.EncodeToPNG();
        //     string path = Application.dataPath + "/miniMap.png";
        //     File.WriteAllBytes(path, bytes);
        //     AssetDatabase.Refresh();
        // }
    }
}