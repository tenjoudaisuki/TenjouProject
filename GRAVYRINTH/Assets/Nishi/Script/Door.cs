using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

    public float mSpeed;

	// Update is called once per frame
	void Update () {
        Vector3 axis = transform.parent.right;
        transform.Rotate(axis,mSpeed * Time.deltaTime);
	
	}
}
