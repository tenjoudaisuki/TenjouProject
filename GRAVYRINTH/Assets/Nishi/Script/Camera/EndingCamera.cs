﻿using UnityEngine;
using System.Collections;

public class EndingCamera : ICamera {

    public enum Phase
    {
        None,
        Phase1,  //大砲に乗った～ガラスに近づく
        Phase2,  //ガラスに近づく～ガラスを超える
        Phase3,  //ガラスを超える～ゴールに近づく
        Phase4,  //ゴールに近付く～終わり
    }

    public Phase mCurrentPhase = Phase.None;
    private GameObject mTarget;
    private Vector3 mOffset;

    public float mFirstDistance = 2.0f;
    public float mLastDistance = 2.0f;
    public float mMoveTime = 1.0f;

    public Vector3 mDisolace;

    private bool isfirst = false;

	// Use this for initialization
	public override void  Start ()
    {
        mOffset = (mDisolace + -Vector3.forward) * mFirstDistance;
        mTarget = GameObject.FindGameObjectWithTag("Player");

        LeanTween.move(gameObject, mTarget.transform.position + mOffset, 0.5f)
            .setOnUpdate((float val) => { transform.LookAt(mTarget.transform); })
            .setOnComplete(()=> { mCurrentPhase = Phase.Phase1; });
	}
	
	// Update is called once per frame
	void Update () {
        PhaseUpdate();
	}

    void PhaseUpdate()
    {
        switch(mCurrentPhase)
        {
            case Phase.Phase1:Phase1();break;
            case Phase.Phase2: Phase2(); break;
            case Phase.Phase3: Phase3(); break;
            case Phase.Phase4: Phase4(); break;
        }
    }

    void Phase1()
    {
        mOffset =  (mDisolace + -Vector3.forward) * mFirstDistance;
        if (!isfirst)
        {
            isfirst = true;
            LeanTween.value(mFirstDistance, mLastDistance, mMoveTime).setOnUpdate((float val) => { mFirstDistance = val; });
        }
        transform.position = mTarget.transform.position + mOffset;
        transform.LookAt(mTarget.transform);
    }

    void Phase2()
    {
        transform.position = mTarget.transform.position + mOffset;
        transform.LookAt(mTarget.transform);
    }

    void Phase3()
    {
        transform.position = mTarget.transform.position + mOffset;
        transform.LookAt(mTarget.transform);
    }

    void Phase4()
    {
        transform.position = mTarget.transform.position + mOffset;
        transform.LookAt(mTarget.transform);
    }

    public void PhaseChange(Phase phase,Vector3 offset)
    {
        mOffset = offset;
        transform.position = mTarget.transform.position + mOffset;
        mCurrentPhase = phase;
    }
}
