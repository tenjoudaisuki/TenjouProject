using UnityEngine;
using System.Collections;

public class Stage4SpinObject : MonoBehaviour
{
    private Transform tr;

    [SerializeField, Tooltip("回転軸")]
    public Vector3 m_Axis = Vector3.up;
    [SerializeField, Tooltip("回転速度")]
    public float m_SpinSpeed;
    [SerializeField, Tooltip("停止する角度")]
    public float m_StopAngle = 90.0f;

    //経過時間
    private float m_Timer;
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
        m_StopCount = 1;

        m_StartRotation = tr.rotation;
        m_CureRotation = tr.rotation;
    }

    void Update()
    {

    }

    /// <summary>
    /// 回転開始
    /// </summary>
    public void StartSpin()
    {
        StartCoroutine(Spin());
    }

    /// <summary>
    /// 回転する
    /// </summary>
    IEnumerator Spin()
    {
        float timer = 0.0f;
        while (true)
        {
            timer += Time.deltaTime;
            //回転
            if (m_SpinSpeed * timer < m_StopAngle)
            {
                tr.Rotate(m_Axis, m_SpinSpeed * Time.deltaTime);
            }
            //回転終了
            else
            {
                //正確な回転量に矯正する
                m_CureRotation = m_StartRotation * Quaternion.AngleAxis(m_StopAngle * m_StopCount, m_Axis);
                tr.rotation = m_CureRotation;
                m_StopCount++;
                yield break;
            }
            yield return null;
        }
    }
}
