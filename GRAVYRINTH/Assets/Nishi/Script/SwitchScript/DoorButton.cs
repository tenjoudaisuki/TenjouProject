using UnityEngine;
using System.Collections;

public class DoorButton : MonoBehaviour
{
    public GameObject mCameraPos;

    public GameObject mButton;
    public GameObject mAura;
    public GameObject mDoor;
    static int mCount = 0;
    public Light mlight;

    public GameObject[] mEventUIs;

    private bool isDown = false;

    // Use this for initialization
    void Start()
    {
        mCount = 0;
        isDown = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" || isDown == true) return;

        SoundManager.Instance.PlaySe("switch");

        GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(2.0f);
        GameObject.Find("Camera").GetComponent<EventCamera>().SetEventEndTime(0.0f);
        GameObject.Find("Camera").GetComponent<EventCamera>().SetBotton(true);
        GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPos);

        StartCoroutine(DelayMethod(0.5f, () => { GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event); }));

        mButton.SetActive(false);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveManager>().SetState(PlayerState.NOT_MOVE);
        Instantiate(mEventUIs[mCount]);


        SoundManager.Instance.PlayLoopSe("rumble");
        SoundManager.Instance.PlayLoopSe("trick");

        GameObject.Find("Camera").GetComponent<EventCamera>().SetCompleteAction(
            () => {
                LeanTween.moveLocalY(mDoor, -0.01f, 1.0f).setDelay(1.0f).setOnComplete(() => { SoundManager.Instance.StopLoopSe(); });
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
