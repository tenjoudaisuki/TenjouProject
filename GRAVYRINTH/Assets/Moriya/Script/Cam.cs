using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform tr;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("カメラを向けるターゲット")]
    private Transform m_Target;
    [SerializeField, TooltipAttribute("肩越し視点のときのカメラの位置")]
    private Transform m_OverShoulder;
    [SerializeField, TooltipAttribute("カメラの注視点の高さ（ターゲットからどれだけ高い座標を視るか）")]
    private float m_TargetHeight = 0.0f;
    [SerializeField, TooltipAttribute("ターゲットからの距離")]
    private float m_TargetMaxDistance = 1.5f;
    [SerializeField, TooltipAttribute("アングルが回る速さ係数(0.0～1.0)")]
    private float m_AngleSpeed = 0.2f;
    [SerializeField, TooltipAttribute("注視点が移行する速さ（通常←→肩越し　のときのカメラの切り替わり）")]
    private float m_LookPosChangeSpeed = 0.2f;
    [SerializeField, TooltipAttribute("注視点が移行し始めるまでにかかる長押しの長さ（秒）")]
    private float m_LookPosChangeTime = 0.2f;
    [SerializeField, TooltipAttribute("ターゲットの高さ")]
    private float m_Height = 1.0f;

    /*==内部設定変数==*/
    //線形補間用
    private Vector3 m_Pos;
    private Vector3 m_StartPos;
    private Vector3 m_EndPos;
    //カメラ注視点
    private Vector3 m_LookPos;
    private Vector3 m_StartLookPos;
    private Vector3 m_EndLookPos;

    Quaternion front;

    //クリックしている時間を計測するタイマー
    private float m_LeftClickTimer;


    /*==================*/
    /* 生成前初期化   */
    /*==================*/
    void Awake()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
    }

    void Start()
    {
        front = m_Target.rotation;
    }

    /*==================*/
    /* 更新後更新処理   */
    /*==================*/
    void LateUpdate()
    {
        //クリックタイマー変化
        if (Input.GetMouseButton(0))
            m_LeftClickTimer += Time.deltaTime;
        else if (Input.GetMouseButtonUp(0))
            m_LeftClickTimer = 0.0f;

        if (m_LeftClickTimer < m_LookPosChangeTime)
        {
            //通常時
            float X = Input.GetAxis("Horizontal2") * 2.0f;
            //angleY = -Input.GetAxis("Vertical2");
            front *= Quaternion.AngleAxis(X, m_Target.up);

            //移動前の座標
            m_StartPos = tr.position;
            //最終的な移動目標
            m_EndPos = m_Target.position + m_Target.up * m_Height - (front * Vector3.forward) * m_TargetMaxDistance;
            //移動後の座標
            m_Pos = Vector3.Lerp(m_StartPos, m_EndPos, m_AngleSpeed);


            //注視点
            m_StartLookPos = m_LookPos;
            m_EndLookPos = new Vector3(m_Target.position.x, m_Target.position.y + m_Height + m_TargetHeight, m_Target.position.z);
            m_LookPos = Vector3.Lerp(m_StartLookPos, m_EndLookPos, m_LookPosChangeSpeed);

            //地面にヒットしているか
            Ray ray = new Ray(m_LookPos, Vector3.Normalize(m_Pos - m_LookPos));
            RaycastHit hit;
            bool ishit = Physics.Raycast(ray, out hit, m_TargetMaxDistance);
            if (ishit)
            {
                //ヒットしていた場合、ヒットした地点にカメラを移動
                m_Pos = hit.point;
            }

            tr.position = m_Pos;
            tr.LookAt(m_LookPos);
        }
    }
}
