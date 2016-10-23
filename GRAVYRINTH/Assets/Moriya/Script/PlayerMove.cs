/**==========================================================================*/
/**
 * プレイヤーの移動（通常時）
 * 歩く、斜面を歩く、段差を登る。
 * 重力の方向のセットもここで行う。
 * 作成者：守屋   作成日：16/10/14
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour 
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private GravityDirection m_GravityDirection;
    private Animator m_Animator;

    /*==外部設定変数==*/
    [SerializeField, TooltipAttribute("移動速度")]
    private float m_MoveSpeed = 3.0f;
    [SerializeField, TooltipAttribute("身長(Rayで使用)")]
    private float m_Height = 2.0f;

    /*==内部設定変数==*/



    /*==外部参照変数==*/


    void Awake()
    {
        //コンポーネント取得
        tr = GetComponent<Transform>();
        m_Animator = GetComponent<Animator>();
    }

	void Start ()
    {
        //コンポーネント取得
        m_GravityDirection = GameObject.Find("GravityDirection").GetComponent<GravityDirection>();
	}
	
	void Update () 
    {
        //移動処理
        Move();

        //重力の方向を設定
        m_GravityDirection.SetDirection(GetDown());
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
    /// 移動方向入力の取得
    /// </summary>
    private Vector2 GetMoveInputWASD()
    {
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) direction.y += 1.0f;
        if (Input.GetKey(KeyCode.S)) direction.y += -1.0f;
        if (Input.GetKey(KeyCode.A)) direction.x += -1.0f;
        if (Input.GetKey(KeyCode.D)) direction.x += 1.0f;
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
    private RaycastHit CheckGroundHit(Vector3 reyPos)
    {
        Ray ray = new Ray(reyPos, GetDown());
        RaycastHit hit;
        Physics.Raycast(ray, out hit, m_Height);
        Debug.DrawRay(reyPos, GetDown() * m_Height, Color.grey, 1.0f, false);

        return hit;
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        //移動
        Vector2 inputVec = MoveInputCorrection(GetMoveInputAxis());

        //カメラのトランスフォーム
        Transform cam = Camera.main.transform;
        //進行方向
        Vector3 right = cam.transform.right;
        Vector3 front = -Vector3.Cross(tr.up, right);
        //移動
        tr.position += (front * inputVec.y + right * inputVec.x) * m_MoveSpeed * Time.deltaTime;

        //仮重力
        tr.position += GetDown() * 1.0f * Time.deltaTime;

        //地面との判定
        RaycastHit hit = CheckGroundHit(tr.position + tr.up * m_Height);
        if (hit.normal != Vector3.zero)
        {
            tr.position = hit.point;
            tr.up = hit.normal;
        }
    }

    /// <summary>
    /// ジャンプ処理（小杉さんのタスク）
    /// </summary>
    private void Jump()
    {

    }

}
