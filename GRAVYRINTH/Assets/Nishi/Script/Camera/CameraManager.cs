using UnityEngine;
using System.Collections;

public enum State
{
    None,
    Title,
    Select,
    GamePlay,
    Clear
}

public class CameraManager : MonoBehaviour {

    public State mStateState = State.None;
    private State mCurrentState;
    private ICamera mCurrentCameraScript;

	// Use this for initialization
	void Start ()
    {
        StateChange(mStateState);
	}

    public void StateChange(State state)
    {
        if (mCurrentState == state) return;
        if (mCurrentCameraScript) mCurrentCameraScript.enabled = false;
        switch(state)
        {
            case State.Title: ScriptChange(GetComponent<TitleCamera>()); break;
            case State.Select: ScriptChange(GetComponent<SelectCamera>()); break;
            case State.GamePlay:ScriptChange(GetComponent<CameraControl>()); break;
            case State.Clear: ScriptChange(GetComponent<ClearCamera>()); break;
        }
        mCurrentState = state;
    }

    void ScriptChange(ICamera camerascript)
    {
        mCurrentCameraScript = camerascript;
        mCurrentCameraScript.enabled = true;
        mCurrentCameraScript.Start();
    }
}
