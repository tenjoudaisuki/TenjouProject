using UnityEngine;
using System.Collections;

public class Stage1EventStart : MonoBehaviour
{
    public float mWaitTime = 2.0f;
    public GameObject mGoalObject;
    public GameObject m_FastDrawTexture;
    public GameObject m_DrawTexture;

    // Use this for initialization
    void Start()
    {
        if(GameManager.Instance.GetMode() == GameManager.GameMode.GamePlay)
        {
            Instantiate(m_FastDrawTexture);
        }
    }

    public void LateUpdate()
    {
        if (GameObject.Find("Camera").GetComponent<CameraManager>().GetCurrentCameraState() == State.GamePlay)
        {
            StartCoroutine(DelayMethod(mWaitTime, () =>
            {
                Instantiate(m_DrawTexture);
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
