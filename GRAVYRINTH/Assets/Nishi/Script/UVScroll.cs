using UnityEngine;
using System.Collections;

public class UVScroll : MonoBehaviour
{

    [SerializeField]
    private float scrollSpeedX = 0.1f;

    [SerializeField]
    private float scrollSpeedY = 0.1f;

    void Start()
    {
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", Vector2.zero);
    }

    void Update()
    {
        var x = Mathf.Repeat(Time.time * scrollSpeedX, 1);
        var y = Mathf.Repeat(Time.time * scrollSpeedY, 1);

        var offset = new Vector2(x, y);

        var skr = transform.GetComponent<SkinnedMeshRenderer>();
        var materials = skr.materials;
        materials[1].SetTextureOffset("_MainTex", offset);
    }
}
