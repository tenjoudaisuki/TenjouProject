using UnityEngine;
using System.Collections;

public class SpinParent : MonoBehaviour {

    private Transform tr;

    [SerializeField, Tooltip("回転軸")]
    public Vector3 m_Axis = Vector3.up;
    [SerializeField, Tooltip("回転速度")]
    public float m_SpinSpeed;
    [SerializeField, Tooltip("指定した角度おきに停止するか？ チェックをつけると停止する")]
    public bool m_IsStopMove = false;
    [SerializeField, Tooltip("停止する角度")]
    public float m_StopAngle = 90.0f;
    [SerializeField, Tooltip("停止する時間(秒)")]
    public float m_StopTime = 2.0f;

    //経過時間
    private float m_Timer;
    //停止経過時間
    private float m_StopMoveTimer;
    //停止回数
    private int m_StopCount;

    //初期回転量
    private Quaternion m_StartRotation;
    //回転量矯正用の回転量
    private Quaternion m_CureRotation;

    void Awake()
    {
        tr = GetComponent<Transform>();
    }

    void Start()
    {
        m_Timer = 0.0f;
        m_StopMoveTimer = 0.0f;
        m_StopCount = 1;

        m_StartRotation = tr.rotation;
        m_CureRotation = tr.rotation;
    }

    void Update()
    {
        //ただ回転するだけならこちら
        if (!m_IsStopMove)
            tr.Rotate(m_Axis, m_SpinSpeed * Time.deltaTime);
        //回転→停止→回転→停止　する動きはこちら
        else
        {
            m_Timer += Time.deltaTime;
            if (m_SpinSpeed * m_Timer > m_StopAngle)
            {
                //正確な回転量に矯正する
                if(m_StopMoveTimer <= 0.0f)
                {
                    m_CureRotation = m_StartRotation * Quaternion.AngleAxis(m_StopAngle * m_StopCount, m_Axis);
                    tr.rotation = m_CureRotation;
                }

                //時間経過で停止解除
                m_StopMoveTimer += Time.deltaTime;
                if (m_StopMoveTimer > m_StopTime)
                {
                    m_Timer = 0.0f;
                    m_StopMoveTimer = 0.0f;
                    m_StopCount++;
                }
            }
            else
            {
                tr.Rotate(m_Axis, m_SpinSpeed * Time.deltaTime);
            }
        }
    }
}
