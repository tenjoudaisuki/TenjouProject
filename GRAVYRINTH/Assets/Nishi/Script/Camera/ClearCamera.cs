using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ClearCamera : MonoBehaviour {

    enum Mode
    {
        Look,
        Approach
    }

    public GameObject mClearObject;

    public float mApproachSpeed = 0.1f;
    public float mLookDistance = 4;


    Vector3 mFromPosition;
    Vector3 mNextPosition;

    Quaternion mFromRotate;
    Quaternion mNextRotate;

    Mode mState;
    float mTimer = 0.0f;

    public string mNextScene;

    // Use this for initialization
    void Start ()
    {
        mFromPosition = transform.position;
        mNextPosition = mClearObject.transform.position + (mClearObject.transform.forward * mLookDistance);
        mFromRotate = transform.localRotation;
        mNextRotate = Quaternion.LookRotation(-mClearObject.transform.forward);
        mState = Mode.Look;
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
            case Mode.Look: Look(); break;
            case Mode.Approach: Approach(); break;
        }
    }

    void Look()
    {
        mTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(mFromPosition, mNextPosition, mTimer);
        transform.localRotation = Quaternion.Slerp(mFromRotate, mNextRotate, mTimer);
        if (mTimer > 1)
        {
            SceneManager.LoadScene("Fade", LoadSceneMode.Additive);
            StartCoroutine(DelayMethod(1, () => { SceneManager.SetActiveScene(SceneManager.GetSceneByName("Fade")); }));
            mState = Mode.Approach;
        }
    }

    void Approach()
    {
        transform.position += (-mClearObject.transform.forward * mApproachSpeed);
        GameObject.Find("Fade").GetComponent<NextStageFade>().mNextScene = mNextScene;
        StartCoroutine(DelayMethod(1, () => { SceneManager.SetActiveScene(SceneManager.GetSceneByName(mNextScene)); }));

        //StartCoroutine(DelayMethod(3, () => { SceneManager.UnloadScene("TutorialNishiTest(Parts)"); }));
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
