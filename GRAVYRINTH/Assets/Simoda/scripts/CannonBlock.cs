using UnityEngine;
using System.Collections;

public class CannonBlock : MonoBehaviour
{
    private BlockCursorDraw cursorDraw;
    private Transform player;
    private Transform tr;
    private Vector3 offset;
    private Vector3 moveDirection;
    private float pushDistance;
    private float offsetY = 0.4f;

    public Vector3 moveVec;
    public bool isPush;
    public float pushDistancePlus = 0.5f;
    public float angle;


    void Start()
    {
        cursorDraw = GetComponent<BlockCursorDraw>();
        player = GameObject.Find("Player").transform;
        tr = gameObject.transform;
        offset = PlayerDirectionOffsetY(offsetY);
        isPush = false;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.M))
        {
            Transform a = tr.FindChild("f_taihoucolone").transform;
            player.RotateAround(a.position, Vector3.forward, angle * Time.deltaTime);
        }

        //offsetを求める
        offset = player.up * offsetY;

        //プレイヤーから自分へのRayがあたっているのが自身でなければ処理しない
        if (GetPlayerDirection().collider.gameObject != gameObject) return;

        //プレイヤーとの距離がpushDistanceより離れたら強制的にisPushをfalseに
        pushDistance = Vector3.Distance(tr.position, GetPlayerDirection().point) + pushDistancePlus;

        float currentDistance = Vector3.Distance(tr.position, player.position + offset);

        if (currentDistance > pushDistance) isPush = false;

        //BlockCursor表示・非表示
        cursorDraw.BlockCursorControl(currentDistance, pushDistance);

        CannonBlockMove();
    }

    public void CannonBlockMove()
    {
        //入力処理
        IsPushDistance();

        if (!Input.GetKey(KeyCode.V) || isPush == false) return;

        float dot = Vector3.Dot(player.up, GetPlayerDirection().normal);
        //内積の数値を補正
        float dotAbs = Mathf.Abs(dot);
        float dotInt = Mathf.FloorToInt(dotAbs);

        //内積が0（90度）じゃなかったらreturn
        if (dotInt != 0) return;

        Transform a = tr.FindChild("f_taihoucolone").transform;

        //地面との判定あり、移動はしないプレイヤーがほしい 入力がなくてもプレイヤーの上方向を設定してくれる
        
        tr.RotateAround(a.position, Vector3.forward, (InputAxisVerticalDirection() * angle) * Time.deltaTime);
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
        //if (collision.gameObject.tag == "Player")
        //    player.GetComponent<NormalMove>().SetCollisionBlock(gameObject);
    }

    /// <summary>
    /// 押せる距離にプレイヤーがいるときの入力処理
    /// </summary>
    public void IsPushDistance()
    {
        if (Vector3.Distance(tr.position, player.position + offset) > pushDistance) return;

        if (Input.GetKey(KeyCode.V))
        {
            //移動方向にプレイヤー方向の面の法線ベクトルを設定
            moveDirection = Vector3.Normalize(GetPlayerDirection().normal);
            isPush = true;
            //player.GetComponent<NormalMove>().SetCollisionBlock(gameObject);
        }
        else
        {
            isPush = false;
            //player.GetComponent<NormalMove>().SetCollisionBlock(null);
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

    /// <summary>
    /// プレイヤーが左右どちらにいるかでinputの方向を変える
    /// </summary>
    /// <returns></returns>
    private float InputAxisVerticalDirection()
    {
        if (tr.right == GetPlayerDirection().normal)
        {
            return Input.GetAxis("Vertical");
        }
        else if (-tr.right == GetPlayerDirection().normal)
        {
            return -Input.GetAxis("Vertical");
        }

        return 0.0f;
    }
}
