using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDropper : MonoBehaviour
{
    private int row;
    private float timer = 0.5f;
    public float waitTime = 0.3f;
    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            GetComponent<GridBoard>().DropRow(row++);
            if (row == GetComponent<GridBoard>().height)
                Destroy(this);
            timer = waitTime;
        }
    }
}
