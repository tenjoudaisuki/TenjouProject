using UnityEngine;
using System.Collections;

//大砲のある部屋に入っているかをチェックする
public class StagefBGMCollider : MonoBehaviour 
{
    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            GameObject.Find("BGMControl").GetComponent<BGMControl>().InTaihouRoom();
        }
    }
}
