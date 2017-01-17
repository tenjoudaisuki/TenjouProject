﻿/**==========================================================================*/
/**
 * 最終ステージの大砲に入った後（クリア時）の動き
 * 丸まり→発射→ガラス突き破る→ステージクリアの扉に入る
 * 作成者：守屋   作成日：16/12/15
/**==========================================================================*/
using UnityEngine;
using System.Collections;

public class StageFinalClearMove : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private Rigidbody rb;
    private Animator anm;
    private CapsuleCollider cc;
    private PlayerMoveManager m_MoveManager;

    /*==外部設定変数==*/
    [SerializeField, Tooltip("最初の定位置への移動にかける時間")]
    private float m_MoveEndTime = 2.0f;
    [SerializeField, Tooltip("大砲に入ったあとの位置")]
    private Vector3 m_SettingPosition;
    [SerializeField, Tooltip("発射後に飛んでいくゴール位置")]
    private Vector3 m_GoalPosition;
    [SerializeField, Tooltip("飛んでいく時間")]
    private float m_FlyEndTime = 10.0f;
    [SerializeField, Tooltip("回転の加速量")]
    private float m_SpinAddSpeed = 20.0f;
    [SerializeField, Tooltip("回転の速さ最大値")]
    private float m_SpinMaxSpeed = 40.0f;
    [SerializeField, Tooltip("発射から当たり判定が復活するまでの時間")]
    private float m_CollideStartTime = 2.0f;

    /*==内部設定変数==*/
    //クリアした瞬間のプレイヤーの位置
    private Vector3 m_ClearPosition;

    void Start()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        anm = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider>();
        m_MoveManager = GetComponent<PlayerMoveManager>();
        StopMove();
    }

    void LateUpdate()
    {

    }

    public void StartMove()
    {
        cc.enabled = false;
        //モデルの位置をずらす
        tr.FindChild("21.!Root").localPosition = new Vector3(0.0f, -0.5f, 0.0f);
        m_ClearPosition = tr.position;
        StartCoroutine(MoveAndRoll());
    }

    public void StopMove()
    {
        StopCoroutine(MoveAndRoll());
        StopCoroutine(Spin());
        StopCoroutine(Shot());
    }

    /// <summary>
    /// 定位置に移動して丸まる
    /// </summary>
    IEnumerator MoveAndRoll()
    {
        float timer = 0.0f;
        while (true)
        {
            cc.enabled = false;
            //時間経過で移動
            timer += Time.deltaTime;
            tr.position = Vector3.Lerp(
                m_ClearPosition,
                m_SettingPosition,
                timer / m_MoveEndTime);

            if (timer > m_MoveEndTime)
            {
                //次のコルーチンを実行
                StartCoroutine(Spin());
                yield break;
            }
            yield return null;
        }
    }


    /// <summary>
    /// 回転
    /// </summary>
    IEnumerator Spin()
    {
        float speed = 0.0f;
        while (true)
        {
            cc.enabled = false;
            tr.position = m_SettingPosition;
            //回転
            tr.rotation *= Quaternion.AngleAxis(speed, Vector3.right);
            if (speed < m_SpinMaxSpeed)
                speed += m_SpinAddSpeed * Time.deltaTime;
            else
            {
                //次のコルーチンを実行
                StartCoroutine(Shot());
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 発射
    /// </summary>
    IEnumerator Shot()
    {
        float timer = 0.0f;
        while (true)
        {
            //時間経過で移動
            timer += Time.deltaTime;

            if (timer > m_CollideStartTime)
                cc.enabled = true;

            tr.position = Vector3.Lerp(
                m_SettingPosition,
                m_GoalPosition,
                timer / m_FlyEndTime);
            //回転
            tr.rotation *= Quaternion.AngleAxis(m_SpinMaxSpeed, Vector3.right);
            yield return null;
        }
    }
}
