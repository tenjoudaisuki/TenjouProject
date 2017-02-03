using UnityEngine;
using System.Collections;

public class EventPoint : MonoBehaviour
{

    public float mMoveTime = 1.0f;
    public GameObject mCameraPoint;

    /// <summary>
    /// 表示するテキスト
    /// </summary>
    public GameObject DrawUI;
    /// <summary>
    /// 待つ時間
    /// </summary>
    public float mWaitSecond;

    /// <summary>
    /// 次のチェックポイント
    /// </summary>
    public EventPoint mNextCheckPoint;
    /// <summary>
    /// イベントが起動しているか
    /// </summary>
    public bool isActive;
    /// <summary>
    /// タイマー
    /// </summary>
    float mTimer;

    /// <summary>
    /// 最後のチェックポイントである
    /// </summary>
    public  bool isCheckEnd;
   
    /// <summary>
    /// 最後のチェックポイントの時のみ入れる　２個目の鉄棒位置
    /// </summary>
    public GameObject mSecondPos;
    /// <summary>
    /// 最後のチェックポイントの時のみ入れる　ゴールを見る位置
    /// </summary>
    public GameObject mGoalPos;

    public void Start()
    {
    }

    public void Update()
    {
        if (isActive)
        {
            mTimer += Time.deltaTime;
            if (mTimer >= mWaitSecond)
            {
                if (isCheckEnd)
                {
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(mMoveTime);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPoint);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetTetubou(mSecondPos);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetGoal(mGoalPos);

                    GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
                }
                else
                {
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(mMoveTime);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetBotton(true);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPoint);

                    GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
                }
                isActive = false;
                //UIを表示
                Instantiate(DrawUI);
            }

        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isActive = false;
            if(mNextCheckPoint)mNextCheckPoint.isActive = true;
        }
    }
}
