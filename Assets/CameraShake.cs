using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraShake : MonoBehaviour
{
    private float _trauma;
    public float TraumaDecrease = 1f;
    public float MaxOffset = 0.5f;
    public float MaxAngle = 5f;
    public float Frequency = 1f;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    private void Start()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
    }

    private void Update()
    {
        if (_trauma > 0)
        {
            _trauma -= Time.deltaTime * TraumaDecrease;
            if (_trauma < 0)
            {
                _trauma = 0;
            }

            float seed = Time.time * Frequency;
            float x = _initialPosition.x + MaxOffset * (Mathf.PerlinNoise(seed, 0) * 2 - 1) * _trauma;
            float y = _initialPosition.y + MaxOffset * (Mathf.PerlinNoise(0, seed +1) * 2 - 1) * _trauma;
            transform.localPosition = new Vector3(x, y, _initialPosition.z);

            float angle = MaxAngle * (Mathf.PerlinNoise(seed+2, 0) * 2 - 1) * _trauma;
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