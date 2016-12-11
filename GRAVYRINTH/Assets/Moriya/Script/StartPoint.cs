using UnityEngine;
using System.Collections;

public class StartPoint : MonoBehaviour {

    private PlayerRespawn m_PlayerRespawn;

    // Use this for initialization
    void Start ()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<NormalMove>().Respawn(transform.position,transform.up,transform.forward);
    }
}
