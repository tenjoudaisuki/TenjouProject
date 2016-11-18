using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public float mSpeed;

	// Update is called once per frame
	void Update () {
        transform.RotateAroundLocal(transform.forward,mSpeed * Time.deltaTime);
	
	}
}
