using UnityEngine;
using System.Collections;

public class SpinParent : MonoBehaviour {

    public float mSpinSpeed;
    	
	// Update is called once per frame
	void Update ()
    {
        gameObject.transform.Rotate(transform.up, mSpinSpeed);
	}
}
