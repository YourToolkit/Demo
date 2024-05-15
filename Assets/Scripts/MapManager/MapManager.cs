using System;
using System.Collections.Generic;
using MyGridSystem;
using MyTiles;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MyMapManager
{
    //请不要用于任何地方，这仅仅只是一个暂存数据，你可以作为map data的参考，但只能是参考，如果你需要改变数据，用Currentstate
    public class MapManager : SerializedMonoBehaviour
    {
        [Header("请不要将以下的数据应用于任何地方")] [Header("如果你想要访问地图的数据信息，请用current state")]
        public List<Dictionary<Vector3Int, TileInfo>> TileDictList;

        public List<Tilemap> TileMapList;
        public Dictionary<Vector3Int, TileInfo> CurrentTileMapDict;
        public Tilemap CurrentTileMap;
        public CurrentState CurrentState;

        private void OnValidate()
        {
            if (CurrentState == null)
            {
                Debug.LogWarning("CurrentState has not been set in the Inspector");
                return;
            }

            if (TileMapList == null || TileMapList.Count == 0)
            {
                return;
            }

            Debug.Log("OnValidate");
            CurrentState.TilemapList = TileMapList;
            CurrentState.TileDictList = TileDictList;
            CurrentState.CurrentTileMap = CurrentTileMap;
            CurrentState.CurrentTileMapDict = CurrentTileMapDict;
            CurrentState.MapManager = this;
        }

        private void FadeInactiveLayers()
        {
            if (CurrentTileMap == null || CurrentState.GameMode != GameMode.EditorMode)
            {
                return;
            }


            foreach (var layerData in TileDictList)
            {
                if (layerData == CurrentTileMapDict)
                {
                    foreach (var tileInfo in layerData.Values)
                    {
                        tileInfo.TileBase.SpriteRenderer.color = new Color(1, 1, 1, 1);
                    }
                }
                else
                {
                    foreach (var tileInfo in layerData.Values)
                    {
                        tileInfo.TileBase.SpriteRenderer.color = new Color(1, 1, 1, 0.5f);
                    }
                }
            }
        }
    }
}