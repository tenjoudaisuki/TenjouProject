﻿using UnityEngine;
using System.Collections;

public class SkyBoxUVScroll : MonoBehaviour {

    [SerializeField]
    private float[] scrollSpeedX;

    [SerializeField]
    private float[] scrollSpeedY;
    public float radius = 0.3f;

    float rad;

    void Start()
    {
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
    }

    void Update()
    {
        //rad += Time.deltaTime * 0.1f;
        //var x = Mathf.Cos(rad) * radius;
        //var y = Mathf.Sin(rad) * radius;

        //var offset = new Vector2(x, y);

        var skr = transform.GetComponent<MeshRenderer>();
        var materials = skr.materials;
        int i = 0;
        foreach (var material in materials)
        {
            var x = Mathf.Repeat(Time.time * scrollSpeedX[i], 1);
            var y = Mathf.Repeat(Time.time * scrollSpeedY[i], 1);

            var offset = new Vector2(x, y);

            material.SetTextureOffset("_MainTex", offset);
            i++;
        }

    }
}
