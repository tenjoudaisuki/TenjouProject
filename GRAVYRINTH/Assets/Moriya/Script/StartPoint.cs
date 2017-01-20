﻿using UnityEngine;
using System.Collections;

public class StartPoint : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveManager>().SetState(PlayerState.NORMAL);
        GameObject.Find("GravityDirection").GetComponent<GravityDirection>().SetDirection(-gameObject.transform.up);
        GameObject.FindGameObjectWithTag("Player").GetComponent<NormalMove>().Respawn(transform.position,transform.up,transform.forward);
    }
}
