using UnityEngine;
using System.Collections;

public class SpinChild : MonoBehaviour {

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
        float speed = transform.parent.GetComponent<SpinParent>().mSpinSpeed;
        transform.Rotate(Vector3.forward,speed * 2);

        m_Movement = transform.position - m_PrevPosition;
        m_PrevPosition = transform.position;
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
