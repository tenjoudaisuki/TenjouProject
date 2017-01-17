using UnityEngine;
using System.Collections;

public class StageFinalClearCollider : MonoBehaviour
{
    private CannonBlockManagar m_CannonManager;
    private GameObject m_Taihou;
    private GameObject m_StairUp;
    private GameObject m_StairRight;
    private GameObject m_StairLeft;
    private GameObject m_TaihouRingArrow1;
    private GameObject m_TaihouRingArrow2;
    private GameObject m_TaihouRingArrow3;

    private Material arrow1Mat;
    private Color arrow1Color;
    private Color arrow1Alpha;

    private Material arrow2Mat;
    private Color arrow2Color;
    private Color arrow2Alpha;

    private Material arrow3Mat;
    private Color arrow3Color;
    private Color arrow3Alpha;

    private bool isFlashing = false;
    private bool isTrigger = false;



    private bool isClear = false;

    public float taihouAngle = -10.0f;
    public float taihouAngleTime = 1.0f;
    public float stairPullDownTime = 1.0f;
    public float cursorFlashingTime = 1.0f;

    public GameObject CameraTarget;

    public float EventCameraEndTime = 8.0f;

    void Start()
    {
        //オブジェクトの検索
        m_CannonManager = GameObject.Find("CannonBlockManagar").GetComponent<CannonBlockManagar>();

        m_Taihou = GameObject.FindGameObjectWithTag("Taihou");
        GetComponent<SphereCollider>().enabled = false;

        m_StairUp = GameObject.Find("f_stair (2)");
        m_StairRight = GameObject.Find("f_stair (3)");
        m_StairLeft = GameObject.Find("f_stair (1)");

        m_TaihouRingArrow1 = GameObject.FindGameObjectWithTag("Taihou").transform.FindChild("block1").transform.FindChild("f_taihou").transform.FindChild("f_taihouringarrow1").gameObject;
        m_TaihouRingArrow2 = GameObject.FindGameObjectWithTag("Taihou").transform.FindChild("block1").transform.FindChild("f_taihou").transform.FindChild("f_taihouringarrow2").gameObject;
        m_TaihouRingArrow3 = GameObject.FindGameObjectWithTag("Taihou").transform.FindChild("block1").transform.FindChild("f_taihou").transform.FindChild("f_taihouringarrow3").gameObject;

        //m_TaihouRingArrow1 = GameObject.Find("f_taihouringarrow1").gameObject;
        //m_TaihouRingArrow2 = GameObject.Find("f_taihouringarrow2").gameObject;
        //m_TaihouRingArrow3 = GameObject.Find("f_taihouringarrow3").gameObject;

        //TaihouRingArrow1の色の保存・変更
        arrow1Mat = m_TaihouRingArrow1.GetComponent<Renderer>().material;
        arrow1Color = arrow1Mat.GetColor("_TintColor");
        arrow1Alpha = new Color(arrow1Color.r, arrow1Color.g, arrow1Color.b, 0.0f);
        arrow1Mat.SetColor("_TintColor", arrow1Alpha);

        //TaihouRingArrow2の色の保存・変更
        arrow2Mat = m_TaihouRingArrow2.GetComponent<Renderer>().material;
        arrow2Color = arrow2Mat.GetColor("_TintColor");
        arrow2Alpha = new Color(arrow2Color.r, arrow2Color.g, arrow2Color.b, 0.0f);
        arrow2Mat.SetColor("_TintColor", arrow2Alpha);

        //TaihouRingArrow3の色の保存・変更
        arrow3Mat = m_TaihouRingArrow3.GetComponent<Renderer>().material;
        arrow3Color = arrow3Mat.GetColor("_TintColor");
        arrow3Alpha = new Color(arrow3Color.r, arrow3Color.g, arrow3Color.b, 0.0f);
        arrow3Mat.SetColor("_TintColor", arrow3Alpha);
    }

