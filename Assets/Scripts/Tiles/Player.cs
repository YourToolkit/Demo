using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace MyTiles
{
    public class Player : NormalTile
    {
        [SerializeField] private float _duration = 0.3f;

        public IEnumerator MoveAlongPath(List<WalkableTile> path)
        {
            path.Reverse();
            foreach (var tile in path)
            {
                var coordinate = tile.GetTileCoordinate();
                var position = GetTileWorldPosition(coordinate, CurrentState);
                position.z = position.z + 2;
                Debug.Log(position);
                transform.DOMove(position, _duration); // if you don't want to use dotween, just delete it.
                coordinate.z += 2;
                this.Init(coordinate, TileData.TileBaseType,
                    CurrentState.CurrentTileMap.GetComponent<TilemapRenderer>().sortingOrder);
                yield return new WaitForSeconds(_duration);
            }
        }
    }
}