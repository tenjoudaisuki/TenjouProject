using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeFactory : MonoBehaviour {
    /// <summary>
    /// フェードプレハブ
    /// </summary>
    public GameObject mFadePrefab;

    /// <summary>
    /// 次のシーンに行くためのフェードを生成する
    /// </summary>
    public void FadeInstance()
    {
        GameObject fadeobj = (GameObject)Instantiate(mFadePrefab, transform,false);
    }

    public void FadeColorChange(Color color)
    {
        mFadePrefab.GetComponent<Image>().color = color;
    }
}
