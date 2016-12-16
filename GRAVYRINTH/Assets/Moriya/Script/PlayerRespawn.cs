/**==========================================================================*/
/**
 * プレイヤーをリスポーンさせる処理
 * 作成者：守屋   作成日：16/11/18
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour 
{
    /*==所持コンポーネント==*/
    private Transform tr;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("リスポーン時の高さ")]
    private float m_RespawnHeight = 3.0f;

    [SerializeField, TooltipAttribute("チェックポイント接触時の座標、初期値は自動的に設定される")]
    private Vector3 m_CheckPosition = Vector3.zero;
    [SerializeField, TooltipAttribute("チェックポイント接触時の上、初期値は自動的に設定される")]
    private Vector3 m_CheckUp = Vector3.up;
    [SerializeField, TooltipAttribute("チェックポイント接触時の前、初期値は自動的に設定される")]
    private Vector3 m_CheckFront = Vector3.forward;

    /*==内部設定変数==*/
    //通常移動処理　これのRespawn関数を実行することでリスポーンする
    private NormalMove m_NormalMove;

    /*==外部参照変数==*/

    void Awake()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        m_NormalMove = GetComponent<NormalMove>();
    }

	void Start ()
    {
        m_CheckPosition = tr.position;
        m_CheckUp = tr.up;
        m_CheckFront = tr.forward;
	}

	void Update ()
    {
        //リスポーンする
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn(GameObject.FindGameObjectWithTag("StartPoint").transform.position);
        }            
	}

    /// <summary>
    /// 現在の状態を保存
    /// </summary>
    public void SaveCurrentStatus()
    {
        m_CheckUp = tr.up;
        m_CheckFront = tr.forward;
        //ちょっと上に設定
        m_CheckPosition = tr.position + (m_CheckUp * m_RespawnHeight);
    }

    /// <summary>
    /// リスポーンする
    /// </summary>
    public void Respawn()
    {
        m_NormalMove.Respawn(m_CheckPosition, m_CheckUp, m_CheckFront);
    }


    /// <summary>
    /// 座標を指定して初期の向きでリスポーンする
    /// </summary>
    public void Respawn(Vector3 position)
    {
        m_NormalMove.Respawn(position, Vector3.up, Vector3.forward);
    }
}

