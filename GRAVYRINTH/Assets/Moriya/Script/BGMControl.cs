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
    private float m_Stagef2BGMFadeOutTime = 1.0f;
    [SerializeField, Tooltip("ステージＦで、stagef4のフェードアウトにかける時間")]
    private float m_Stagef4BGMFadeOutTime = 2.0f;
    [SerializeField, Tooltip("ステージＦで、stagef5が開始してからstagef6を再生するまでの待ち時間")]
    private float m_Stagef5to6BGMWaitTime = 24.5f;
    [SerializeField, Tooltip("ステージＦで、stagef6のフェードアウトにかける時間")]
    private float m_Stagef6BGMFadeOutTime = 1.0f;

    [SerializeField, Tooltip("ステージＦのデバッグモード")]
    private bool m_IsStageFDebugMode = false;

    private GameManager m_GameManager;
    private Coroutine m_StartGameCor;
    private Coroutine m_StageFinalCor;
    private bool m_IsTouchFinalDoorSwitch;
    private bool m_IsPlayerInTaihouRoom;
    private bool m_IsCreatedTaihou;
    private bool m_IsBreakedWall;

	void Start () 
    {
        m_GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_IsTouchFinalDoorSwitch = false;
        m_IsPlayerInTaihouRoom = false;

        if (m_IsStageFDebugMode)
            StageFinalSelected();
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

        //ＢＧＭをフェードアウト
        timer = 0.0f;
        while (timer < m_Stagef2BGMFadeOutTime)
        {
            timer += Time.deltaTime;
            SoundManager.Instance.volume.bgm = Mathf.Lerp(1.0f, 0.0f, timer / m_Stagef2BGMFadeOutTime);
            yield return null;
        }

        //プレイヤーが大砲の部屋に入るまで待機
        m_IsPlayerInTaihouRoom = false;
        while (!m_IsPlayerInTaihouRoom)
        {
            yield return null;
        }

        //入ったら再生
        SoundManager.Instance.volume.bgm = 1.0f;
        SoundManager.Instance.PlayBgm("stagef4");

        //大砲が完成するまで待機
        m_IsCreatedTaihou = false;
        while (!m_IsCreatedTaihou)
        {
            yield return null;
        }

        

        //大砲が完成したら
        //再生
        SoundManager.Instance.PlaySe("stagef5");
        //ＢＧＭをフェードアウト
        timer = 0.0f;
        while (timer < m_Stagef4BGMFadeOutTime)
        {
            timer += Time.deltaTime;
            SoundManager.Instance.volume.bgm = Mathf.Lerp(1.0f, 0.0f, timer / m_Stagef4BGMFadeOutTime);
            yield return null;
        }


        //stagef5が終わるまで待機（フェードアウトの時間分も足して終わるまでの時間を計測するのでtimer = 0.0fしない）
        while (timer < m_Stagef5to6BGMWaitTime)
        {
            timer += Time.deltaTime;
            if (timer >= m_Stagef5to6BGMWaitTime)
            {
                //stagef5が終わったら再生
                SoundManager.Instance.volume.bgm = 1.0f;
                SoundManager.Instance.PlayBgm("stagef6");
            }
            yield return null;
        }

        //壁が破壊されるまで待機
        m_IsBreakedWall = false;
        while (!m_IsBreakedWall)
        {
            yield return null;
        }

        //壁が破壊されたらＢＧＭをフェードアウト
        timer = 0.0f;
        while (timer < m_Stagef6BGMFadeOutTime)
        {
            timer += Time.deltaTime;
            SoundManager.Instance.volume.bgm = Mathf.Lerp(1.0f, 0.0f, timer / m_Stagef6BGMFadeOutTime);
            yield return null;
        }

        //再生
        SoundManager.Instance.PlaySe("stagef7");
        SoundManager.Instance.volume.bgm = 1.0f;
        SoundManager.Instance.StopBgm();
        yield break;
    }

    /// <summary>
    /// 最終ステージの大砲部屋を塞ぐスイッチを押したらこれを呼ぶ
    /// </summary>
    public void PlayerFinalDoorSwitchTouched()
    {
        m_IsTouchFinalDoorSwitch = true;
    }

    /// <summary>
    /// 大砲のある部屋に入ったらこれを呼ぶ
    /// </summary>
    public void InTaihouRoom()
    {
        m_IsPlayerInTaihouRoom = true;
    }

    /// <summary>
    /// 最終ステージの大砲が完成したらこれを呼ぶ
    /// </summary>
    public void CreatedTaihou()
    {
        m_IsCreatedTaihou = true;
    }


    /// <summary>
    /// 壁が破壊されたらこれを呼ぶ
    /// </summary>
    public void BreakedWall()
    {
        m_IsBreakedWall = true;
    }
}
