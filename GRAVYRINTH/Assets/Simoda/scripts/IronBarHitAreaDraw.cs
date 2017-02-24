using UnityEngine;
using System.Collections;

public class IronBarHitAreaDraw : MonoBehaviour
{
    [SerializeField, TooltipAttribute("起動してからチェックを入れてください")]
    public bool isEnable = false;

    [SerializeField, TooltipAttribute("鉄棒をよじ登りで判定するときの当たり判定を表示するかどうか")]
    public bool crimb = false;
    [SerializeField, TooltipAttribute("上方向の鉄棒をぶら下がりで判定するときの当たり判定を表示するかどうか")]
    public bool dangleDown = false;
    [SerializeField, TooltipAttribute("下方向の鉄棒をぶら下がりで判定するときの当たり判定をするかどうか")]
    public bool dangleUp = false;

    [SerializeField, TooltipAttribute("鉄棒をよじ登りで判定するときの当たり判定の大きさ")]
    private float m_CrimbHitSize = 0.1f;
    [SerializeField, TooltipAttribute("上方向の鉄棒をぶら下がりで判定するときの当たり判定の大きさ")]
    private float m_DangleUpHitSize = 0.1f;
    [SerializeField, TooltipAttribute("下方向の鉄棒をぶら下がりで判定するときの当たり判定の大きさ")]
    private float m_DangleDownHitSize = 0.2f;
    [SerializeField, TooltipAttribute("鉄棒をよじ登りで判定するときのレイの長さ 変えない方がいいかも")]
    private float m_CrimbRayLength = 0.1f;
    [SerializeField, TooltipAttribute("上方向の鉄棒をぶら下がりで判定するときのレイの長さ 変えない方がいいかも")]
    private float m_DangleUpRayLength = 0.7f;
    [SerializeField, TooltipAttribute("下方向の鉄棒をぶら下がりで判定するときのレイの長さ 変えない方がいいかも")]
    private float m_DangleDownRayLength = 0.2f;
    [SerializeField, TooltipAttribute("鉄棒をよじ登りで判定するときのレイの位置の上方向への調整")]
    private float m_CrimbPositionOffset = 0.0f;
    [SerializeField, TooltipAttribute("下方向の鉄棒をぶら下がりで判定するときのレイの長さ 変えない方がいいかも")]
    private float m_DangleDownPositionOffset = 0.0f;

    private Transform tr;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void Update()
    {

    }

    public void OnDrawGizmos()
    {
        if (isEnable == false) return;

        if (crimb == true)
        {
            Ray forward = new Ray(tr.position, tr.forward);
            Gizmos.DrawRay(forward.origin + m_CrimbPositionOffset * tr.up, forward.direction * m_CrimbRayLength);
            Gizmos.DrawWireCube(forward.origin + m_CrimbPositionOffset * tr.up + forward.direction * m_CrimbRayLength, Vector3.one * m_CrimbHitSize * 2);
        }

        if (dangleDown == true)
        {
            Ray down = new Ray(tr.position, -tr.up);
            Gizmos.DrawRay(down.origin + m_DangleDownPositionOffset * tr.up, down.direction * m_DangleDownRayLength);
            Gizmos.DrawWireCube(down.origin + m_DangleDownPositionOffset * tr.up + down.direction * m_DangleDownRayLength, Vector3.one * m_DangleDownHitSize * 2);
        }

        if (dangleUp == true)
        {
            Ray up = new Ray(tr.position, tr.up);
            Gizmos.DrawRay(up.origin, up.direction * m_DangleUpRayLength);
            Gizmos.DrawWireCube(up.origin + up.direction * m_DangleUpRayLength, Vector3.one * m_DangleUpHitSize * 2);
        }

        //if (isEnable == false) return;

        //int layerMask = 1 << 8;

        //Ray forward = new Ray(tr.position, tr.forward);
        //RaycastHit forwardHitInto;

        ////鉄棒をポールとして判定
        //if (Physics.BoxCast(forward.origin, Vector3.one * m_CrimbHitSize, forward.direction, out forwardHitInto, tr.localRotation, m_CrimRayLength, layerMask, QueryTriggerInteraction.Ignore)
        //    && crimb == true)
        //{
        //    Gizmos.DrawRay(forward.origin, forward.direction * m_CrimRayLength);
        //    Gizmos.DrawWireCube(forward.origin + forward.direction * m_CrimRayLength, Vector3.one * m_CrimbHitSize * 2);
        //}

        //Ray down = new Ray(tr.position, -tr.up);
        //RaycastHit downHitInto;

        ////鉄棒を鉄棒として判定
        //if (Physics.BoxCast(down.origin, Vector3.one * m_DangleDownHitSize, down.direction, out downHitInto, tr.localRotation, m_DangleDownRayLength, layerMask, QueryTriggerInteraction.Ignore)
        //    && dangleDown == true)
        //{
        //    Gizmos.DrawRay(down.origin, down.direction * m_DangleDownRayLength);
        //    Gizmos.DrawWireCube(down.origin + down.direction * m_DangleDownRayLength, Vector3.one * m_DangleDownHitSize * 2);
        //}


        //Ray up = new Ray(tr.position, tr.up);
        //RaycastHit upHitInto;

        ////鉄棒を鉄棒として判定
        //if (Physics.BoxCast(up.origin, Vector3.one * m_DangleUpHitSize, up.direction, out upHitInto, tr.localRotation, m_DangleUpRayLength, layerMask, QueryTriggerInteraction.Ignore)
        //    && dangleUp == true)
        //{
        //    Gizmos.DrawRay(up.origin, up.direction * m_DangleUpRayLength);
        //    Gizmos.DrawWireCube(up.origin + up.direction * m_DangleUpRayLength, Vector3.one * m_DangleUpHitSize * 2);
        //}
    }
}
