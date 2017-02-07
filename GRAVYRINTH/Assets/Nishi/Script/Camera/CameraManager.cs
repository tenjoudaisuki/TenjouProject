using UnityEngine;
using System.Collections;

public enum State
{
    None,
    Title,
    Select,
    GamePlay,
    Clear,
    Event,
    Ending,
    Stage1Event
}

public class CameraManager : MonoBehaviour
{

    public State mStateState = State.None;
    private State mCurrentState;
    private ICamera mCurrentCameraScript;

    // Use this for initialization
    void Start()
    {
        StateChange(mStateState);
    }

    public void Update()
    {
    }

    public void StateChange(State state)
    {
        if (mCurrentState == state) return;
        if (mCurrentCameraScript) mCurrentCameraScript.enabled = false;
        switch (state)
        {
            case State.Title: ScriptChange(GetComponent<TitleCamera>()); break;
            case State.Select: ScriptChange(GetComponent<SelectCamera>()); break;
            case State.GamePlay: ScriptChange(GetComponent<CameraControl>()); break;
            case State.Clear: ScriptChange(GetComponent<ClearCamera>()); break;
            case State.Ending: ScriptChange(GetComponent<EndingCamera>()); break;
            case State.Event: ScriptChange(GetComponent<EventCamera>()); break;
            case State.Stage1Event: ScriptChange(GetComponent<Stage1EventCamera>()); break;
        }
        mCurrentState = state;
    }

    void ScriptChange(ICamera camerascript)
    {
        mCurrentCameraScript = camerascript;
        mCurrentCameraScript.enabled = true;
        mCurrentCameraScript.Start();
    }

    public void CameraReset()
    {
        mCurrentCameraScript.Start();
    }

    public void CameraWarp()
    {
        mCurrentCameraScript.Warp();
    }

    public State GetCurrentCameraState()
    {
        return mCurrentState;
    }
}
