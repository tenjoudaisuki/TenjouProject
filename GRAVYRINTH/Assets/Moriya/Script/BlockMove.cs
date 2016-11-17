/**==========================================================================*/
/**
 * プレイヤーの移動（ブロック掴み時）
 * 作成者：守屋   作成日：16/11/05
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class BlockMove : MonoBehaviour 
{
    /*==所持コンポーネント==*/
    private Transform tr;
    private Rigidbody rb;
    private Animator anm;

    //動かす対象のブロックオブジェクト
    private Block m_CollisionBlock;

	void Start() 
    {
	
	}

	void Update() 
    {
	
	}

    /// <summary>
    /// ブロックをセットする
    /// </summary>
    /// <param name="obj"></param>
    public void SetCollisionBlock(GameObject obj)
    {
        try
        {
            m_CollisionBlock = obj.GetComponent<Block>();
        }
        catch
        {
            m_CollisionBlock = null;
        }
    }
}
