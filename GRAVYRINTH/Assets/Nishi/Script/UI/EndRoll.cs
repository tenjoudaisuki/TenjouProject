using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndRoll : MonoBehaviour {

    [SerializeField, TooltipAttribute("表示するテキスト群(順番)")]
    public GameObject[] mTexts;
    /// <summary>
    /// 表示してるテキストのイメージコンポーネント
    /// </summary>
    private Image mCurrentImage;
    /// <summary>
    /// 配列の添え字
    /// </summary>
    private int mIndex;
    [SerializeField, TooltipAttribute("エンドロールが開始する時間")]
    public float mDelay;
    [SerializeField, TooltipAttribute("フェードインの時間 スロー中なので小さくして")]
    public float mFadeInTime = 0.3f;
    [SerializeField, TooltipAttribute("フェードアウトの時間　スロー中なので小さくして")]
    public float mFadeOutTime = 0.3f;
    [SerializeField, TooltipAttribute("文字が生きている時間　スロー中なので小さくして")]
    public float mAliveTime = 0.8f;


    // Use this for initialization
    void Start ()
    {
        mIndex = 0;
        Image back = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        LeanTween.alpha(back.rectTransform, 1.0f, mDelay).setOnComplete(() =>
            {
                TextCreate();
            }
        );
	}

    void TextCreate()
    {
        GameObject textObj = (GameObject)Instantiate(mTexts[mIndex], transform.GetChild(0), false);
        mCurrentImage = textObj.GetComponent<Image>();
        TextEffect();
    }

    //エンドロール処理
    void TextEffect()
    {
        LeanTween.alpha(mCurrentImage.rectTransform, 1.0f, mFadeInTime)
        .setOnComplete(()=>
        {
            mIndex++;
            LeanTween.alpha(mCurrentImage.rectTransform, 0.0f, mFadeOutTime)
            .setOnComplete(() => 
            {
                Destroy(mCurrentImage.gameObject);
                if (mIndex < mTexts.Length) TextCreate();
            }
            ).setDelay(mAliveTime);
        }
        );
    }

}
