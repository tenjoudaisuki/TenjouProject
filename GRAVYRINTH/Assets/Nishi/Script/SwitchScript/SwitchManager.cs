using UnityEngine;
using System.Collections;

public class SwitchManager : MonoBehaviour
{

    public GameObject mCameraPos;

    public DoorButton[] mDoorButtons;
    public GameObject mFinalDoor; 
    bool isActive;

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
            GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(2.0f);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetEventEndTime(3.0f);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPos);

            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);

            LeanTween.rotate(mFinalDoor, new Vector3(90, 0, 0), 1.0f);
            gameObject.SetActive(false);
        }
    }
}
