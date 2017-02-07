using UnityEngine;
using System.Collections;

/// <summary>
/// UIとイベントの同期システム
/// </summary>
public class UIandCameraSync : MonoBehaviour {

    EventCamera m_EventCamera;
    StageEvent m_StageEvent;


    System.Action m_CameraAction = ()=> { };
    System.Action m_UIAction = () => { };

	// Use this for initialization
	void Start ()
    {
        m_StageEvent = GetComponent<StageEvent>();
        m_EventCamera = GameObject.Find("Camera").GetComponent<EventCamera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_StageEvent.IsChanging() || !m_EventCamera.IsCameraMoveEnd()) return;

        if(Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return))
        {
            m_CameraAction();
            m_UIAction();
        }
	}

    public void SetUIAction(System.Action action)
    {
        m_UIAction = action;
    }

    public void SetCameraAction(System.Action action)
    {
        m_CameraAction = action;
    }
}
