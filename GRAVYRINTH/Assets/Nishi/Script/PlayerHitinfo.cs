using UnityEngine;
using System.Collections;

public class PlayerHitinfo : MonoBehaviour
{
    public Vector3 m_CheckPoint;

    public void OnTriggerEnter(Collider other)
    {
        if(other.name == "CheckPoint")
        {
            m_CheckPoint = other.gameObject.transform.position;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.name == "Inside")
        {
            transform.parent.transform.position = m_CheckPoint;
        }
    }
}
