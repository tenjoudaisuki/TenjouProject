﻿using UnityEngine;
using System.Collections;

public class WallBreak : MonoBehaviour {

    public void Break()
    {
        var walls = transform.GetComponentsInChildren<Rigidbody>();
        foreach(var wall in walls)
        {
            wall.isKinematic = false;
            wall.interpolation = RigidbodyInterpolation.Interpolate;
            wall.AddForceAtPosition(new Vector3(Random.Range(-50, 50), Random.Range(20, 50), Random.Range(50,100)), transform.position, ForceMode.Impulse);
        }
    }
}
