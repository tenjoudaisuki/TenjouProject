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

    public void FadeInstance(float speed)
    {
        mFadePrefab.GetComponent<NextStageFade>().mSpeed = speed;
        GameObject fadeobj = (GameObject)Instantiate(mFadePrefab, transform, false);
    }

    public void FadeInstance(Color color, float speed)
    {
        mFadePrefab.GetComponent<Image>().color = color;
        mFadePrefab.GetComponent<NextStageFade>().mSpeed = speed;
        GameObject fadeobj = (GameObject)Instantiate(mFadePrefab, transform, false);
    }

    public void FadeColorChange(Color color)
    {
        mFadePrefab.GetComponent<Image>().color = color;
    }
}
