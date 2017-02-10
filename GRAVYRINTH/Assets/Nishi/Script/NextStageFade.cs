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
    public System.Action mAction = ()=>{};
    Color mColor;

    public string mNextScene;

    FadeMode mState;
    bool isLoad;

    public bool mLastMode = false;
    public GameObject mlastmessage;
    public GameObject mCurrentMessage;

    public bool mButtonMode;

	// Use this for initialization
	void Start ()
    {
        GameManager.Instance.SetPausePossible(false);
        mCurrentMessage = null;
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
        //不透明であるなら
        if (mColor.a >= 1)
        {
            //シーンをロードした　かつ　シーンがロードされた
            if (isLoad && GameManager.Instance.isSceneload())
            {
                //最後のUI表示状態である
                if (mLastMode)
                {
                    if(!mCurrentMessage)
                    {
                        mCurrentMessage = (GameObject)Instantiate(mlastmessage,GameObject.Find("FadeCanvas").transform,false);
                        LeanTween.alpha(mCurrentMessage.GetComponent<Image>().rectTransform, 1.0f, 1.0f);
                    }
                    else if(Input.GetButtonDown("PS4_Circle") || Input.GetKeyDown(KeyCode.Return))
                    {
                        LeanTween.alpha(mCurrentMessage.GetComponent<Image>().rectTransform, 0.0f, 1.0f).setOnComplete(()=>
                        {
                            mLastMode = false;
                        }
                        );
                    }
                }
                else //UI表示モードでない
                {
                    //ボタンでフェードアウトにする状態である
                    if (mButtonMode)
                    {
                        var sync = GameObject.FindObjectOfType<UIandCameraSync>();
                        if (!sync) return;
                        sync.SetIsCameraEvent(true);
                        sync.SetCameraAction(() => 
                        {
                            GameManager.Instance.Reset();
                            mState = FadeMode.FadeOut;
                            mButtonMode = false;
                        });
                    }
                    else
                    {
                        GameManager.Instance.Reset();
                        mState = FadeMode.FadeOut;
                    }
                }

                Time.timeScale = 1.0f;
                mColor.a = 1;
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
            GameManager.Instance.SetPausePossible(true);
            mAction();
            mAction = () => { };
            Destroy(gameObject);
            Destroy(mCurrentMessage);
        }
    }
}
