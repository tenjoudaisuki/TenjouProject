using UnityEngine;
using System.Collections;

public class DoorButton : MonoBehaviour
{

    public GameObject mButton;
    public GameObject mAura;
    public GameObject mDoor;
    public Light mlight;

    private bool isDown = false;

    // Use this for initialization
    void Start()
    {
        isDown = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" || isDown == true) return;
        LeanTween.moveLocalY(mButton, -0.015f, 1.0f);
        //mButton.SetActive(false);
        mDoor.transform.localPosition = new Vector3(0.0f, -0.01f, 0.0f);
        mAura.SetActive(false);
        mlight.gameObject.SetActive(true);
        LeanTween.value(0.1f,8.0f, 2.0f).setOnUpdate((float val)=> { mlight.intensity = val; });
        isDown = true;
    }

    public bool IsButtonDown()
    {
        return isDown;
    }
}