    void Update()
    {
        //クリアしたら
        if (isClear == false && m_CannonManager.GetIsSetAll() == true)
        {
            GameObject.Find("Camera").GetComponent<EventCamera>().SetMoveTime(0.0f);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetEventEndTime(EventCameraEndTime);
            GameObject.Find("Camera").GetComponent<EventCamera>().SetTarget(CameraTarget);

            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Event);

            //当たり判定を有効にする
            GetComponent<SphereCollider>().enabled = true;

            //上・右・左の階段をひっこめる 1段目
            GameObject stairUp1 = m_StairUp.transform.FindChild("object2").gameObject;
            float stairUp1Y = stairUp1.transform.position.y;
            LeanTween.move(stairUp1, new Vector3(stairUp1.transform.position.x, stairUp1Y, stairUp1.transform.position.z), stairPullDownTime);

            GameObject stairRight1 = m_StairRight.transform.FindChild("object2").gameObject;
            float stairRight1X = stairRight1.transform.position.x;
            LeanTween.move(stairRight1, new Vector3(stairRight1X, stairRight1.transform.position.y, stairRight1.transform.position.z), stairPullDownTime);

            GameObject stairLeft1 = m_StairLeft.transform.FindChild("object2").gameObject;
            float stairLeft1X = stairLeft1.transform.position.x;
            LeanTween.move(stairLeft1, new Vector3(stairLeft1X, stairLeft1.transform.position.y, stairLeft1.transform.position.z), stairPullDownTime);

            //2段目
            StartCoroutine(DelayMethod(stairPullDownTime, () =>
            {
                GameObject stairUp2 = m_StairUp.transform.FindChild("object2 (1)").gameObject;
                LeanTween.move(stairUp2, new Vector3(stairUp2.transform.position.x, stairUp1Y, stairUp2.transform.position.z), stairPullDownTime);

                GameObject stairRight2 = m_StairRight.transform.FindChild("object2 (1)").gameObject;
                LeanTween.move(stairRight2, new Vector3(stairRight1X, stairRight2.transform.position.y, stairRight2.transform.position.z), stairPullDownTime);

                GameObject stairLeft2 = m_StairLeft.transform.FindChild("object2 (1)").gameObject;
                LeanTween.move(stairLeft2, new Vector3(stairLeft1X, stairLeft2.transform.position.y, stairLeft2.transform.position.z), stairPullDownTime);
            }));

            //3段目
            StartCoroutine(DelayMethod(stairPullDownTime * 2.0f, () =>
            {
                GameObject stairUp3 = m_StairUp.transform.FindChild("object2 (2)").gameObject;
                LeanTween.move(stairUp3, new Vector3(stairUp3.transform.position.x, stairUp1Y, stairUp3.transform.position.z), stairPullDownTime);

                GameObject stairRight3 = m_StairRight.transform.FindChild("object2 (2)").gameObject;
                LeanTween.move(stairRight3, new Vector3(stairRight1X, stairRight3.transform.position.y, stairRight3.transform.position.z), stairPullDownTime);

                GameObject stairLeft3 = m_StairLeft.transform.FindChild("object2 (2)").gameObject;
                LeanTween.move(stairLeft3, new Vector3(stairLeft1X, stairLeft3.transform.position.y, stairLeft3.transform.position.z), stairPullDownTime);
            }));

            //4段目
            StartCoroutine(DelayMethod(stairPullDownTime * 3.0f, () =>
            {
                GameObject stairUp4 = m_StairUp.transform.FindChild("object2 (3)").gameObject;
                LeanTween.move(stairUp4, new Vector3(stairUp4.transform.position.x, stairUp1Y, stairUp4.transform.position.z), stairPullDownTime);

                GameObject stairRight4 = m_StairRight.transform.FindChild("object2 (3)").gameObject;
                LeanTween.move(stairRight4, new Vector3(stairRight1X, stairRight4.transform.position.y, stairRight4.transform.position.z), stairPullDownTime);

                GameObject stairLeft4 = m_StairLeft.transform.FindChild("object2 (3)").gameObject;
                LeanTween.move(stairLeft4, new Vector3(stairLeft1X, stairLeft4.transform.position.y, stairLeft4.transform.position.z), stairPullDownTime);
            }));

            //5段目
            StartCoroutine(DelayMethod(stairPullDownTime * 4.0f, () =>
            {
                GameObject stairUp5 = m_StairUp.transform.FindChild("object2 (4)").gameObject;
                LeanTween.move(stairUp5, new Vector3(stairUp5.transform.position.x, stairUp1Y, stairUp5.transform.position.z), stairPullDownTime);

                GameObject stairRight5 = m_StairRight.transform.FindChild("object2 (4)").gameObject;
                LeanTween.move(stairRight5, new Vector3(stairRight1X, stairRight5.transform.position.y, stairRight5.transform.position.z), stairPullDownTime);

                GameObject stairLeft5 = m_StairLeft.transform.FindChild("object2 (4)").gameObject;
                LeanTween.move(stairLeft5, new Vector3(stairLeft1X, stairLeft5.transform.position.y, stairLeft5.transform.position.z), stairPullDownTime);
            }));

            //6段目
            StartCoroutine(DelayMethod(stairPullDownTime * 5.0f, () =>
            {
                GameObject stairUp6 = m_StairUp.transform.FindChild("object2 (5)").gameObject;
                LeanTween.move(stairUp6, new Vector3(stairUp6.transform.position.x, stairUp1Y, stairUp6.transform.position.z), stairPullDownTime);

                GameObject stairRight6 = m_StairRight.transform.FindChild("object2 (5)").gameObject;
                LeanTween.move(stairRight6, new Vector3(stairRight1X, stairRight6.transform.position.y, stairRight6.transform.position.z), stairPullDownTime);

                GameObject stairLeft6 = m_StairLeft.transform.FindChild("object2 (5)").gameObject;
                LeanTween.move(stairLeft6, new Vector3(stairLeft1X, stairLeft6.transform.position.y, stairLeft6.transform.position.z), stairPullDownTime);
            }));


            StartCoroutine(DelayMethod(stairPullDownTime * 6.0f, () =>
              {
                  //大砲を傾ける
                  LeanTween.rotate(m_Taihou, new Vector3(taihouAngle, 0, 0), taihouAngleTime)
                        .setEase(LeanTweenType.easeInOutBack)
                        .setOnComplete(() =>
                        {
                            isFlashing = true;
                        });
              }));

            //LeanTween.move(m_StairRight, m_StairRight.transform.position + new Vector3(2, 0, 0), stairPullDownTime);
            //LeanTween.move(m_StairLeht, m_StairLeht.transform.position + new Vector3(-2, 0, 0), stairPullDownTime);

            //m_TaihouRingArrow1.active = true;
            //m_TaihouRingArrow2.active = true;
            //m_TaihouRingArrow3.active = true;




            isClear = true;
        }

