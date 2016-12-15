using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameMode
    {
        Title,
        Select,
        GamePlay
    }

    public bool isDebug = true;

    private static GameManager sInstance;

    public static GameManager Instance
    {
        get
        {
            if (sInstance == null)
                sInstance = GameObject.FindObjectOfType<GameManager>();
            return sInstance;
        }
    }

    /// <summary>
    /// 次のシーンの名前
    /// </summary>
    private string mNextStageName;

    /// <summary>
    /// 現在のシーン
    /// </summary>
    private Scene mCurrentScene;


    /// <summary>
    /// 現在のモード
    /// </summary>
    private GameMode mCureentMode;

    public string mFastStageName;

    public GameObject mPauseMenuprefab;
    private GameObject mPauseMenu;

    public void Awake()
    {
        if (isDebug)
        {
            mCureentMode = GameMode.GamePlay;
            GameObject.Find("Camera").GetComponent<CameraManager>().mStateState = State.GamePlay;
        }
        else
        {
            mCureentMode = GameMode.Title;
            SceneManager.LoadScene("Title", LoadSceneMode.Additive);
            SceneManager.LoadScene(mFastStageName, LoadSceneMode.Additive);
            StartCoroutine("CameraWait", SceneManager.GetSceneByName(mFastStageName));
            //SceneManager.LoadScene("Title", LoadSceneMode.Additive);
            mCurrentScene = SceneManager.GetSceneByName(mFastStageName);
        }
    }



    // Use this for initialization
    void Start()
    {
    }

    public void Update()
    {
        if (mCureentMode == GameMode.GamePlay)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Pause();
            }
        }
    }

    public void GameModeChange(GameMode mode)
    {
        if (mCureentMode == mode) return;
        mCureentMode = mode;
        switch (mCureentMode)
        {
            case GameMode.Title: TitleMode(); break;
            case GameMode.Select: SelectMode(); break;
            case GameMode.GamePlay: GamePlayMode(); break;
        }
    }

    void TitleMode()
    {
        SceneManager.LoadScene("Title", LoadSceneMode.Additive);
        if (!SceneManager.GetSceneByName(mFastStageName).isLoaded)
        {
            GameManager.Instance.SetNextSceneName(mFastStageName);
            GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeFactory>().FadeInstance();
        }
    }

    void SelectMode()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Select);
        GameManager.Instance.SetNextSceneName("Tutorial0_1");
        GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeFactory>().FadeInstance();
    }

    void GamePlayMode()
    {
        GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.GamePlay);
    }

    /// <summary>
    /// 次のシーンを設定する
    /// </summary>
    /// <param name="nextname">次のシーンの名前</param>
    public void SetNextSceneName(string nextname)
    {
        mNextStageName = nextname;
    }

    /// <summary>
    /// 設定したシーンにチェンジする
    /// </summary>
    public void SceneChange()
    {
        SceneManager.UnloadScene(mCurrentScene);
        SceneManager.LoadScene(mNextStageName, LoadSceneMode.Additive);
        mCurrentScene = SceneManager.GetSceneByName(mNextStageName);
    }

    /// <summary>
    /// シーンが読み込まれたか？
    /// </summary>
    /// <returns>ture=読み込まれた</returns>
    public bool isSceneload()
    {
        return mCurrentScene.isLoaded;
    }

    /// <summary>
    /// ゲームモードの最初の画面にリセットする
    /// </summary>
    public void Reset()
    {
        switch(mCureentMode)
        {
            case GameMode.Title: GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Title); ; break;

            case GameMode.Select: GameObject.Find("Camera").GetComponent<CameraManager>().CameraWarp();; break;

            case GameMode.GamePlay:
                GameObject player = GameObject.Find("Player");
                player.GetComponent<PlayerMoveManager>().SetState(PlayerState.NORMAL);
                GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.GamePlay);
                ; break;
        }
    }

    public GameMode GetMode()
    {
        return mCureentMode;
    }

    public string GetCurrentSceneName()
    {
        return mCurrentScene.name;
    }

    IEnumerator CameraWait(Scene scene)
    {
        while (!scene.isLoaded)
        {
            yield return null;
        }
        GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Title);
        yield break; ;
    }

    public void Pause()
    {
        if (mPauseMenu) return;
        Pause(true);
        mPauseMenu = GameObject.Instantiate(mPauseMenuprefab);
    }

    public void Pause(bool pause)
    {
        var pausebles = GameObject.FindGameObjectsWithTag("Pausable");
        foreach (GameObject pauseble in pausebles)
        {
            var script = pauseble.GetComponent<Pausable>();
            script.pausing = pause;
        }
    }

    public void ReStart()
    {
        GameManager.Instance.SetNextSceneName(mCurrentScene.name);
        GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeFactory>().FadeInstance();
    }
}
