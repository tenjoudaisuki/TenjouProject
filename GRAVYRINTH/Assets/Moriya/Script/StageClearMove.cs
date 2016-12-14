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

    /*==外部設定変数==*/
    [SerializeField, Tooltip("移動にかける時間")]
    private float m_MoveEndTime = 1.0f;

    /*==内部設定変数==*/
    //クリア時の移動目標である、ステージクリアのドアのTransform
    private Transform m_ClearDoorTr;
    //クリアした瞬間のプレイヤーの座標
    private Vector3 m_ClearPosition;

    /*==外部参照変数==*/
    
    
    void Awake()
    {
        tr = GetComponent<Transform>();
    }

    void Start()
    {
        //オブジェクト取得
        //m_ClearDoorTr = GameObject.FindGameObjectWithTag("ClearDoor").transform;
    }

    /// <summary>
    /// ステージクリア状態に移行したときに実行する処理
    /// </summary>
    public void StartClearMove()
    {
        m_ClearDoorTr = GameObject.FindGameObjectWithTag("ClearDoor").transform;
        m_ClearPosition = tr.position;
        StartCoroutine(ClearMove());
    }

    IEnumerator ClearMove()
    {
        float timer = 0.0f;
        while (true)
        {
            //時間経過で移動
            timer += Time.deltaTime;
            tr.position = Vector3.Lerp(m_ClearPosition, m_ClearDoorTr.position, timer / m_MoveEndTime);
            if (timer > m_MoveEndTime)
            {
                yield break;
            }
            yield return null;
        }
    }
}
