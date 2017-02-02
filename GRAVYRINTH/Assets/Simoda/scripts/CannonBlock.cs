using UnityEngine;
using System.Collections;

public class CannonBlock : MonoBehaviour
{
    private CannonBlockManagar managar;
    private BlockCursorDraw cursorDraw;
    private GameObject blockCursorPrefab;
    private GameObject blockCursor;
    private Transform player;
    private Transform tr;
    private Vector3 offset;
    private Vector3 moveDirection;
    private float pushDistance;
    private float currentDistance;
    private float offsetY = 0.4f;
    private bool isSet = false;
    private bool isSetIgnore = false;
    private float ignoreTime = 0.0f;
    private bool pushDecision = false;

    private GameObject blockBlue;
    private Light blueLight;
    private GameObject blockRed;
    private GameObject taihouActived;
    private GameObject taihouDesactived;
    private Transform rotateCenter;

    public Vector3 moveVec;
    public bool isPush;
    public float pushDistancePlus = 0.5f;
    public float angle = 20.0f;
    public Transform cannonSetPoint;

    // 01/17アニメーション
    private Animator anm;
    private float anm_value;

    void Start()
    {
        managar = GameObject.Find("CannonBlockManagar").GetComponent<CannonBlockManagar>();
        blockCursorPrefab = (GameObject)Resources.Load("Cursor/BlockCursor");
        blockCursor = Instantiate(blockCursorPrefab);
        cursorDraw = blockCursor.GetComponent<BlockCursorDraw>();
        cursorDraw.SetBlock(gameObject);
        cursorDraw.ChangeBlockCursorType(BlockCursorDraw.BlockCursorType.CannonBlock);

        player = GameObject.FindGameObjectWithTag("Player").transform;

        // 01/17アニメーション
        anm = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();

        tr = gameObject.transform;
        offset = PlayerDirectionOffsetY(offsetY);
        isPush = false;

        blockBlue = tr.FindChild("blockblue").gameObject;
        blockRed = tr.FindChild("blockred").gameObject;
        taihouActived = tr.FindChild("f_taihou").gameObject.transform.FindChild("f_taihou_actived").gameObject;
        taihouDesactived = tr.FindChild("f_taihou").gameObject.transform.FindChild("f_taihou_desactived").gameObject;
        rotateCenter = tr.FindChild("f_taihoucolone").transform;

        blueLight = blockBlue.transform.FindChild("Point light blockblue").GetComponent<Light>();
        blueLight.intensity = 8;
        blueLight.range = 1;

        if (Vector3.Distance(tr.position, cannonSetPoint.position) < 0.1f && isSet == false)
        {
            if (isSetIgnore == false)
            {
                tr.position = cannonSetPoint.position;
                tr.rotation = Quaternion.Euler(Vector3.zero);
                isSet = true;
                managar.IsSetTrue();
                blockBlue.SetActive(false);
                blockRed.SetActive(true);
                taihouActived.SetActive(true);
                taihouDesactived.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (managar.GetIsSetAll() == true)
        {
            Destroy(blockCursor);
            blockBlue.SetActive(false);
            blockRed.SetActive(true);
            taihouActived.SetActive(true);
            taihouDesactived.SetActive(false);
            return;
        }

        if (Input.GetButtonUp("Action"))
        {
            player.GetComponent<PlayerMoveManager>().SetState(PlayerState.NORMAL);
            //ライトの明るさを変更
            if (blockRed.active == false)
                blueLight.range = 1;
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

        currentDistance = Vector3.Distance(tr.position, player.position + offset);

        if (currentDistance > pushDistance) isPush = false;

        CannonBlockMove();
    }

    public void CannonBlockMove()
    {
        //入力処理
        IsPushDistance();

        //cursorDraw.SetBlock(tr);

        if (Vector3.Distance(tr.position, cannonSetPoint.position) < 0.1f && isSet == false)
        {
            if (isSetIgnore == false)
            {
                tr.position = cannonSetPoint.position;
                tr.rotation = Quaternion.Euler(Vector3.zero);
                isSet = true;
                managar.IsSetTrue();
                blockBlue.SetActive(false);
                blockRed.SetActive(true);
                taihouActived.SetActive(true);
                taihouDesactived.SetActive(false);
                SoundManager.Instance.PlaySe("hell_bell");
            }
        }

        if (isSet == true)
        {
            ignoreTime += Time.deltaTime;
        }

        if (Mathf.Abs(InputAxisDirection()) >= 0.1f && isSet == true && ignoreTime >= 1.0f && Input.GetButton("Action") && currentDistance <= pushDistance)
        {
            isSetIgnore = true;
            isSet = false;
            managar.IsSetFalse();
            ignoreTime = 0.0f;
            blockBlue.SetActive(true);
            blockRed.SetActive(false);
            taihouActived.SetActive(false);
            taihouDesactived.SetActive(true);
            StartCoroutine(DelayMethod(1.0f, () =>
            {
                isSetIgnore = false;
            }));
        }

        if (isSet == true) return;

        //壁に埋まらないようにする処理
        //ブロックの端までの長さ×ステージのスケール倍率
        float distanceToWall = 0.07f * 5.0f;

        RaycastHit hitInto;
        Ray ray = new Ray(tr.position, -GetPlayerDirection().normal);
        Debug.DrawRay(ray.origin, ray.direction * distanceToWall, Color.black);


        PushDecitionSetUp();

        //[StopWall]レイヤーのみと判定させる
        int layermask = 1 << 12;
        if (Physics.Raycast(ray, out hitInto, distanceToWall, layermask, QueryTriggerInteraction.Ignore))
        {
            if (pushDecision)
            {
                return;
            }
        }

        if (!Input.GetButton("Action") || isPush == false) return;

        //float dot = Vector3.Dot(player.up, GetPlayerDirection().normal);
        ////内積の数値を補正
        //float dotAbs = Mathf.Abs(dot);
        //float dotInt = Mathf.FloorToInt(dotAbs);

        ////内積が0（90度）じゃなかったらreturn
        //if (dotInt != 0) return;

        //ライトの明るさを変更
        blueLight.range = 2;

        player.GetComponent<PlayerMoveManager>().SetState(PlayerState.CANNON_BLOCK);
        player.GetComponent<CannonBlockMove>().SetCannonBlockObject(gameObject);

        //rotateCenterを軸に回転
        player.RotateAround(rotateCenter.position, Vector3.forward, (InputAxisDirection() * angle) * Time.deltaTime);
        tr.RotateAround(rotateCenter.position, Vector3.forward, (InputAxisDirection() * angle) * Time.deltaTime);

        // 01/17アニメーション
        anm.SetBool("Block", true);
        anm_value = InputAxisDirection();
        if (anm_value == 0)
        {
            anm.SetBool("BlockMove", false);
        }
        else
        {
            anm.SetBool("BlockMove", true);
        }

        if (tr.right == GetPlayerDirection().normal)
        {
            anm.SetFloat("Block_x", anm_value);
            anm.SetFloat("Block_y", anm_value);
            anm.SetFloat("Block_z", anm_value);
        }
        else if (-tr.right == GetPlayerDirection().normal)
        {
            anm.SetFloat("Block_x", -anm_value);
            anm.SetFloat("Block_y", -anm_value);
            anm.SetFloat("Block_z", -anm_value);
        }
    }

    public RaycastHit GetPlayerDirection()
    {
        RaycastHit hitInto;
        Ray ray = new Ray(player.position + offset, tr.position - (player.position + offset));
        Physics.Raycast(ray, out hitInto);

        //Debug.DrawRay(tr.position, hitInto.normal, Color.red);
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

        if (Input.GetButton("Action"))
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
    private float InputAxisDirection()
    {
        float angle = Vector3.Angle(tr.right, Camera.main.transform.right);
        BlockArrow blockArrow = GameObject.Find("BlockArrow").GetComponent<BlockArrow>();

        if (tr.right == GetPlayerDirection().normal)
        {
            if (angle <= 45.0f)
            {
                blockArrow.SetInfo(true, "Horizontal");
                return -Input.GetAxis("Horizontal");
            }
            else if (angle >= 135.0f)
            {
                blockArrow.SetInfo(true, "Horizontal");
                return Input.GetAxis("Horizontal");
            }
            else
            {
                angle = Vector3.Angle(tr.forward, Camera.main.transform.right);
                if (angle <= 45.0f)
                {
                    blockArrow.SetInfo(true, "Vertical");
                    return -Input.GetAxis("Vertical");
                }
                else if (angle >= 135.0f)
                {
                    blockArrow.SetInfo(true, "Vertical");
                    return Input.GetAxis("Vertical");
                }
            }
        }
        else if (-tr.right == GetPlayerDirection().normal)
        {
            if (angle <= 45.0f)
            {
                blockArrow.SetInfo(true, "Horizontal");
                return -Input.GetAxis("Horizontal");
            }
            else if (angle >= 135.0f)
            {
                blockArrow.SetInfo(true, "Horizontal");
                return Input.GetAxis("Horizontal");
            }
            else
            {
                angle = Vector3.Angle(tr.forward, Camera.main.transform.right);
                if (angle <= 45.0f)
                {
                    blockArrow.SetInfo(true, "Vertical");
                    return -Input.GetAxis("Vertical");
                }
                else if (angle >= 135.0f)
                {
                    blockArrow.SetInfo(true, "Vertical");
                    return Input.GetAxis("Vertical");
                }
            }
        }

        return 0.0f;
    }

    private void PushDecitionSetUp()
    {
        float angle = Vector3.Angle(tr.right, Camera.main.transform.right);
        BlockArrow blockArrow = GameObject.Find("BlockArrow").GetComponent<BlockArrow>();

        if (tr.right == GetPlayerDirection().normal)
        {
            if (angle <= 45.0f)
            {
                pushDecision = -Input.GetAxis("Horizontal") > 0.1f;
            }
            else if (angle >= 135.0f)
            {
                pushDecision = -Input.GetAxis("Horizontal") < -0.1f;
            }
            else
            {
                angle = Vector3.Angle(tr.forward, Camera.main.transform.right);
                if (angle <= 45.0f)
                {
                    pushDecision = -Input.GetAxis("Vertical") > 0.1f;
                }
                else if (angle >= 135.0f)
                {
                    pushDecision = -Input.GetAxis("Vertical") < -0.1f;
                }
            }
        }
        else if (-tr.right == GetPlayerDirection().normal)
        {
            if (angle <= 45.0f)
            {
                pushDecision = -Input.GetAxis("Horizontal") < -0.1f;
            }
            else if (angle >= 135.0f)
            {
                pushDecision = -Input.GetAxis("Horizontal") > 0.1f;
            }
            else
            {
                angle = Vector3.Angle(tr.forward, Camera.main.transform.right);
                if (angle <= 45.0f)
                {
                    pushDecision = -Input.GetAxis("Vertical") < -0.1f;
                }
                else if (angle >= 135.0f)
                {
                    pushDecision = -Input.GetAxis("Vertical") > 0.1f;
                }
            }
        }
    }

    public bool GetIsSet()
    {
        return isSet;
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
