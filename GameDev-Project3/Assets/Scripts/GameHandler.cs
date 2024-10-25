using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [Header("Collectibles")]
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalText;


    [Header("Assests")]
    public GameObject Player;
    public Canvas inGame;
    public Canvas endGameL;
    public Canvas endGameW;

    private void Start() {
        endGameL.enabled = false;
        endGameW.enabled = false;

        inGame.enabled = true;
    }
    private void Update() {
        if ((endGameL.enabled || endGameW.enabled) == true && Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene(0);
        }
    }


    public void addScore(int increase) {
        score += increase;
        scoreText.text = "Score: " + score.ToString();
    }

    public void gameOver(bool win) {
        Destroy(Player);
        finalText.text = "Final Score: " + score.ToString();
        inGame.enabled = false;
        if (win == false) {
            endGameL.enabled = true;
        }
        else if (win == true) {
            endGameW.enabled = true;
        }
    }
}
