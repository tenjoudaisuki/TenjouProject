using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
        mCurrentScene = SceneManager.GetSceneByName("Stage0");
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

        player.transform.position = startPoint.transform.position;
        player.transform.localRotation = startPoint.transform.localRotation;

        GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.GamePlay);
    }
}
