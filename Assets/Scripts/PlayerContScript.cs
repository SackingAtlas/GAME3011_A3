using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContScript : MonoBehaviour
{
    //public GameObject skillMenu;
    public Vector3 currentPos;
    public Quaternion currentRot;
    public int speed;
    public bool pausedControls;
    private float vert, horiz;

    void Update()
    {
        if (!pausedControls)
        {
            vert = Input.GetAxisRaw("Vertical");
            horiz = Input.GetAxisRaw("Horizontal");
            transform.Translate(new Vector3(horiz, 0, vert) * speed * Time.deltaTime);
        }
    }
}

