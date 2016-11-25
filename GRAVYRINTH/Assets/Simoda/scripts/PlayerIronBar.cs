﻿using UnityEngine;
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
    public Vector3 collisionIronBarPosition;
    //public Vector3 touchIronBarPlayerPosition;

    //public Transform headPoint;
    public GameObject ironBarTouchPoint;

    public BarType barType;
    private Transform tr;
    private Rigidbody rb;
    private Collision barCollision;
    private Vector3 barVectorNor;
    private GravityDirection m_GravityDir;
    //プレイヤーの状態管理クラス
    private PlayerMoveManager m_MoveManager;

    // アニメーション
    private Animator anm;

    void Start()
    {
        tr = gameObject.transform;
        rb = gameObject.GetComponent<Rigidbody>();
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
        m_MoveManager = GetComponent<PlayerMoveManager>();

        anm = GetComponent<Animator>();
    }

    void Update()
    {
        Debug.DrawRay(tr.position, tr.up, Color.red, 1.0f, false);
        Debug.DrawRay(tr.position, tr.right);
        //Debug.DrawRay(ironBarTouchPoint.transform.position, ironBarTouchPoint.transform.right);

        if (touchIronBar == true)
        {
            rb.velocity = Vector3.zero;
            //CheckBarHit(tr.position);
            switch (barType)
            {
                case BarType.IRON_BAR:

                    Vector3 point = ironBar.transform.position;
                    Debug.DrawRay(point, Vector3.up);
                    ironBarTouchPoint.transform.RotateAround(point, tr.right, Input.GetAxis("Vertical") * 60.0f * Time.deltaTime);

                    float moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
                    Vector3 barPos = ironBar.transform.position;
                    Vector3 movement = barVectorNor * Input.GetAxis("Horizontal") * 0.1f;
                    ironBarTouchPoint.transform.position += movement;
                    ironBarTouchPoint.transform.position =
                        new Vector3(
                            Mathf.Clamp(ironBarTouchPoint.transform.position.x, barPos.x - moveArea, barPos.x + moveArea),
                            Mathf.Clamp(ironBarTouchPoint.transform.position.y, barPos.y - moveArea, barPos.y + moveArea),
                            Mathf.Clamp(ironBarTouchPoint.transform.position.z, barPos.z - moveArea, barPos.z + moveArea));

                    //アニメーション変更
                    anm.SetBool("PoleH", true);
                    if (movement != Vector3.zero)
                        anm.SetBool("PoleHMove", true);
                    else
                        anm.SetBool("PoleHMove", false);

                    //Vector3 newPos = ironBarTouchPoint.transform.position + movement;
                    //Vector3 offset = newPos - ironBar.transform.position;
                    //ironBarTouchPoint.transform.position = ironBar.transform.position + Vector3.ClampMagnitude(offset, moveArea);
                    //ironBarTouchPoint.transform.position += barVectorNor * Input.GetAxis("Horizontal") * 0.1f;

                    break;
                case BarType.POLE:

                    point = ironBar.transform.position;
                    Debug.DrawRay(point, Vector3.up);
                    ironBarTouchPoint.transform.RotateAround(point, tr.up, -Input.GetAxis("Horizontal") * 90.0f * Time.deltaTime);

                    moveArea = ironBar.GetComponent<IronBar>().GetMoveArea();
                    barPos = ironBar.transform.position;
                    movement = barVectorNor * -Input.GetAxis("Vertical") * -0.1f;
                    ironBarTouchPoint.transform.position += movement;
                    ironBarTouchPoint.transform.position =
                        new Vector3(
                            Mathf.Clamp(ironBarTouchPoint.transform.position.x, barPos.x - moveArea, barPos.x + moveArea),
                            Mathf.Clamp(ironBarTouchPoint.transform.position.y, barPos.y - moveArea, barPos.y + moveArea),
                            Mathf.Clamp(ironBarTouchPoint.transform.position.z, barPos.z - moveArea, barPos.z + moveArea));

                    //アニメーション変更
                    anm.SetBool("PoleV", true);
                    
                    if (movement != Vector3.zero)
                        anm.SetBool("PoleVMove", true);
                    else
                    {
                        anm.SetBool("PoleVMove", false);
                    }

                    //moveArea = ironBar.transform.localScale.y;
                    //movement = barVectorNor * -Input.GetAxis("Vertical") * -0.1f;
                    //newPos = ironBarTouchPoint.transform.position + movement;
                    //offset = newPos - ironBar.transform.position;
                    //ironBarTouchPoint.transform.position = ironBar.transform.position + Vector3.ClampMagnitude(offset, moveArea);
                    //ironBarTouchPoint.transform.Rotate(tr.up, -Input.GetAxis("Horizontal") * 45.0f * Time.deltaTime);

                    break;
            }
        }

        if (touchIronBar == true && Input.GetKeyDown(KeyCode.Space))
        {
            tr.parent = null;
            touchIronBar = false;

            switch (barType)
            {
                case BarType.IRON_BAR:

                    //アニメーション変更
                    anm.SetBool("PoleH", false);
                    anm.SetBool("PoleHJump", true);

                    m_GravityDir.SetDirection(-tr.up);
                    m_MoveManager.SetState(PlayerState.NORMAL);
                    //プレイヤーの向きを更新する
                    m_MoveManager.SetPlayerUpFront(tr.up, Vector3.Cross(tr.up, Camera.main.transform.right));
                    //rb.AddForce(-tr.up * 200.0f);

                    break;
                case BarType.POLE:

                    //アニメーション変更
                    anm.SetBool("PoleV", false);
                    anm.SetBool("PoleVJump", true);

                    m_MoveManager.SetState(PlayerState.NORMAL);
                    //背面斜め上方向に方向にジャンプする
                    m_MoveManager.PlayerPoleKick(Vector3.Normalize(-tr.forward + tr.up));

                    break;
            }
        }
    }

    void LateUpdate()
    {
        //print(tr.up);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Transform touchTr = tr;
        if (touchIronBar == true) return;

        if (collision.gameObject.tag == "IronBar")
        {
            rb.isKinematic = true;

            GetComponent<NormalMove>().enabled = false;
            //GetComponent<PlayerBlockPush>().enabled = false;

            tr = touchTr;
            touchIronBar = true;
            ironBarTouchPoint.GetComponent<IronBarTouchPoint>().SetIsHit(touchIronBar);
            rb.velocity = Vector3.zero;
            rb.isKinematic = false;
            barCollision = collision;
            collisionIronBarPosition = barCollision.contacts[0].point;


            ironBar = collision.gameObject;
            barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());
            //print(Vector3.Dot(transform.up, barVectorNor));


            if (Vector3.Dot(transform.up, barVectorNor) < 0.7071068)
            {
                barType = BarType.IRON_BAR;
                ironBarTouchPoint.GetComponent<IronBarTouchPoint>().
                    SetPlayerDirection(-tr.up, tr.position - collisionIronBarPosition);

                tr.right = barVectorNor;
            }
            else
            {
                barType = BarType.POLE;
                ironBarTouchPoint.GetComponent<IronBarTouchPoint>().
                    SetPlayerDirection(-tr.forward, tr.position - collisionIronBarPosition);

                tr.up = barVectorNor;

                //Vector3 a = ironBar.transform.position;
                //a.y = tr.position.y;
                //tr.forward = a;
            }

            //print(barType);

            ironBarTouchPoint.transform.position = collisionIronBarPosition;

            //headPoint = tr;
            //tr.parent = headPoint;
        }
        else
        {
            GetComponent<NormalMove>().enabled = true;
            //GetComponent<PlayerBlockPush>().enabled = true;
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


    //void Update()
    //{
    //    if (touchIronBar == true)
    //    {
    //        switch (barType)
    //        {
    //            case BarType.IRON_BAR:
    //                //Vector3 barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());
    //                //Vector3 axis = barVectorNor;
    //                //transform.RotateAround(touchIronBarPosition, Vector3.Normalize(axis), Input.GetAxis("Vertical") * 50.0f * Time.deltaTime);
    //                RayHitInfo hitInfo = CheckBarHit(tr.position);

    //                rb.velocity = Vector3.zero;
    //                if (hitInfo.isHit)
    //                {

    //                    //当たった地点に移動
    //                    tr.position = hitInfo.hit.point + Vector3.Normalize(hitInfo.hit.normal) * 0.5f;
    //                    print("Aa");

    //                    //下方向を当たった平面の法線方向に変更
    //                    //down = hitInfo.hit.normal;
    //                }

    //                break;
    //            case BarType.POLE:
    //                break;
    //        }

    //    }
    //}

    //public void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "IronBar")
    //    {
    //        rb.velocity = Vector3.zero;
    //        print("鉄棒と接触");
    //        touchIronBar = true;
    //        ironBar = collision.gameObject;
    //        touchIronBarPosition = collision.contacts[0].point;
    //        touchIronBarPlayerPosition = transform.position;

    //        Vector3 barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());
    //        print(Vector3.Dot(transform.up, barVectorNor));

    //        if (Vector3.Dot(transform.up, barVectorNor) < 0.7071068)
    //        {
    //            barType = BarType.IRON_BAR;
    //        }
    //        else
    //        {
    //            barType = BarType.POLE;
    //        }

    //        print(barType);

    //        GetComponent<PlayerMove>().enabled = false;
    //        GetComponent<PlayerBlockPush>().enabled = false;

    //        //print(touchIronBarPosition);
    //    }
    //}


}
