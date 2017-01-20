using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextStageFade : MonoBehaviour {

    enum FadeMode
    {
        FadeIn,
        FadeOut
    }

    Image mImage;
    public float mSpeed;
    public System.Action mAction = ()=>{ };
    Color mColor;

    public string mNextScene;

    FadeMode mState;
    bool isLoad;

	// Use this for initialization
	void Start ()
    {
        isLoad = false;
        mState = FadeMode.FadeIn;
        mImage = GetComponent<Image>();
        mColor = mImage.color;
        mColor.a = 0;
        mImage.color = mColor;
    }
	
	// Update is called once per frame
	void Update ()
    {
        StateUpdate();
	}

    void StateUpdate()
    {
        switch(mState)
        {
            case FadeMode.FadeIn: FadeIn(); break;
            case FadeMode.FadeOut: FadeOut(); break;
        }
    }

    void FadeIn()
    {
        mColor.a += mSpeed * Time.deltaTime;
        mImage.color = mColor;
        if (mColor.a >= 1)
        {
            if (isLoad && GameManager.Instance.isSceneload())
            {
                GameManager.Instance.Reset();
                mState = FadeMode.FadeOut;
            }
            if (!isLoad)
            {
                GameManager.Instance.SceneChange();
                isLoad = true;
            }
        }

    }

    void FadeOut()
    {
        mColor.a -= mSpeed * Time.deltaTime;
        mImage.color = mColor;
        if (mColor.a <= 0)
        {
            Time.timeScale = 1.0f;
            Destroy(gameObject);
        }
    }
}
