using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
    private BlockCursorDraw cursorDraw;
    private GameObject blockCursorPrefab;
    private GameObject blockCursor;
    private Transform player;
    private Transform tr;
    private Vector3 offset;
    private Vector3 moveDirection;
    private float pushDistance;
    private float offsetY = 0.4f;
    private Light blockLight;
    private float pushDistancePlus;
    private float distanceToWallPlus;
    private float distanceToWallDivide;


    public Vector3 moveVec;
    public bool isPush;
    public string colorBlockName = "blockblue";
    public string colorLightName = "Point light blockblue";

    void Start()
    {
        blockCursorPrefab = (GameObject)Resources.Load("Cursor/BlockCursor");
        blockCursor = Instantiate(blockCursorPrefab);
        cursorDraw = blockCursor.GetComponent<BlockCursorDraw>();
        cursorDraw.SetBlock(gameObject);
        cursorDraw.ChangeBlockCursorType(BlockCursorDraw.BlockCursorType.Block);



        player = GameObject.FindGameObjectWithTag("Player").transform;
        tr = gameObject.transform;
        offset = PlayerDirectionOffsetY(offsetY);
        isPush = false;
        pushDistancePlus = GameObject.Find("BlockManager").GetComponent<BlockManager>().GetPushDistancePlus();
        distanceToWallPlus = GameObject.Find("BlockManager").GetComponent<BlockManager>().GetDistanceToWallPlus();
        distanceToWallDivide = GameObject.Find("BlockManager").GetComponent<BlockManager>().GetDistanceToWallDivide();

        blockLight = tr.FindChild(colorBlockName).transform.FindChild(colorLightName).GetComponent<Light>();
        blockLight.intensity = 2;
    }

    void Update()
    {
        //offsetを求める
        offset = player.up * offsetY;

        //Cursorにoffsetを渡す
        cursorDraw.SetOffset(offset);

        if (Input.GetButtonUp("Action"))
        {
            //ライトの明るさを変更
            blockLight.intensity = 2;
        }

        if (player.GetComponent<NormalMove>().GetIsGroundHit() == false) return;

        //プレイヤーから自分へのRayがあたっているのが自身でなければ処理しない
        if (GetPlayerDirection().collider.gameObject != gameObject) return;

        //プレイヤーの前方向とプレイヤー方向の面の法線ベクトルの逆方向の角度が30度以上なら処理しない
        if (Mathf.Abs(Vector3.Angle(player.forward, -GetPlayerDirection().normal)) >= 30.0f) return;

        //プレイヤーとの距離がpushDistanceより離れたら強制的にisPushをfalseに
        RaycastHit hitInto;
        Ray ray = new Ray(player.position + offset, -GetPlayerDirection().normal);
        //[IgnoredObj]レイヤー以外と判定させる
        int layermask = ~(1 << 10);
        Physics.Raycast(ray, out hitInto, Mathf.Infinity, layermask, QueryTriggerInteraction.Ignore);

        if (hitInto.collider == null) return;
        if (hitInto.collider.gameObject != gameObject) return;

        Vector3 length = tr.position - hitInto.point;

        //print(a.magnitude);
        pushDistance = length.magnitude + pushDistancePlus;

        //pushDistance = Vector3.Distance(tr.position, GetPlayerDirection().point) + pushDistancePlus;

        float currentDistance = Vector3.Distance(tr.position, player.position + offset);

        if (currentDistance > pushDistance)
        {
            isPush = false;
            //player.GetComponent<NormalMove>().SetCollisionBlock(null);
        }
        else player.GetComponent<NormalMove>().SetCollisionBlock(gameObject);

        BlockMove();
    }

    /// <summary>
    /// ブロックの移動処理
    /// </summary>
    public void BlockMove()
    {
        //ブロックの中心から端までの距離
        float distanceToWall = 0.0f;
        //ブロックを持っている面に対して上方向のScale
        Vector3 scale = Vector3.zero;
        //Scaleの補正値
        float scaleCorrect = -0.1f;

        float xDistance = Vector3.Distance(tr.position, tr.FindChild("x").position);
        float yDistance = Vector3.Distance(tr.position, tr.FindChild("y").position);
        float zDistance = Vector3.Distance(tr.position, tr.FindChild("z").position);

        //print(-GetPlayerDirection().normal + ":::" + tr.right + ":::" + tr.up + ":::" + tr.forward);

        if (-GetPlayerDirection().normal == tr.right || -GetPlayerDirection().normal == -tr.right)
        {
            distanceToWall = xDistance;

            if (player.up == tr.up || player.up == -tr.up)
                scale = new Vector3(0, yDistance + scaleCorrect, 0);
            else
                scale = new Vector3(0, 0, zDistance + scaleCorrect);
        }

        if (-GetPlayerDirection().normal == tr.up || -GetPlayerDirection().normal == -tr.up)
        {
            distanceToWall = yDistance;

            if (player.up == tr.right || player.up == -tr.right)
                scale = new Vector3(xDistance + scaleCorrect, 0, 0);
            else
                scale = new Vector3(0, 0, zDistance + scaleCorrect);
        }

        if (-GetPlayerDirection().normal == tr.forward || -GetPlayerDirection().normal == -tr.forward)
        {
            distanceToWall = zDistance;

            if (player.up == tr.right || player.up == -tr.right)
                scale = new Vector3(xDistance + scaleCorrect, 0, 0);
            else
                scale = new Vector3(0, yDistance + scaleCorrect, 0);
        }

        RaycastHit hitInto;
        //Ray forward_Center = new Ray(tr.position, -GetPlayerDirection().normal);
        //Ray forward_Up = new Ray(tr.position + scale, -GetPlayerDirection().normal);
        //Ray forward_Down = new Ray(tr.position - scale, -GetPlayerDirection().normal);

        //Ray back_Center = new Ray(tr.position, GetPlayerDirection().normal);
        //Ray back_Up = new Ray(tr.position + scale, GetPlayerDirection().normal);
        //Ray back_Down = new Ray(tr.position - scale, GetPlayerDirection().normal);

        //Ray[] forward_Rays = { forward_Center, forward_Up, forward_Down };
        //Ray[] back_rays = { back_Center, back_Up, back_Down };

        distanceToWall += distanceToWallPlus;

        //int layermask = ~(1 << 10);
        int layermask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9;
        //new Vector3(xDistance, yDistance, zDistance) / 14.0f
        if (Physics.BoxCast(tr.position, new Vector3(xDistance, yDistance, zDistance), -GetPlayerDirection().normal, out hitInto, tr.rotation, distanceToWall / distanceToWallDivide, layermask, QueryTriggerInteraction.Ignore))
        {
            if (Input.GetAxis("Vertical") > 0.1f)
            {
                return;
            }
        }

        if (Physics.BoxCast(tr.position, new Vector3(xDistance, yDistance, zDistance), GetPlayerDirection().normal, out hitInto, tr.rotation, distanceToWall / distanceToWallDivide, layermask, QueryTriggerInteraction.Ignore))
        {
            if (Input.GetAxis("Vertical") < -0.1f)
            {
                return;
            }
        }

        //if (Physics.BoxCast(tr.position, new Vector3(xDistance, yDistance, zDistance) / 14.0f, GetPlayerDirection().normal, out hitInto, tr.rotation, distanceToWall, layermask, QueryTriggerInteraction.Ignore))
        //{
        //    if (Input.GetAxis("Vertical") < -0.1f)
        //    {
        //        return;
        //    }
        //}

        //foreach (Ray ray in forward_Rays)
        //{
        //    if (Physics.Raycast(ray, out hitInto, distanceToWall, layermask, QueryTriggerInteraction.Ignore))
        //    {
        //        if (Input.GetAxis("Vertical") > 0.1f)
        //        {
        //            return;
        //        }
        //    }
        //}

        //foreach (Ray ray in back_rays)
        //{
        //    if (Physics.Raycast(ray, out hitInto, distanceToWall, layermask, QueryTriggerInteraction.Ignore))
        //    {
        //        if (Input.GetAxis("Vertical") < -0.1f)
        //        {
        //            return;
        //        }
        //    }
        //}

        //Debug.DrawRay(forward_Center.origin, forward_Center.direction * distanceToWall, Color.black);
        //Debug.DrawRay(forward_Up.origin, forward_Up.direction * distanceToWall, Color.white);
        //Debug.DrawRay(forward_Down.origin, forward_Down.direction * distanceToWall, Color.yellow);

        //[IgnoredObj]レイヤー以外と判定させる
        //int layermask = ~(1 << 10);
        //int layermask = 1 << 0 | 1 << 1 | 1 << 2 | 1 << 3 | 1 << 4 | 1 << 5 | 1 << 6 | 1 << 7 | 1 << 8 | 1 << 9;
        //壁に埋まらないようにする処理
        //if (Physics.Raycast(center, out hitInto, distanceToWall, layermask, QueryTriggerInteraction.Ignore))
        //{
        //    if (Input.GetAxis("Vertical") > 0.1f)
        //    {
        //        return;
        //    }
        //}
        //if (Physics.Raycast(up, out hitInto, distanceToWall, layermask, QueryTriggerInteraction.Ignore))
        //{
        //    if (Input.GetAxis("Vertical") > 0.1f)
        //    {
        //        return;
        //    }
        //}
        //if (Physics.Raycast(down, out hitInto, distanceToWall, layermask, QueryTriggerInteraction.Ignore))
        //{
        //    if (Input.GetAxis("Vertical") > 0.1f)
        //    {
        //        return;
        //    }
        //}

        if (!Input.GetButton("Action") || isPush == false) return;

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

        //ライトの明るさを変更
        blockLight.intensity = 8;

        //位置を移動
        tr.position += moveVec * Time.deltaTime;
    }

    public RaycastHit GetPlayerDirection()
    {
        RaycastHit hitInto;
        Ray ray = new Ray(player.position + offset, tr.position - (player.position + offset));
        //[IgnoredObj]レイヤー以外と判定させる
        int layermask = ~(1 << 10);
        Physics.Raycast(ray, out hitInto, Mathf.Infinity, layermask, QueryTriggerInteraction.Ignore);

        //Debug.DrawRay(tr.position, hitInto.normal, Color.red);
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
        if (Vector3.Distance(tr.position, player.position + offset) > pushDistance)
        {
            isPush = false;
            //player.GetComponent<NormalMove>().SetCollisionBlock(null);
            return;
        }

        if (Input.GetButton("Action"))
        {
            //移動方向にプレイヤー方向の面の法線ベクトルを設定
            moveDirection = Vector3.Normalize(GetPlayerDirection().normal);
            isPush = true;
            player.GetComponent<NormalMove>().SetCollisionBlock(gameObject);
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

    public float GetPushDistance()
    {
        return pushDistance;
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
