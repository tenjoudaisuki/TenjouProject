﻿using UnityEngine;
using System.Collections;

public class ImageRotation : MonoBehaviour
{
    private RectTransform rectTr;
    private bool isStop = false;

    public Vector3 axis = Vector3.zero;
    public float angle = 45.0f;

    void Start()
    {
        rectTr = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isStop == true) return;

        rectTr.Rotate(axis, angle * Time.deltaTime);

        //if (rectTr.rotation.z > 360.0f)
        //    rectTr.rotation = Quaternion.AxisAngle(axis, 0.0f);
    }

    public void RotateStop()
    {
        isStop = true;
    }

    public void RotateStart()
    {
        isStop = false;
    }
}
