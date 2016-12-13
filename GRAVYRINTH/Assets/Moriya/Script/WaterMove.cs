/**==========================================================================*/
/**
 * 水面の移動
 * 作成者：守屋   作成日：16/11/25
/**==========================================================================*/
using UnityEngine;
using System.Collections;

public class WaterMove : MonoBehaviour 
{
    Transform tr;

    [SerializeField,Tooltip("移動速度最大値")]
    private float m_SpeedMax = 0.02f;
    [SerializeField, Tooltip("移動速度最小値")]
    private float m_SpeedMin = 0.008f;
    [SerializeField, Tooltip("原点からの距離")]
    private float m_Length = 5.01f;

    void Awake()
    {
        tr = GetComponent<Transform>();
    }

	void Start()
    {
        
	} 

	void Update()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;

        float angle = Vector3.Angle(tr.up, player.up) / 90.0f;

        angle = Mathf.Clamp(angle, m_SpeedMin, m_SpeedMax);

        //座標を計算
        tr.position = Vector3.Lerp(tr.up, player.up, angle) * - m_Length;

        //回転を計算
        tr.rotation = Quaternion.Slerp(tr.rotation, player.rotation, angle);

	}
}
