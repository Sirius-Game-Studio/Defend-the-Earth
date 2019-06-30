using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float timeBetweenWaves = 5;
    [SerializeField] private GameObject[] enemies;

    [Header("UI")]
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text restartText;
    [SerializeField] private Text scoreText;

    [Header("Miscellanous")]
    public bool gameOver = false;

    private int score = 0;
    private int enemyAmount = 8;
    private bool loading = false;

    void Awake()
    {
        gameOver = false;
        StartCoroutine(spawnWave());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.R) && gameOver && !loading)
        {
            loading = true;
            SceneManager.LoadSceneAsync("SampleScene");
        }
        if (!gameOver)
        {
            gameOverText.enabled = false;
            restartText.enabled = false;
        } else
        {
            gameOverText.enabled = true;
            restartText.enabled = true;
        }
        if (scoreText) scoreText.text = "Score: " + score;
    }

    IEnumerator spawnWave()
    {
        while (!gameOver)
        {
            for (int i = 0; i < enemyAmount; i++)
            {
                if (!gameOver)
                {
                    Instantiate(enemies[Random.Range(0, enemies.Length)], new Vector3(Random.Range(-11, 11), 16, 0), Quaternion.Euler(90, 180, 0));
                    yield return new WaitForSeconds(1);
                } else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return new WaitForSeconds(timeBetweenWaves);
            if (gameOver) break;
        }
    }

    public void addScore(int value)
    {
        if (value > 0) score += value;
    }
}
