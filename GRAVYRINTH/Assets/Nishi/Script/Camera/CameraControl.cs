﻿using UnityEngine;
using System.Collections;

public class CameraControl : ICamera
{
    private enum State
    {
        StartMove,
        Normal,
        BraDown
    }

    [SerializeField, TooltipAttribute("注視点の対象")]
    public Transform m_Target;
    [SerializeField, TooltipAttribute("注視点との距離")]
    public float m_Distance;
    [SerializeField, TooltipAttribute("X回転の上限下限")]
    public float m_XAngleLimit;
    [SerializeField, TooltipAttribute("注視点の調整")]
    public Vector3 m_TargetOffset;
    [SerializeField, TooltipAttribute("回転のスピード")]
    public Vector2 m_RotateSpeed = new Vector2(2.0f,2.0f);
    [SerializeField, TooltipAttribute("ぶら下り時のカメラ回転のスピード")]
    public Vector2 m_DangleRotate;

    //横入力量
    private float m_Horizontal;

    /// <summary>
    /// offsetをモデル座標化
    /// </summary>
    private Vector3 offset;
    /// <summary>
    /// カメラの位置の方向
    /// </summary>
    private Vector3 CameraPosDirection;
    /// <summary>
    /// X軸（right）の回転合計
    /// </summary>
    private float XAxisTotal = 0;
    /// <summary>
    /// Y軸（up）の回転合計
    /// </summary>
    private float YAxisTotal = 0;
    /// <summary>
    /// 最初のカメラ位置
    /// </summary>
    private Vector3 mFastPosition;

    private State mCurrentState = State.StartMove;

    private Vector3 mParallel;
    private Vector3 mInitParallel;


    private float mTimer = 0.0f;

    private Vector3 mUp;

    public override void Start()
    {
        mFastPosition = transform.position;
        mCurrentState = State.StartMove;
    }

    public void LateUpdate()
    {
        StateUpdate();

        if (Input.GetButtonDown("CameraReset"))
        {
            //カメラを元の位置に移動
            CameraReset();
        }
    }

    /// <summary>
    /// ターゲットの周りを回転移動
    /// </summary>
    private void TargetAroundMove(Vector3 up,Vector3 right)
    {
        float horizontal = -Input.GetAxisRaw("Horizontal2") * m_RotateSpeed.x * Time.deltaTime;
        float vertical = -Input.GetAxis("Vertical2") * m_RotateSpeed.y * Time.deltaTime;

        XAxisTotal += vertical;
        //X軸の回転の限界を設定
        XAxisTotal = Mathf.Clamp(XAxisTotal, -m_XAngleLimit, m_XAngleLimit);

        //ターゲットの上ベクトルと自身の横ベクトルの外積で地面と平行なベクトルを作る
        Vector3 parallel = Vector3.Cross(up, right);
        //mParallel = Quaternion.AngleAxis(horizontal, up) * Vector3.Lerp(mParallel,parallel,0.08f);
        mParallel = Quaternion.AngleAxis(horizontal, up) * parallel;
        m_Horizontal = Mathf.Lerp(m_Horizontal, horizontal, 0.3f);

        //平行ベクトルをターゲットの上ベクトルを軸に回転さらに自身の横ベクトルを軸に回転しカメラの位置を計算
        Vector3 temp = Quaternion.AngleAxis(XAxisTotal, transform.right) * mParallel;

        //CameraPosDirection = Vector3.Lerp(CameraPosDirection,temp,0.1f);
        CameraPosDirection = temp;
        //カメラを移動させる
        CameraMove();
    }

    /// <summary>
    /// カメラ位置を移動させる
    /// </summary>
    private void CameraMove()
    {
        //移動先
        Vector3 next;
        //ターゲットを原点にrayを飛ばす
        Ray ray = new Ray(m_Target.position + offset, CameraPosDirection.normalized);

        RaycastHit hit;
        //rayの方向の指定距離以内に障害物が無いか？
        //[IgnoredObj]レイヤー以外と判定させる
        int layermask = ~((1 << 10) | (1 << 8));
        if (Physics.Raycast(ray, out hit, m_Distance + 0.5f,layermask, QueryTriggerInteraction.Ignore))
        {
            //当たらなかったらray* Disをカメラ位置に
            next = hit.point + hit.normal * 0.2f;
            transform.position = next;
        }
        else
        {
            //当たらなかったらray* Disをカメラ位置に
            next = (m_Target.position + offset) +(CameraPosDirection.normalized * m_Distance);
            transform.position = next;
        }
        //next = (Target.position + offset) + (CameraPosDirection * Distance);
        //デバック表示
        Debug.DrawRay(m_Target.position + offset, CameraPosDirection, Color.yellow);

        //補間あり移動
        //transform.position = Vector3.Lerp(transform.position, next, 0.1f);
        //補間なし移動
        //transform.position = next;

    }

