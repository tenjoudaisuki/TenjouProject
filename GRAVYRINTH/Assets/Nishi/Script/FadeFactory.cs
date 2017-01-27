using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeFactory : MonoBehaviour
{
    /// <summary>
    /// フェードプレハブ
    /// </summary>
    public GameObject mFadePrefab;

    public bool mlast = false;

    public void Start()
    {
        mlast = false;
    }

    /// <summary>
    /// 次のシーンに行くためのフェードを生成する
    /// </summary>
    public void FadeInstance()
    {
        GameObject fadeobj = (GameObject)Instantiate(mFadePrefab, transform, false);
        fadeobj.GetComponent<NextStageFade>().mLastMode = false;
    }

    public void FadeInstance(System.Action action)
    {
        GameObject fadeobj = (GameObject)Instantiate(mFadePrefab, transform, false);
        fadeobj.GetComponent<NextStageFade>().mAction = action;
        fadeobj.GetComponent<NextStageFade>().mLastMode = true;
    }

    public void FadeInstance(float speed)
    {
        mFadePrefab.GetComponent<NextStageFade>().mSpeed = speed;
        GameObject fadeobj = (GameObject)Instantiate(mFadePrefab, transform, false);
        fadeobj.GetComponent<NextStageFade>().mLastMode = false;
    }

    public void FadeInstance(Color color, float speed)
    {
        mFadePrefab.GetComponent<Image>().color = color;
        mFadePrefab.GetComponent<NextStageFade>().mSpeed = speed;
        GameObject fadeobj = (GameObject)Instantiate(mFadePrefab, transform, false);
        fadeobj.GetComponent<NextStageFade>().mLastMode = false;
    }

    public void FadeInstance(Color color, float speed, System.Action action)
    {
        mFadePrefab.GetComponent<Image>().color = color;
        mFadePrefab.GetComponent<NextStageFade>().mSpeed = speed;
        GameObject fadeobj = (GameObject)Instantiate(mFadePrefab, transform, false);
        fadeobj.GetComponent<NextStageFade>().mLastMode = false;
    }

    public void FadeColorChange(Color color)
    {
        mFadePrefab.GetComponent<Image>().color = color;
    }

    public void SetLast(bool islast)
    {
        mlast = islast;
    }
}
