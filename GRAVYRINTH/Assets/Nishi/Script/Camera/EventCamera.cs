using UnityEngine;
using System.Collections;

public class EventCamera : ICamera {

    Vector3 mToPosition;
    Vector3 mToRotate;

    float mMoveTime;
    float mEventEndTime;

    float timer;

    System.Action mCompleteAction = () => { };

    // Use this for initialization
    public override void Start ()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveManager>().SetState(PlayerState.NONE);
        LeanTween.move(gameObject, mToPosition, mMoveTime)
            .setOnComplete(()=> {
                mCompleteAction();
                mCompleteAction = () => { };
                LeanTween.move(gameObject, gameObject.transform.position, 0.0f)
                .setDelay(mEventEndTime)
                .setOnComplete(() => { GetComponent<CameraManager>().StateChange(State.GamePlay);
                    GetComponent<CameraManager>().CameraReset();
                });
            });
        LeanTween.rotateLocal(gameObject, mToRotate, mMoveTime);

    }

    /// <summary>
    /// 移動時間の設定
    /// </summary>
    /// <param name="time"></param>
    public void SetMoveTime(float time)
    {
        mMoveTime = time;
    }

    /// <summary>
    /// 終了時間の設定
    /// </summary>
    /// <param name="time"></param>
    public void SetEventEndTime(float time)
    {
        mEventEndTime = time;
    }

    /// <summary>
    /// ターゲットの位置と向きになるように設定
    /// </summary>
    /// <param name="obj"></param>
    public void SetTarget(GameObject obj)
    {
        mToPosition = obj.transform.position;
        mToRotate = obj.transform.localEulerAngles;
    }

    /// <summary>
    /// 位置を設定
    /// </summary>
    /// <param name="position"></param>
    public void SetPosition(Vector3 position)
    {
        mToPosition = position;
    }

    /// <summary>
    /// 回転を設定
    /// </summary>
    /// <param name="rotate"></param>
    public void SetRotate(Vector3 rotate)
    {
        mToRotate = rotate;
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="delayFrameCount"></param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(int delayFrameCount, System.Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }
        action();
    }

    public void SetCompleteAction(System.Action action)
    {
        mCompleteAction = action;
    }
}
