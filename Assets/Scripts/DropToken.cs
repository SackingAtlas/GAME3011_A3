using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropToken : MonoBehaviour
{
    public Vector3 goTo;
    public float speed;
    [HideInInspector]public bool canDrop;
    private void Start()
    {
        goTo = GetComponent<GameToken>().currPos;
    }
    private void Update()
    {
        if (canDrop)
        {
            transform.position = Vector3.MoveTowards(transform.position, goTo, speed * Time.deltaTime);
            speed += speed * Time.deltaTime;
            if (transform.position == goTo)
                Destroy(this);
        }
    }
}
