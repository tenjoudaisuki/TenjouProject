using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ClearCamera : ICamera {

    enum Mode
    {
        Look,
        Approach
    }

    public GameObject mClearObject;

    public float mApproachSpeed = 0.1f;
    public float mLookDistance = 4;


    Vector3 mFromPosition;
    Vector3 mNextPosition;

    Quaternion mFromRotate;
    Quaternion mNextRotate;

    Mode mState;
    float mTimer = 0.0f;
    public Color mFadeColor;

    public string mNextScene;

    Vector3 mBackVec;

    // Use this for initialization
    public override void Start ()
    {
        mTimer = 0.0f;
        mClearObject = GameObject.FindGameObjectWithTag("ClearDoor");
        mFromPosition = transform.position;
        mNextPosition = mClearObject.transform.position + (mClearObject.transform.forward * mLookDistance);
        mFromRotate = transform.localRotation;
        mNextRotate = Quaternion.LookRotation(-mClearObject.transform.forward,mClearObject.transform.up);
        mState = Mode.Look;
    }
	
	// Update is called once per frame
	void Update ()
    {
        StateUpdate();
	}

    void StateUpdate()
    {
        switch(mState)
        {
            case Mode.Look: Look(); break;
            case Mode.Approach: Approach(); break;
        }
    }

    /// <summary>
    /// ゴールの真正面に行く処理
    /// </summary>
    void Look()
    {
        mTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(mFromPosition, mNextPosition, mTimer);
        transform.localRotation = Quaternion.Slerp(mFromRotate, mNextRotate, mTimer);
        if (mTimer > 1)
        {
            mBackVec = -mClearObject.transform.forward * mApproachSpeed;
            if(GameManager.Instance.GetCurrentSceneName() == "StageF") GameObject.FindGameObjectWithTag("Fade").GetComponent<FadeFactory>().FadeInstance(true);
            mTimer = 0.0f;
            mState = Mode.Approach;
        }
    }

    /// <summary>
    /// ゴールに近づく処理
    /// </summary>
    void Approach()
    {
        mTimer += Time.deltaTime;
        if (mTimer <= 1) transform.position += mBackVec;
    }
}
