using UnityEngine;
using System.Collections;

public class PlayerIronBar : MonoBehaviour
{
    public enum BarType
    {
        IRON_BAR,
        POLE,
    }

    //当たったかどうか付のRaycastHit
    struct RayHitInfo
    {
        public RaycastHit hit;
        //当たったか？
        public bool isHit;
    };

    public bool touchIronBar = false;
    public GameObject ironBar;
    public Vector3 touchIronBarPosition;
    public Vector3 touchIronBarPlayerPosition;

    private BarType barType;
    private Transform tr;


    void Start()
    {
        tr = gameObject.transform;
    }

    void Update()
    {
        if (touchIronBar == true)
        {
            //transform.position = touchIronBarPlayerPosition;
            switch (barType)
            {
                case BarType.IRON_BAR:
                    //Vector3 barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());
                    //Vector3 axis = barVectorNor;
                    //transform.RotateAround(touchIronBarPosition, Vector3.Normalize(axis), Input.GetAxis("Vertical") * 50.0f * Time.deltaTime);
                    RayHitInfo hitInfo = CheckBarHit(tr.position);

                    if (hitInfo.isHit)
                    {
                        ////上方向と平面の法線方向のなす角
                        //float angle = Vector3.Angle(tr.up, hitInfo.hit.normal);
                        ////斜面として認識する角度以上なら何もしない
                        //if (angle > m_SlopeDeg) return;

                        //当たった地点に移動
                        tr.position = hitInfo.hit.point;

                        //下方向を当たった平面の法線方向に変更
                        //down = hitInfo.hit.normal;
                    }

                    break;
                case BarType.POLE:
                    break;
            }

        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "IronBar")
        {
            print("鉄棒と接触");
            touchIronBar = true;
            ironBar = collision.gameObject;
            touchIronBarPosition = collision.contacts[0].point;
            touchIronBarPlayerPosition = transform.position;

            Vector3 barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());
            print(Vector3.Dot(transform.up, barVectorNor));

            if (Vector3.Dot(transform.up, barVectorNor) > 0.7071068)
            {
                barType = BarType.IRON_BAR;
            }
            else
            {
                barType = BarType.POLE;
            }

            print(barType);

            GetComponent<PlayerMove>().enabled = false;
            GetComponent<PlayerBlockPush>().enabled = false;

            //print(touchIronBarPosition);
        }
    }

    private RayHitInfo CheckBarHit(Vector3 reyPos)
    {
        RayHitInfo result;
        Ray ray = new Ray(reyPos, tr.up);
        RaycastHit hit;
        result.isHit = Physics.Raycast(ray, out hit, 0.5f);
        result.hit = hit;

        //レイをデバッグ表示
        //Debug.DrawRay(reyPos, GetDown() * m_Height, Color.grey, 1.0f, false);       

        return result;
    }
}