        //大砲カーソルの点滅
        if (isFlashing == true && isTrigger == false)
        {
            isFlashing = false;
            if (isTrigger == true) return;
            LeanTween.value(m_TaihouRingArrow1, arrow1Alpha, arrow1Color, cursorFlashingTime).setOnUpdate((Color c) => { arrow1Mat.SetColor("_TintColor", c); })
                .setOnComplete(() =>
                {
                    if (isTrigger == true) return;
                    LeanTween.value(m_TaihouRingArrow1, arrow1Color, arrow1Alpha, cursorFlashingTime).setOnUpdate((Color c) => { arrow1Mat.SetColor("_TintColor", c); });
                    LeanTween.value(m_TaihouRingArrow2, arrow2Alpha, arrow2Color, cursorFlashingTime).setOnUpdate((Color c) => { arrow2Mat.SetColor("_TintColor", c); })
                    .setOnComplete(() =>
                    {
                        if (isTrigger == true) return;
                        LeanTween.value(m_TaihouRingArrow2, arrow2Color, arrow2Alpha, cursorFlashingTime).setOnUpdate((Color c) => { arrow2Mat.SetColor("_TintColor", c); });
                        LeanTween.value(m_TaihouRingArrow3, arrow3Alpha, arrow3Color, cursorFlashingTime).setOnUpdate((Color c) => { arrow3Mat.SetColor("_TintColor", c); })
                        .setOnComplete(() =>
                        {
                            if (isTrigger == true) return;
                            LeanTween.value(m_TaihouRingArrow3, arrow3Color, arrow3Alpha, cursorFlashingTime).setOnUpdate((Color c) => { arrow3Mat.SetColor("_TintColor", c); });
                            isFlashing = true;
                        });
                    });
                });
        }

        //イベントシーンに入ったら大砲カーソルの点滅を停止
        if (isTrigger == true)
        {
            isFlashing = false;

            StartCoroutine(DelayMethod(0.3f, () =>
            {
                //LeanTween.value(m_TaihouRingArrow1, arrow1Color, arrow1Alpha, cursorFlashingTime).setOnUpdate((Color c) => { arrow1Mat.SetColor("_TintColor", c); });
                //LeanTween.value(m_TaihouRingArrow2, arrow2Color, arrow2Alpha, cursorFlashingTime).setOnUpdate((Color c) => { arrow2Mat.SetColor("_TintColor", c); });
                //LeanTween.value(m_TaihouRingArrow3, arrow3Color, arrow3Alpha, cursorFlashingTime).setOnUpdate((Color c) => { arrow3Mat.SetColor("_TintColor", c); });

                m_TaihouRingArrow1.SetActive(false);
                m_TaihouRingArrow2.SetActive(false);
                m_TaihouRingArrow3.SetActive(false);
            }));
        }
    }


    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            isTrigger = true;
            coll.gameObject.GetComponent<PlayerMoveManager>().SetState(PlayerState.STAGE_FINAL_CLEAR);
            GameObject.Find("Camera").GetComponent<CameraManager>().StateChange(State.Ending);
        }
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
