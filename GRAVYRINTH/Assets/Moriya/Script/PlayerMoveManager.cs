/**==========================================================================*/
/**
 * プレイヤーの移動管理クラス
 * 作成者：守屋   作成日：16/11/13
/**==========================================================================*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveManager : MonoBehaviour
{
    ///*==所持コンポーネント==*/

    ///*==外部設定変数==*/

    ///*==内部設定変数==*/
    //現在のプレイヤーの状態
    private PlayerState m_PlayerState;
    //各状態中の処理はこの配列へ格納
    private Dictionary<PlayerState,MonoBehaviour> m_Moves;


    ///*==外部参照変数==*/
    void Awake()
    {

    }

    void Start()
    {
        m_PlayerState = PlayerState.NORMAL;
        m_Moves = new Dictionary<PlayerState, MonoBehaviour>()
        {
            {PlayerState.NORMAL, GetComponent<NormalMove>() },
            {PlayerState.DANGLE, GetComponent<PlayerIronBar>() }
        };
    }

    void Update()
    {
        //m_Moves[PlayerState.NORMAL].enabled = true;
        //m_Moves[PlayerState.JUMP].enabled = true;
        //m_Moves[PlayerState.DANGLE].enabled = true;

        //現在の状態のみを実行
        Action(m_PlayerState);
    }

    /// <summary>
    /// 指定した状態のみを有効にする
    /// </summary>
    void Action(PlayerState state)
    {
        foreach(KeyValuePair<PlayerState, MonoBehaviour> move in m_Moves)
        {
            if (state == move.Key)
                //move.Valueからだと変更できないみたい
                m_Moves[move.Key].enabled = true; 
            else
                m_Moves[move.Key].enabled = false;
        }
    }

    /// <summary>
    /// 状態を変更する
    /// </summary>
    public void SetState(PlayerState state)
    {
        m_PlayerState = state;
    }
}