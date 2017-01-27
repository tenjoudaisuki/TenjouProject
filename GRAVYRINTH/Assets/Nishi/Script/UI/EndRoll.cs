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


	// Use this for initialization
	void Start ()
    {
        mIndex = 0;
        Image back = transform.GetChild(0).GetComponent<Image>();
        LeanTween.alpha(back.rectTransform, 1.0f, mDelay).setOnComplete(() =>
            {
                TextCreate();
            }
        );
	}

    void TextCreate()
    {
        GameObject textObj = (GameObject)Instantiate(mTexts[mIndex], transform, false);
        mCurrentImage = textObj.GetComponent<Image>();
        TextEffect();
    }

    //エンドロール処理
    void TextEffect()
    {
        if (mIndex % 2 == 0)
        {
            mCurrentImage.fillOrigin = 3;
        }
        LeanTween.value(0.0f, 1.0f, 5.0f).setOnUpdate((float val) => 
        {
            mCurrentImage.fillAmount = val;
        }
        ).setOnComplete(()=>
        {
            mIndex++;
            Destroy(mCurrentImage.gameObject);
            if(mIndex < mTexts.Length) TextCreate();
        }
        );
    }

}
