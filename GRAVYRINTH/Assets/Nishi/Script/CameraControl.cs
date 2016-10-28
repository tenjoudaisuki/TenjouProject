using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    [SerializeField, TooltipAttribute("注視点の対象")]
    public Transform Target;
    [SerializeField, TooltipAttribute("注視点との距離")]
    public float Distance;
    [SerializeField, TooltipAttribute("X回転の上限下限")]
    public float XAngleLimit;
    [SerializeField, TooltipAttribute("注視点の調整")]
    public Vector3 TargetOffset;

    private Vector3 offset;
    /// <summary>
    /// カメラの位置の方向
    /// </summary>
    private Vector3 CameraPosDirection;
    private float XAxisTotal = 0;
    private float YAxisTotal = 0;
    private Transform FastTransform;

    // Use this for initialization
    void Start()
    {
        offset = Target.right * TargetOffset.x + Target.up * TargetOffset.y + Target.forward * TargetOffset.z;

        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.LookRotation((Target.position + offset) - transform.position, Target.up), 0.5f);

        CameraPosDirectionRotation();

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
        offset = Target.right * TargetOffset.x + Target.up * TargetOffset.y + Target.forward * TargetOffset.z;

        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.LookRotation((Target.position + offset) - transform.position,Target.up),0.5f);

        CameraPosDirectionRotation();

        Ray ray = new Ray(Target.position + offset, CameraPosDirection.normalized);

        Debug.DrawRay(Target.position + offset, CameraPosDirection,Color.yellow);

        //RaycastHit hit;
        //if (Physics.Raycast(ray,out hit,Distance))
        //{
        //    CameraMove(hit.point);
        //}
        //else
        //{
        //    CameraMove((Target.position + offset) + (CameraPosDirection * Distance));
        //}
        CameraMove((Target.position + offset) + (CameraPosDirection * Distance));

        if (Input.GetKeyDown(KeyCode.T))
        {
            CameraReset();
        }
    }

    /// <summary>
    /// カメラの位置の方向を回転させる
    /// </summary>
    private void CameraPosDirectionRotation()
    {
        float horizontal = -Input.GetAxis("Horizontal2");
        float vertical = -Input.GetAxis("Vertical2");

        YAxisTotal += horizontal;
        XAxisTotal += vertical;
        XAxisTotal = Mathf.Clamp(XAxisTotal, -XAngleLimit, XAngleLimit);

        //CameraPosDirection = Quaternion.AngleAxis(YAxisTotal, Target.up) * Quaternion.AngleAxis(XAxisTotal, Target.right) * -Target.forward;
        CameraPosDirection = Quaternion.AngleAxis(YAxisTotal, transform.up) * Quaternion.AngleAxis(XAxisTotal, transform.right) * new Vector3(0,0,-1); //プレイヤーの後方ベクトルを使わない
    }

    /// <summary>
    /// カメラ位置を移動させる
    /// </summary>
    /// <param name="next">移動先</param>
    private void CameraMove(Vector3 next)
    {
         transform.position = Vector3.Lerp(transform.position,next,0.08f);
    }

    /// <summary>
    /// カメラを最初の状態に
    /// </summary>
    private void CameraReset()
    {
        CameraPosDirection = new Vector3(0, 0, -1);
        transform.position = FastTransform.position;
        transform.localRotation = FastTransform.localRotation;
        XAxisTotal = 0;
    }
}
