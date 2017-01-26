using UnityEngine;
using System.Collections;

public class UIInputManager : MonoBehaviour
{
    private System.Action submitAction;

    private System.Action cancelAction;

    void Start()
    {

    }

    void Update()
    {

    }

    //↑入力時、処理
    public void UpAction(System.Action action)
    {
        if (Input.GetAxis("Vertical") > 0.1f)
            action();
    }

    //↓入力時、処理
    public void DownAction(System.Action action)
    {
        if (Input.GetAxis("Vertical") < -0.1f)
            action();
    }

    //→入力時、処理
    public void RightAction(System.Action action)
    {
        if (Input.GetAxis("Horizontal") > 0.1f)
            action();
    }

    //←入力時、処理
    public void LeftAction(System.Action action)
    {
        if (Input.GetAxis("Horizontal") < -0.1f)
            action();
    }

    //決定入力時の処理を設定
    public void SetSubmitAction(System.Action action)
    {
        submitAction = action;
    }

    //決定入力時、処理
    public void Submit()
    {
        submitAction();
    }

    //キャンセル入力時の処理を設定
    public void SetCancelAction(System.Action action)
    {
        cancelAction = action;
    }

    //キャンセル入力時、処理
    public void Cancel()
    {
        cancelAction();
    }
}
