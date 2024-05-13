using System.Collections.Generic;
using MyTiles;
using UnityEngine;

[CreateAssetMenu(fileName = "GridConfigDatabase", menuName = "Config Database", order = 1)]
public class GridConfigDatabase : ScriptableObject
{
    public GridTileBase[] Prefabs;
}