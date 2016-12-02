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
    [SerializeField, TooltipAttribute("最初の状態")]
    private PlayerState m_StartState = PlayerState.NORMAL;

    ///*==内部設定変数==*/
    //現在のプレイヤーの状態
    private PlayerState m_PlayerState;
    //１つ前のプレイヤーの状態
    private PlayerState m_PrevPlayerState;
    //各状態中の処理はこの配列へ格納
    private Dictionary<PlayerState,MonoBehaviour> m_Moves;

    ///*==外部参照変数==*/
    void Awake()
    {

    }

    void Start()
    {
        m_PrevPlayerState = PlayerState.NONE;
        m_PlayerState = m_StartState;
        m_Moves = new Dictionary<PlayerState, MonoBehaviour>()
        {
            {PlayerState.NORMAL, GetComponent<NormalMove>() },
            {PlayerState.IRON_BAR_DANGLE, GetComponent<PlayerIronBar>() },
            {PlayerState.IRON_BAR_CLIMB, GetComponent<PlayerIronBar>() },
            {PlayerState.STAGE_CLEAR, GetComponent<StageClearMove>() }
        };
    }

    void Update()
    {
        //m_Moves[PlayerState.NORMAL].enabled = true;
        //m_Moves[PlayerState.JUMP].enabled = true;
        //m_Moves[PlayerState.IRON_BAR].enabled = true;

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
        m_PrevPlayerState = m_PlayerState;
        m_PlayerState = state;

        //特定の変更時に行う処理
        //鉄棒→通常への変更時
        if ((m_PrevPlayerState == PlayerState.IRON_BAR_DANGLE || m_PrevPlayerState == PlayerState.IRON_BAR_DANGLE)
            && m_PlayerState == PlayerState.NORMAL)
        {
            //地面との当たり判定を有効にする
            m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().IronbarToNormal();
        }           
        //通常→ステージクリアへの変更時
        else if(m_PrevPlayerState == PlayerState.NORMAL && m_PlayerState == PlayerState.STAGE_CLEAR)
        {
            print("called");
            //ステージクリア移動開始
            m_Moves[PlayerState.STAGE_CLEAR].GetComponent<StageClearMove>().StartClearMove();
        }
    }

    /// <summary>
    ///　プレイヤーの上と前を更新する
    /// </summary>
    public void SetPlayerUpFront(Vector3 up,Vector3 front)
    {
        m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().SetUpFront(up, front);
    }

    /// <summary>
    /// 鉄棒よじ登り状態からジャンプして背面に飛ばせる
    /// </summary>
    public void PlayerPoleKick(Vector3 v)
    {
        m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().StartPoleKick(v);
    }
}