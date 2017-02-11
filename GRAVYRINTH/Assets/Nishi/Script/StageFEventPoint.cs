using UnityEngine;
using System.Collections;

public class StageFEventPoint : MonoBehaviour
{

    public float m_MoveTime = 2.0f;
    public GameObject m_LookPoint;
    public GameObject m_EventUI;

    // Use this for initialization
    void Start()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Instantiate(m_EventUI);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(m_MoveTime);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetEventEndTime(0.0f);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetBotton(true);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(m_LookPoint);

            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);
            Destroy(this);
        }
    }
}
