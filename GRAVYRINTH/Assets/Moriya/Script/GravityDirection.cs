/**==========================================================================*/
/**
 * 現在の重力の方向
 * 歩く、斜面を歩く、段差を登る
 * 作成者：守屋   作成日：16/10/14
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class GravityDirection : MonoBehaviour 
{
    /*==内部設定変数==*/
    [SerializeField, TooltipAttribute("重力の方向")]
    private Vector3 m_GravityDirection = Vector3.down;

    /*============================外部参照関数============================*/
    /// <summary>
    /// 現在の重力の方向を取得する
    /// </summary>
    public Vector3 GetDirection()
    {
        return m_GravityDirection;
    }

    /// <summary>
    /// 重力の方向をセットする
    /// </summary>
    public void SetDirection(Vector3 v)
    {
        m_GravityDirection = v;
    }

}
