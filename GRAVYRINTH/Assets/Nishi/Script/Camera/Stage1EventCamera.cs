using UnityEngine;
using System.Collections;


/// <summary>
/// Stage1のイベントカメラ
/// </summary>
public class Stage1EventCamera : ICamera {

    private enum Steps
    {
        Step1,
        Step2,
        Step3,
        Step4
    }

    [SerializeField, TooltipAttribute("注視点の対象")]
    public Transform m_Target;
    [SerializeField, TooltipAttribute("注視点の調整")]
    public Vector3 m_TargetOffset = new Vector3(0.0f,0.4f,0.0f);
    [SerializeField, TooltipAttribute("注視点との距離")]
    public float m_Distance = 2.0f;
    [SerializeField, TooltipAttribute("歩く時間")]
    public float m_WalkTime = 2.0f;
    [SerializeField, TooltipAttribute("カメラを回すスピード")]
    public float m_Speed = -15.0f;

    /// <summary>
    /// センターオフセット
    /// </summary>
    private Vector3 m_CenterOffsetPosition;
    /// <summary>
    /// ゴールのオブジェクト
    /// </summary>
    private GameObject m_GoalObject;
    /// <summary>
    /// ローカルに変換したオフセット
    /// </summary>
    private Vector3 m_Offset;
    /// <summary>
    /// タイマー
    /// </summary>
    private float m_Timer;
    /// <summary>
    /// 現在のステップ
    /// </summary>
    private Steps m_CurrentStep;
    /// <summary>
    /// ボタンが押せる状態である
    /// </summary>
    private bool m_ButtonEnable;
    /// <summary>
    /// stageのセンター
    /// </summary>
    private Transform m_StageCenter;
    /// <summary>
    /// UIとの同期
    /// </summary>
    private UIandCameraSync m_SyncSystem;

    // Use this for initialization
    public override void Start ()
    {
        m_ButtonEnable = false;
        m_Timer = 0.0f;
        m_CurrentStep = Steps.Step1;
        m_StageCenter = GameObject.Find("StageCenter").transform;
        //Instantiate(m_DrawTexture);
        m_SyncSystem = GameObject.FindObjectOfType<UIandCameraSync>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        StepUpdate();
	}

    /// <summary>
    /// 2秒歩く
    /// </summary>
    void Step1()
    {
        m_Timer += Time.deltaTime;
        m_Offset = m_Target.right * m_TargetOffset.x + m_Target.up * m_TargetOffset.y + m_Target.forward * m_TargetOffset.z;
        Vector3 position = (m_Target.position + m_Offset) + (-m_Target.forward * m_Distance);

        transform.position = position;
        transform.localRotation = Quaternion.LookRotation(m_Target.forward,m_Target.up);

        //歩く時間を過ぎたら
        if (m_WalkTime > m_Timer)
        {
            transform.position = m_StageCenter.position + m_CenterOffsetPosition;
            StepChange(Steps.Step2);
        }
    }

    /// <summary>
    /// タイトルシーンのように回転
    /// </summary>
    void Step2()
    {
        m_Timer += Time.deltaTime;
        //カメラから中心に向かうベクトル
        Vector3 sub = m_StageCenter.position - transform.position;
        transform.localRotation = Quaternion.LookRotation(sub.normalized , m_StageCenter.up);
        transform.RotateAround(m_StageCenter.transform.position, m_StageCenter.up , m_Speed * Time.deltaTime);
        if (!m_ButtonEnable)
        {
            m_SyncSystem.SetCameraAction(() =>
            {
                StartCoroutine(DelayMethod(1.0f, () => {
                    StepChange(Steps.Step3);
                    m_ButtonEnable = false;
                }));
            });
        }
        m_ButtonEnable = true;
    }

    /// <summary>
    /// ゴールの方を見る
    /// </summary>
    void Step3()
    {
        m_Timer += Time.deltaTime;
        transform.position = m_GoalObject.transform.position;
        transform.localRotation = m_GoalObject.transform.localRotation;
        m_SyncSystem.SetCameraAction(() => { StepChange(Steps.Step4); m_ButtonEnable = false; });
        m_ButtonEnable = true;
    }

    /// <summary>
    /// シームレスに戻る
    /// </summary>
    void Step4()
    {
        GetComponent<CameraManager>().StateChange(State.GamePlay);
        GetComponent<CameraManager>().CameraReset();
    }

    void StepUpdate()
    {
        switch(m_CurrentStep)
        {
            case Steps.Step1: Step1(); break;
            case Steps.Step2: Step2(); break;
            case Steps.Step3: Step3(); break;
            case Steps.Step4: Step4(); break;
        }
    }

    void StepChange(Steps step)
    {
        m_ButtonEnable = false;
        m_Timer = 0.0f;
        m_CurrentStep = step;
    }

    public void SetPosition(GameObject obj)
    {
        m_GoalObject = obj;
    }

    public bool IsCameraMoveEnd()
    {
        return m_ButtonEnable;
    }

    public void SetCenterOffset(Vector3 offset)
    {
        m_CenterOffsetPosition = offset;
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="delayFrameCount"></param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float delayFrameCount, System.Action action)
    {

        yield return new WaitForSeconds(delayFrameCount);
        action();
    }

}
