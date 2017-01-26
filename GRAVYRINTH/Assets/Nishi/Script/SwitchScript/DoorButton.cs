using UnityEngine;
using System.Collections;

public class DoorButton : MonoBehaviour
{
    public GameObject mCameraPos;

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

        GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(2.0f);
        GameObject.Find("Camera").GetComponent<EventCamera>().SetEventEndTime(3.0f);
        GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPos);

        LeanTween.moveLocalX(gameObject,transform.position.x,0).setDelay(0.5f).setOnComplete(() => {
            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
        });

        mButton.SetActive(false);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveManager>().SetState(PlayerState.NOT_MOVE);

        GameObject.Find("Camera").GetComponent<EventCamera>().SetCompleteAction(
            () => {
                LeanTween.moveLocalY(mDoor,-0.01f,1.0f).setDelay(1.0f);
                LeanTween.alpha(mAura, 0.0f, 1.0f).setOnComplete(() => { Destroy(mAura); }).setDelay(3.0f);
                mlight.gameObject.SetActive(true);
                LeanTween.value(0.1f, 8.0f, 1.0f).setOnUpdate((float val) => { mlight.intensity = val; }).setDelay(5.0f);
            });

        isDown = true;
    }

    public bool IsButtonDown()
    {
        return isDown;
    }
}
