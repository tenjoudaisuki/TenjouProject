using UnityEngine;
using System.Collections;

public class DangleMoveTest : MonoBehaviour
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

    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;

    private Vector3 forward;

    void Start()
    {
        tr = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        //ironBarTouchPoint = GameObject.Find("IronBarTouchPoint");
        m_MoveManager = GetComponent<PlayerMoveManager>();
    }

    void Update()
    {
        if (touchIronBar == true)
        {
            rb.velocity = Vector3.zero;

            //プレイヤーの位置から回転軸までの距離　0.009はIronBarの半径
            float distance = hitInto.distance + 0.009f;

            tr.RotateAround(tr.position + tr.up * distance, tr.right, Input.GetAxis("Vertical") * angleSpeed * Time.deltaTime);


            float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
            Vector3 barPos = ironBar.transform.position;

            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetIronBarVector());
            Vector3 movement = barVectorNor * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            tr.localPosition += movement;
            tr.localPosition =
                new Vector3(
                    Mathf.Clamp(tr.localPosition.x, barPos.x - moveArea, barPos.x + moveArea),
                    Mathf.Clamp(tr.localPosition.y, barPos.y - moveArea + 0.2f, barPos.y + moveArea - 0.2f),
                    Mathf.Clamp(tr.localPosition.z, barPos.z - moveArea, barPos.z + moveArea));

            Debug.DrawRay(tr.position, forward * 5.0f);
        }

        if (touchIronBar == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
        {
            //tr.parent = null;
            //tr.parent = GameObject.Find("Pausable").transform;
            //touchIronBar = false;

            m_GravityDir.SetDirection(-tr.up);
            m_MoveManager.SetState(PlayerState.NORMAL);
            //プレイヤーの向きを更新する
            m_MoveManager.SetPlayerUpFront(tr.up, Vector3.Cross(tr.up, Camera.main.transform.right));
            //rb.AddForce(-tr.up * 200.0f);

            //カメラの視点をプレイヤーにする
            GameObject.Find("Camera").GetComponent<CameraControl>().SetTarget(gameObject);

            GetComponent<NormalMove>().SetIronBarHitDelay(1.0f);

            touchIronBar = false;
        }
    }

    public void SetTouchIronBar(bool ishit, RaycastHit hitInto)
    {
        this.hitInto = hitInto;
        touchIronBar = true;

        ironBar = hitInto.collider.gameObject;

        //tr.parent = ironBar.transform;
        StartCoroutine(DelayMethod(1.0f, () =>
        {
            forward = Vector3.Cross(tr.up, ironBar.GetComponent<IronBar>().GetIronBarVector());
            Quaternion rotate = Quaternion.LookRotation(-forward, tr.up);
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

//public class DangleMove : MonoBehaviour
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
//        ironBarTouchPoint = GameObject.Find("IronBarTouchPoint");
//        tr = gameObject.transform;
//        rb = gameObject.GetComponent<Rigidbody>();
//        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
//        m_MoveManager = GetComponent<PlayerMoveManager>();

//    }

//    void Update()
//    {
//        if (touchIronBar == true)
//        {
//            rb.velocity = Vector3.zero;
//            Vector3 point = ironBar.transform.position;
//            Debug.DrawRay(point, Vector3.up);
//            ironBarTouchPoint.transform.RotateAround(point, tr.right, Input.GetAxis("Vertical") * angleSpeed * Time.deltaTime);

//            float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
//            Vector3 barPos = ironBar.transform.position;
//            Vector3 movement = barVectorNor * Input.GetAxis("Horizontal") * 0.1f * moveSpeed;
//            ironBarTouchPoint.transform.position += movement;
//            ironBarTouchPoint.transform.position =
//                new Vector3(
//                    Mathf.Clamp(ironBarTouchPoint.transform.position.x, barPos.x - moveArea, barPos.x + moveArea),
//                    Mathf.Clamp(ironBarTouchPoint.transform.position.y, barPos.y - moveArea, barPos.y + moveArea),
//                    Mathf.Clamp(ironBarTouchPoint.transform.position.z, barPos.z - moveArea, barPos.z + moveArea));
//        }

//        if (touchIronBar == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")))
//        {
//            tr.parent = null;
//            tr.parent = GameObject.Find("Pausable").transform;
//            touchIronBar = false;

//            m_GravityDir.SetDirection(-tr.up);
//            m_MoveManager.SetState(PlayerState.NORMAL);
//            //プレイヤーの向きを更新する
//            m_MoveManager.SetPlayerUpFront(tr.up, Vector3.Cross(tr.up, Camera.main.transform.right));
//            //rb.AddForce(-tr.up * 200.0f);

//            //カメラの視点をプレイヤーにする
//            GameObject.Find("Camera").GetComponent<CameraControl>().SetTarget(gameObject);
//        }
//    }

//    public void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject.tag == "IronBar" && Vector3.Angle(tr.up, Vector3.Normalize(collision.gameObject.GetComponent<IronBar>().GetBarVector())) < 45.0f) return;

//        Transform touchTr = tr;
//        if (touchIronBar == true) return;

//        if (collision.gameObject.tag == "IronBar")
//        {
//            print("Dangle");

//            rb.isKinematic = true;

//            tr = touchTr;
//            touchIronBar = true;
//            ironBarTouchPoint.GetComponent<IronBarTouchPoint>().SetIsHit(touchIronBar);
//            rb.velocity = Vector3.zero;
//            rb.isKinematic = false;
//            barCollision = collision;
//            collisionIronBarPosition = barCollision.contacts[0].point;


//            ironBar = collision.gameObject;
//            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());

//            ironBarTouchPoint.transform.position = collisionIronBarPosition;

//            //とりあえずコメントアウト
//            //ironBarTouchPoint.GetComponent<IronBarTouchPoint>().
//            //    SetPlayerDirection(-tr.up, tr.position - collisionIronBarPosition);

//            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetIronBarVector());
//            tr.right = barVectorNor;

//            ////カメラの視点をアイアンバーにする
//            //GameObject.Find("Camera").GetComponent<CameraControl>().SetTarget(ironBarTouchPoint);
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

