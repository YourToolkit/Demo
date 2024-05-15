using System;
using System.Collections.Generic;
using System.Linq;
using MyMapManager;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using MyTiles;
using UnityEditor;

namespace MyGridSystem
{
    public class GridSystem : MonoBehaviour
    {
        [SerializeField] private CurrentState _currentState;
        [SerializeField] private MapManager _mapManager;
        private Grid _grid;
        private Camera _cam;
        private Vector3Int _cellPosition;
        private GridTileBase _cursorTile;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
            _cam = Camera.main;
            // if currentState GameMode is EditorMode, then AddNewTilemap()
            if (_currentState.GameMode == GameMode.EditorMode)
            {
                if (!_mapManager)
                {
                    AddNewTileMap();
                    _mapManager.TileDictList = _currentState.TileDictList;
                    _mapManager.TileMapList = _currentState.TilemapList;
                    _mapManager.CurrentTileMapDict = _currentState.CurrentTileMapDict;
                    _mapManager.CurrentTileMap = _currentState.CurrentTileMap;
                }
                else
                {
                    LoadMapData();
                }
            }

            // if currentState GameMode is PlayMode, then LoadMapData()
            if (_currentState.GameMode == GameMode.PlayMode)
                LoadMapData();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _currentState.GameMode = GameMode.PlayMode;
                //restart the scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                _currentState.GameMode = GameMode.EditorMode;
                //restart the scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (_currentState.GameMode != GameMode.EditorMode)
                return;
            _cellPosition = _grid.WorldToCell(_cam.ScreenToWorldPoint(Input.mousePosition));
            _cellPosition.z = _currentState.CurrentZPosition;


            DisplayCursor(_cellPosition);


            if (Input.GetMouseButtonDown(0))
            {
                _currentState.CurrentTool.OnToolMouseDown(_cellPosition);
                _currentState.IsDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _currentState.IsDragging = false;
                _currentState.CurrentTool.OnToolMouseUp(_cellPosition);
            }

            if (_currentState.IsDragging)
            {
                _currentState.CurrentTool.OnToolMouseDrag(_cellPosition);
                return;
            }
            else
            {
                _currentState.CurrentTool.OnToolMouseUnClicked(_cellPosition);
            }

            // all of these just for testing, need to be deleted in the future
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SwitchTilemap();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                AddNewTileMap();
            }

