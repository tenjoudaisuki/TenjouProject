using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    private Scene StageSelect;
    private Scene TutorialTest;

    public void Awake()
    {
        StageSelect = SceneManager.GetSceneByName("StageSelect");
        TutorialTest = SceneManager.GetSceneByName("TutorialTestSimoda");
    }

    void Start()
    {
        StartCoroutine(DelayMethod(0, () => { SceneManager.LoadScene("TutorialTestSimoda", LoadSceneMode.Additive); }));
        StartCoroutine(DelayMethod(1, () => { SceneManager.SetActiveScene(TutorialTest); }));
        StartCoroutine(DelayMethod(2, () => { GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Select); }));
        StartCoroutine(DelayMethod(3, () => { SceneManager.SetActiveScene(StageSelect); }));
    }

    void Update()
    {

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
