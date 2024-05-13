using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{
    public Transform Target;
    public float SmoothSpeed = 0.1f;

    private void FixedUpdate()
    {
        if (Target != null)
        {
            var difference = Target.position - transform.position;
            transform.position += new Vector3(difference.x * SmoothSpeed, difference.y * SmoothSpeed, 0);
        }
    }
}