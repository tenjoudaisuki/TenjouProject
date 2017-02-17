using UnityEngine;
using System.Collections;

public class CameraControl : ICamera
{
    private enum State
    {
        None,
        StartMove,
        Normal,
        BraDown,
        Crimb,
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
    /// <summary>
    /// 最初のカメラ回転
    /// </summary>
    private Quaternion mFastRotate;

    private State mCurrentState = State.StartMove;

    private Vector3 mParallel;
    private Vector3 mInitParallel;


    private float mTimer = 0.0f;

    public Vector3 mUp;

    public override void Start()
    {
        mTimer = 0.0f;
        mFastPosition = transform.position;
        mFastRotate = transform.localRotation;
        mCurrentState = State.StartMove;
    }

    public void LateUpdate()
    {
        StateUpdate();

        //if (Input.GetButtonDown("CameraReset"))
        //{
        //    //カメラを元の位置に移動
        //    CameraReset();
        //}
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
        mUp = Vector3.Lerp(mUp, up, 0.05f);
        Vector3 parallel = Vector3.Cross(mUp, right);
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
        //    Quaternion.LookRotation(-CameraPosDirection, m_Target.up) ,0.5f);
        //補間なし版
        transform.localRotation = Quaternion.LookRotation(-CameraPosDirection, mUp);

        Debug.DrawRay(m_Target.position, Quaternion.AngleAxis(XAxisTotal, transform.right) * m_Target.up, Color.green);

        if (m_Target.GetComponent<PlayerMoveManager>().GetState() == PlayerState.IRON_BAR_DANGLE)
        {
            mCurrentState = State.None;
            XAxisTotal = 90;
            CameraPosDirection = m_Target.up;
            offset = m_Target.right * m_TargetOffset.x + m_Target.up * m_TargetOffset.y + m_Target.forward * m_TargetOffset.z;

            var position = (m_Target.position + offset) + (CameraPosDirection.normalized * m_Distance); ;
            var rotate = Quaternion.LookRotation(-CameraPosDirection, m_Target.up);

            LeanTween.move(gameObject, position, 0.6f).setOnComplete(() => { mCurrentState = State.BraDown; });
            LeanTween.rotateLocal(gameObject, rotate.eulerAngles, 0.5f);
        }

        if (m_Target.GetComponent<PlayerMoveManager>().GetState() == PlayerState.IRON_BAR_CLIMB)
        {
            mCurrentState = State.None;
            XAxisTotal = 0;
            offset = m_Target.right * m_TargetOffset.x + m_Target.up * m_TargetOffset.y + m_Target.forward * m_TargetOffset.z;

            CameraPosDirection = m_Target.forward;
            var position = (m_Target.position + offset) + (CameraPosDirection.normalized * m_Distance); ;
            var rotate = Quaternion.LookRotation(-CameraPosDirection, m_Target.up);

            var from = transform.localRotation;
            LeanTween.move(gameObject, position, 0.6f).setOnComplete(() => { mCurrentState = State.Crimb; });

            LeanTween.value(0, 1, 0.5f).setOnUpdate((float vel) => {
                transform.localRotation = Quaternion.Slerp(from, rotate, vel);
            });
        }
    }

