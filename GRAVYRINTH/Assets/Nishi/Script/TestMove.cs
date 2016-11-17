using UnityEngine;
using System.Collections;

public class TestMove : MonoBehaviour {

    public GameObject origin;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = origin.transform.position + new Vector3(0,0,0.5f);
	}
}
