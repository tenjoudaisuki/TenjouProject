﻿/**==========================================================================*/
/**
 * ステージ4のスイッチ
 * 作成者：守屋   作成日：16/02/03
/**==========================================================================*/

using UnityEngine;
using System.Collections;

public class Stage4Switch : MonoBehaviour 
{

    /*==外部設定変数==*/

    /*==内部設定変数==*/
    //スイッチマネージャー
    private Stage4SwitchManager m_SwitchManager;
    //ボタンが押されているか？
    private bool m_IsDown = false;
    //スイッチのボタン部分のオブジェクト　押されている場合にActiveをfalseにして姿を消す
    private GameObject m_Button;
    //スイッチのボタン部分の根元のオブジェクト　↑とは逆に押されている場合にActiveをtrue
    private GameObject m_NonacttiveButton;

    void Start()
    {
        m_SwitchManager = GameObject.FindGameObjectWithTag("SwitchManager").GetComponent<Stage4SwitchManager>();
        m_IsDown = false;
        m_Button = transform.FindChild("active").gameObject;
        m_NonacttiveButton = transform.FindChild("nonactive").gameObject;
        m_NonacttiveButton.SetActive(false);
    }

    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player" || m_IsDown == true) return;
        SoundManager.Instance.PlaySe("switch");
        SoundManager.Instance.PlayLoopSe("trick");
        m_IsDown = true;
        m_Button.SetActive(!m_IsDown);
        m_NonacttiveButton.SetActive(m_IsDown);
    }

    public bool GetIsDown()
    {
        return m_IsDown;
    }

    public void SetIsDown(bool state)
    {
        m_IsDown = state;
        m_Button.SetActive(!m_IsDown);
        m_NonacttiveButton.SetActive(m_IsDown);
    }
}
