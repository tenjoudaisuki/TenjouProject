using UnityEngine;
using System.Collections;

public class Block_Save2 : MonoBehaviour
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
    private bool pushDecision = false;


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
        blockLight.intensity = 8;
        blockLight.range = 1;
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
            blockLight.range = 1;
        }

        ////Blockの各方向のSizeを取得
        //float xDistance = Vector3.Distance(tr.position, tr.FindChild("x").position);
        //float yDistance = Vector3.Distance(tr.position, tr.FindChild("y").position);
        //float zDistance = Vector3.Distance(tr.position, tr.FindChild("z").position);
        ////プレイヤーとの方向によってdistanceToWallを変更
        ////ブロックの中心から端までの距離
        //float distanceToWall = 0.0f;
        //if (-GetPlayerDirection().normal == tr.right || -GetPlayerDirection().normal == -tr.right)
        //{
        //    distanceToWall = xDistance + 0.5f;
        //}

        //if (-GetPlayerDirection().normal == tr.up || -GetPlayerDirection().normal == -tr.up)
        //{
        //    distanceToWall = yDistance + 0.5f;
        //}

        //if (-GetPlayerDirection().normal == tr.forward || -GetPlayerDirection().normal == -tr.forward)
        //{
        //    distanceToWall = zDistance + 0.5f;
        //}
        //RaycastHit hitIntoBox;
        //int layermaskBox = ~(1 << LayerMask.NameToLayer("IgnoredObj") | 1 << LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("StopWall"));
        //if (Physics.CheckBox(tr.position, new Vector3(xDistance, yDistance, zDistance), tr.rotation, layermaskBox, QueryTriggerInteraction.Ignore))
        //{
        //    if (Physics.BoxCast(tr.position, new Vector3(xDistance, yDistance, zDistance), -GetPlayerDirection().normal, out hitIntoBox, tr.rotation, 10.0f, layermaskBox, QueryTriggerInteraction.Ignore))
        //    {
        //        print(hitIntoBox.distance);
        //        if (hitIntoBox.distance < distanceToWall)
        //            tr.position += GetPlayerDirection().normal * Time.deltaTime;

        //    }
        //}

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
        pushDistance = length.magnitude + pushDistancePlus;
        float currentDistance = Vector3.Distance(tr.position, player.position + offset);

        if (currentDistance > pushDistance)
        {
            isPush = false;
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

        //Blockの各方向のSizeを取得
        float xDistance = Vector3.Distance(tr.position, tr.FindChild("x").position);
        float yDistance = Vector3.Distance(tr.position, tr.FindChild("y").position);
        float zDistance = Vector3.Distance(tr.position, tr.FindChild("z").position);

        //float xDistanceBox = 0;
        //float yDistanceBox = 0;
        //float zDistanceBox = 0;

        //if (-GetPlayerDirection().normal == tr.right || -GetPlayerDirection().normal == -tr.right)
        //{
        //    xDistanceBox = xDistance;
        //    yDistanceBox = yDistance;
        //    zDistanceBox = zDistance;
        //}

        //if (-GetPlayerDirection().normal == tr.up || -GetPlayerDirection().normal == -tr.up)
        //{
        //    distanceToWall = yDistance;

        //    xDistanceBox = xDistance;
        //    yDistanceBox = yDistance;
        //    zDistanceBox = zDistance;
        //}

        //if (-GetPlayerDirection().normal == tr.forward || -GetPlayerDirection().normal == -tr.forward)
        //{
        //    distanceToWall = zDistance;

        //    xDistanceBox = xDistance;
        //    yDistanceBox = yDistance;
        //    zDistanceBox = zDistance;
        //}

        //int layermask = ~(1 << LayerMask.NameToLayer("IgnoredObj") | 1 << LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("StopWall"));
        //Vector3 boxPos = tr.position + -GetPlayerDirection().normal * (distanceToWall + 0.025f);
        //if (Physics.CheckBox(boxPos, new Vector3(xDistanceBox, yDistanceBox, zDistanceBox), tr.rotation, layermask, QueryTriggerInteraction.Ignore))
        //{
        //    tr.position += GetPlayerDirection().normal * Time.deltaTime;
        //    return;
        //}

        //プレイヤーとの方向によってdistanceToWallを変更
        if (-GetPlayerDirection().normal == tr.right || -GetPlayerDirection().normal == -tr.right)
        {
            distanceToWall = xDistance;

            //if (player.up == tr.up || player.up == -tr.up)
            //    scale = new Vector3(0, yDistance + scaleCorrect, 0);
            //else
            //    scale = new Vector3(0, 0, zDistance + scaleCorrect);
        }

        if (-GetPlayerDirection().normal == tr.up || -GetPlayerDirection().normal == -tr.up)
        {
            distanceToWall = yDistance;

            //if (player.up == tr.right || player.up == -tr.right)
            //    scale = new Vector3(xDistance + scaleCorrect, 0, 0);
            //else
            //    scale = new Vector3(0, 0, zDistance + scaleCorrect);
        }

        if (-GetPlayerDirection().normal == tr.forward || -GetPlayerDirection().normal == -tr.forward)
        {
            distanceToWall = zDistance;

            //if (player.up == tr.right || player.up == -tr.right)
            //    scale = new Vector3(xDistance + scaleCorrect, 0, 0);
            //else
            //    scale = new Vector3(0, yDistance + scaleCorrect, 0);
        }

        //壁に埋まった時に戻す処理
        RaycastHit hitIntoBox;
        int layermaskBox = ~(1 << LayerMask.NameToLayer("IgnoredObj") | 1 << LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("StopWall"));
        if (Physics.BoxCast(tr.position + GetPlayerDirection().normal * distanceToWall, new Vector3(xDistance, yDistance, zDistance), -GetPlayerDirection().normal, out hitIntoBox, tr.rotation, 10.0f, layermaskBox, QueryTriggerInteraction.Ignore))
        {
            //print(hitIntoBox.distance);
            if (hitIntoBox.distance < distanceToWall)
            {
                tr.position += GetPlayerDirection().normal * 0.2f * Time.deltaTime;
                return;
            }
        }


        //Ray当たり判定を行う時の値の準備
        RaycastHit hitInto;
        distanceToWall += distanceToWallPlus;
        int layermask = 1 << LayerMask.NameToLayer("StopWall");

        //Rayで壁と判定　StopWallのみ
        if (Physics.BoxCast(tr.position, new Vector3(xDistance, yDistance, zDistance), -GetPlayerDirection().normal, out hitInto, tr.rotation, distanceToWall / distanceToWallDivide, layermask))
        {
            if (pushDecision)
            {
                return;
            }
        }

        if (Physics.BoxCast(tr.position, new Vector3(xDistance, yDistance, zDistance), GetPlayerDirection().normal, out hitInto, tr.rotation, distanceToWall / distanceToWallDivide, layermask))
        {
            if (!pushDecision)
            {
                return;
            }
        }

        //Rayで壁と判定
        layermask = ~(1 << LayerMask.NameToLayer("IgnoredObj") | 1 << LayerMask.NameToLayer("Player") | LayerMask.NameToLayer("StopWall"));
        if (Physics.BoxCast(tr.position, new Vector3(xDistance, yDistance, zDistance), -GetPlayerDirection().normal, out hitInto, tr.rotation, distanceToWall / distanceToWallDivide, layermask, QueryTriggerInteraction.Ignore))
        {
            if (pushDecision)
            {
                return;
            }
        }

        if (Physics.BoxCast(tr.position, new Vector3(xDistance, yDistance, zDistance), GetPlayerDirection().normal, out hitInto, tr.rotation, distanceToWall / distanceToWallDivide, layermask, QueryTriggerInteraction.Ignore))
        {
            if (!pushDecision)
            {
                return;
            }
        }

        if (!Input.GetButton("Action") || isPush == false) return;

        float angle = Vector3.Angle(player.up, GetPlayerDirection().normal);
        //print(angle);
        if (angle <= 89.0f || angle >= 91.0f) return;

        //ライトの明るさを変更
        blockLight.range = 2;

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

    /// <summary>
    /// 押せる距離にプレイヤーがいるときの入力処理
    /// </summary>
    public void IsPushDistance()
    {
        if (Vector3.Distance(tr.position, player.position + offset) > pushDistance)
        {
            isPush = false;
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

    public void SetPushDecision(bool pushDecision)
    {
        this.pushDecision = pushDecision;
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
