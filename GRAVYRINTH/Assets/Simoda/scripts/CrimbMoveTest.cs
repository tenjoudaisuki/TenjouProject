using UnityEngine;
using System.Collections;

public class CrimbMoveTest : MonoBehaviour
{
    public bool touchIronBar = false;

    private Transform tr;
    private Rigidbody rb;
    private GravityDirection m_GravityDir;
    private GameObject ironBarTouchPoint;
    private RaycastHit hitInto;
    private Vector3 barVectorNor;
    private GameObject ironBar;

    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;

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
            print("aaaaaaaaa");

            rb.velocity = Vector3.zero;

            Debug.DrawRay(tr.position, barVectorNor * 5.0f, Color.yellow);

            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetPoleVector());
            Vector3 movement = barVectorNor * -Input.GetAxis("Vertical") * -0.1f;
            tr.localPosition += movement;
        }
    }

    public void SetTouchIronBar(bool ishit, RaycastHit hitInto)
    {
        this.hitInto = hitInto;
        touchIronBar = true;

        
        ironBar = hitInto.collider.gameObject;

        tr.parent = ironBar.transform;

        //Quaternion rotate = Quaternion.LookRotation(tr.forward, ironBar.GetComponent<IronBar>().GetBarVector());
        //tr.localRotation = rotate;
    }
}
