/**==========================================================================*/
/**
 * 水面の移動、水面関係の制御はここで行う
 * カメラやプレイヤーが水に埋まっているときの処理もここで行う
 * 作成者：守屋   作成日：16/11/25
/**==========================================================================*/
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaterMove : MonoBehaviour
{
    Transform tr;
    MeshRenderer mr;
    AudioSource se;

    private Plane m_Plane;
    private Image m_Image;
    private Color m_Color;
    private float m_Timer;
    //子オブジェクトのWaterBack
    private GameObject m_WaterBack;

    private bool m_IsCameraInWaterPrev = false;
    private bool m_IsCameraInWater = false;

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
    [SerializeField, Tooltip("水面の効果音の音量")]
    private float m_SurfaceSEVolume = 1.0f;
    [SerializeField, Tooltip("水面の効果音が聞こえる限界の距離　距離がこの値に近くなる（水面から遠ざかる）ほど聞こえなくなる")]
    private float m_SurfaceSEMaxDistance = 4.0f;


    void Awake()
    {
        tr = GetComponent<Transform>();
        mr = GetComponent<MeshRenderer>();
        se = GetComponent<AudioSource>();
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

        //カメラから水面までの距離に応じて音量を変更
        se.volume = m_SurfaceSEVolume - DistanceCameraToWater() / m_SurfaceSEMaxDistance;


        //キャンバスに描画するかを判定　画像描画用オブジェクトが無い場合は実行しない
        if (m_Image == null) return;
        m_Color = m_Image.color;
        //水中にカメラが入っているかを判定
        m_IsCameraInWaterPrev = m_IsCameraInWater;
        m_IsCameraInWater = IsCameraInWater();
        if (m_IsCameraInWater)
        {
            //最初のフレームだけSEを再生
            if (!m_IsCameraInWaterPrev)
                SoundManager.Instance.PlaySe("diving1");
            m_Timer += Time.deltaTime;
            m_Color.a = Mathf.Lerp(0.0f, m_ImageAlpaMax, m_Timer / m_ImageAlphaMaxTime);
            mr.enabled = false;
            if (m_IsWaterBackDraw)
                m_WaterBack.SetActive(true);
        }
        else
        {
            //最初のフレームだけSEを再生
            if (m_IsCameraInWaterPrev)
                SoundManager.Instance.PlaySe("summer_beach1");
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

    /// <summary>
    /// カメラと水面の距離
    /// </summary>
    public float DistanceCameraToWater()
    {
        return m_Plane.GetDistanceToPoint(Camera.main.transform.position);
    }
}
