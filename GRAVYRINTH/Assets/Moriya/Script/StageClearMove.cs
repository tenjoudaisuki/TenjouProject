/**==========================================================================*/
/**
 * ステージクリア時の移動
 * 作成者：守屋   作成日：16/12/02
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class StageClearMove : MonoBehaviour
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private Rigidbody rb;

    /*==外部設定変数==*/
    [SerializeField, Tooltip("位置を揃える移動にかける時間")]
    private float m_JustMoveEndTime = 1.0f;
    [SerializeField, Tooltip("奥へ進む移動にかける時間")]
    private float m_ForwardMoveEndTime = 1.0f;
    [SerializeField, Tooltip("奥へ進む距離")]
    private float m_ForwardLength = 1.8f;

    /*==内部設定変数==*/
    //ステージクリアのドアのTransform
    private Transform m_ClearDoorTr;
    //クリアした瞬間のプレイヤーの座標
    private Vector3 m_ClearPosition;
    //アニメーション
    private Animator anm;

    /*==外部参照変数==*/


    void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        anm = GetComponent<Animator>();
    }

    void Start()
    {
        //オブジェクト取得
        //m_ClearDoorTr = GameObject.FindGameObjectWithTag("ClearDoor").transform;
    }

    void Update()
    {
        //rigidbodyによる移動を制限
        rb.velocity = Vector3.zero;
    }

    /// <summary>
    /// ステージクリア状態に移行したときに実行する処理
    /// </summary>
    public void StartClearMove()
    {
        // アニメーション
        anm.SetTrigger("Goal");

        m_ClearDoorTr = GameObject.FindGameObjectWithTag("ClearDoor").transform;
        m_ClearPosition = tr.position;
        StartCoroutine(ClearMove());
    }

    IEnumerator ClearMove()
    {
        float timer = 0.0f;
        Vector3 end = m_ClearDoorTr.position + (-tr.up * 0.3f);
        while (timer <= m_JustMoveEndTime)
        {
            //時間経過で移動
            timer += Time.deltaTime;
            tr.position = Vector3.Lerp(m_ClearPosition, end, timer / m_JustMoveEndTime);
            yield return null;
        }
        timer = 0.0f;
        Vector3 start = end;
        end = m_ClearDoorTr.position + (-m_ClearDoorTr.forward * m_ForwardLength) + (-tr.up * 0.3f);
        while (timer <= m_ForwardMoveEndTime)
        {
            //時間経過で移動
            timer += Time.deltaTime;
            tr.position = Vector3.Lerp(start, end, timer / m_ForwardMoveEndTime);
            yield return null;
        }
        yield break;
    }
}
