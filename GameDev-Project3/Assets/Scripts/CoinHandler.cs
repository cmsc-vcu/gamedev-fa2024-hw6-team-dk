using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinHandler : MonoBehaviour
{
    public GameObject handler;

    void Start() {
    }
    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("in");

        if (other.CompareTag("Player")) {
            handler.GetComponent<GameHandler>().addScore(1);
            Destroy(gameObject);
        }
    }
}
