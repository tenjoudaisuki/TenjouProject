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
    private Transform tr;
    private Animator anm;
    private CapsuleCollider cc;

    ///*==外部設定変数==*/
    [SerializeField, TooltipAttribute("最初の状態")]
    private PlayerState m_StartState = PlayerState.NORMAL;
    [SerializeField, TooltipAttribute("現在の状態")]
    private PlayerState m_PlayerState;

    ///*==内部設定変数==*/
    //１つ前のプレイヤーの状態
    private PlayerState m_PrevPlayerState;
    //各状態中の処理はこの配列へ格納
    private Dictionary<PlayerState, MonoBehaviour> m_Moves;

    ///*==外部参照変数==*/
    void Awake()
    {
        tr = GetComponent<Transform>();
        anm = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        m_PrevPlayerState = PlayerState.NONE;
        m_PlayerState = m_StartState;
        m_Moves = new Dictionary<PlayerState, MonoBehaviour>()
        {
            {PlayerState.NORMAL, GetComponent<NormalMove>() },
            {PlayerState.NOT_MOVE, GetComponent<NotMove>() },
            {PlayerState.IRON_BAR_DANGLE, GetComponent<DangleMove>() },
            {PlayerState.IRON_BAR_CLIMB, GetComponent<CrimbMove>() },
            {PlayerState.CANNON_BLOCK, GetComponent<CannonBlockMove>() },
            {PlayerState.STAGE_CLEAR, GetComponent<StageClearMove>() },
            {PlayerState.STAGE_FINAL_CLEAR, GetComponent<StageFinalClearMove>() }
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
        foreach (KeyValuePair<PlayerState, MonoBehaviour> move in m_Moves)
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

        //同じ状態への変更は行わない
        if (m_PrevPlayerState == m_PlayerState) return;

        //特定の変更時に行う処理
        //ＡＮＹ→ＮＯＮＥへの変更時
        if (m_PlayerState == PlayerState.NONE)
        {
            //アニメーションを止める
            AnimationInitialize();
        }        
        //ＡＮＹ→通常への変更時
        if(m_PlayerState == PlayerState.NORMAL)
        {
            //アニメーションを止める
            AnimationInitialize();
        }

        //鉄棒→通常への変更時
        if ((m_PrevPlayerState == PlayerState.IRON_BAR_DANGLE || m_PrevPlayerState == PlayerState.IRON_BAR_CLIMB)
            && m_PlayerState == PlayerState.NORMAL)
        {
            //地面との当たり判定を有効にする
            m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().IronbarToNormal();

            if(m_PrevPlayerState == PlayerState.IRON_BAR_DANGLE)
            {
                //一定時間カプセルの当たり判定をオフにする処理を実行
                m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().DangleToNormal();
            }
            else if (m_PrevPlayerState == PlayerState.IRON_BAR_CLIMB)
            {
                PlayerPoleKick(Vector3.Normalize(-tr.forward + tr.up));
            }
        }
        //通常→ステージクリアへの変更時
        else if (m_PrevPlayerState == PlayerState.NORMAL && m_PlayerState == PlayerState.STAGE_CLEAR)
        {
            //ステージクリア移動開始
            m_Moves[PlayerState.STAGE_CLEAR].GetComponent<StageClearMove>().StartClearMove();
        }
        //ステージクリア→通常への変更時
        else if (m_PrevPlayerState == PlayerState.STAGE_CLEAR && m_PlayerState == PlayerState.NORMAL)
        {
            // アニメーション
            anm.SetTrigger("Reset");
        }
        //大砲ブロック→通常への変更時
        else if (m_PrevPlayerState == PlayerState.CANNON_BLOCK && m_PlayerState == PlayerState.NORMAL)
        {
            //上と前をNormalMoveへ引き継ぐ
            SetPlayerUpFront(tr.up, tr.forward);
        }
        //最終ステージクリア時
        else if (m_PlayerState == PlayerState.STAGE_FINAL_CLEAR)
        {
            //SEを止める
            m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().StopSE();
            //移動開始
            m_Moves[PlayerState.STAGE_FINAL_CLEAR].GetComponent<StageFinalClearMove>().StartMove();
        }
        else if(m_PlayerState == PlayerState.NOT_MOVE)
        {
            // アニメーション
            anm.SetTrigger("Reset");
            anm.SetFloat("Jump_Velo", 0);

            //SEを止める
            m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().StopSE();
            NotMove nm = m_Moves[PlayerState.NOT_MOVE].GetComponent<NotMove>();
            nm.SetUpFront(tr.up, tr.forward);
            AnimationInitialize();
        }
        else if (
            m_PrevPlayerState == PlayerState.NOT_MOVE
            && m_PlayerState == PlayerState.NORMAL)
        {
            //SE再開
            m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().RestartSE();
            m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().SetUpFront(tr.up, tr.forward);
        }
        else if (
            m_PrevPlayerState == PlayerState.NORMAL && m_PlayerState == PlayerState.IRON_BAR_DANGLE ||
            m_PrevPlayerState == PlayerState.NORMAL && m_PlayerState == PlayerState.IRON_BAR_CLIMB)
        {
            //カプセル無効
            cc.enabled = false;
            // アニメーション
            anm.SetBool("Pole_Jump", false);
        }
    }

    /// <summary>
    ///　プレイヤーの上と前を更新する
    /// </summary>
    public void SetPlayerUpFront(Vector3 up, Vector3 front)
    {
        m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().SetUpFront(up, front);
    }

    /// <summary>
    /// 鉄棒よじ登り状態からジャンプ
    /// </summary>
    public void PlayerPoleKick(Vector3 v)
    {
        m_Moves[PlayerState.NORMAL].GetComponent<NormalMove>().StartPoleKick(v);
    }

    /// <summary>
    /// 現在の状態を取得する
    /// </summary>
    /// <returns></returns>
    public PlayerState GetState()
    {
        return m_PlayerState;
    }

    /// <summary>
    /// アニメーション状態初期化
    /// </summary>
    public void AnimationInitialize()
    {
        anm.SetBool("IsTaihouRoll", false);
        anm.SetBool("Move", false);
    }

}