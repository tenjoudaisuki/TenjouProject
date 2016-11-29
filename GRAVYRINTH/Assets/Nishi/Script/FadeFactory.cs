using UnityEngine;
using System.Collections;

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
}
