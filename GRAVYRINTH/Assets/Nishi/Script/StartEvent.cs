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
    public float mWaitTime = 2.0f;

    private float mTimer = 0;

    public void Start()
    {
        mTimer = 0;
    }

    public void Update()
    {
        if (GameManager.Instance.GetMode() == GameManager.GameMode.GamePlay)
        {
            mTimer += Time.deltaTime;
            if(mTimer > mWaitTime)
            {
                GameObject.Find("Camera").GetComponent<EventCamera>().SetmButtonActiveCompleateAction(() => { mNextPoint.isActive = true; });
                GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(1.0f);
                GameObject.Find("Camera").GetComponent<EventCamera>().SetEventEndTime(0.0f);
                GameObject.Find("Camera").GetComponent<EventCamera>().SetBotton(true);
                GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPoint);

                GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
                Instantiate(DrawUI);
                Destroy(this);
            }
        }
    }
}
