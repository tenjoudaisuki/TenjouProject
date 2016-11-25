/**==========================================================================*/
/**
 * プレイヤーのリスポーン地点であるチェックポイントの処理
 * 作成者：守屋   作成日：16/11/25
/**==========================================================================*/
using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour 
{

    private PlayerRespawn m_PlayerRespawn;

    void Start()
    {
        m_PlayerRespawn = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRespawn>();
    }


    void OnTriggerEnter(Collider coll)
    {
        //プレイヤーが当たってきた瞬間
        if (coll.gameObject.tag == "Player")
        {
            //状態を保存させる
            m_PlayerRespawn.SaveCurrentStatus();
        }
    }

}
