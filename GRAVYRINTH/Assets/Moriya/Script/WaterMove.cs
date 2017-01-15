/**==========================================================================*/
/**
 * 水面の移動、水面関係の制御はここで行う
 * 作成者：守屋   作成日：16/11/25
/**==========================================================================*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaterMove : MonoBehaviour
{
    Transform tr;
    MeshRenderer mr;


    private Plane m_Plane;
    private Image m_Image;
    private Color m_Color;
    private float m_Timer;
    //子オブジェクトのWaterBack
    private GameObject m_WaterBack;

    [SerializeField, Tooltip("移動速度最大値")]
    private float m_SpeedMax = 0.02f;
    [SerializeField, Tooltip("移動速度最小値")]
    private float m_SpeedMin = 0.008f;
    [SerializeField, Tooltip("原点からの距離")]
    private float m_Length = 5.01f;
    [SerializeField, Tooltip("水に入ってから透明度が最大まで下がるまでの時間")]
    private float m_ImageAlphaMaxTime = 0.5f;
    [SerializeField, Tooltip("最大透明度")]
    private float m_ImageAlpaMax = 0.5f;
    [SerializeField, Tooltip("水面裏の描画を行うか？")]
    private bool m_IsWaterBackDraw = true;


    void Awake()
    {
        tr = GetComponent<Transform>();
        mr = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        m_Plane = new Plane(tr.up, m_Length);

        GameObject image = GameObject.FindGameObjectWithTag("InWaterImage");
        if (image != null)
            m_Image = image.GetComponent<Image>();
        if(m_Image == null) return;
        m_Color = m_Image.color;

        m_WaterBack = tr.FindChild("WaterBack").gameObject;
        m_WaterBack.SetActive(false);
    }

    void Update()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        float angle = Vector3.Angle(tr.up, player.up) / 90.0f;

        angle = Mathf.Clamp(angle, m_SpeedMin, m_SpeedMax);

        //座標を計算
        tr.position = Vector3.Lerp(tr.up, player.up, angle) * -m_Length;

        //回転を計算
        tr.rotation = Quaternion.Slerp(tr.rotation, player.rotation, angle);

        m_Plane = new Plane(tr.up, m_Length);


        if (m_Image == null) return;
        m_Color = m_Image.color;
        //水中にカメラが入っているかを判定
        if(IsCameraInWater())
        {
            m_Timer += Time.deltaTime;
            m_Color.a = Mathf.Lerp(0.0f, m_ImageAlpaMax, m_Timer / m_ImageAlphaMaxTime);
            mr.enabled = false;
            if (m_IsWaterBackDraw)
                m_WaterBack.SetActive(true);
        }
        else
        {
            m_Timer = 0.0f;
            m_Color.a = 0.0f;
            mr.enabled = true;
            m_WaterBack.SetActive(false);
        }

        m_Image.color = m_Color;
    }

    public Plane GetPlane()
    {
        return m_Plane;
    }

    /// <summary>
    /// 指定した座標が平面の表側にあるかどうかを調べる
    /// </summary>
    /// <returns></returns>
    public bool IsPlaneFront(Vector3 position)
    {
        return m_Plane.SameSide(tr.position + tr.up, position);
    }

    /// <summary>
    /// カメラが水面に入っているか
    /// </summary>
    public bool IsCameraInWater()
    {
        return m_Plane.SameSide(tr.position - tr.up, Camera.main.transform.position);
    }
}
