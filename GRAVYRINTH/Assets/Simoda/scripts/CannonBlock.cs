using UnityEngine;
using System.Collections;

public class CannonBlock : MonoBehaviour
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
    private bool isSet = false;
    private bool isSetIgnore = false;
    private Light blueLight;

    public Vector3 moveVec;
    public bool isPush;
    public float pushDistancePlus = 0.5f;
    public float angle;
    public Transform cannonSetPoint;


    void Start()
    {
        blockCursorPrefab = (GameObject)Resources.Load("Cursor/BlockCursor");
        blockCursor = Instantiate(blockCursorPrefab);
        cursorDraw = blockCursor.GetComponent<BlockCursorDraw>();
        cursorDraw.SetBlock(gameObject);
        cursorDraw.ChangeBlockCursorType(BlockCursorDraw.BlockCursorType.CannonBlock);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        tr = gameObject.transform;
        offset = PlayerDirectionOffsetY(offsetY);
        isPush = false;

        blueLight = tr.FindChild("blockblue").transform.FindChild("Point light blockblue").GetComponent<Light>();
        blueLight.intensity = 2;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            player.GetComponent<PlayerMoveManager>().SetState(PlayerState.NORMAL);
            //ライトの明るさを変更
            blueLight.intensity = 2;
        }

        //offsetを求める
        offset = player.up * offsetY;

        //Cursorにoffsetを渡す
        cursorDraw.SetOffset(offset);

        //プレイヤーから自分へのRayがあたっているのが自身でなければ処理しない
        if (GetPlayerDirection().collider.gameObject != gameObject)
        {
            cursorDraw.NotShow();
            return;
        }

        //プレイヤーとの距離がpushDistanceより離れたら強制的にisPushをfalseに
        pushDistance = Vector3.Distance(tr.position, GetPlayerDirection().point) + pushDistancePlus;

        float currentDistance = Vector3.Distance(tr.position, player.position + offset);

        if (currentDistance > pushDistance) isPush = false;

        CannonBlockMove();
    }

    public void CannonBlockMove()
    {
        //入力処理
        IsPushDistance();

        //cursorDraw.SetBlock(tr);

        if (Vector3.Distance(tr.position, cannonSetPoint.position) < 0.1f)
        {
            if (isSetIgnore == false)
            {
                tr.position = cannonSetPoint.position;
                isSet = true;
                tr.FindChild("blockblue").gameObject.active = false;
                tr.FindChild("blockred").gameObject.active = true;
            }
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) >= 0.9f && isSet == true)
        {
            StartCoroutine(DelayMethod(1.0f, () =>
            {
                isSetIgnore = true;
                isSet = false;
                tr.FindChild("blockblue").gameObject.active = true;
                tr.FindChild("blockred").gameObject.active = false;
                StartCoroutine(DelayMethod(1.0f, () =>
                {
                    isSetIgnore = false;
                }));
            }));
        }

        if (isSet == true) return;

        if (!Input.GetKey(KeyCode.B) || isPush == false) return;

        float dot = Vector3.Dot(player.up, GetPlayerDirection().normal);
        //内積の数値を補正
        float dotAbs = Mathf.Abs(dot);
        float dotInt = Mathf.FloorToInt(dotAbs);

        //内積が0（90度）じゃなかったらreturn
        if (dotInt != 0) return;

        //ライトの明るさを変更
        blueLight.intensity = 8;

        player.GetComponent<PlayerMoveManager>().SetState(PlayerState.CANNON_BLOCK);
        player.GetComponent<CannonBlockMove>().SetCannonBlockObject(gameObject);

        //地面との判定あり、移動はしないプレイヤーがほしい 入力がなくてもプレイヤーの上方向を設定してくれる
        Transform rotateCenter = tr.FindChild("f_taihoucolone").transform;
        player.RotateAround(rotateCenter.position, Vector3.forward, (InputAxisVerticalDirection() * 20.0f) * Time.deltaTime);
        tr.RotateAround(rotateCenter.position, Vector3.forward, (InputAxisVerticalDirection() * 20.0f) * Time.deltaTime);
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

        if (Input.GetKey(KeyCode.B))
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
}
