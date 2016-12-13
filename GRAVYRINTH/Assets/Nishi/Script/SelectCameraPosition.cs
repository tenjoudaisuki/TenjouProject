using UnityEngine;
using System.Collections;
/// <summary>
/// ステージセレクト時カメラを固定位置に移動させる
/// </summary>
public class SelectCameraPosition : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        if (GameManager.Instance.GetMode() == GameManager.GameMode.Select)
        {
            GameObject.Find("Camera").GetComponent<CameraManager>().CameraWarp();
        }
	}
}
