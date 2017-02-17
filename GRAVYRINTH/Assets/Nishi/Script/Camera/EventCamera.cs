using UnityEngine;
using System.Collections;

public class EventCamera : ICamera
{

    Vector3 mToPosition;
    Vector3 mToRotate;

    float mMoveTime;
    float mEventEndTime;

    float timer;

    System.Action mCompleteAction = () => { };
    /// <summary>
    /// ボタンが押せる状態になったら起動させるaction
    /// </summary>
    System.Action mButtonActiveCompleateAction = () => { };

    /// <summary>
    /// ボタンでイベントを終了するように
    /// </summary>
    bool mButtonMode;
    /// <summary>
    /// ボタンが押されてイベントが終了d出来る合図
    /// </summary>
    bool mButtonEventEnd;

    /// <summary>
    /// 鉄棒用カメラ
    /// </summary>
    GameObject mTetubou;
    /// <summary>
    /// ゴールをみる用
    /// </summary>
    GameObject mGoalPos;

    // Use this for initialization
    public override void Start()
    {
        mButtonEventEnd = false;
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveManager>().SetState(PlayerState.NOT_MOVE);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveManager>().SetEventInputDisable(true);
        print("call EventCamera Start()");

        GameManager.Instance.SetPausePossible(false);

        LeanTween.move(gameObject, mToPosition, mMoveTime)
            .setOnComplete(() =>
            {
                mCompleteAction();
                mCompleteAction = () => { };
                //ボタンで終了しなければ
                if (!mButtonMode)
                {
                    if (mTetubou)
                    {
                        LeanTween.move(gameObject, gameObject.transform.position, 0.0f)
                        .setDelay(mEventEndTime)
                        .setOnComplete(() =>
                        {
                            SetTarget(mTetubou);
                            SetBotton(true);
                            Start();
                            mTetubou = null;
                            return;
                        });
                    }
                    else
                    {
                        //ダミー
                        LeanTween.move(gameObject, gameObject.transform.position, 0.0f)
                        .setDelay(mEventEndTime)
                        .setOnComplete(() =>
                        {
                            GetComponent<CameraManager>().StateChange(State.GamePlay);
                            GetComponent<CameraManager>().CameraReset();
                        });
                    }
                }
                else
                {
                    mButtonEventEnd = true;
                    //同期システムにラムダを送る
                    var syncSystem = GameObject.FindObjectOfType<UIandCameraSync>();
                    if (!syncSystem) return;
                    syncSystem.SetCameraAction(() =>
                    {
                        mButtonEventEnd = false;
                        mButtonMode = false;

                        //アクションを起動
                        mButtonActiveCompleateAction();
                        mButtonActiveCompleateAction = () => { };

                        //ゴールPositionがあれば
                        if (mGoalPos)
                        {
                            //ダミー　ディレイをかける
                            LeanTween.move(gameObject, gameObject.transform.position, 0.0f)
                                    .setOnComplete(() =>
                                    {
                                        SetTarget(mGoalPos);
                                        SetBotton(true);
                                        Start();
                                        mGoalPos = null;
                                        return;
                                    });
                        }
                        else
                        {
                            LeanTween.move(gameObject, gameObject.transform.position, 0.0f)
                            .setDelay(mEventEndTime)
                            .setOnComplete(() =>
                            {
                                GetComponent<CameraManager>().StateChange(State.GamePlay);
                                GetComponent<CameraManager>().CameraReset();
                            });
                        }
                    });
                    
                }
            });

        LeanTween.rotateLocal(gameObject, mToRotate, mMoveTime);
    }

    public void Update()
    {
        //GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveManager>().SetState(PlayerState.NOT_MOVE);
        var syncSystem = GameObject.FindObjectOfType<UIandCameraSync>();
        if (syncSystem) return;

        if (mButtonMode && mButtonEventEnd && (Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return)) )
        {
            mButtonEventEnd = false;
            mButtonMode = false;

            //アクションを起動
            mButtonActiveCompleateAction();
            mButtonActiveCompleateAction = () => { };

            //ゴールPositionがあれば
            if (mGoalPos)
            {
                //ダミー　ディレイをかける
                LeanTween.move(gameObject, gameObject.transform.position, 0.0f)
                        .setOnComplete(() =>
                        {
                            SetTarget(mGoalPos);
                            SetBotton(true);
                            Start();
                            mGoalPos = null;
                            return;
                        });
            }
            else
            {
                LeanTween.move(gameObject, gameObject.transform.position, 0.0f)
                .setDelay(mEventEndTime)
                .setOnComplete(() =>
                        {
                            GetComponent<CameraManager>().StateChange(State.GamePlay);
                            GetComponent<CameraManager>().CameraReset();
                        });
            }
        }
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
    /// buttonでイベントを終了する
    /// </summary>
    /// <param name="rotate"></param>
    public void SetBotton(bool isCheck)
    {
        mButtonMode = isCheck;
    }

    public void SetTetubou(GameObject tetubou)
    {
        mTetubou = tetubou;
    }

    public void SetGoal(GameObject Goal)
    {
        mGoalPos = Goal;
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

    public void SetmButtonActiveCompleateAction(System.Action action)
    {
        mButtonActiveCompleateAction = action;
    }

    /// <summary>
    /// カメラの移動が終了したか？
    /// </summary>
    /// <returns>true = イベント終了</returns>
    public bool IsCameraMoveEnd()
    {
        return mButtonEventEnd;
    }
}
