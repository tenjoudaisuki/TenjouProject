using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
    
    private Transform player;
    private Transform tr;
    private Vector3 offset;
    private Vector3 moveDirection;

    public Vector3 moveVec;
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
        //print(moveDirection);

        //プレイヤーとの距離がpushDistanceより離れたら強制的にisPushをfalseに
        if (Vector3.Distance(tr.position, player.position + offset) > pushDistance) isPush = false;

        BlockMove();
    }

    /// <summary>
    /// ブロックの移動処理
    /// </summary>
    public void BlockMove()
    {
        if (!Input.GetKey(KeyCode.B) || isPush == false) return;

        //print(player.up);
        //print(GetPlayerDirection().normal);
        //print(player.up + " " + GetPlayerDirection().normal + " " + Vector3.Dot(Vector3.Normalize(player.up), Vector3.Normalize(GetPlayerDirection().normal)));
        //print("up,-forward" + tr.up + " " + -tr.forward + " " + Vector3.Dot(tr.up, -tr.forward));

        //プレイヤーの上方向とブロックのプレイヤー方向の面の法線ベクトルで内積を作る
        float dot = Vector3.Dot(player.up, GetPlayerDirection().normal);
        //内積の数値を補正
        float dotAbs = Mathf.Abs(dot);
        float dotInt = Mathf.FloorToInt(dotAbs);
        //print(dotInt);
        //dot = Mathf.Clamp(dot, 0.0f, 1.0f);
        //print(player.up);
        //print(GetPlayerDirection().normal);

        //内積が0（90度）じゃなかったらreturn
        if (dotInt != 0) return;
        //位置を移動
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

    public void OnCollisionEnter(Collision collision)
    {
        //プレイヤーと当たったらブロック（自分）をセットする
        if (collision.gameObject.tag == "Player")
            player.GetComponent<NormalMove>().SetCollisionBlock(gameObject);
    }

    //public void OnCollisionStay(Collision collision)
    //{
    //    if (Input.GetKeyDown(KeyCode.B))
    //    {
    //        moveDirection = Vector3.Normalize(GetPlayerDirection().normal);
    //        isPush = true;
    //        player.GetComponent<PlayerBlockPush>().SetCollisionBlock(gameObject);
    //    }

    //    if (Input.GetKeyUp(KeyCode.B))
    //    {
    //        isPush = false;
    //        player.GetComponent<PlayerBlockPush>().SetCollisionBlock(null);
    //    }
    //}


    /// <summary>
    /// 押せる距離にプレイヤーがいるときの入力処理
    /// </summary>
    public void IsPushDistance()
    {
        if (Vector3.Distance(tr.position, player.position + offset) > pushDistance) return;

        if (Input.GetKey(KeyCode.B))
        {
            //移動方向にプレイヤー方向の面の法線ベクトルを設定
            moveDirection = Vector3.Normalize(GetPlayerDirection().normal);
            isPush = true;
            player.GetComponent<NormalMove>().SetCollisionBlock(gameObject);
        }
        else
        {
            isPush = false;
            player.GetComponent<NormalMove>().SetCollisionBlock(null);
        }

        Debug.DrawRay(tr.position, Vector3.Normalize
            (((player.position + offset) - tr.position)) * pushDistance, Color.blue);
    }

    /// <summary>
    /// 移動方向ベクトルを返す
    /// </summary>
    /// <returns></returns>
    public Vector3 GetBlockMoveDirection()
    {
        return moveDirection;
    }

    /// <summary>
    /// 押しているかの判定を返す
    /// </summary>
    /// <returns></returns>
    public bool GetIsPush()
    {
        return isPush;
    }

    /// <summary>
    /// 移動量ベクトルの設定
    /// </summary>
    /// <param name="vector"></param>
    public void SetMoveVector(Vector3 vector)
    {
        moveVec = vector;
    }

    /// <summary>
    /// プレイヤーの足元から中心へのOffsetをベクトルにして返す
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    private Vector3 PlayerDirectionOffsetY(float offset)
    {
        return new Vector3(0.0f, offset, 0.0f);
    }
}
