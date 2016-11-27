using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextStageFade : MonoBehaviour {

    enum FadeMode
    {
        FadeIn,
        FadeOut
    }

    Image mImage;
    public float mSpeed;
    public Color mColor;

    public string mNextScene;

    FadeMode mState = FadeMode.FadeOut;

    Scene mFromScene;

	// Use this for initialization
	void Start ()
    {
        mFromScene = SceneManager.GetActiveScene();
        mState = FadeMode.FadeIn;
        mImage = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        StateUpdate();
	}

    void StateUpdate()
    {
        switch(mState)
        {
            case FadeMode.FadeIn: FadeIn(); break;
            case FadeMode.FadeOut: FadeOut(); break;
        }
    }

    void FadeIn()
    {
        mColor.a += mSpeed * Time.deltaTime;
        mImage.color = mColor;
        if (mColor.a >= 1)
        {
            mState = FadeMode.FadeOut;

            SceneManager.LoadScene(mNextScene, LoadSceneMode.Additive);
            SceneManager.UnloadScene(mFromScene);
        }

    }

    void FadeOut()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mNextScene));
        GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.GamePlay);
        mColor.a -= mSpeed * Time.deltaTime;
        mImage.color = mColor;
        if (mColor.a <= 0)
        {
            SceneManager.UnloadScene("Fade");
        }
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="delayFrameCount"></param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(int delayFrameCount, System.Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }
        action();
    }
}
