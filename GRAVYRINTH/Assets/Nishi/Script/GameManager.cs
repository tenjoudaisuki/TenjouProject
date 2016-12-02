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

    public void Awake()
    {
        mCureentMode = GameMode.Title;
        SceneManager.LoadScene("Stage0", LoadSceneMode.Additive);
        StartCoroutine("CameraWait", SceneManager.GetSceneByName("Stage0"));
        //SceneManager.LoadScene("Title", LoadSceneMode.Additive);
        mCurrentScene = SceneManager.GetSceneByName("Stage0");
    }



    // Use this for initialization
    void Start()
    {

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
        SceneManager.UnloadScene("Menu");
        GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Title);
    }

    void SelectMode()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Additive);
        SceneManager.UnloadScene("Title");
        GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Select);
    }

    void GamePlayMode()
    {
        SceneManager.UnloadScene("Title");
        SceneManager.UnloadScene("Menu");
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
        SceneManager.LoadScene(mNextStageName, LoadSceneMode.Additive);
        SceneManager.UnloadScene(mCurrentScene);
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
    /// プレイヤーやカメラをリセットする
    /// </summary>
    public void Reset()
    {
        GameObject startPoint = GameObject.Find("StartPoint");
        GameObject player = GameObject.Find("Player");

        player.GetComponent<PlayerMoveManager>().SetState(PlayerState.NORMAL);
        player.transform.position = startPoint.transform.position;
        player.transform.localRotation = startPoint.transform.localRotation;

        GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.GamePlay);
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
}
