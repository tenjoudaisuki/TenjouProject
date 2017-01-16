using UnityEngine;
using System.Collections;

public class SwitchManager : MonoBehaviour
{

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
            LeanTween.rotate(mFinalDoor, new Vector3(90, 0, 0), 1.0f);
            LeanTween.moveLocalY(gameObject, -0.03f, 1.0f);
        }
    }
}
