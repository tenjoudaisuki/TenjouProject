﻿using UnityEngine;
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

    public void Start()
    {
        isFirst = false;
    }

    public void Update()
    {
        if (GameManager.Instance.GetMode() == GameManager.GameMode.GamePlay && !isFirst)
        {
            GameObject.Find("Camera").GetComponent<EventCamera>().SetCompleteAction(
                () => { mNextPoint.isActive = true; }
                );
            GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(1.0f);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetBotton(true);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPoint);

            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
            Instantiate(DrawUI);
            isFirst = true;
        }

    }
}
