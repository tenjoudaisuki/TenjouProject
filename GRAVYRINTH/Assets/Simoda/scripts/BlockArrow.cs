using UnityEngine;
using System.Collections;

public class BlockArrow : MonoBehaviour
{
    private Transform player;
    private Transform tr;
    private bool isPush;
    private string inputDirection;
    private GameObject forwardArrowStart;
    private GameObject backArrowStart;
    private GameObject forwardArrow;
    private GameObject backArrow;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        tr = gameObject.transform;
        forwardArrowStart = transform.FindChild("ForwardStart").gameObject;
        backArrowStart = transform.FindChild("BackStart").gameObject;
        forwardArrow = transform.FindChild("Forward").gameObject;
        backArrow = transform.FindChild("Back").gameObject;
        SetArrowsActive(false);
    }

    void Update()
    {
        tr.position = player.position + player.up * 0.67f;

        if (GameObject.FindGameObjectWithTag("Fade").transform.childCount != 0)
        {
            SetArrowsActive(false);
        }

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
            forwardArrowStart.SetActive(true);
            backArrowStart.SetActive(true);
            forwardArrow.SetActive(true);
            backArrow.SetActive(true);
        }
        else
        {
            forwardArrowStart.SetActive(false);
            backArrowStart.SetActive(false);
            forwardArrow.SetActive(false);
            backArrow.SetActive(false);
        }
    }
}
