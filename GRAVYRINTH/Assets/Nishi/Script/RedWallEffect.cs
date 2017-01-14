using UnityEngine;
using System.Collections;

public class RedWallEffect : MonoBehaviour
{

    GameObject mCamera;
    public float mMax;
    public float mMin;

    // Use this for initialization
    void Start()
    {
        mCamera = GameObject.Find("Camera");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = mCamera.transform.position;

        float distance = Vector3.Distance(transform.position, cameraPos);
        distance = Mathf.Clamp(distance, mMin,mMax);
        float f = ((distance - mMin) / (mMax - mMin));

        var skr = transform.GetComponent<MeshRenderer>();
        var material = skr.material;

        Color color = material.GetColor("_TintColor");
        color.a = Mathf.Lerp(0.5f, 0.1f, f);
        material.SetColor("_TintColor", color);

        var chaildRenderer = gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in chaildRenderer)
        {
            renderer.material.SetColor("_TintColor", color);
        }

    }
}