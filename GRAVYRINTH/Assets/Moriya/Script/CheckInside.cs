/**==========================================================================*/
/**
 * プレイヤーがステージの外に出たらリスポーンさせる処理
 * 作成者：守屋   作成日：16/11/25
/**==========================================================================*/
using UnityEngine;
using System.Collections;

public class CheckInside : MonoBehaviour 
{
    [SerializeField, Tooltip("黒画面へのフェードアウトにかかる時間")]
    private float m_FadeOutTime = 1.0f;
    [SerializeField, Tooltip("元の画面へのフェードイン開始前の待機時間")]
    private float m_FadeWaitTime = 1.0f;
    [SerializeField, Tooltip("元の画面へのフェードインにかかる時間")]
    private float m_FadeInTime = 1.0f;

    private PlayerRespawn m_PlayerRespawn;
    private FadeImage m_FadeImage;

    void Start()
    {
        m_PlayerRespawn = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRespawn>();
        m_FadeImage = GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeImage>();
    }

    void LateUpdate()
    {
        //print(m_FadeImage.Range);
    }

    void OnTriggerExit(Collider coll)
    {
        //プレイヤーが場外に出た瞬間瞬間
        if (coll.gameObject.tag == "Player")
        {
            //コルーチン開始
            StartCoroutine(FadeAndRespawn());
        }
    }

    /// <summary>
    /// 黒画面にフェードアウト→フェードインしながらリスポーンする処理
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeAndRespawn()
    {
        float timer = 0.0f;
        bool respawned = false;
        while(true)
        {
            timer += Time.deltaTime;
            //黒画面へフェードアウト
            if (timer < m_FadeOutTime)
            {
      
                m_FadeImage.Range = timer / m_FadeOutTime;
            }
            //待機
            else if (timer > m_FadeOutTime + m_FadeWaitTime)
            {
                if(!respawned)
                {
                    respawned = true;
                    //リスポーンする
                    m_PlayerRespawn.Respawn();
                }

                m_FadeImage.Range -= Time.deltaTime / m_FadeInTime;
            }
            //元の画面にフェードイン
            if (timer > m_FadeOutTime + m_FadeWaitTime + m_FadeInTime)
            {
                m_FadeImage.Range = 0.0f;
                yield break;
            }

            m_FadeImage.Range = Mathf.Clamp(m_FadeImage.Range, 0.0f, 1.0f);

            yield return null;
        }
    }
}
