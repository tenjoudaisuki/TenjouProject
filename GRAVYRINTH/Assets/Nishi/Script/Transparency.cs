using UnityEngine;
using System.Collections;

public class Transparency : MonoBehaviour {

    /// <summary>
    /// 透明開始距離
    /// </summary>
    public float m_StartDistance = 1f;
    /// <summary>
    /// 透明が最小になる距離
    /// </summary>
    public float m_AlphaMinDistance = 0.1f;
    /// <summary>
    /// 透明度の最小値
    /// </summary>
    public float m_MinAlpha = 0.1f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        float distance = Vector3.Distance(transform.position, cameraPos);

        var skr = transform.GetComponent<SkinnedMeshRenderer>();
        var materials = skr.materials;
        float f = (distance / m_StartDistance);

        if (distance <= m_StartDistance)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i == 3) return;
                Color color = materials[i].color;
                color.a = Mathf.Lerp(-2.0f, 1.0f, f);
                materials[i].color = color;
            }
        }
        if(distance < m_AlphaMinDistance)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i == 3) return;
                Color color = materials[i].color;
                color.a = m_MinAlpha;
                materials[i].color = color;
            }
        }
        if(distance > m_StartDistance)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i == 3) return;
                Color color = materials[i].color;
                color.a = 1.0f;
                materials[i].color = color;
            }
        }


        //if (distance <= m_StartDistance)
        //{
        //    var skr = transform.GetComponent<SkinnedMeshRenderer>();
        //    var materials = skr.materials;
        //    float f = (distance / m_StartDistance);
        //    for(int i = 0;i < 4;i++)
        //    {
        //        if (i == 3) return;
        //        Color color = materials[i].color;
        //        color.a = Mathf.Lerp(0.0f,1.0f,f) / 2;
        //        materials[i].color = color;
        //    }
        //}
        //else
        //{
        //    var skr = transform.GetComponent<SkinnedMeshRenderer>();
        //    var materials = skr.materials;
        //    float f = (distance / m_StartDistance);
        //    for (int i = 0; i < 4; i++)
        //    {
        //        if (i == 3) return;
        //        Color color = materials[i].color;
        //        color.a = 1;
        //        materials[i].color = color;
        //    }
        //}
	}
}
