using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    private enum State
    {
        Normal,
        BraDown
    }

    [SerializeField, TooltipAttribute("注視点の対象")]
    public Transform Target;
    [SerializeField, TooltipAttribute("注視点との距離")]
    public float Distance;
    [SerializeField, TooltipAttribute("X回転の上限下限")]
    public float XAngleLimit;
    [SerializeField, TooltipAttribute("注視点の調整")]
    public Vector3 TargetOffset;
    [SerializeField, TooltipAttribute("回転のスピード")]
    public Vector2 m_RotateSpeed = new Vector2(2.0f,2.0f);

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
    private Transform FastTransform;

    private State mCurrentState = State.Normal;

    //重力の方向
    private GravityDirection m_GravityDir;

    void Start()
    {
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();

        offset = Target.right * TargetOffset.x + Target.up * TargetOffset.y + Target.forward * TargetOffset.z;

        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.LookRotation((Target.position + offset) - transform.position, Target.up), 0.5f);

        TargetAroundMove(Target.up, transform.right);

        Ray ray = new Ray(Target.position + offset, CameraPosDirection.normalized);

        Debug.DrawRay(Target.position + offset, CameraPosDirection, Color.yellow);

        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit, Distance))
        //{
        //    CameraMove(hit.point);
        //}
        //else
        //{
        //    CameraMove((Target.position + offset) + (CameraPosDirection * Distance));
        //}

        FastTransform = transform;
    }

    public void LateUpdate()
    {

        StateUpdate();

        //Tキーが押されたら
        if (Input.GetKeyDown(KeyCode.T))
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
        float horizontal = -Input.GetAxisRaw("Horizontal2") * m_RotateSpeed.x;
        float vertical = -Input.GetAxisRaw("Vertical2") * m_RotateSpeed.y;

        XAxisTotal += vertical;
        //X軸の回転の限界を設定
        XAxisTotal = Mathf.Clamp(XAxisTotal, -XAngleLimit, XAngleLimit);

        ////ターゲットの上ベクトルと自身の横ベクトルの外積で地面と平行なベクトルを作る
        //Vector3 parallel = Vector3.Cross(Target.up, transform.right);
        ////平行ベクトルをターゲットの上ベクトルを軸に回転さらに自身の横ベクトルを軸に回転しカメラの位置を計算
        //CameraPosDirection = Quaternion.AngleAxis(XAxisTotal, transform.right) * Quaternion.AngleAxis(horizontal, Target.up) * parallel;

        //ターゲットの上ベクトルと自身の横ベクトルの外積で地面と平行なベクトルを作る
        Vector3 parallel = Vector3.Cross(up, right);
        //平行ベクトルをターゲットの上ベクトルを軸に回転さらに自身の横ベクトルを軸に回転しカメラの位置を計算
        CameraPosDirection = Quaternion.AngleAxis(XAxisTotal,right) * Quaternion.AngleAxis(horizontal,up) * parallel;

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
        Ray ray = new Ray(Target.position + offset, CameraPosDirection.normalized);

        //RaycastHit hit;
        //rayの方向の指定距離以内に障害物が無いか？
        //if (Physics.Raycast(ray, out hit, Distance))
        //{
        //    //壁に当たった位置をカメラ位置に
        //    next = hit.point;
        //}
        //else
        //{
        //    //当たらなかったらray* Disをカメラ位置に
        //    next = Target.position + offset) + (CameraPosDirection * Distance;
        //｝
        next = (Target.position + offset) + (CameraPosDirection * Distance);
        //デバック表示
        Debug.DrawRay(Target.position + offset, CameraPosDirection, Color.yellow);

        //補間あり移動
        //transform.position = Vector3.Lerp(transform.position, next, 0.1f);
        //補間なし移動
        transform.position = next;

    }

    /// <summary>
    /// 通常時カメラ
    /// </summary>
    private void Normal()
    {
        //モデル座標でのオフセット座標を求める
        offset = Target.right * TargetOffset.x + Target.up * TargetOffset.y + Target.forward * TargetOffset.z;

        //ターゲットから見てカメラの方向でカメラの前ベクトルを求める
        Vector3 f = CameraPosDirection;
        //ターゲットの上ベクトルを　右ベクトルを軸にXAxisTotal度回転して　カメラの上ベクトルを求める
        Vector3 up = Quaternion.AngleAxis(XAxisTotal, transform.right) * Target.up;

        //ターゲットの周りをステイックによって回転移動
        TargetAroundMove(Target.up, transform.right);

        //カメラを回転させる
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
          Quaternion.LookRotation((Target.position + offset) - transform.position,Target.up), 0.8f);
        //補間なし版
        //transform.localRotation = Quaternion.LookRotation((Target.position + offset) - transform.position, Target.up);
    }

    private void BraDown()
    {

    }

    /// <summary>
    /// カメラを最初の状態に
    /// </summary>
    private void CameraReset()
    {
        CameraPosDirection = -Target.forward;
        CameraMove();
        //カメラを回転させる
        transform.localRotation = Quaternion.LookRotation((Target.position + offset) - transform.position, Target.up);
        XAxisTotal = 0;
    }

    private void StateUpdate()
    {
        mCurrentState = State.Normal;
        switch (mCurrentState)
        {
            case State.Normal: Normal(); break;
            case State.BraDown: BraDown(); break;
        }
    }
}
