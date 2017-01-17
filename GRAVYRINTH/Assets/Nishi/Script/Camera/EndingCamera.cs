using UnityEngine;
using System.Collections;

public class EndingCamera : ICamera {

    public enum Phase
    {
        Phase1,  //大砲に乗った～ガラスに近づく
        Phase2,  //ガラスに近づく～ガラスを超える
        Phase3,  //ガラスを超える～ゴールに近づく
        Phase4,  //ゴールに近付く～終わり
    }

    private Phase mCurrentPhase;
    private GameObject mTarget;
    private Vector3 mOffset;

    private float mDistance = 2.0f;
    public float mSpeed = 1.0f;

	// Use this for initialization
	public override void  Start ()
    {
        mCurrentPhase = Phase.Phase1;
        mDistance = 2;
        mOffset = -Vector3.forward * mDistance;
        mTarget = GameObject.FindGameObjectWithTag("Player");
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

        mDistance -= mSpeed * Time.deltaTime;
        mOffset = -Vector3.forward * mDistance;
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
