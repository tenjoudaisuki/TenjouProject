﻿

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
    /// イベントが起動したことがあるか
    /// </summary>
    private bool isSwitch;
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

    public float mEventEndTime = 2.0f;

    public void Start()
    {
        isSwitch = false;
    }

    public void Update()
    {
        if (isActive && !isSwitch)
        {
            mTimer += Time.deltaTime;
            if (mTimer >= mWaitSecond)
            {
                if (isCheckEnd)
                {
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(mMoveTime);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetEventEndTime(mEventEndTime);

                    GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPoint);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetTetubou(mSecondPos);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetGoal(mGoalPos);

                    GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
                }
                else
                {
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(mMoveTime);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetTetubou(mSecondPos);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetGoal(mGoalPos);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetEventEndTime(0.0f);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetBotton(true);
                    GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPoint);

                    GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
                }
                isSwitch = true;
                //UIを表示
                Instantiate(DrawUI);
            }

        }
    }

    public void Active()
    {
        isActive = true;
    }

    /// <summary>
    /// あたり判定の起動
    /// </summary>
    public void TriggerActive()
    {
        isSwitch = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && isActive)
        {

            isActive = false;
            isSwitch = true;
            if (mNextCheckPoint)
            {
                mNextCheckPoint.Active();
                Destroy(this);
            }

        }
    }
}
