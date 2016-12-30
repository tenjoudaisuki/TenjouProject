using UnityEngine;
using System.Collections;

public enum State
{
    None,
    Title,
    Select,
    GamePlay,
    Clear,
    Ending
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
        //if(Input.GetButtonDown("PS4_Square"))
        //{
        //    Debug.Log("しかく");
        //}
        //if (Input.GetButtonDown("PS4_Cross"))
        //{
        //    Debug.Log("ばつ");
        //}
        //if (Input.GetButtonDown("PS4_Circle"))
        //{
        //    Debug.Log("まる");
        //}
        //if (Input.GetButtonDown("PS4_Triangle"))
        //{
        //    Debug.Log("さんかく");
        //}
        //if (Input.GetButtonDown("PS4_R3"))
        //{
        //    Debug.Log("R3");
        //}
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
}
