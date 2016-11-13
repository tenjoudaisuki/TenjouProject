/**==========================================================================*/
/**
 * プレイヤーの影
 * プロジェクターを移動させるだけ
 * 作成者：守屋   作成日：16/11/13
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class PlayerShadow : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private Transform player;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("プレイヤーからずらす量")]
    private Vector3 m_Displace = Vector3.zero;
    [SerializeField, TooltipAttribute("プレイヤーからの高さ")]
    private float m_Height = 5.0f;

    void Awake()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
    }

    void Start()
    {
        //オブジェクト取得
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    void LateUpdate()
    {
        tr.LookAt(tr.position - player.up);
        Vector3 displace = player.right * m_Displace.x + player.up * m_Displace.y + player.forward * m_Displace.z;
        tr.position = player.position + displace + player.up * m_Height;
    }
}
