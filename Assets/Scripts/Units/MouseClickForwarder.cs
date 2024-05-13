using System;
using MyTiles;
using UnityEngine;

public class MouseClickForwarder : MonoBehaviour
{
    [SerializeField] private WalkableTile _walkableTile;

    private void Awake()
    {
        if (!_walkableTile) Debug.LogWarning("You have not assign your walkable tile");
    }

    private void OnMouseDown()
    {
        _walkableTile.OnHoverTileInvoker();
    }
}