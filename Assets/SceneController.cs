using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class SceneController : MonoBehaviour
{
    public float StartValue = 16f;
    public float Duration = 1.0f;
    public float EndValue = 0.0f;


    void Start()
    {
        transform.position = new Vector3(0, StartValue, 0);
        transform.DOMoveY(EndValue, Duration)
            .SetEase(Ease.OutElastic);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //restart the scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
}