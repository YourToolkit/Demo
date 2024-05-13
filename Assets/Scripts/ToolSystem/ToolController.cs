using System;
using MyGridSystem;
using MyToolSystem;
using UnityEngine;

public class ToolController : MonoBehaviour
{
    [SerializeField] private CurrentState _currentState;
    private Camera _cam;

    [Header("Pan tool parameter")] [SerializeField]
    private float _panSpeed = 20f;

    private Vector3 _lastPanPosition;

    [Header("Zoom tool parameter")] [SerializeField]
    private float _zoomSpeed = 10f;

    private float _targetZoom;


    private void Awake()
    {
        _cam = Camera.main;
        _targetZoom = _cam.orthographicSize;
    }

    private void Update()
    {
        if (_currentState.IsDragging || _currentState.GameMode != GameMode.EditorMode)
            return;
        var scrollData = Input.GetAxis("Mouse ScrollWheel");
        _targetZoom -= scrollData * _zoomSpeed;
        _targetZoom = Mathf.Clamp(_targetZoom, 4.5f, 20.0f);
        _cam.orthographicSize =
            Mathf.Lerp(_cam.orthographicSize, _targetZoom, Time.deltaTime * _zoomSpeed);
        if (Input.GetMouseButtonDown(2))
        {
            _lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2))
        {
            PanCamera(Input.mousePosition);
        }


        if (Input.GetKeyDown(KeyCode.B))
        {
            _currentState.CurrentTool.UnPreview();
            _currentState.CurrentTool = new BrushTool(_currentState);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            _currentState.CurrentTool.UnPreview();
            _currentState.CurrentTool = new RectangleTool(_currentState);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            _currentState.CurrentTool.UnPreview();
            _currentState.CurrentTool = new EraserTool(_currentState);
        }
    }

    private void PanCamera(Vector3 newPanPosition)
    {
        var offset = _cam.ScreenToViewportPoint(_lastPanPosition - newPanPosition);

        var move = new Vector3(offset.x * _panSpeed, offset.y * _panSpeed, 0);

        _cam.transform.Translate(move, Space.World);
        _lastPanPosition = newPanPosition;
    }
}