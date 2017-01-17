using UnityEngine;
using System.Collections;

public class DangleMove : MonoBehaviour
{
    public bool touchIronBar = false;
    public GameObject ironBar;
    public Vector3 collisionIronBarPosition;

    public float moveSpeed = 1.0f;
    public float angleSpeed = 90.0f;

    private Transform tr;
    private Rigidbody rb;
    private Collision barCollision;
    private Vector3 barVectorNor;
    private GravityDirection m_GravityDir;
    private GameObject ironBarTouchPoint;
    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;


    void Start()
    {
        ironBarTouchPoint = GameObject.Find("IronBarTouchPoint");
        tr = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        m_MoveManager = GetComponent<PlayerMoveManager>();

    }

    void Update()
    {
        if (touchIronBar == true)
        {
            rb.velocity = Vector3.zero;
            Vector3 point = ironBar.transform.position;
            Debug.DrawRay(point, Vector3.up);
            ironBarTouchPoint.transform.RotateAround(point, tr.right, Input.GetAxis("Vertical") * angleSpeed * Time.deltaTime);

            float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
            Vector3 barPos = ironBar.transform.position;
            Vector3 movement = barVectorNor * Input.GetAxis("Horizontal") * 0.1f * moveSpeed;
            ironBarTouchPoint.transform.position += movement;
            ironBarTouchPoint.transform.position =
                new Vector3(
                    Mathf.Clamp(ironBarTouchPoint.transform.position.x, barPos.x - moveArea, barPos.x + moveArea),
                    Mathf.Clamp(ironBarTouchPoint.transform.position.y, barPos.y - moveArea, barPos.y + moveArea),
                    Mathf.Clamp(ironBarTouchPoint.transform.position.z, barPos.z - moveArea, barPos.z + moveArea));
        }

        if (touchIronBar == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            tr.parent = null;
            tr.parent = GameObject.Find("Pausable").transform;
            touchIronBar = false;

            m_GravityDir.SetDirection(-tr.up);
            m_MoveManager.SetState(PlayerState.NORMAL);
            //プレイヤーの向きを更新する
            m_MoveManager.SetPlayerUpFront(tr.up, Vector3.Cross(tr.up, Camera.main.transform.right));
            //rb.AddForce(-tr.up * 200.0f);

            //カメラの視点をプレイヤーにする
            GameObject.Find("Camera").GetComponent<CameraControl>().SetTarget(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "IronBar" && Vector3.Angle(tr.up, Vector3.Normalize(collision.gameObject.GetComponent<IronBar>().GetBarVector())) < 45.0f) return;

        Transform touchTr = tr;
        if (touchIronBar == true) return;

        if (collision.gameObject.tag == "IronBar")
        {
            print("Dangle");

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

            ironBarTouchPoint.transform.position = collisionIronBarPosition;

            //とりあえずコメントアウト
            //ironBarTouchPoint.GetComponent<IronBarTouchPoint>().
            //    SetPlayerDirection(-tr.up, tr.position - collisionIronBarPosition);

            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetIronBarVector());
            tr.right = barVectorNor;

            ////カメラの視点をアイアンバーにする
            //GameObject.Find("Camera").GetComponent<CameraControl>().SetTarget(ironBarTouchPoint);
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
