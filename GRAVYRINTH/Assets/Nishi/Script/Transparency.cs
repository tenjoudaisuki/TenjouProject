using UnityEngine;
using System.Collections;

public class Transparency : MonoBehaviour {

    /// <summary>
    /// 透明開始距離
    /// </summary>
    public float m_Start = 1f;
    /// <summary>

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float distance = Vector3.Distance(transform.position, cameraPos);
        if (distance <= m_Start)
        {
            var skr = transform.GetComponent<SkinnedMeshRenderer>();
            var materials = skr.materials;
            float f = (distance / m_Start);
            for(int i = 0;i < 4;i++)
            {
                if (i == 3) return;
                Color color = materials[i].color;
                color.a = Mathf.Clamp(f, 0.2f, 1.0f) / 2;
                materials[i].color = color;
            }
        }
	}
}
