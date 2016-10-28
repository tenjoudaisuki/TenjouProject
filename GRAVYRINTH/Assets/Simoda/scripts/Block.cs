using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
    private Transform player;
    private Transform tr;
    private Vector3 offset;
    private Vector3 moveDirection;
    private Vector3 moveVec;
    public bool isPush;
    public float offsetY;
    public float pushDistance;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        tr = gameObject.transform;
        offset = PlayerDirectionOffsetY(offsetY);
        isPush = false;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    moveDirection = Vector3.Normalize(GetPlayerDirection().normal);
        //    isPush = true;
        //    player.GetComponent<PlayerBlockPush>().SetCollisionBlock(gameObject);
        //}

        //if (Input.GetKeyUp(KeyCode.T))
        //{
        //    isPush = false;
        //    player.GetComponent<PlayerBlockPush>().SetCollisionBlock(null);
        //}

        print(moveDirection);

        BlockMove();
    }

    public void BlockMove()
    {
        if (isPush == false) return;

        if (Vector3.Dot(player.up, GetPlayerDirection().normal) != 0) return;

        tr.position += moveVec * Time.deltaTime;
    }

    public RaycastHit GetPlayerDirection()
    {
        RaycastHit hitInto;
        Ray ray = new Ray(player.position + offset, tr.position - (player.position + offset));
        Physics.Raycast(ray, out hitInto);

        Debug.DrawRay(tr.position, hitInto.normal, Color.red);
        return hitInto;
    }

    public void OnCollisionStay(Collision collision)
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            moveDirection = Vector3.Normalize(GetPlayerDirection().normal);
            isPush = true;
            player.GetComponent<PlayerBlockPush>().SetCollisionBlock(gameObject);
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            isPush = false;
            player.GetComponent<PlayerBlockPush>().SetCollisionBlock(null);
        }
    }

    public void IsPushDistance()
    {
        if (Vector3.Distance(tr.position, player.position + offset) > pushDistance) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            moveDirection = Vector3.Normalize(GetPlayerDirection().normal);
            isPush = true;
            player.GetComponent<PlayerBlockPush>().SetCollisionBlock(gameObject);
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            isPush = false;
            player.GetComponent<PlayerBlockPush>().SetCollisionBlock(null);
        }

        Debug.DrawRay(tr.position, Vector3.Normalize
            (((player.position + offset) - tr.position)) * pushDistance, Color.blue);
    }

    public Vector3 GetBlockMoveDirection()
    {
        return moveDirection;
    }

    public bool GetIsPush()
    {
        return isPush;
    }

    public void SetMoveVector(Vector3 vector)
    {
        moveVec = vector;
    }

    private Vector3 PlayerDirectionOffsetY(float offset)
    {
        return new Vector3(0.0f, offset, 0.0f);
    }
}
