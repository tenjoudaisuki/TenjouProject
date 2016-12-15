using UnityEngine;
using System.Collections;

public class SpinParent : MonoBehaviour {

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

    void Start()
    {
        m_Timer = 0.0f;
        m_StopMoveTimer = 0.0f;
    }

    void Update()
    {
        if (!m_IsStopMove)
            gameObject.transform.Rotate(m_Axis, m_SpinSpeed * Time.deltaTime);
        else
        {
            m_Timer += Time.deltaTime;
            if (m_SpinSpeed * m_Timer > m_StopAngle)
            {
                m_StopMoveTimer += Time.deltaTime;
                if (m_StopMoveTimer > m_StopTime)
                {
                    m_Timer = 0.0f;
                    m_StopMoveTimer = 0.0f;
                }
            }
            else
            {
                gameObject.transform.Rotate(m_Axis, m_SpinSpeed * Time.deltaTime);
            }
        }
    }
}
