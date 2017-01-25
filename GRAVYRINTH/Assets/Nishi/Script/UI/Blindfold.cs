using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Blindfold : MonoBehaviour {

    public float mFadeInStartTime;
    public float mFadeInTime;
    public float mFadeOutStartTime;
    public float mFadeOutTime;

    // Use this for initialization
    void Start ()
    {
        RectTransform image = GetComponent<Image>().rectTransform;
        LeanTween.alpha(image, 1, mFadeInTime).setDelay(mFadeInStartTime)
        .setOnComplete(() => { LeanTween.alpha(image, 0, mFadeOutTime).setDelay(mFadeOutStartTime).setOnComplete(() => { Destroy(gameObject); }); });
	}
}
