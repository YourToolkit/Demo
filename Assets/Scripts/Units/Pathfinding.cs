using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyTiles;

public static class Pathfinding
{
    private static readonly Color PathColor = new Color(0.65f, 0.35f, 0.35f);

    public static List<WalkableTile> FindPath(WalkableTile startNode, WalkableTile targetNode)
    {
        var toSearch = new List<WalkableTile>() { startNode };
        var processed = new List<WalkableTile>();
        foreach (var tiles in startNode.CurrentState.TileDictList)
        {
            foreach (var tile in tiles)
            {
                var tileBase = tile.Value.TileBase as WalkableTile;
                if (tileBase != null)
                {
                    tileBase.ResetCost();
                }
            }
        }

        while (toSearch.Any())
        {
            var current = toSearch[0];
            foreach (var t in toSearch)
                if (t.F < current.F || t.F == current.F && t.H < current.H)
                    current = t;

            processed.Add(current);
            toSearch.Remove(current);

            if (current == targetNode)
            {
                var currentPathTile = targetNode;
                var path = new List<WalkableTile>();
                var count = 100;
                while (currentPathTile != startNode)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.Connection;
                    count--;
                    if (count < 0) throw new Exception();
                }

                foreach (var tile in path) tile.SetColor(PathColor);
                startNode.SetColor(PathColor);
                return path;
            }

            foreach (var neighbor in current.Neighbors.Where(t =>
                         (t.TileData as WalkableTileData).Walkable && !processed.Contains(t)))
            {
                var inSearch = toSearch.Contains(neighbor);

                var costToNeighbor = current.G + current.GetDistance(neighbor);

                if (!inSearch || costToNeighbor < neighbor.G)
                {
                    neighbor.SetG(costToNeighbor);
                    neighbor.SetConnection(current);

                    if (!inSearch)
                    {
                        neighbor.SetH(neighbor.GetDistance(targetNode));
                        toSearch.Add(neighbor);
                    }
                }
            }
        }

        Debug.LogWarning("No Path Found");
        return null;
    }
}