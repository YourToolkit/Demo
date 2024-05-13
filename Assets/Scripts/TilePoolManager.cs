using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyGridSystem;
using Unity.VisualScripting;
using UnityEngine;
using MyTiles;

public class TilePoolManager : MonoBehaviour
{
    public static List<PooledTileInfo> TilePools = new List<PooledTileInfo>();

    public static GridTileBase SpawnTile(GridTileBase tileToSpawn, Vector3 spawnPosition,
        Quaternion spawnRotation, Transform spawnParTransform, TileBaseType tileBaseType)
    {
        PooledTileInfo pool = TilePools.Find(p => p.TileBaseType == tileBaseType);

        // if the pool does not exist, create it 
        if (pool == null)
        {
            pool = new PooledTileInfo() { TileBaseType = tileBaseType };
            Debug.Log(tileBaseType);
            TilePools.Add(pool);
        }

        // check if there are any inactive tiles in pool

        GridTileBase spawnableTile = pool.InactiveObjects.FirstOrDefault();
        if (spawnableTile == null)
        {
            // if there are no inactive objects, create a new one
            Debug.Log("Creating new tile");
            spawnableTile = Instantiate(tileToSpawn, spawnPosition, spawnRotation, spawnParTransform);
        }

        else
        {
            Debug.Log("maybe That is the problem");
            var spawnableTileTransform = spawnableTile.transform;
            spawnableTileTransform.parent = spawnParTransform;
            spawnableTileTransform.position = spawnPosition;
            spawnableTileTransform.rotation = spawnRotation;
            pool.InactiveObjects.Remove(spawnableTile);
            spawnableTile.gameObject.SetActive(true);
            Debug.Log(spawnableTile.name);
        }

        return spawnableTile;
    }

    public static void ReturnTileToPool(GridTileBase tile)
    {
        PooledTileInfo pool = TilePools.Find(p => p.TileBaseType == tile.TileData.TileBaseType);
        if (pool == null)
        {
            Debug.LogWarning("Trying to release an object that is not pooled: " + tile.TileData.TileBaseType);
        }
        else
        {
            tile.gameObject.SetActive(false);
            pool.InactiveObjects.Add(tile);
        }
    }
}

public class PooledTileInfo
{
    public TileBaseType TileBaseType;
    public List<GridTileBase> InactiveObjects = new List<GridTileBase>();
}