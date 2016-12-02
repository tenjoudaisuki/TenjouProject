using UnityEngine;
using System.Collections;

public class SpinChild : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        float speed = transform.parent.GetComponent<SpinParent>().mSpinSpeed;
        transform.Rotate(Vector3.forward,speed * 2);
	}
}
