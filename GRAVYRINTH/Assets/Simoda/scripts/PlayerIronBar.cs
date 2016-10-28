using UnityEngine;
using System.Collections;

public class PlayerIronBar : MonoBehaviour
{
    public enum BarType
    {
        IRON_BAR,
        POLE,
    }

    public Vector3 upVector;
    public bool touchIronBar = false;
    public GameObject ironBar;
    public Vector3 touchIronBarPosition;
    public Vector3 touchIronBarPlayerPosition;

    private BarType barType;


    void Start()
    {
        upVector = transform.up;
    }

    void Update()
    {
        upVector = transform.up;

        if (touchIronBar == true)
        {
            //transform.position = touchIronBarPlayerPosition;
            switch (barType)
            {
                case BarType.IRON_BAR:
                    Vector3 barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());
                    Vector3 axis = barVectorNor;
                    transform.RotateAround(touchIronBarPosition, Vector3.Normalize(axis), Input.GetAxis("Vertical") * 50.0f * Time.deltaTime);
                    break;
                case BarType.POLE:
                    break;
            }

        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "IronBar")
        {
            print("鉄棒と接触");
            touchIronBar = true;
            ironBar = collision.gameObject;
            touchIronBarPosition = collision.contacts[0].point;
            touchIronBarPlayerPosition = transform.position;

            Vector3 barVectorNor = Vector3.Normalize(ironBar.GetComponent<IronBar>().GetBarVector());
            print(Vector3.Dot(transform.up, barVectorNor));

            if (Vector3.Dot(transform.up, barVectorNor) > 0.7071068)
            {
                barType = BarType.IRON_BAR;
            }
            else
            {
                barType = BarType.POLE;
            }

            print(barType);


            GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl>().enabled = false;
            GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>().enabled = false;

            //print(touchIronBarPosition);
        }
    }
}
