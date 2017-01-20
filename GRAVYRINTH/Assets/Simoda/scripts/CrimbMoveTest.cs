using UnityEngine;
using System.Collections;

public class CrimbMoveTest : MonoBehaviour
{
    public bool touchIronBar = false;
    public float moveSpeed = 1.0f;
    public float angleSpeed = 90.0f;

    private Transform tr;
    private Rigidbody rb;
    private GravityDirection m_GravityDir;
    //private GameObject ironBarTouchPoint;
    private RaycastHit hitInto;
    private Vector3 barVectorNor;
    private GameObject ironBar;
    private JumpCursorDraw jumpCursor;

    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;

    void Start()
    {
        tr = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        //ironBarTouchPoint = GameObject.Find("IronBarTouchPoint");
        jumpCursor = GameObject.Find("JumpCursor").GetComponent<JumpCursorDraw>();
        m_MoveManager = GetComponent<PlayerMoveManager>();
    }

    void Update()
    {
        //Ray forward = new Ray(tr.position, tr.forward);
        //RaycastHit hitInto;

        //int layerMask = 1 << 8;

        //if (Physics.Raycast(forward.origin, forward.direction, out hitInto, 1.0f, layerMask, QueryTriggerInteraction.Ignore))
        //{
        //    if (hitInto.collider.tag == ("IronBar"))
        //    {
        //        touchIronBar = true;
        //    }
        //}

        if (touchIronBar == true)
        {
            rb.velocity = Vector3.zero;

            //プレイヤーの位置から回転軸までの距離　0.009はIronBarの半径
            float distance = hitInto.distance + 0.009f;

            tr.RotateAround(tr.position + tr.forward * distance, tr.up, -Input.GetAxis("Horizontal") * angleSpeed * Time.deltaTime);


            float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
            Vector3 barPos = ironBar.transform.position;

            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
            Vector3 movement = barVectorNor * -Input.GetAxis("Vertical") * -moveSpeed * Time.deltaTime;
            tr.localPosition += movement;
            tr.localPosition =
                new Vector3(
                    Mathf.Clamp(tr.localPosition.x, barPos.x - moveArea, barPos.x + moveArea),
                    Mathf.Clamp(tr.localPosition.y, barPos.y - moveArea + 0.62f, barPos.y + moveArea),
                    Mathf.Clamp(tr.localPosition.z, barPos.z - moveArea, barPos.z + moveArea));
        }

        if (touchIronBar == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            //tr.parent = null;
            //tr.parent = GameObject.Find("Pausable").transform;
            m_MoveManager.SetState(PlayerState.NORMAL);
            //背面斜め上方向に方向にジャンプする
            m_MoveManager.PlayerPoleKick(Vector3.Normalize(-tr.forward + tr.up));

            GetComponent<NormalMove>().SetIronBarHitDelay(1.0f);

            touchIronBar = false;

            jumpCursor.IsHit(false);
        }
    }

