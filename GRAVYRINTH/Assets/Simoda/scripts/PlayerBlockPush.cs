using UnityEngine;
using System.Collections;

public class PlayerBlockPush : MonoBehaviour
{
    //当たったかどうか付のRaycastHit
    struct RayHitInfo
    {
        public RaycastHit hit;
        //当たったか？
        public bool isHit;
    };

    [SerializeField, TooltipAttribute("移動速度")]
    private float m_MoveSpeed = 3.0f;
    [SerializeField, TooltipAttribute("身長(地面との判定を行うRayで使用)")]
    private float m_Height = 2.0f;
    [SerializeField, TooltipAttribute("斜面と認識する角度（壁と斜面の境界値）")]
    private float m_SlopeDeg = 45.0f;

    private Transform tr;
    public Vector3 m_MoveVec;
    private GravityDirection m_GravityDir;
    private Block collisionBlock;
    private Transform m_Camera;

    // アニメーション
    private Animator anm;

    void Start()
    {
        m_GravityDir = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();

        tr = GetComponent<Transform>();
        m_Camera = Camera.main.transform;

        anm = GetComponent<Animator>();
    }

    void Update()
    {
        BlockPushMove();

        //重力の方向を設定
        m_GravityDir.SetDirection(GetDown());
    }

    /// <summary>
    /// 移動方向入力の取得
    /// </summary>
    private Vector2 GetMoveInputAxis()
    {
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        return direction;
    }

    /// <summary>
    /// 移動方向入力を補正して返す
    /// </summary>
    private Vector2 MoveInputCorrection(Vector2 input)
    {
        Vector2 direction = input;
        //加速と減速をいい感じに補正
        if (direction != Vector2.zero)
        {
            float length = direction.magnitude;
            length = Mathf.Min(1, length);
            length = length * length;
            direction = direction.normalized * length;
        }
        return direction;
    }

    /// <summary>
    /// 現在の下方向を取得
    /// </summary>
    private Vector3 GetDown()
    {
        return Vector3.Normalize(-tr.up);
    }

    /// <summary>
    /// 現在向いている下方向にレイを飛ばし、ヒットした情報を返す。
    /// </summary>
    /// <param name="reyPos">レイを飛ばす位置</param>
    /// <returns></returns>
    private RayHitInfo CheckGroundHit(Vector3 reyPos)
    {
        RayHitInfo result;
        Ray ray = new Ray(reyPos, GetDown());
        RaycastHit hit;
        result.isHit = Physics.Raycast(ray, out hit, m_Height);
        result.hit = hit;

        //レイをデバッグ表示
        Debug.DrawRay(reyPos, GetDown() * m_Height, Color.grey, 1.0f, false);

        return result;
    }

    private void BlockPushMove()
    {
        tr.GetComponent<NormalMove>().enabled = true;

        if (collisionBlock == null) return;
        collisionBlock.IsPushDistance();
        if (collisionBlock == null) return;

        if (collisionBlock.isPush == false) return;

        tr.GetComponent<NormalMove>().enabled = false;

        //移動方向入力
        Vector2 inputVec = GetMoveInputAxis();
        //入力された値を移動用に補正
        Vector2 moveVec = MoveInputCorrection(inputVec);

        Vector3 moveDirection = collisionBlock.GetBlockMoveDirection();
        m_MoveVec = (moveDirection * -moveVec.y + moveDirection * 0.0f) * m_MoveSpeed;

        collisionBlock.SetMoveVector(m_MoveVec);

        //移動
        tr.position += m_MoveVec * Time.deltaTime;
        if (inputVec.y > 0)
        {
            anm.SetBool("PushBlock", true);
            anm.SetBool("PullBlock", false);
        }
        else if (inputVec.y < 0)
        {
            anm.SetBool("PushBlock", false);
            anm.SetBool("PullBlock", true);
        }
        else if (inputVec.y == 0)
        {
            anm.SetBool("PushBlock", false);
            anm.SetBool("PullBlock", false);
        }
        print(inputVec);

        //仮重力
        tr.position += GetDown() * 1.0f * Time.deltaTime;
        //地面との判定
        RayHitInfo hitInfo = CheckGroundHit(tr.position + tr.up * m_Height);
        if (hitInfo.isHit)
        {
            //上方向と平面の法線方向のなす角
            float angle = Vector3.Angle(tr.up, hitInfo.hit.normal);
            //斜面として認識する角度以上なら何もしない
            if (angle > m_SlopeDeg) return;

            //当たった地点に移動
            tr.position = hitInfo.hit.point;
            //上方向を当たった平面の法線方向に変更
            tr.up = hitInfo.hit.normal;
        }
    }

    public void SetCollisionBlock(GameObject obj)
    {
        try
        {
            collisionBlock = obj.GetComponent<Block>();
        }
        catch
        {
            collisionBlock = null;
        }
    }

}
