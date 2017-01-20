using UnityEngine;
using System.Collections;

public class TitleCamera : ICamera {

    public GameObject mStageCenter;
    public float mSpeed;
    public float mDistance = 5.0f;
    public Vector3 mOffset;

    bool isStart = false;
    Vector3 mFromPos;
    float mTimer = 0.0f;

	// Use this for initialization
	public override void Start ()
    {
        isStart = false;
        mStageCenter = GameObject.Find("StageCenter");
	}

    // Update is called once per frame
    void Update()
    {
        if (!mStageCenter) return;
        if (!isStart)
        {
            isStart = true;
            transform.position = (mStageCenter.transform.forward * -mDistance) + mOffset;
        }
        transform.LookAt(mStageCenter.transform);
        transform.RotateAround(mStageCenter.transform.position, Vector3.up, mSpeed * Time.deltaTime);
    }
}
