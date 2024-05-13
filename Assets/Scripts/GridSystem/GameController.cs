using System.Collections.Generic;
using MyTiles;
using UnityEngine;

namespace MyGridSystem
{
    public class GameController : MonoBehaviour
    {
        private Grid _grid;
        private Camera _cam;
        private Vector3Int _cellPosition;
        private List<WalkableTile> _walkableTiles;
        private WalkableTile _playerWalkableTileBase, _goalTileBase;
        private Player _playerTile;

        [SerializeField] private CurrentState _currentState;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
            _cam = Camera.main;
            if (_currentState == null)
                Debug.LogWarning("You have not assign the Scriptable object in currentState, please assign to it.");
        }

        private void Start()
        {
            _playerTile = FindPlayer();
            _walkableTiles = GetAllWalkableTiles();
            foreach (var walkableTile in _walkableTiles)
            {
                walkableTile.transform.GetComponentInChildren<PolygonCollider2D>().enabled = true;
                walkableTile.CacheNeighbors();
            }

            WalkableTile.OnHoverTile += OnTileHover;
            //需删除
            _playerWalkableTileBase = GetTileUnderPlayer();
        }


        private void OnDisable() => WalkableTile.OnHoverTile -= OnTileHover;

        private Player FindPlayer()
        {
            var playerTile = transform.GetComponentInChildren<Player>();
            return playerTile;
        }

        private List<WalkableTile> GetAllWalkableTiles()
        {
            var walkableTiles = new List<WalkableTile>();
            var highestZTiles =
                new Dictionary<Vector2Int, WalkableTile>();
            foreach (var tileDatas in _currentState.TileDictList)
            {
                foreach (var tileData in tileDatas)
                {
                    var walkableTile = tileData.Value.TileBase as WalkableTile;
                    if (walkableTile != null)
                    {
                        var xy = new Vector2Int(tileData.Key.x, tileData.Key.y);
                        if (!highestZTiles.ContainsKey(xy) || tileData.Key.z > highestZTiles[xy].GetTileCoordinate().z)
                        {
                            highestZTiles[xy] = walkableTile;
                        }
                    }
                }
            }

            walkableTiles.AddRange(highestZTiles.Values);
            return walkableTiles;
        }

        //这个function日后需要删除，现在暂时用于测试
        private WalkableTile GetTileUnderPlayer()
        {
            foreach (var walkableTile in _walkableTiles)
            {
                var xy = new Vector2Int(walkableTile.GetTileCoordinate().x, walkableTile.GetTileCoordinate().y);
                if (xy.x == _playerTile.GetTileCoordinate().x && xy.y == _playerTile.GetTileCoordinate().y)
                    return walkableTile;
            }

            Debug.LogWarning("cannot find any player or player is not placed by any tiles");
            return null;
        }

        private void OnTileHover(WalkableTile tileBase)
        {
            _goalTileBase = tileBase;
            _playerWalkableTileBase = GetTileUnderPlayer();
            var path = Pathfinding.FindPath(_playerWalkableTileBase, _goalTileBase);
            StartCoroutine(_playerTile.MoveAlongPath(path));
        }
    }
}