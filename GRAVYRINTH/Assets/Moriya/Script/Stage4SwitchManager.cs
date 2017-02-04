/**===================================================================================*/
/**
 * ステージ4のスイッチ管理クラス
 * スイッチのいずれか１つが押されていたら、すべてのスイッチを押された状態に変更する。
 * 作成者：守屋   作成日：16/02/03
/**===================================================================================*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stage4SwitchManager : MonoBehaviour 
{

    [SerializeField, Tooltip("スイッチが押されてから、再度押せるようになるまでの時間")]
    private float m_SwitchUpTime = 5.5f;

    private GameObject[] m_SpinObjects;
    private GameObject[] m_Switchs;

    private float m_Timer = 0.0f;
    //ブロックが移動中か？
    private bool m_IsMoving = false;

	void Start ()
    {
        m_SpinObjects = GameObject.FindGameObjectsWithTag("Spin");
        m_Switchs = GameObject.FindGameObjectsWithTag("Switch");
	}

	void Update ()
    {
        if (m_IsMoving)
        {
            //時間経過でもう一度押せる状態にする
            m_Timer += Time.deltaTime;
            if(m_Timer > m_SwitchUpTime)
            {
                m_Timer = 0.0f;
                m_IsMoving = false;
                SetAllSwitchIsDown(false);
            }
        }
        else
        {
            //スイッチのどれかが押されていたらブロックが動くコルーチン実行
            for (int i = 0; i < m_Switchs.Length; i++)
            {
                if (m_Switchs[i].GetComponent<Stage4Switch>().GetIsDown())
                {
                    //コルーチン実行
                    for (int j = 0; j < m_SpinObjects.Length; j++)
                    {
                        m_SpinObjects[j].GetComponent<Stage4SpinObject>().StartSpin();
                    }

                    //すべてのスイッチを押された状態に変更
                    SetAllSwitchIsDown(true);
                    m_IsMoving = true;
                    break;
                }
            }
        }
	}

    /// <summary>
    /// すべてのスイッチの状態を変更する
    /// </summary>
    public void SetAllSwitchIsDown(bool state)
    {
        for(int i = 0; i < m_Switchs.Length; i++)
        {
            m_Switchs[i].GetComponent<Stage4Switch>().SetIsDown(state);
        }
    }
}