            //use the key "S" to save the map, and the key "L" to load the map
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("haha");
                SaveMapData();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadMapData();
            }
        }

        private void DisplayCursor(Vector3Int cellPosition)
        {
            var location = GridTileBase.GetTileWorldPosition(cellPosition, _currentState);
            if (_cursorTile == null)
            {
                _cursorTile = Instantiate(
                    _currentState.TilePrefabs.First(c => c.TileData.TileBaseType == TileBaseType.Cursor)
                    ,
                    location, quaternion.identity,
                    transform);
                _cursorTile.Init(cellPosition, TileBaseType.Cursor, sortingOrder: 1000);
            }

            _cursorTile.transform.position = location;
        }


        /// <summary>
        /// 创建一个tilemap Gameobject, gameobject 有tilemap这个component
        ///  tilemap的父transform为transform, tilemap 中的sortingOrder = tilemapList.count
        /// </summary>
        private void CreateTilemap()
        {
            if (_mapManager == null)
            {
                _mapManager = new GameObject(SceneManager.GetActiveScene().name).AddComponent<MapManager>();
                _mapManager.transform.SetParent(transform);
            }

            var newTilemap = new GameObject("Tilemap").AddComponent<Tilemap>();
            newTilemap.transform.SetParent(_mapManager.transform);
            newTilemap.gameObject.AddComponent<TilemapRenderer>().sortingOrder =
                _currentState.TilemapList.Count;
            _currentState.TilemapList.Add(newTilemap);
            _currentState.CurrentTileMap = newTilemap;
            _mapManager.CurrentTileMap = _currentState.CurrentTileMap;
        }


        private void AddNewTileMap()
        {
            CreateTilemap();
            _currentState.TileDictList.Add(new Dictionary<Vector3Int, TileInfo>());
            _currentState.CurrentTileMapDict =
                _currentState.TileDictList[^1];
            _mapManager.CurrentTileMapDict = _currentState.CurrentTileMapDict;
        }


        private void SwitchTilemap()
        {
            int currentIndex = _currentState.TilemapList.IndexOf(_currentState.CurrentTileMap);
            int nextIndex = (currentIndex + 1) % _currentState.TilemapList.Count;
            _currentState.CurrentTileMap = _currentState.TilemapList[nextIndex];
            _currentState.CurrentTileMapDict = _currentState.TileDictList[nextIndex];
        }


        #region Save and Load Map

        public void SaveMapData()
        {
            if (_mapManager)
            {
                _mapManager.TileMapList = _currentState.TilemapList;
                _mapManager.TileDictList = _currentState.TileDictList;
                _mapManager.CurrentTileMap = _currentState.TilemapList.Last();
                _mapManager.CurrentTileMapDict = _currentState.TileDictList.Last();
            }

            var dataPath = "Assets/MapData/" + SceneManager.GetActiveScene().name + ".json";
            MapIO.SaveMapData(dataPath, _currentState);
        }

        public void LoadMapData()
        {
            if (_mapManager)
            {
                _currentState.TilemapList = _mapManager.TileMapList;
                _currentState.TileDictList = _mapManager.TileDictList;
                _currentState.CurrentTileMap = _mapManager.TileMapList.Last();
                _currentState.CurrentTileMapDict = _mapManager.TileDictList.Last();
            }

            var dataPath = "Assets/MapData/" + SceneManager.GetActiveScene().name + ".json";
            var mapData = MapIO.LoadMapData(dataPath);
            GenerateSavedMap(mapData);
        }

        private void GenerateSavedMap(MapData mapData)
        {
            var temp = GenerateTileDictList(mapData);
            ProcessDifferenceFromTileMap(temp);
            for (int i = 0; i < temp.Count; i++)
            {
                _currentState.CurrentTileMap = _currentState.TilemapList[i];
                _currentState.CurrentTileMapDict = _currentState.TileDictList[i];
                var dict1 = temp[i];
                var dict2 = _currentState.TileDictList[i];
                var savedDataIds = dict1.Select(pair => pair.Value.TileData.Id).ToList();
                var mapManagerIds = dict2.Select(pair => pair.Value.TileData.Id).ToList();
                ProcessDifferenceFromMapManagerData(mapManagerIds, savedDataIds, dict2);
                ProcessDifferenceFromSavedData(savedDataIds, mapManagerIds, dict1);
            }

            _mapManager.TileDictList = _currentState.TileDictList;
            _mapManager.TileMapList = _currentState.TilemapList;
        }

        private void ProcessDifferenceFromSavedData(List<string> savedDataIds, List<string> mapManagerIds,
            Dictionary<Vector3Int, TileInfo> dict)
        {
            var diff = savedDataIds.Except(mapManagerIds).ToList();
            foreach (var Id in diff)
            {
                Debug.Log("Process Difference from save Data");
                var tilesToGenerate = dict.Where(pair => pair.Value.TileData.Id == Id);
                GenerateTiles(tilesToGenerate);
            }
        }

        private void ProcessDifferenceFromMapManagerData(List<string> mapManagerIds, List<string> savedDataIds,
            Dictionary<Vector3Int, TileInfo> dict)
        {
            var diff = mapManagerIds.Except(savedDataIds);
            foreach (var Id in diff)
            {
                Debug.Log("Process Difference from map manager Data");
                var tileDataToDestroy = dict.First(pair => pair.Value.TileData.Id == Id);
                var tileToDestroy = tileDataToDestroy.Value.TileBase;
                DestroyImmediate(tileToDestroy.gameObject);
                _currentState.CurrentTileMapDict.Remove(tileDataToDestroy.Key);
            }
        }

        private void ProcessDifferenceFromTileMap(List<Dictionary<Vector3Int, TileInfo>> temp)
        {
            if (temp.Count != _currentState.TileDictList.Count)
            {
                Debug.Log("Process Difference from tile map");
                var countDiff = temp.Count - _currentState.TileDictList.Count;
                if (countDiff > 0)
                {
                    for (int i = 0; i < countDiff; i++)
                    {
                        AddNewTileMap();
                        GenerateTiles(temp[_currentState.TileDictList.Count - 1]);
                    }
                }
                else if (countDiff < 0)
                {
                    for (int i = 0; i < Math.Abs(countDiff); i++)
                    {
                        var index = _currentState.TilemapList.Count - i;
                        DestroyImmediate(_currentState.TilemapList[index].gameObject);
                        _currentState.TilemapList.RemoveAt(index);
                        _currentState.TileDictList.RemoveAt(index);
                    }
                }
            }
        }


        public void RecordTile(Vector3Int cellPosition, GridTileBase tileBase)
        {
            _currentState.CurrentTileMapDict[cellPosition] = new TileInfo(tileBase: tileBase);
        }

        private void GenerateTiles(IEnumerable<KeyValuePair<Vector3Int, TileInfo>> diff)
        {
            foreach (var tile in diff)
            {
                var coordinate = tile.Key;
                var tileInfo = tile.Value;
                var location = GridTileBase.GetTileWorldPosition(coordinate, _currentState);
                var tileBase = _currentState.TilePrefabs
                    .First(c => c.TileData.TileBaseType == tileInfo.TileData.TileBaseType);
                var spawned = TilePoolManager.SpawnTile(tileBase,
                    location,
                    Quaternion.identity, _currentState.CurrentTileMap.transform, tileInfo.TileData.TileBaseType);

                spawned.Init(coordinate, tileInfo.TileData,
                    sortingOrder: _currentState.CurrentTileMap.GetComponent<TilemapRenderer>().sortingOrder);
                spawned.name = $"{spawned.TileData.TileBaseType} ({coordinate.x}, {coordinate.y}, {coordinate.z})";
                RecordTile(coordinate, spawned);
            }
        }

        private List<Dictionary<Vector3Int, TileInfo>> GenerateTileDictList(MapData mapData)
        {
            var tileDictList = new List<Dictionary<Vector3Int, TileInfo>>();
            foreach (var tilemapInfo in mapData.TilemapInfoList)
            {
                tileDictList.Add(tilemapInfo.TileCellPositions
                    .Zip(tilemapInfo.TileDataset,
                        (position, tileData) => new { position, tileInfo = new TileInfo(tileData) })
                    .ToDictionary(x => new Vector3Int(x.position.x, x.position.y, x.position.z), x => x.tileInfo));
            }

            return tileDictList;
        }

        #endregion


        private void DestroyAllTiles()
        {
            foreach (var tilemap in _currentState.TilemapList)
            {
                DestroyImmediate(tilemap.gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            _currentState.Reset();
        }
    }
}