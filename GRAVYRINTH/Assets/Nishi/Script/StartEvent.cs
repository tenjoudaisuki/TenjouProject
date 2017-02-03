using UnityEngine;
using System.Collections;

public class StartEvent : MonoBehaviour {

    public GameObject mCameraPoint;
    /// <summary>
    /// 表示するテキスト
    /// </summary>
    public GameObject DrawUI;
    // Use this for initialization
    void Start () {
        GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(1.0f);
        GameObject.Find("Camera").GetComponent<EventCamera>().SetBotton(true);
        GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(mCameraPoint);

        GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
        Instantiate(DrawUI);

    }
}
