/**==========================================================================*/
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
    private SwitchManager m_SwitchManager;
    //ボタンが押されているか？
    private bool m_IsDown = false;
    //スイッチのボタン部分のオブジェクト　押されている場合にActiveをfalseにして姿を消す
    private GameObject m_Button;

    void Start()
    {
        m_SwitchManager = GameObject.FindGameObjectWithTag("SwitchManager").GetComponent<SwitchManager>();
        m_IsDown = false;
        m_Button = transform.FindChild("active").gameObject;
    }

    void Update()
    {

    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player" || m_IsDown == true) return;
        SoundManager.Instance.PlaySe("switch");
        m_IsDown = true;
        m_Button.SetActive(!m_IsDown);
    }

    public bool GetIsnDown()
    {
        return m_IsDown;
    }

    public void SetIsDown(bool state)
    {
        m_IsDown = state;
        m_Button.SetActive(!m_IsDown);
    }
}
