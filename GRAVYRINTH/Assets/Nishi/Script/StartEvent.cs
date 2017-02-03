using UnityEngine;
using System.Collections;

public class StartEvent : MonoBehaviour
{

    public EventPoint mNextPoint;
    public GameObject mCameraPoint;
    /// <summary>
    /// 表示するテキスト
    /// </summary>
    public GameObject DrawUI;
    private bool isFirst;
    public float mWaitTime = 2.0f;

    public void Start()
    {
        isFirst = false;
    }

    public void Update()
    {
        if (GameManager.Instance.GetMode() == GameManager.GameMode.GamePlay && !isFirst)
        {
            StartCoroutine(DelayMethod(mWaitTime , () =>
             {
                 GameObject.Find("Camera").GetComponent<EventCamera>().SetmButtonActiveCompleateAction(() => { mNextPoint.isActive = true; });
                 GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(1.0f);
                 GameObject.Find("Camera").GetComponent<EventCamera>().SetBotton(true);
                 GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPoint);

                 GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
                 Instantiate(DrawUI);
             }));
            isFirst = true;
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