    public void SetTouchIronBar(bool ishit, RaycastHit hitInto)
    {
        this.hitInto = hitInto;
        touchIronBar = true;

        ironBar = hitInto.collider.gameObject;

        //tr.parent = ironBar.transform;

        jumpCursor.IsHit(true);

        StartCoroutine(DelayMethod(1.0f, () =>
        {
            Quaternion rotate = Quaternion.LookRotation(tr.forward, ironBar.GetComponent<IronBar>().GetBarVector());
            tr.localRotation = rotate;
        }));
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="waitTime">遅延時間[ミリ秒]</param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float waitTime, System.Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
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
}

//using UnityEngine;
//using System.Collections;

//public class CrimbMove : MonoBehaviour
//{
//    public bool touchIronBar = false;
//    public GameObject ironBar;
//    public Vector3 collisionIronBarPosition;

//    public float moveSpeed = 1.0f;
//    public float angleSpeed = 90.0f;

//    private Transform tr;
//    private Rigidbody rb;
//    private Collision barCollision;
//    private Vector3 barVectorNor;
//    private GravityDirection m_GravityDir;
//    private GameObject ironBarTouchPoint;
//    //プレイヤーの状態管理クラス
//    private PlayerMoveManager m_MoveManager;


//    void Start()
//    {
//        tr = gameObject.transform;
//        rb = gameObject.GetComponent<Rigidbody>();
//        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
//        ironBarTouchPoint = GameObject.Find("IronBarTouchPoint");
//        m_MoveManager = GetComponent<PlayerMoveManager>();

//    }

//    void Update()
//    {
//        Debug.DrawRay(tr.position, tr.up, Color.red, 1.0f, false);
//        Debug.DrawRay(tr.position, tr.right);
//        //Debug.DrawRay(ironBarTouchPoint.transform.position, ironBarTouchPoint.transform.right);

//        if (touchIronBar == true)
//        {
//            rb.velocity = Vector3.zero;

//            Vector3 point = ironBar.transform.position;
//            Debug.DrawRay(point, Vector3.up);
//            ironBarTouchPoint.transform.RotateAround(point, tr.up, -Input.GetAxis("Horizontal") * angleSpeed * Time.deltaTime);


//            float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
//            Vector3 barPos = ironBar.transform.position;
//            Vector3 movement = barVectorNor * -Input.GetAxis("Vertical") * -0.1f * moveSpeed;

//            float distant = Mathf.Abs(tr.position.y - (ironBarTouchPoint.transform.position.y + movement.y));
//            if (distant > 0.35f)
//            {
//                tr.parent = null;
//                ironBarTouchPoint.transform.position =
//                    new Vector3(ironBarTouchPoint.transform.position.x, tr.position.y, ironBarTouchPoint.transform.position.z);
//                tr.parent = ironBarTouchPoint.transform;

//            }
//            else
//            {
//                ironBarTouchPoint.transform.position += movement;
//                ironBarTouchPoint.transform.position =
//                    new Vector3(
//                        Mathf.Clamp(ironBarTouchPoint.transform.position.x, barPos.x - moveArea, barPos.x + moveArea),
//                        Mathf.Clamp(ironBarTouchPoint.transform.position.y, barPos.y - moveArea, barPos.y + moveArea),
//                        Mathf.Clamp(ironBarTouchPoint.transform.position.z, barPos.z - moveArea, barPos.z + moveArea));

//            }
//        }

//        if (touchIronBar == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
//        {
//            tr.parent = null;
//            tr.parent = GameObject.Find("Pausable").transform;
//            touchIronBar = false;
//            m_MoveManager.SetState(PlayerState.NORMAL);
//            //背面斜め上方向に方向にジャンプする
//            m_MoveManager.PlayerPoleKick(Vector3.Normalize(-tr.forward + tr.up));
//        }
//    }


//    public void OnCollisionEnter(Collision collision)
//    {
//        //if (collision.gameObject.tag == "IronBar" && Vector3.Angle(tr.up, Vector3.Normalize(collision.gameObject.GetComponent<IronBar>().GetBarVector())) > 45.0f) return;

//        Transform touchTr = tr;
//        if (touchIronBar == true) return;

//        if (collision.gameObject.tag == "IronBar")
//        {
//            print("Crimb");

//            rb.isKinematic = true;

//            tr = touchTr;
//            touchIronBar = true;
//            ironBarTouchPoint.GetComponent<IronBarTouchPoint>().SetIsHit(touchIronBar);
//            rb.velocity = Vector3.zero;
//            rb.isKinematic = false;
//            barCollision = collision;
//            collisionIronBarPosition = barCollision.contacts[0].point;
//            ironBarTouchPoint.transform.position = collisionIronBarPosition;

//            ironBar = collision.gameObject;
//            //barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());

//            ironBarTouchPoint.GetComponent<IronBarTouchPoint>().
//                SetPlayerDirection(-tr.forward, tr.position - collisionIronBarPosition);

//            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
//            //tr.up = barVectorNor;

//            //コメントアウト
//            ironBarTouchPoint.transform.position = collisionIronBarPosition;

//            //ironBarTouchPoint.GetComponent<IronBarTouchPoint>().
//            //       SetPlayerDirection(-tr.forward, tr.position - collisionIronBarPosition);

//            //barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
//            //tr.up = barVectorNor;
//        }
//    }

//    public void OnDrawGizmos()
//    {
//        if (barCollision == null) return;
//        collisionIronBarPosition = barCollision.contacts[0].point;

//        Gizmos.DrawWireSphere(collisionIronBarPosition, 0.05f);
//        Gizmos.DrawRay(collisionIronBarPosition, tr.position - collisionIronBarPosition);
//    }

//    private RayHitInfo CheckBarHit(Vector3 reyPos)
//    {
//        RayHitInfo result;
//        Ray ray = new Ray(reyPos, tr.position - collisionIronBarPosition);
//        RaycastHit hit;
//        result.isHit = Physics.Raycast(ray, out hit, 0.5f);
//        result.hit = hit;

//        //レイをデバッグ表示
//        Debug.DrawRay(reyPos, tr.up * 0.5f, Color.green);

//        return result;
//    }

//    private Vector3 Vector3Abs(Vector3 origin)
//    {
//        return new Vector3(Mathf.Abs(origin.x), Mathf.Abs(origin.y), Mathf.Abs(origin.z));
//    }
//}