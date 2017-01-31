using UnityEngine;
using System.Collections;

public class BlockArrow : MonoBehaviour
{
    private Transform player;
    private Transform tr;
    private bool isPush;
    private string inputDirection;
    private GameObject forwardArrow;
    private GameObject backArrow;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        tr = gameObject.transform;
        forwardArrow = transform.FindChild("Forward").gameObject;
        backArrow = transform.FindChild("Back").gameObject;
        SetArrowsActive(true);
    }

    void Update()
    {
        tr.position = player.position + player.up * 0.67f;

        if (isPush == true)
            SetArrowsActive(true);
        else
            SetArrowsActive(false);

        if (inputDirection == "Horizontal")
            tr.rotation = Quaternion.LookRotation(player.up, player.forward);

        if (inputDirection == "Vertical")
            tr.rotation = Quaternion.LookRotation(player.right, player.forward);
    }

    public void SetInfo(bool isPush, string inputDirection)
    {
        this.isPush = isPush;
        this.inputDirection = inputDirection;
    }

    private void SetArrowsActive(bool active)
    {
        if (active == true)
        {
            forwardArrow.SetActive(true);
            backArrow.SetActive(true);
        }
        else
        {
            forwardArrow.SetActive(false);
            backArrow.SetActive(false);
        }
    }
}
