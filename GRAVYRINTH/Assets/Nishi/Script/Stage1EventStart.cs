using UnityEngine;
using System.Collections;

public class Stage1EventStart : MonoBehaviour
{
    public float mWaitTime = 2.0f;
    public GameObject mGoalObject;

    // Use this for initialization
    void Start()
    {

    }

    public void LateUpdate()
    {
        if (GameManager.Instance.GetMode() == GameManager.GameMode.GamePlay)
        {
            StartCoroutine(DelayMethod(mWaitTime, () =>
            {
                GameObject.Find("Camera").GetComponent<Stage1EventCamera>().SetPosition(mGoalObject);
                GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Stage1Event);
                Destroy(this);
            }));
        }
    }



    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="delayFrameCount"></param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float delayFrameCount, System.Action action)
    {

        yield return new WaitForSeconds(delayFrameCount);
        action();
    }
}