    /// <summary>
    /// 通常時カメラ
    /// </summary>
    private void Normal()
    {
        //モデル座標でのオフセット座標を求める
        offset = m_Target.right * m_TargetOffset.x + m_Target.up * m_TargetOffset.y + m_Target.forward * m_TargetOffset.z;

        //ターゲットから見てカメラの方向でカメラの前ベクトルを求める
        Vector3 f = CameraPosDirection;
        //ターゲットの上ベクトルを　右ベクトルを軸にXAxisTotal度回転して　カメラの上ベクトルを求める
        Vector3 up = Quaternion.AngleAxis(XAxisTotal, transform.right) * m_Target.up;

        //ターゲットの周りをステイックによって回転移動
        TargetAroundMove(m_Target.up, transform.right);

        //カメラを回転させる
        //transform.localRotation = Quaternion.Slerp(transform.localRotation,
        //Quaternion.LookRotation(-CameraPosDirection, Quaternion.AngleAxis(XAxisTotal, transform.right) * Target.up), 0.5f);
        //補間なし版
        transform.localRotation = Quaternion.LookRotation(-CameraPosDirection, m_Target.up);

        Debug.DrawRay(m_Target.position, Quaternion.AngleAxis(XAxisTotal, transform.right) * m_Target.up, Color.green);

        if (m_Target.GetComponent<PlayerMoveManager>().GetState() == PlayerState.IRON_BAR_DANGLE)
        {
            CameraPosDirection = -m_Target.forward;
            transform.position = (m_Target.position) + (CameraPosDirection * m_Distance);
            transform.localRotation = Quaternion.LookRotation(-CameraPosDirection, m_Target.up);
            mCurrentState = State.BraDown;
        }
    }

    private void StartMove()
    {
        if (mTimer > 1) mCurrentState = State.Normal;

        CameraPosDirection = -m_Target.forward;
        offset = m_Target.right * m_TargetOffset.x + m_Target.up * m_TargetOffset.y + m_Target.forward * m_TargetOffset.z;
        Vector3 next = (m_Target.position + offset) + (CameraPosDirection * m_Distance);

        transform.position = Vector3.Lerp(mFastPosition, next, mTimer);
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            Quaternion.LookRotation((m_Target.position + offset) - transform.position, m_Target.up),
            mTimer);

        XAxisTotal = 0;
        mTimer += Time.deltaTime;
    }

    private void BraDown()
    {
        float horizontal = -Input.GetAxisRaw("Horizontal2") * m_DangleRotate.x * Time.deltaTime;
        float vertical = -Input.GetAxisRaw("Vertical2") * m_DangleRotate.y * Time.deltaTime;

        XAxisTotal += vertical;
        //X軸の回転の限界を設定
        XAxisTotal = Mathf.Clamp(XAxisTotal, -90, 90);

        YAxisTotal += horizontal;
        //YAxisTotal = Mathf.Clamp(YAxisTotal, -89, 89);

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            XAxisTotal = 90;
            CameraPosDirection = m_Target.up;
            transform.position = (m_Target.position) + (CameraPosDirection * m_Distance);
            transform.localRotation = Quaternion.LookRotation(-CameraPosDirection, m_Target.forward);
        }
        else
        {
            //ターゲットの上ベクトルと自身の横ベクトルの外積で地面と平行なベクトルを作る
            Vector3 parallel = Vector3.Cross(m_Target.transform.up, transform.right);
            //Vector3 parallel = -transform.forward;
            mParallel = parallel;

            Vector3 temp = Quaternion.AngleAxis(horizontal, m_Target.transform.up) * Quaternion.AngleAxis(XAxisTotal, transform.right) * mParallel;
            CameraPosDirection = temp;

            //カメラを移動させる
            Vector3 next;
            //ターゲットを原点にrayを飛ばす
            Ray ray = new Ray(m_Target.position, CameraPosDirection.normalized);

            //RaycastHit hit;
            ////rayの方向の指定距離以内に障害物が無いか？
            //if (Physics.Raycast(ray, out hit, Distance))
            //{
            //    //壁に当たった位置をカメラ位置に
            //    next = hit.point;
            //}
            //else
            //{
            //    //当たらなかったらray* Disをカメラ位置に
            //    next = (Target.position) + (CameraPosDirection * Distance);
            //}
            next = (m_Target.position) + (CameraPosDirection * m_Distance);
            //デバック表示
            Debug.DrawRay(m_Target.position, CameraPosDirection, Color.yellow);

            //補間あり移動
            //transform.position = Vector3.Lerp(transform.position, next, 0.3f);
            //補間なし移動
            transform.position = next;

            Vector3 DirUp = Vector3.Cross(transform.right, CameraPosDirection);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation,
            //        Quaternion.LookRotation((player.transform.position) - transform.position,up), 0.5f);
            transform.localRotation = Quaternion.LookRotation(-CameraPosDirection, Quaternion.AngleAxis(XAxisTotal, transform.right) * m_Target.up);
            Debug.DrawRay(m_Target.position, Quaternion.AngleAxis(XAxisTotal, transform.right) * m_Target.up, Color.green);
        }

        if (m_Target.GetComponent<PlayerMoveManager>().GetState() != PlayerState.IRON_BAR_DANGLE)
        {
            XAxisTotal = m_XAngleLimit;
            mCurrentState = State.Normal;
        }

    }

    /// <summary>
    /// カメラを最初の状態に
    /// </summary>
    private void CameraReset()
    {
        CameraPosDirection = -m_Target.forward;
        CameraMove();
        //カメラを回転させる
        transform.localRotation = Quaternion.LookRotation((m_Target.position + offset) - transform.position, m_Target.up);
        XAxisTotal = 0;
    }

    private void StateUpdate()
    {
        switch (mCurrentState)
        {
            case State.StartMove: StartMove(); break;
            case State.Normal: Normal(); break;
            case State.BraDown: BraDown(); break;
        }
    }

    public void SetTarget(GameObject target)
    {
        m_Target = target.transform;
        CameraReset();
    }

    public float GetHorizontalInput()
    {
        return m_Horizontal;
    }
}
