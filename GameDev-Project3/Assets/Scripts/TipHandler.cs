using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipHandler : MonoBehaviour
{
    public TextMeshProUGUI thisTip;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            thisTip.enabled = true;
            // Debug.Log("WORKS");

        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            thisTip.enabled = false;
            // Debug.Log("WORKS");

        }
    }
}
