using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    [SerializeField, TooltipAttribute("注視点の対象")]
    public Transform Target;
    [SerializeField, TooltipAttribute("注視点との距離")]
    public float Offset;
    [SerializeField, TooltipAttribute("X回転の上限下限")]
    public float XAngleLimit;

    private Vector3 nextPoint;
    private float XRotationTotal = 0;

    // Use this for initialization
    void Start()
    {
        nextPoint = new Vector3(0, 0, -1);
        //transform.position = Target.position * Offset;
    }

    public void LateUpdate()
    {
        transform.LookAt(Target);
        NextPointMove();

        Ray ray = new Ray(Target.position, nextPoint.normalized);

        //Debug.DrawRay(Target.position,nextPoint);

        CameraMove(Target.position + nextPoint * Offset);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,10))
        {
            CameraMove(hit.point);
        }
    }

    //カメラが次に行きたい位置を変更
    private void NextPointMove()
    {
        float X = -Input.GetAxis("Horizontal2");
        float Y = -Input.GetAxis("Vertical2");

        nextPoint = Quaternion.Euler(0,X,0) * nextPoint;

        XRotationTotal += Y;
        XRotationTotal = Mathf.Clamp(XRotationTotal, -XAngleLimit, XAngleLimit);
        if (XRotationTotal < XAngleLimit && -XAngleLimit < XRotationTotal){
            nextPoint = Quaternion.AngleAxis(Y, transform.right) * nextPoint;
        }
    }

    private void CameraMove(Vector3 next)
    {
         transform.position = Vector3.Lerp(transform.position,next,0.8f);
    }
}
