using System.Collections;
using System.Collections.Generic;
using MyGridSystem;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform Target;
    public float SmoothSpeed = 0.1f;

    private float _trauma;
    public float TraumaDecrease = 1f;
    public float MaxOffset = 0.5f;
    public float MaxAngle = 5f;
    public float Frequency = 1f;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    [SerializeField] private CurrentState _currentState;

    private void Start()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
    }

    private void FixedUpdate()
    {
        if (_currentState.GameMode == GameMode.EditorMode)
        {
            return;
        }

        if (Target != null)
        {
            var difference = Target.position - transform.position;
            _initialPosition += new Vector3(difference.x * SmoothSpeed, difference.y * SmoothSpeed, 0);
        }
    }

    private void Update()
    {
        if (_currentState.GameMode == GameMode.EditorMode)
        {
            return;
        }

        if (_trauma > 0)
        {
            _trauma -= Time.deltaTime * TraumaDecrease;
            if (_trauma < 0)
            {
                _trauma = 0;
            }

            float seed = Time.time * Frequency;
            float x = MaxOffset * (Mathf.PerlinNoise(seed, 0) * 2 - 1) * _trauma;
            float y = MaxOffset * (Mathf.PerlinNoise(0, seed + 1) * 2 - 1) * _trauma;
            transform.localPosition = _initialPosition + new Vector3(x, y, 0);

            float angle = MaxAngle * (Mathf.PerlinNoise(seed + 2, 0) * 2 - 1) * _trauma;
            transform.localRotation =
                Quaternion.Euler(_initialRotation.eulerAngles.x, _initialRotation.eulerAngles.y, angle);
        }
        else
        {
            transform.localPosition = _initialPosition;
            transform.localRotation = _initialRotation;
        }
    }

    public void AddTrauma(float amount)
    {
        _trauma = Mathf.Clamp01(_trauma + amount);
    }
}