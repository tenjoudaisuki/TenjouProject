using UnityEngine;
using System.Collections;

/// <summary>
/// UIとイベントの同期システム
/// </summary>
public class UIandCameraSync : MonoBehaviour {

    /// <summary>
    /// イベントカメラ
    /// </summary>
    EventCamera m_EventCamera;
    /// <summary>
    /// ステージ最初のイベントカメラ
    /// </summary>
    Stage1EventCamera m_StageEventCamera;
    StageEvent m_StageEvent;

    bool m_CameraEventEnd;

    System.Action m_CameraAction = ()=> { };
    System.Action m_UIAction = () => { };

	// Use this for initialization
	void Start ()
    {
        m_CameraEventEnd = false;
        m_StageEvent = GetComponent<StageEvent>();
        m_EventCamera = GameObject.Find("Camera").GetComponent<EventCamera>();
        m_StageEventCamera = GameObject.Find("Camera").GetComponent<Stage1EventCamera>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //イベントカメラがアクティブであるなら
        if(m_EventCamera.isActiveAndEnabled)
        {
            m_CameraEventEnd = m_EventCamera.IsCameraMoveEnd();
        }
        if(m_StageEventCamera.isActiveAndEnabled)
        {
            m_CameraEventEnd = m_StageEventCamera.IsCameraMoveEnd();
        }

        if (m_StageEvent.IsChanging() || !m_CameraEventEnd) return;

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

    public void SetIsCameraEvent(bool check)
    {
        //Debug.Log(check);
        m_CameraEventEnd = check;
    }
}
