/**==========================================================================*/
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
    private PlayerMoveManager m_MoveManager;

    /*==外部設定変数==*/
    [SerializeField, Tooltip("最初の定位置への移動にかける時間")]
    private float m_MoveEndTime = 2.0f;
    [SerializeField, Tooltip("発射の方向、移動量")]
    private Vector3 m_ShotVelocity = new Vector3(0.0f, 10.0f, 20.0f);

    /*==内部設定変数==*/
    //クリアした瞬間のプレイヤーの位置
    private Vector3 m_ClearPosition;
    //定位置
    private Vector3 m_TargetPosition;

    void Start()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        anm = GetComponent<Animator>();
        m_MoveManager = GetComponent<PlayerMoveManager>();
        StopMove();
    }

    public void StartMove()
    {
        m_ClearPosition = tr.position;
        m_TargetPosition = GameObject.FindGameObjectWithTag("StageFClearCollider").transform.position;
        StartCoroutine(MoveAndRoll());
    }

    public void StopMove()
    {
        StopCoroutine(MoveAndRoll());
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
            //時間経過で移動
            timer += Time.deltaTime;
            tr.position = Vector3.Lerp(m_ClearPosition, m_TargetPosition, timer / m_MoveEndTime);
            //ここで丸まりのアニメーションも行いたい
            //anm->marumari

            //回転
            //tr.FindChild("Model").rotation *= Quaternion.AngleAxis(4000.0f, tr.right);

            if (timer > m_MoveEndTime)
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
        while (true)
        {
            tr.position += m_ShotVelocity * Time.deltaTime;
            yield return null;
        }
    }
}
