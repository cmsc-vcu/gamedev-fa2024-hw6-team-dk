using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public GameHandler handler;
    void OnTriggerEnter2D(Collider2D other) {
       if (other.CompareTag("Player")) {
            handler.gameOver();
            // Debug.Log("WORKS");

        }
    }
}
