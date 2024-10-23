using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
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
    public Canvas endGame;

    private void Start() {
        endGame.enabled = false;
        inGame.enabled = true;
    }
    private void Update() {
        if (endGame.enabled == true && Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene(0);
        }
    }


    public void addScore(int increase) {
        score += increase;
        scoreText.text = "Score: " + score.ToString();
    }

    public void gameOver() {
        Destroy(Player);
        finalText.text = "Final Score: " + score.ToString();
        inGame.enabled = false;
        endGame.enabled = true;
    }
}
