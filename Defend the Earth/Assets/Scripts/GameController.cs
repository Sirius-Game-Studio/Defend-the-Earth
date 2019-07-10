using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float enemySpawnTime = 2.5f;
    [SerializeField] private float asteroidSpawnTime = 4;
    [SerializeField] private float timeBetweenWaves = 5;
    [SerializeField] private float moneyTimer = 0;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] asteroids;

    [Header("UI")]
    [SerializeField] private Text gameOverText;
    [SerializeField] private Text restartText;
    [SerializeField] private Text waveText;
    [SerializeField] private Text scoreText;

    [Header("Miscellanous")]
    public bool gameOver = false;
    [SerializeField] private GameObject[] playerShips;

    private long wave = 1;
    private int score = 0;
    private int enemyAmount = 8;
    private bool loading = false;

    void Awake()
    {
        gameOver = false;
        if (PlayerPrefs.GetString("Spaceship") == "SpaceFighter")
        {
            Instantiate(playerShips[0], new Vector3(0, -7, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "AlienMower")
        {
            Instantiate(playerShips[1], new Vector3(0, -6.5f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "BlazingRocket")
        {
            Instantiate(playerShips[2], new Vector3(0, -6.75f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "HeavyCannon")
        {
            Instantiate(playerShips[3], new Vector3(0, -6, 0), Quaternion.Euler(0, 0, 0));
        } else
        {
            Instantiate(playerShips[0], new Vector3(0, -7, 0), Quaternion.Euler(-90, 0, 0));
        }
        StartCoroutine(spawnWaves());
        StartCoroutine(spawnAsteroids());
    }

    void Update()
    {
        if (gameOver && !loading)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                loading = true;
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            } else if (Input.GetKeyDown(KeyCode.Escape))
            {
                loading = true;
                SceneManager.LoadSceneAsync("Main Menu");
            }
        }
        if (!gameOver)
        {
            gameOverText.enabled = false;
            restartText.enabled = false;
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().UnPause();
        } else
        {
            gameOverText.enabled = true;
            restartText.enabled = true;
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();

        }
        if (waveText) waveText.text = "Wave: " + wave;
        if (scoreText) scoreText.text = "Score: " + score;
    }

    IEnumerator spawnWaves()
    {
        while (!gameOver)
        {
            for (int i = 0; i < enemyAmount; i++)
            {
                if (!gameOver)
                {
                    Instantiate(enemies[Random.Range(0, enemies.Length)], new Vector3(Random.Range(-11, 11), 16, 0), Quaternion.Euler(90, 180, 0));
                    yield return new WaitForSeconds(enemySpawnTime);
                } else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            if (!gameOver)
            {
                ++wave;
                ++enemyAmount;
                yield return new WaitForSeconds(timeBetweenWaves);
            } else
            {
                break;
            }
        }
    }

    IEnumerator spawnAsteroids()
    {
        while (!gameOver)
        {
            yield return new WaitForSeconds(asteroidSpawnTime);
            if (!gameOver) Instantiate(asteroids[Random.Range(0, asteroids.Length)], new Vector3(Random.Range(-11, 11), 9.5f, 0), Quaternion.Euler(0, 0, 0));
        }
    }

    public void addScore(int value)
    {
        if (value > 0) score += value;
    }
}
