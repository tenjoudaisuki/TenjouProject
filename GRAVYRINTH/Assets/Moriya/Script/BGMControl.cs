/**==========================================================================*/
/**
 * SoundManagerで鳴らすＢＧＭの制御
 * 作成者：守屋   作成日：16/02/04
/**==========================================================================*/
using UnityEngine;
using System.Collections;

public class BGMControl : MonoBehaviour 
{
    [SerializeField, Tooltip("チュートリアルステージで、ＢＧＭの切り替えを行うまでの時間")]
    private float m_TutorialBGMWaitTime = 12.2f;
    [SerializeField, Tooltip("ステージＦで、ＢＧＭの切り替えを行うまでの時間(stagef1→stagef2)")]
    private float m_Stagef1to2BGMWaitTime = 30.0f;
    [SerializeField, Tooltip("ステージＦで、stagef2のフェードアウトにかける時間")]
    private float m_Stagef3BGMFadeOutTime = 1.0f;

    private GameManager m_GameManager;
    private Coroutine m_StartGameCor;
    private Coroutine m_StageFinalCor;
    private bool m_IsTouchFinalDoorSwitch;
    private bool m_IsPlayerInTaihouRoom;

	void Start () 
    {
        m_GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_IsTouchFinalDoorSwitch = false;
        m_IsPlayerInTaihouRoom = false;
    }
	

	void Update () 
    {

	}

    ///// <summary>
    ///// BGMをシーンの切り替わりに応じて変化させる
    ///// </summary>
    //public void ChangeBGM(string currentSceneName,string nextSceneName)
    //{
    //    print("currentSceneName:" + currentSceneName);
    //    print("nextSceneName;" + nextSceneName);
    //    if(nextSceneName == "")
    //    {

    //    }
    //}

    /// <summary>
    /// メニュー、ポーズ画面からStartGameが選択されたとき
    /// </summary>
    public void StartGameSelected()
    {
        if (m_StartGameCor != null)
            StopCoroutine(m_StartGameCor);
        m_StartGameCor = StartCoroutine(StartGameCor());
    }
    IEnumerator StartGameCor()
    {
        float timer = 0.0f;
        SoundManager.Instance.PlayBgm("stage0-1");
        while(true)
        {
            timer += Time.deltaTime;
            if (timer > m_TutorialBGMWaitTime)
            {
                SoundManager.Instance.PlayBgm("stage0-2");
                yield break;
            }
            yield return null;
        } 
    }

    /// <summary>
    /// メニュー、ポーズ画面からStageSelectでＳＴＡＧＥ１～４が選択されたとき
    /// </summary>
    public void Stage1_4Selected()
    {
        SoundManager.Instance.PlayBgm("stage1");
    }

    /// <summary>
    /// メニュー、ポーズ画面から最終ステージが選択されたとき
    /// </summary>
    public void StageFinalSelected()
    {
        if (m_StageFinalCor != null)
            StopCoroutine(m_StageFinalCor);
        m_StageFinalCor = StartCoroutine(StageFinalCor());
    }
    IEnumerator StageFinalCor()
    {
        float timer = 0.0f;

        //stagef1→stagef2
        SoundManager.Instance.PlayBgm("stagef1");
        while (timer < m_Stagef1to2BGMWaitTime)
        {
            timer += Time.deltaTime;
            if (timer >= m_Stagef1to2BGMWaitTime)
            {
                SoundManager.Instance.PlayBgm("stagef2");
            }
            yield return null;
        }

        //プレイヤーがfinaldoorswitchに触れるまで待機
        m_IsTouchFinalDoorSwitch = false;
        while (!m_IsTouchFinalDoorSwitch)
        {            
            yield return null;
        }
        //触れたらSEとして曲の後奏を再生
        SoundManager.Instance.PlaySe("stagef3");


        //1秒かけてＢＧＭをフェードアウト
        timer = 0.0f;
        while (timer < m_Stagef3BGMFadeOutTime)
        {
            timer += Time.deltaTime;
            SoundManager.Instance.volume.bgm = Mathf.Lerp(1.0f, 0.0f, timer / m_Stagef3BGMFadeOutTime);
            yield return null;
        }

        //プレイヤーが大砲の部屋に入るまで待機
        m_IsPlayerInTaihouRoom = false;
        while (!m_IsPlayerInTaihouRoom)
        {
            yield return null;
        }

        //入ったら再生
        SoundManager.Instance.PlayBgm("stagef4");

        //大砲が完成したらフェードアウト

        

        yield break;
    }


    public void PlayerFinalDoorSwitchTouched()
    {
        m_IsTouchFinalDoorSwitch = true;
    }
}
