using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    GridBoard gb;
    PlayerContScript ps;
    bool inRange, inGame;
    public GameObject gemGame;
    private void Start()
    {
        gb = GetComponentInChildren<GridBoard>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("Fire2") && inRange)
        {
            inGame = true;
            ps.pausedControls = true;
            gemGame.SetActive(true);
        }
        if (Input.GetButtonDown("Cancel") && inGame)
        {
            inGame = false;
            ps.pausedControls = false;
            gemGame.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = true;
            ps = other.GetComponent<PlayerContScript>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inRange = false;
        }
    }
}
