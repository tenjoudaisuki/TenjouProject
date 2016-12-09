using UnityEngine;
using System.Collections;

public class CrimbMove : MonoBehaviour
{
    public bool touchIronBar = false;
    public GameObject ironBar;
    public Vector3 collisionIronBarPosition;

    public GameObject ironBarTouchPoint;

    private Transform tr;
    private Rigidbody rb;
    private Collision barCollision;
    private Vector3 barVectorNor;
    private GravityDirection m_GravityDir;
    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;


    void Start()
    {
        tr = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        m_MoveManager = GetComponent<PlayerMoveManager>();

    }

    void Update()
    {
        Debug.DrawRay(tr.position, tr.up, Color.red, 1.0f, false);
        Debug.DrawRay(tr.position, tr.right);
        //Debug.DrawRay(ironBarTouchPoint.transform.position, ironBarTouchPoint.transform.right);

        if (touchIronBar == true)
        {
            rb.velocity = Vector3.zero;

            Vector3 point = ironBar.transform.position;
            Debug.DrawRay(point, Vector3.up);
            ironBarTouchPoint.transform.RotateAround(point, tr.up, -Input.GetAxis("Horizontal") * 90.0f * Time.deltaTime);


            float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
            Vector3 barPos = ironBar.transform.position;
            Vector3 movement = barVectorNor * -Input.GetAxis("Vertical") * -0.1f;

            float distant = Mathf.Abs(tr.position.y - (ironBarTouchPoint.transform.position.y + movement.y));
            if (distant > 0.35f)
            {
                tr.parent = null;
                ironBarTouchPoint.transform.position = 
                    new Vector3(ironBarTouchPoint.transform.position.x, tr.position.y, ironBarTouchPoint.transform.position.z);
                tr.parent = ironBarTouchPoint.transform;
                
            }
            else
            {
                ironBarTouchPoint.transform.position += movement;
                ironBarTouchPoint.transform.position =
                    new Vector3(
                        Mathf.Clamp(ironBarTouchPoint.transform.position.x, barPos.x - moveArea, barPos.x + moveArea),
                        Mathf.Clamp(ironBarTouchPoint.transform.position.y, barPos.y - moveArea, barPos.y + moveArea),
                        Mathf.Clamp(ironBarTouchPoint.transform.position.z, barPos.z - moveArea, barPos.z + moveArea));

            }
        }

        if (touchIronBar == true && Input.GetKeyDown(KeyCode.Space))
        {
            tr.parent = null;
            touchIronBar = false;
            m_MoveManager.SetState(PlayerState.NORMAL);
            //背面斜め上方向に方向にジャンプする
            m_MoveManager.PlayerPoleKick(Vector3.Normalize(-tr.forward + tr.up));
        }
    }


    public void OnCollisionEnter(Collision collision)
    {
        Transform touchTr = tr;
        if (touchIronBar == true) return;

        if (collision.gameObject.tag == "IronBar")
        {
            rb.isKinematic = true;

            tr = touchTr;
            touchIronBar = true;
            ironBarTouchPoint.GetComponent<IronBarTouchPoint>().SetIsHit(touchIronBar);
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
            barCollision = collision;
            collisionIronBarPosition = barCollision.contacts[0].point;

            ironBar = collision.gameObject;
            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());

            ironBarTouchPoint.GetComponent<IronBarTouchPoint>().
                SetPlayerDirection(-tr.forward, tr.position - collisionIronBarPosition);

            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
            tr.up = barVectorNor;

            ironBarTouchPoint.transform.position = collisionIronBarPosition;

            //ironBarTouchPoint.GetComponent<IronBarTouchPoint>().
            //       SetPlayerDirection(-tr.forward, tr.position - collisionIronBarPosition);

            //barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
            //tr.up = barVectorNor;
        }
    }

    public void OnDrawGizmos()
    {
        if (barCollision == null) return;
        collisionIronBarPosition = barCollision.contacts[0].point;

        Gizmos.DrawWireSphere(collisionIronBarPosition, 0.05f);
        Gizmos.DrawRay(collisionIronBarPosition, tr.position - collisionIronBarPosition);
    }

    private RayHitInfo CheckBarHit(Vector3 reyPos)
    {
        RayHitInfo result;
        Ray ray = new Ray(reyPos, tr.position - collisionIronBarPosition);
        RaycastHit hit;
        result.isHit = Physics.Raycast(ray, out hit, 0.5f);
        result.hit = hit;

        //レイをデバッグ表示
        Debug.DrawRay(reyPos, tr.up * 0.5f, Color.green);

        return result;
    }

    private Vector3 Vector3Abs(Vector3 origin)
    {
        return new Vector3(Mathf.Abs(origin.x), Mathf.Abs(origin.y), Mathf.Abs(origin.z));
    }
}