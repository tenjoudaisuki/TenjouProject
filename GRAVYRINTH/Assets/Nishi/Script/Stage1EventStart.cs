using UnityEngine;
using System.Collections;

public class Stage1EventStart : MonoBehaviour
{
    public float mWaitTime = 2.0f;
    public GameObject mGoalObject;

    public GameObject m_FastDrawTexture;
    public GameObject m_DrawTexture;

    public Vector3 m_CenterOffset;

    public float m_Timer;

    // Use this for initialization
    void Start()
    {
        m_Timer = 0;
        if(GameObject.Find("Camera").GetComponent<CameraManager>().GetCurrentCameraState() == State.Clear)
        {
            if (GameManager.Instance.GetCurrentSceneName() == "stagef")
            {
                GameObject.FindObjectOfType<BGMControl>().StageFinalSelected();
            }
            Instantiate(m_FastDrawTexture);
        }
        GameObject.Find("Camera").GetComponent<Stage1EventCamera>().SetCenterOffset(m_CenterOffset);
    }

    public void LateUpdate()
    {
        if (GameObject.Find("Camera").GetComponent<CameraManager>().GetCurrentCameraState() == State.GamePlay)
        {
            m_Timer += Time.deltaTime;
            if(mWaitTime < m_Timer)
            {
                Instantiate(m_DrawTexture);
                GameObject.Find("Camera").GetComponent<Stage1EventCamera>().SetPosition(mGoalObject);
                GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Stage1Event);
                Destroy(this);
            }
        }
    }
}
