using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.up * -5.01f;
        transform.rotation = GameObject.FindGameObjectWithTag("Player").transform.rotation;
	}
}