    private void StartMove()
    {
        if (mTimer > 1)
        {
            PlayerMoveManager mm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveManager>();
            if(mm.GetState() == PlayerState.NONE)
                mm.SetState(PlayerState.NORMAL);
            mm.SetEventInputDisable(false);

            mCurrentState = State.Normal;
            GameManager.Instance.SetPausePossible(true);
            return;
        }

        CameraPosDirection = -m_Target.forward;
        offset = m_Target.right * m_TargetOffset.x + m_Target.up * m_TargetOffset.y + m_Target.forward * m_TargetOffset.z;
        Vector3 next = (m_Target.position + offset) + (CameraPosDirection * m_Distance);

        transform.position = Vector3.Lerp(mFastPosition, next, mTimer);

        transform.localRotation = Quaternion.Slerp(
            mFastRotate,
            Quaternion.LookRotation(-CameraPosDirection, m_Target.up),
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

        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            XAxisTotal = 90;
            YAxisTotal = 0;
            CameraPosDirection = m_Target.up;
            var position = (m_Target.position) + (CameraPosDirection * m_Distance);
            var rotate = Quaternion.LookRotation(-CameraPosDirection, m_Target.forward);

            transform.position = Vector3.Lerp(transform.position, position, 0.3f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 0.3f);
        }
        else
        {
            //if(XAxisTotal == 90)
            //{

            //}
            //ターゲットの上ベクトルと自身の横ベクトルの外積で地面と平行なベクトルを作る
            Vector3 parallel = Vector3.Cross(m_Target.transform.up, transform.right);
            //Vector3 parallel = -transform.forward;
            mParallel = parallel;

            Vector3 temp = Quaternion.AngleAxis(horizontal, m_Target.transform.up) * Quaternion.AngleAxis(XAxisTotal, transform.right) * mParallel;
            CameraPosDirection = temp;

            //カメラを移動させる
            Vector3 next = (m_Target.position) + (CameraPosDirection * m_Distance);
            //デバック表示
            Debug.DrawRay(m_Target.position, CameraPosDirection, Color.yellow);

            //補間あり移動
            //transform.position = Vector3.Lerp(transform.position, next, 0.3f);
            //補間なし移動
            transform.position = next;

            //transform.localRotation = Quaternion.Slerp(transform.localRotation,
            //        Quaternion.LookRotation((player.transform.position) - transform.position,up), 0.5f);
            Vector3 up = Quaternion.AngleAxis(XAxisTotal, m_Target.right) * m_Target.up;
            up = Quaternion.AngleAxis(YAxisTotal, m_Target.up) * up;
            transform.localRotation = Quaternion.LookRotation(-CameraPosDirection, up);
            Debug.DrawRay(m_Target.position, up, Color.green);
        }

        if (m_Target.GetComponent<PlayerMoveManager>().GetState() != PlayerState.IRON_BAR_DANGLE)
        {
            mUp = m_Target.up;
            XAxisTotal = m_XAngleLimit;
            YAxisTotal = 0;
            mCurrentState = State.Normal;
            
        }

    }

    private void Crimb()
    {

        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            XAxisTotal = 0;
            offset = m_Target.right * m_TargetOffset.x + m_Target.up * m_TargetOffset.y + m_Target.forward * m_TargetOffset.z;

            CameraPosDirection = m_Target.forward;
            var position = (m_Target.position + offset) + (CameraPosDirection.normalized * m_Distance); ;
            var rotate = Quaternion.LookRotation(-CameraPosDirection,m_Target.up);

            transform.position = Vector3.Lerp(transform.position, position, 0.3f);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, rotate, 0.3f);
        }
        else
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
            //    Quaternion.LookRotation(-CameraPosDirection, m_Target.up) ,0.5f);
            //補間なし版
            transform.localRotation = Quaternion.LookRotation(-CameraPosDirection, m_Target.up);

            Debug.DrawRay(m_Target.position, Quaternion.AngleAxis(XAxisTotal, transform.right) * m_Target.up, Color.green);
        }

        if (m_Target.GetComponent<PlayerMoveManager>().GetState() != PlayerState.IRON_BAR_CLIMB)
        {
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
        transform.localRotation = Quaternion.LookRotation(-CameraPosDirection, m_Target.up);
        XAxisTotal = 0;
    }

    private void StateUpdate()
    {
        switch (mCurrentState)
        {
            case State.StartMove: StartMove(); break;
            case State.Normal: Normal(); break;
            case State.BraDown: BraDown(); break;
            case State.Crimb: Crimb(); break;
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

    public override void Warp()
    {
        CameraReset();
        mCurrentState = State.Normal;
    }
}
