using UnityEngine;
using System.Collections;

public class SpinChild : MonoBehaviour {

    //回転速度
    public float m_SpinSpeed = 2.0f;
    //基準位置の方向
    public Vector3 m_OffsetDirection = Vector3.up; 
    //基準位置への距離
    public float m_OffsetLength = 0.0f;

    //移動量
    private Vector3 m_Movement;
    //1フレーム前のトランスフォーム
    private Vector3 m_PrevPosition;

	// Use this for initialization
	void Start () {
        m_PrevPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.right, m_SpinSpeed * Time.deltaTime);
        Vector3 pos = transform.position + (transform.rotation * m_OffsetDirection) * m_OffsetLength;
        m_Movement = pos - m_PrevPosition;
        m_PrevPosition = pos;
	}

    void LateUpdate()
    {
        
    }

    /// <summary>
    /// 移動量を取得する
    /// </summary>
    public Vector3 GetMovement()
    {
        return m_Movement;
    }
}
