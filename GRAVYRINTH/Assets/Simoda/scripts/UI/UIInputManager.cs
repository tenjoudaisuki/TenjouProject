using UnityEngine;
using System.Collections;

public class UIInputManager : MonoBehaviour
{
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

}
