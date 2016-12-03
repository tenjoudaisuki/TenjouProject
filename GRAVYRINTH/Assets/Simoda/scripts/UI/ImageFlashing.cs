using UnityEngine;
using System.Collections;

public class ImageFlashing : MonoBehaviour
{
    //読み込まれたら点滅するするかどうか
    public bool playStart = false;
    //読み込んだら点滅する時、表示から始めるかどうか
    public bool startIn = true;
    //読み込んだら点滅する時、何秒で点滅するか
    public float second = 1.0f;
    //読み込んだら点滅する時、消した後、表示するまでのディレイ
    public float inDelay = 0.0f;
    //読み込んだら点滅する時、表示後、消えるまでのディレイ
    public float outDelay = 0.0f;

    private RectTransform tr;
    private bool flashing = true;

    void Start()
    {
        tr = gameObject.GetComponent<RectTransform>();

        if (playStart == true)
        {
            if (startIn == true)
            {
                FlashingIn(tr, second, inDelay, outDelay);
            }
            else
            {
                FlashingOut(tr, second, inDelay, outDelay);
            }
        }
    }

    void Update()
    {

    }


    /// <summary>
    /// 表示から点滅開始
    /// </summary>
    /// <param name="rectTr">RectTransform</param>
    /// <param name="second">表示するまでの時間</param>
    /// <param name="inDelay">消えた後、表示するまでのディレイ</param>
    /// <param name="outDelay">表示後、消えるまでのディレイ</param>
    public void FlashingIn(RectTransform rectTr, float second, float inDelay, float outDelay)
    {
        LeanTween.alpha(rectTr, 1.0f, second)
            .setOnComplete(() =>
            {
                //ディレイをかけるための処理
                LeanTween.alpha(rectTr, 1.0f, 0.0f)
                .setDelay(outDelay)
                .setOnComplete(() =>
                {
                    if (flashing == true)
                        FlashingOut(rectTr, second, inDelay, outDelay);
                });
            });
    }


    /// <summary>
    /// 消えるから点滅開始
    /// </summary>
    /// <param name="rectTr">RectTransform</param>
    /// <param name="second">表示するまでの時間</param>
    /// <param name="inDelay">消えた後、表示するまでのディレイ</param>
    /// <param name="outDelay">表示後、消えるまでのディレイ</param>
    public void FlashingOut(RectTransform rectTr, float second, float inDelay, float outDelay)
    {
        LeanTween.alpha(rectTr, 0.0f, second)
            .setOnComplete(() =>
            {
                //ディレイをかけるための処理
                LeanTween.alpha(rectTr, 0.0f, 0.0f)
                .setDelay(inDelay)
                .setOnComplete(() =>
                {
                    if (flashing == true)
                        FlashingIn(rectTr, second, inDelay, outDelay);
                });

            });
    }

    public void FlashingStart()
    {
        flashing = true;
    }

    public void FlashingStop(float alpha)
    {

        //Color color = gameObject.GetComponent<UnityEngine.UI.Image>().color;
        //gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(color.r, color.g, color.b, alpha);
        flashing = false;
        LeanTween.alpha(tr, 0.0f, 0.0f);
    }
}
