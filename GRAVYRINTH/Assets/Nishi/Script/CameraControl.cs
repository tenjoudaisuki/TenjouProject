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
    private Vector3 nextPoint;
    private float XAxisTotal = 0;
    private float YAxisTotal = 0;
    private Transform FastTransform;

    // Use this for initialization
    void Start()
    {
        nextPoint = -Target.forward;

        transform.LookAt(Target.position + TargetOffset);
        NextPointMove();

        Ray ray = new Ray(Target.position + TargetOffset, nextPoint.normalized);

        Debug.DrawRay(Target.position + TargetOffset, nextPoint);

        CameraMove((Target.position + TargetOffset) + nextPoint * Distance);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Distance))
        {
            CameraMove(hit.point);
        }

        FastTransform = transform;
    }

    public void LateUpdate()
    {
        offset = Target.right * TargetOffset.x + Target.up * TargetOffset.y + Target.forward * TargetOffset.z;

        transform.LookAt(Target.position + offset,Target.up);
        NextPointMove();

        Ray ray = new Ray(Target.position + offset, nextPoint.normalized);

        Debug.DrawRay(Target.position + offset, nextPoint);

        CameraMove((Target.position + offset) + nextPoint * Distance);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,Distance))
        {
            CameraMove(hit.point);
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            CameraReset();
        }
    }

    //カメラが次に行きたい位置を変更
    private void NextPointMove()
    {
        float X = -Input.GetAxis("Horizontal2");
        float Y = -Input.GetAxis("Vertical2");

        YAxisTotal += X;
        XAxisTotal += Y;
        XAxisTotal = Mathf.Clamp(XAxisTotal, -XAngleLimit, XAngleLimit);

        //nextPoint = Target.localRotation * nextPoint; * Target.localRotation * nextPoint;
        nextPoint = Quaternion.AngleAxis(YAxisTotal, Target.up) * Quaternion.AngleAxis(XAxisTotal, Target.right) * -Target.forward;

        //XAxisTotal += Y;
        //XAxisTotal = Mathf.Clamp(XAxisTotal, -XAngleLimit, XAngleLimit);
        //if (XAxisTotal < XAngleLimit && -XAngleLimit < XAxisTotal){
        //   nextPoint = Quaternion.AngleAxis(Y, Target.right) * -Target.forward;
        //}
    }

    private void CameraMove(Vector3 next)
    {
         transform.position = Vector3.Lerp(transform.position,next,0.8f);
    }

    private void CameraReset()
    {
        nextPoint = new Vector3(0, 0, -1);
        transform.position = FastTransform.position;
        transform.localRotation = FastTransform.localRotation;
        XAxisTotal = 0;
    }
}
