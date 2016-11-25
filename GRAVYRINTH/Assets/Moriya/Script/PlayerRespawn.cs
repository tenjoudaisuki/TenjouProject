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
    [SerializeField, TooltipAttribute("チェックポイント接触時の座標、初期値をここに設定")]
    private Vector3 m_CheckPosition = Vector3.zero;
    [SerializeField, TooltipAttribute("チェックポイント接触時の上、初期値をここに設定")]
    private Vector3 m_CheckUp = Vector3.up;
    [SerializeField, TooltipAttribute("チェックポイント接触時の前、初期値をここに設定")]
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
        
	}

	void Update ()
    {
	    
	}

    public void OnCollisionEnter(Collision collision)
    {
        //チェックポイントにあたった瞬間
        if (collision.gameObject.tag == "CheckPoint")
        {
            print("check point hit");
            //状態を保存
            m_CheckPosition = tr.position;
            m_CheckUp = tr.up;
            m_CheckFront = tr.forward;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        //チェックポイントにあたった瞬間
        if (collision.gameObject.tag == "Inside")
        {
            print("respawn");
            m_NormalMove.Respawn(m_CheckPosition, m_CheckUp, m_CheckFront);
        }
    }

}

