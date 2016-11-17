using UnityEngine;
using System.Collections;

public class SkyBoxUVScroll : MonoBehaviour {

    [SerializeField]
    private float scrollSpeedX = 0.1f;

    [SerializeField]
    private float scrollSpeedY = 0.1f;
    public float radius = 0.3f;

    float rad;

    void Start()
    {
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
    }

    void Update()
    {
        rad += Time.deltaTime * 0.1f;
        var x = Mathf.Cos(rad) * radius;
        var y = Mathf.Sin(rad) * radius;

        var offset = new Vector2(x, y);

        var skr = transform.GetComponent<MeshRenderer>();
        var materials = skr.materials;
        foreach (var material in materials)
        {
            material.SetTextureOffset("_MainTex", offset);
        }

    }
}
