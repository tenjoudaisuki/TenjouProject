using UnityEngine;
using System.Collections;

public class SelectCamera : ICamera {

    Vector3 mNextPosition;
    Quaternion mNextRotate;
    Vector3 mFromPos;
    Quaternion mFromRotate;

    float mTimer = 0.0f;

	// Use this for initialization
	public override void Start ()
    {
        mTimer = 0.0f;
        mNextPosition = GameObject.Find("SelectCameraPosition").transform.position;
        mNextRotate = GameObject.Find("SelectCameraPosition").transform.localRotation;
        mFromPos = transform.position;
        mFromRotate = transform.localRotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        mTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(mFromPos, mNextPosition, mTimer);
        transform.localRotation = Quaternion.Slerp(mFromRotate, mNextRotate, mTimer);
        if (Input.GetKeyDown(KeyCode.C))
        {
            GetComponent<CameraManager>().StateChange(State.GamePlay);
        }

    }
}
