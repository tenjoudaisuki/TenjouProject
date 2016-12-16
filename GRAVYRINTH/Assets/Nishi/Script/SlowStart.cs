using UnityEngine;
using System.Collections;

public class SlowStart : MonoBehaviour
{

    public float speed = 0.1f;
    public float slowTime = 1f;

    public WallBreak Wall;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var state = other.gameObject.GetComponent<PlayerMoveManager>().GetState();
            if (state == PlayerState.STAGE_FINAL_CLEAR)
            {
                Time.timeScale = speed;
                Wall.Break();
                StartCoroutine(DelayMethod(slowTime * 60,() => { Time.timeScale = 1.0f; }));
            }
        }
    }

    /// <summary>
    /// 渡された処理を指定時間後に実行する
    /// </summary>
    /// <param name="delayFrameCount"></param>
    /// <param name="action">実行したい処理</param>
    /// <returns></returns>
    private IEnumerator DelayMethod(float delayFrameCount, System.Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }
        action();
    }
}
