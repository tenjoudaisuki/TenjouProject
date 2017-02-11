﻿using UnityEngine;
using System.Collections;

public class SwitchManager : MonoBehaviour
{

    public GameObject mCameraPos;

    public DoorButton[] mDoorButtons;
    public GameObject mFinalDoor;
    public GameObject mEventUI;
    bool isActive;

    public float mOpenTime = 5.0f;

    // Use this for initialization
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        ButtonCheck();
    }

    void ButtonCheck()
    {
        if (isActive) return;

        foreach (DoorButton button in mDoorButtons)
        {
            if (!button.IsButtonDown()) return;
        }
        isActive = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(isActive && other.tag == "Player")
        {
            Instantiate(mEventUI);
            //ゴゴゴと扉が開く音
            SoundManager.Instance.PlayLoopSe("rumble");
            GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(2.0f);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetEventEndTime(0.0f);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetBotton(true);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPos);

            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);

            //扉が開いたときの音
            LeanTween.rotate(mFinalDoor, new Vector3(90, 0, 0), mOpenTime).setOnComplete(()=> {
                SoundManager.Instance.PlaySe("doom");
                SoundManager.Instance.StopLoopSe();
            });
            gameObject.SetActive(false);

            GameObject.Find("BGMControl").GetComponent<BGMControl>().PlayerFinalDoorSwitchTouched();
        }
    }
}
