using UnityEngine;
using System.Collections;

public class PlayerHitinfo : MonoBehaviour
{
    Vector3 m_CheckPoint;
    Quaternion m_Rotate;
    Vector3 m_GravityDir;


    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "CheckPoint")
        {
            m_CheckPoint = other.transform.position;
            m_Rotate = transform.parent.localRotation;
            m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>().GetDirection();
            Debug.Log("チェックポイント" + m_GravityDir);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.name == "Inside")
        {
            Debug.Log("もどれ" + m_GravityDir);
            GameObject.Find("GravityDirection").GetComponent<GravityDirection>().SetDirection(m_GravityDir);
            transform.parent.transform.position = m_CheckPoint;
            transform.parent.transform.localRotation = m_Rotate;
        }
    }
}
