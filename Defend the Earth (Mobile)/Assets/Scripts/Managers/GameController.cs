using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Analytics;
using UnityEditor;

struct PlayerPosition
{
    public Vector3 position;
    public Quaternion rotation;
}

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("Game Settings")]
    [SerializeField] private long maxWaves = 2;
    [SerializeField] private Vector2 enemySpawnTime = new Vector2(3.75f, 4);
    [SerializeField] private Vector2 asteroidSpawnTime = new Vector2(7.5f, 8);
    public int maxAliensReached = 15;
    [SerializeField] private float bossFinalYPosition = 4.5f;
    [SerializeField] private Vector3 bossRotation = new Vector3(90, 180, 0);
    [Tooltip("Leave blank to not have a boss in the last wave.")] [SerializeField] private GameObject boss = null;
    [Tooltip("The enemy spawn to limit.")] [SerializeField] private GameObject enemyToLimit = null;
    [SerializeField] private int limitedEnemySpawns = 2;

    [Header("UI")]
    [SerializeField] private Canvas gameHUD = null;
    [SerializeField] private Canvas gamePausedMenu = null;
    [SerializeField] private Canvas gameOverMenu = null;
    [SerializeField] private Canvas levelCompletedMenu = null;
    [SerializeField] private Canvas settingsMenu = null;
    [SerializeField] private Canvas quitGameMenu = null;
    [SerializeField] private Canvas restartPrompt = null;
    [SerializeField] private Canvas revivePrompt = null;
    public Text pauseButton = null;
    [SerializeField] private Text levelCount = null;
    [SerializeField] private Text scoreCount = null;
    [SerializeField] private Text waveCount = null;
    [SerializeField] private Text moneyCount = null;
    [SerializeField] private Text bossName = null;
    [SerializeField] private Slider bossHealthBar = null;
    [SerializeField] private Text bossHealthText = null;
    [SerializeField] private Text saveMeCountdown = null;
    [SerializeField] private Slider soundSlider = null;
    [SerializeField] private Slider musicSlider = null;
    [SerializeField] private Text newHighScoreText = null;
    [SerializeField] private Text deathMessage = null;
    [SerializeField] private GameObject loadingText = null;
    [SerializeField] private Slider loadingSlider = null;
    [SerializeField] private Text loadingPercentage = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;
    [SerializeField] private AudioClip loseJingle = null;
    [SerializeField] private AudioClip winJingle = null;

    [Header("Miscellanous")]
    public int enemiesLeft = 8;
    public bool isCampaignLevel = true;
    [SerializeField] private AudioClip[] randomMusic = new AudioClip[0];
    public int aliensReached = 0;
    public GameObject currentBoss;
    public string deathMessageToShow = "";
    public bool gameOver = false;
    public bool won = false;
    public bool paused = false;

    [Header("Setup")]
    [SerializeField] private GameObject[] playerShips = new GameObject[0];
    [SerializeField] private GameObject[] enemies = new GameObject[0];
    [Tooltip("Only used if enemyToLimit is set and the enemy spawns reach the limit.")] [SerializeField] private GameObject[] otherEnemies = new GameObject[0];
    [SerializeField] private GameObject[] asteroids = new GameObject[0];
    [SerializeField] private GameObject[] backgrounds = new GameObject[0];

    private AudioSource audioSource;
    private PlayerPosition playerPosition;
    private long wave = 1;
    private long score = 0;
    private long endlessMoneyReward = 150;
    private int enemyAmount = 0; //Stores the amount of enemies
    private long bossMaxHealth = 0; //If the value is above 0, the boss health bar's max value is not updated
    private bool reachedNextWave = false; //Checks if the player just reached the next wave, preventing wave skyrocketing
    private bool canWin = false; //Checks if the player is on the last wave, thus allowing the player to win
    private long endlessWavesClearedForEnemyAmount = 0;
    private long endlessWavesClearedForMoneyReward = 0;
    private bool canSetNewHighScore = false;
    private bool saveMeInProgress = false;
    private bool showRevivePrompt = true;
    private bool playedLoseSound = false, playedWinSound = false;
    private int clickSource = 1; //1 is game paused menu, 2 is game over menu, 3 is level completed menu
    private long storedMaxWaves = 2;
    private bool loading = false;

    //Analytics Events
    private bool sentGameOverData = false;
    private bool sentLevelCompleted = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 60;
        audioSource = GetComponent<AudioSource>();
        if (audioSource) audioSource.ignoreListenerPause = true;
        gameOver = false;
        won = false;
        paused = false;
        currentBoss = null;
        bossMaxHealth = 0;
        if (enemiesLeft < 6) enemiesLeft = 6; //Checks if the starting amount of enemies left is less than 6
        enemyAmount = enemiesLeft;
        aliensReached = 0;
        if (maxAliensReached < 7) maxAliensReached = 7; //Checks if maximum aliens reached is less than 7
        deathMessageToShow = "";
        storedMaxWaves = maxWaves;
        Time.timeScale = 1;
        AudioListener.pause = false;

        //Destroy all player, enemy and projectile objects in the scene
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player) Destroy(player);
        }
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy) Destroy(enemy);
        }
        foreach (GameObject projectile in GameObject.FindGameObjectsWithTag("Projectile"))
        {
            if (projectile) Destroy(projectile);
        }

        //If backgrounds array's length is more than 0, destroy all background objects in the scene
        if (backgrounds.Length > 0)
        {
            foreach (GameObject background in GameObject.FindGameObjectsWithTag("Background"))
            {
                if (background) Destroy(background);
            }
        }

        if (!PlayerPrefs.HasKey("Difficulty")) //Sets the difficulty to Normal if no difficulty key is found
        {
            PlayerPrefs.SetInt("Difficulty", 2);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetInt("Difficulty") <= 1) //Easy
        {
            enemySpawnTime *= 0.95f;
            maxAliensReached += 2;
        } else if (PlayerPrefs.GetInt("Difficulty") == 3) //Hard
        {
            asteroidSpawnTime *= 0.95f;
            maxAliensReached -= 1;
        } else if (PlayerPrefs.GetInt("Difficulty") >= 4) //Nightmare
        {
            asteroidSpawnTime *= 0.85f;
            enemyAmount += 1;
            maxAliensReached -= 2;
        }
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            soundSlider.value = getVolumeData(true);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = getVolumeData(false);
            musicSlider.value = getVolumeData(false);
        }
        if (Camera.main.GetComponent<AudioSource>())
        {
            Camera.main.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
            Camera.main.GetComponent<AudioSource>().Play();
        }
        GameObject playerShip;
        if (PlayerPrefs.GetString("Spaceship") == "SpaceFighter")
        {
            playerShip = Instantiate(playerShips[0], new Vector3(0, -7, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "AlienMower")
        {
            playerShip = Instantiate(playerShips[1], new Vector3(0, -6.5f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "BlazingRocket")
        {
            playerShip = Instantiate(playerShips[2], new Vector3(0, -6.75f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "QuadShooter")
        {
            playerShip = Instantiate(playerShips[3], new Vector3(0, -6.75f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "PointVoidBreaker")
        {
            playerShip = Instantiate(playerShips[4], new Vector3(0, -6.5f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "Annihilator")
        {
            playerShip = Instantiate(playerShips[5], new Vector3(0, -6.5f, 0), Quaternion.Euler(-90, 0, 0));
        } else //Instantiates Space Fighter if the currently used spaceship is invalid
        {
            PlayerPrefs.SetString("Spaceship", "SpaceFighter");
            PlayerPrefs.Save();
            playerShip = Instantiate(playerShips[0], new Vector3(0, -7, 0), Quaternion.Euler(-90, 0, 0));
        }
        playerPosition.position = playerShip.transform.position;
        playerPosition.rotation = playerShip.transform.rotation;
        if (backgrounds.Length > 0) Instantiate(backgrounds[Random.Range(0, backgrounds.Length)], new Vector3(0, 0, 5), Quaternion.Euler(0, 0, 0));
        if (Camera.main.GetComponent<AudioSource>() && randomMusic.Length > 0)
        {
            Camera.main.GetComponent<AudioSource>().clip = randomMusic[Random.Range(0, randomMusic.Length)];
            Camera.main.GetComponent<AudioSource>().loop = true;
            Camera.main.GetComponent<AudioSource>().Stop();
            Camera.main.GetComponent<AudioSource>().Play();
        }
        gameHUD.enabled = true;
        gamePausedMenu.enabled = false;
        gameOverMenu.enabled = false;
        levelCompletedMenu.enabled = false;
        settingsMenu.enabled = false;   
        quitGameMenu.enabled = false;
        restartPrompt.enabled = false;
        revivePrompt.enabled = false;
        newHighScoreText.enabled = false;
        saveMeCountdown.text = "";
        pauseButton.gameObject.SetActive(true);
        pauseButton.color = pauseButton.GetComponent<ButtonHover>().normalColor;
        StartCoroutine(spawnWaves());
        StartCoroutine(spawnAsteroids());
        AnalyticsEvent.LevelStart(SceneManager.GetActiveScene().name, new Dictionary<string, object>());
    }

    void Update()
    {
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().volume = getVolumeData(false);
        if (Input.GetKeyDown(KeyCode.Escape)) pause(false);
        if (Input.GetKeyDown(KeyCode.Escape) && paused)
        {
            if (settingsMenu.enabled)
            {
                settingsMenu.enabled = false;
                if (clickSource <= 1)
                {
                    gamePausedMenu.enabled = true;
                } else if (clickSource == 2)
                {
                    gameOverMenu.enabled = true;
                } else if (clickSource >= 3)
                {
                    levelCompletedMenu.enabled = true;
                }
            } else if (quitGameMenu.enabled)
            {
                quitGameMenu.enabled = false;
                if (clickSource <= 1)
                {
                    gamePausedMenu.enabled = true;
                } else if (clickSource == 2)
                {
                    gameOverMenu.enabled = true;
                } else if (clickSource >= 3)
                {
                    levelCompletedMenu.enabled = true;
                }
            } else if (restartPrompt.enabled)
            {
                restartPrompt.enabled = false;
                if (clickSource <= 1)
                {
                    gamePausedMenu.enabled = true;
                } else if (clickSource == 2)
                {
                    gameOverMenu.enabled = true;
                } else if (clickSource >= 3)
                {
                    levelCompletedMenu.enabled = true;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape) && revivePrompt.enabled) closeRevivePrompt();
        if (storedMaxWaves > 2)
        {
            maxWaves = storedMaxWaves;
        } else
        {
            maxWaves = 2;
        }
        if (!gameOver && aliensReached >= maxAliensReached)
        {
            gameOver = true;
            showRevivePrompt = false;
            revivePrompt.enabled = false;
            deathMessageToShow = "You failed to protect the Earth!";
        }
        if (gameOver)
        {
            clickSource = 2;
            if (showRevivePrompt)
            {
                showRevivePrompt = false;
                revivePrompt.enabled = true;
            }
            if (!quitGameMenu.enabled && !revivePrompt.enabled && !saveMeInProgress && !loading) gameOverMenu.enabled = true;
            pauseButton.gameObject.SetActive(false);
            pauseButton.color = pauseButton.GetComponent<ButtonHover>().normalColor;
            if (canSetNewHighScore)
            {
                canSetNewHighScore = false;
                if (!PlayerPrefs.HasKey("HighScore") && score > 0)
                {
                    PlayerPrefs.SetString("HighScore", score.ToString());
                    StartCoroutine(showNewHighScore());
                } else if (PlayerPrefs.HasKey("HighScore") && score > long.Parse(PlayerPrefs.GetString("HighScore")))
                {
                    PlayerPrefs.SetString("HighScore", score.ToString());
                    StartCoroutine(showNewHighScore());
                }
                PlayerPrefs.Save();
            }
            if (audioSource && loseJingle && !playedLoseSound)
            {
                playedLoseSound = true;
                audioSource.PlayOneShot(loseJingle, getVolumeData(true));
            }
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            if (!sentGameOverData)
            {
                sentGameOverData = true;
                if (isCampaignLevel)
                {
                    string currentBossName = "None";
                    if (currentBoss)
                    {
                        currentBossName = currentBoss.name;
                    } else
                    {
                        currentBossName = "None";
                    }
                    AnalyticsEvent.Custom("campaign_game_over", new Dictionary<string, object>
                    {
                        {"level_name", SceneManager.GetActiveScene().name},
                        {"wave", wave},
                        {"enemy_amount", enemyAmount},
                        {"boss", currentBossName}
                    });
                } else
                {
                    AnalyticsEvent.Custom("endless_game_over", new Dictionary<string, object>
                    {
                        {"level_name", SceneManager.GetActiveScene().name},
                        {"score", score},
                        {"wave", wave},
                        {"enemy_amount", enemyAmount},
                        {"enemy_spawn_time", enemySpawnTime},
                        {"asteroid_spawn_time", asteroidSpawnTime},
                        {"money_reward", endlessMoneyReward}
                    });
                }
            }
        }
        if (isCampaignLevel)
        {
            if (!gameOver && !won && enemiesLeft <= 0)
            {
                if (wave < maxWaves && !canWin)
                {
                    if (!reachedNextWave)
                    {
                        reachedNextWave = true;
                        if (wave < maxWaves + 1) ++wave;
                    }
                } else if (wave >= maxWaves && canWin)
                {
                    won = true;
                    clickSource = 3;
                    if (PlayerPrefs.GetInt("Level") < PlayerPrefs.GetInt("MaxLevels"))
                    {
                        if (!loading && !quitGameMenu.enabled) levelCompletedMenu.enabled = true;
                        if (!PlayerPrefs.HasKey("Restarted"))
                        {
                            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
                            PlayerPrefs.Save();
                        }
                        if (audioSource && winJingle && !playedWinSound)
                        {
                            playedWinSound = true;
                            audioSource.PlayOneShot(winJingle, getVolumeData(true));
                        }
                        if (!sentLevelCompleted)
                        {
                            sentLevelCompleted = true;
                            AnalyticsEvent.LevelComplete(SceneManager.GetActiveScene().name, new Dictionary<string, object>{});
                        }
                    } else
                    {
                        if (!loading)
                        {
                            StartCoroutine(loadScene("Ending"));
                            if (!sentLevelCompleted)
                            {
                                sentLevelCompleted = true;
                                AnalyticsEvent.LevelComplete("Campaign", new Dictionary<string, object>{});
                            }
                        }
                    }
                    if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
                }
            }
        } else
        {
            if (!gameOver && enemiesLeft <= 0 && !reachedNextWave)
            {
                reachedNextWave = true;
                ++wave;
                ++endlessWavesClearedForEnemyAmount;
                ++endlessWavesClearedForMoneyReward;
            }
            if (endlessWavesClearedForEnemyAmount >= 2)
            {
                endlessWavesClearedForEnemyAmount = 0;
                if (enemyAmount < 25) enemyAmount += 1;
                if (enemySpawnTime.x > 2.75f) enemySpawnTime *= 0.98f;
                if (asteroidSpawnTime.x > 5) asteroidSpawnTime *= 0.98f;
                if (maxAliensReached > 14) --maxAliensReached;
            }
            if (endlessWavesClearedForMoneyReward >= 5)
            {
                endlessWavesClearedForMoneyReward = 0;
                long money = long.Parse(PlayerPrefs.GetString("Money"));
                money += endlessMoneyReward;
                PlayerPrefs.SetString("Money", money.ToString());
                PlayerPrefs.Save();
                endlessMoneyReward += 25;
            }
        }

        //Updates volume data to match the slider values
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();

        if (isCampaignLevel)
        {
            levelCount.transform.parent.gameObject.SetActive(true);
            scoreCount.transform.parent.gameObject.SetActive(false);
            if (PlayerPrefs.GetInt("Level") > 0)
            {
                levelCount.text = PlayerPrefs.GetInt("Level").ToString();
            } else
            {
                levelCount.text = "1";
            }
            waveCount.text = wave + "/" + maxWaves;
        } else
        {
            levelCount.transform.parent.gameObject.SetActive(false);
            scoreCount.transform.parent.gameObject.SetActive(true);
            scoreCount.text = score.ToString();
            waveCount.text = wave.ToString();
        }
        if (PlayerPrefs.GetString("Money") != "")
        {
            moneyCount.text = "$" + PlayerPrefs.GetString("Money");
        } else
        {
            moneyCount.text = "$0";
        }
        if (!currentBoss)
        {
            currentBoss = null;
            bossMaxHealth = 0;
            bossName.gameObject.SetActive(false);
        } else
        {
            if (bossMaxHealth <= 0) bossMaxHealth = currentBoss.GetComponent<EnemyHealth>().health;
            bossName.gameObject.SetActive(true);
            bossName.text = currentBoss.name;
            bossHealthBar.value = currentBoss.GetComponent<EnemyHealth>().health;
            bossHealthBar.maxValue = bossMaxHealth;
            bossHealthText.text = bossHealthBar.value + " / " + bossHealthBar.maxValue;
        }
        deathMessage.text = deathMessageToShow;
        if (!loading)
        {
            Camera.main.transform.position = new Vector3(0, 0, -10);
            foreach (GameObject background in GameObject.FindGameObjectsWithTag("Background"))
            {
                if (background)
                {
                    background.transform.position = new Vector3(0, background.transform.position.y, 5);
                    background.GetComponent<BackgroundScroll>().enabled = true;
                }
            }
            loadingText.SetActive(false);
        } else
        {
            Camera.main.transform.position = new Vector3(500, 0, -10);
            foreach (GameObject background in GameObject.FindGameObjectsWithTag("Background"))
            {
                if (background)
                {
                    background.transform.position = new Vector3(500, background.transform.position.y, 5);
                    background.GetComponent<BackgroundScroll>().enabled = false;
                }
            }
            loadingText.SetActive(true);
        }

        //Checks if the player upgrades are above maximum values
        if (PlayerPrefs.GetFloat("DamageMultiplier") > 1.5f)
        {
            PlayerPrefs.SetFloat("DamageMultiplier", 1.5f);
            PlayerPrefs.SetInt("DamagePercentage", 50);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("SpeedMultiplier") > 1.2f)
        {
            PlayerPrefs.SetFloat("SpeedMultiplier", 1.2f);
            PlayerPrefs.SetInt("SpeedPercentage", 20);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("HealthMultiplier") > 2)
        {
            PlayerPrefs.SetFloat("HealthMultiplier", 2);
            PlayerPrefs.SetInt("HealthPercentage", 100);
            PlayerPrefs.Save();
        }
        if (PlayerPrefs.GetFloat("MoneyMultiplier") > 3)
        {
            PlayerPrefs.SetFloat("MoneyMultiplier", 3);
            PlayerPrefs.SetInt("MoneyPercentage", 200);
            PlayerPrefs.Save();
        }

        //Checks if money is below 0
        if (long.Parse(PlayerPrefs.GetString("Money")) < 0)
        {
            PlayerPrefs.SetString("Money", "0");
            PlayerPrefs.Save();
        }

        if (wave > maxWaves) wave = maxWaves; //Checks if current wave is above max waves
        if (maxAliensReached < 5) maxAliensReached = 5; //Checks if maximum aliens reached is less than 5
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("Difficulty");
        PlayerPrefs.DeleteKey("Restarted");
    }

    IEnumerator spawnWaves()
    {
        while (!gameOver && !won && wave < maxWaves + 1)
        {
            if (!gameOver && !won)
            {
                Vector3 left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
                Vector3 right = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
                if (enemiesLeft > 0)
                {
                    if (!gameOver && !won && !paused)
                    {
                        yield return new WaitForSeconds(Random.Range(enemySpawnTime.x, enemySpawnTime.y));
                        if (!enemyToLimit)
                        {
                            if (isCampaignLevel)
                            {
                                Instantiate(enemies[Random.Range(0, enemies.Length)], new Vector3(Random.Range(left.x, right.x), 16, 0), Quaternion.Euler(90, 180, 0));
                            } else
                            {
                                if (wave < 6)
                                {
                                    Instantiate(enemies[Random.Range(0, enemies.Length - 1)], new Vector3(Random.Range(left.x, right.x), 16, 0), Quaternion.Euler(90, 180, 0));
                                } else
                                {
                                    Instantiate(enemies[Random.Range(0, enemies.Length)], new Vector3(Random.Range(left.x, right.x), 16, 0), Quaternion.Euler(90, 180, 0));
                                }
                            }
                        } else
                        {
                            int foundEnemies = 0;
                            GameObject[] enemyArray = GameObject.FindGameObjectsWithTag("Enemy");
                            for (int i = 0; i < enemyArray.Length; i++)
                            {
                                if (enemyArray[i].CompareTag("Enemy") && enemyArray[i].name == enemyToLimit.name + "(Clone)")
                                {
                                    ++foundEnemies;
                                }
                            }
                            if (foundEnemies < limitedEnemySpawns)
                            {
                                if (isCampaignLevel)
                                {
                                    Instantiate(enemies[Random.Range(0, enemies.Length)], new Vector3(Random.Range(left.x, right.x), 16, 0), Quaternion.Euler(90, 180, 0));
                                } else
                                {
                                    if (wave < 6)
                                    {
                                        Instantiate(enemies[Random.Range(0, enemies.Length - 1)], new Vector3(Random.Range(left.x, right.x), 16, 0), Quaternion.Euler(90, 180, 0));
                                    } else
                                    {
                                        Instantiate(enemies[Random.Range(0, enemies.Length)], new Vector3(Random.Range(left.x, right.x), 16, 0), Quaternion.Euler(90, 180, 0));
                                    }
                                }
                            } else
                            {
                                Instantiate(otherEnemies[Random.Range(0, otherEnemies.Length)], new Vector3(Random.Range(left.x, right.x), 16, 0), Quaternion.Euler(90, 180, 0));
                            }
                        }
                    }
                } else
                {
                    if (isCampaignLevel)
                    {
                        if (!boss)
                        {
                            yield return new WaitForSeconds(3);
                            if (!gameOver && !won && !paused)
                            {
                                if (wave >= maxWaves) canWin = true;
                                enemiesLeft = enemyAmount;
                                reachedNextWave = false;
                            }
                        } else
                        {
                            if (wave < maxWaves)
                            {
                                yield return new WaitForSeconds(3);
                                if (!gameOver && !won && !paused)
                                {
                                    if (wave >= maxWaves) canWin = true;
                                    enemiesLeft = enemyAmount;
                                    reachedNextWave = false;
                                }
                            } else
                            {
                                if (wave < maxWaves)
                                {
                                    yield return new WaitForSeconds(3);
                                    if (!gameOver && !won && !paused)
                                    {
                                        if (wave >= maxWaves) canWin = true;
                                        enemiesLeft = enemyAmount;
                                        reachedNextWave = false;
                                    }
                                } else
                                {
                                    yield return new WaitForSeconds(3);
                                    if (!gameOver && !won && !paused)
                                    {
                                        GameObject enemy = Instantiate(boss, new Vector3(0, 16, 0), Quaternion.Euler(bossRotation.x, bossRotation.y, bossRotation.z));
                                        enemy.name = boss.name;
                                        currentBoss = enemy;
                                        StartCoroutine(scrollEnemy(enemy, bossFinalYPosition));
                                        enemiesLeft = 1;
                                        reachedNextWave = false;
                                        if (wave >= maxWaves) canWin = true;
                                    }
                                    yield break;
                                }
                            }
                        }
                    } else
                    {
                        yield return new WaitForSeconds(3);
                        enemiesLeft = enemyAmount;
                        reachedNextWave = false;
                    }
                }
            } else
            {
                yield return null;
            }
        }
    }

    IEnumerator spawnAsteroids()
    {
        while (!gameOver && !won)
        {
            if (!gameOver && !won)
            {
                yield return new WaitForSeconds(Random.Range(asteroidSpawnTime.x, asteroidSpawnTime.y));
                if (!currentBoss && !gameOver && !won && !paused)
                {
                    Vector3 left = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
                    Vector3 right = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));
                    Instantiate(asteroids[Random.Range(0, asteroids.Length)], new Vector3(Random.Range(left.x, right.x), 9.5f, 0), Quaternion.Euler(0, 0, 0));
                }
            } else
            {
                yield return null;
            }
        }
    }

    public void addScore(long newScore)
    {
        if (!isCampaignLevel && !gameOver && newScore > 0) score += newScore;
    }

    IEnumerator scrollEnemy(GameObject enemy, float y)
    {
        if (enemy && enemy.CompareTag("Enemy") && enemy.GetComponent<Mover>() && y > 0)
        {
            while (enemy && enemy.transform.position.y > y)
            {
                enemy.GetComponent<Mover>().enabled = true;
                if (enemy.GetComponent<HorizontalOnlyMover>()) enemy.GetComponent<HorizontalOnlyMover>().enabled = false;
                yield return new WaitForEndOfFrame();
            }
            if (enemy)
            {
                enemy.GetComponent<Mover>().enabled = false;
                if (enemy.GetComponent<HorizontalOnlyMover>()) enemy.GetComponent<HorizontalOnlyMover>().enabled = true;
            }
        }
    }

    public void startSaveMe()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        saveMeInProgress = true;
        showRevivePrompt = false;
        saveMeCountdown.text = "3";
        revivePrompt.enabled = false;
        gameOverMenu.enabled = false;
        StopCoroutine(spawnWaves());
        StopCoroutine(spawnAsteroids());
        StartCoroutine(saveMe());
    }

    IEnumerator saveMe()
    {
        GameObject playerShip;
        int t = 3;
        while (t > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            --t;
            saveMeCountdown.text = t.ToString();
        }
        Time.timeScale = 1;
        AudioListener.pause = false;
        gameOver = false;
        saveMeInProgress = false;
        playedLoseSound = false;
        deathMessageToShow = "";
        clickSource = 1;
        if (PlayerPrefs.GetString("Spaceship") == "SpaceFighter")
        {
            playerShip = Instantiate(playerShips[0], playerPosition.position, playerPosition.rotation);
        } else if (PlayerPrefs.GetString("Spaceship") == "AlienMower")
        {
            playerShip = Instantiate(playerShips[1], playerPosition.position, playerPosition.rotation);
        } else if (PlayerPrefs.GetString("Spaceship") == "BlazingRocket")
        {
            playerShip = Instantiate(playerShips[2], playerPosition.position, playerPosition.rotation);
        } else if (PlayerPrefs.GetString("Spaceship") == "QuadShooter")
        {
            playerShip = Instantiate(playerShips[3], playerPosition.position, playerPosition.rotation);
        } else if (PlayerPrefs.GetString("Spaceship") == "PointVoidBreaker")
        {
            playerShip = Instantiate(playerShips[4], playerPosition.position, playerPosition.rotation);
        } else if (PlayerPrefs.GetString("Spaceship") == "Annihilator")
        {
            playerShip = Instantiate(playerShips[5], playerPosition.position, playerPosition.rotation);
        } else //Instantiates Space Fighter if the currently used spaceship is invalid
        {
            PlayerPrefs.SetString("Spaceship", "SpaceFighter");
            PlayerPrefs.Save();
            playerShip = Instantiate(playerShips[0], playerPosition.position, playerPosition.rotation);
        }
        playerShip.GetComponent<PlayerController>().startInvulnerability(5);
        StartCoroutine(setupPlayerOnRevive(playerShip.GetComponent<PlayerController>()));
        saveMeCountdown.text = "";
        pauseButton.gameObject.SetActive(true);
        pauseButton.color = pauseButton.GetComponent<ButtonHover>().normalColor;
        if (!currentBoss) StartCoroutine(spawnWaves());
        StartCoroutine(spawnAsteroids());
        canSetNewHighScore = true;
        if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Play();
    }

    IEnumerator setupPlayerOnRevive(PlayerController playerController)
    {
        yield return new WaitForEndOfFrame();
        if (playerController)
        {
            playerController.health = (long)(playerController.health * 0.66);
            playerController.lives = 1;
        }
    }

    IEnumerator showNewHighScore()
    {
        int a = 0;
        while (a < 6)
        {
            newHighScoreText.enabled = true;
            ++a;
            yield return new WaitForSeconds(0.5f);
            newHighScoreText.enabled = false;
            ++a;
            yield return new WaitForSeconds(0.5f);
        }
        newHighScoreText.enabled = false;
    }

    public void pause(bool clicked)
    {
        if (!gameOver && !won && !gameOverMenu.enabled && !levelCompletedMenu.enabled)
        {
            if (clicked && audioSource)
            {
                if (buttonClick)
                {
                    audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                } else
                {
                    audioSource.volume = getVolumeData(true);
                    audioSource.Play();
                }
            }
            if (!paused) //Pauses the game
            {
                clickSource = 1;
                paused = true;
                Time.timeScale = 0;
                AudioListener.pause = true;
                gamePausedMenu.enabled = true;
                pauseButton.gameObject.SetActive(false);
                pauseButton.color = pauseButton.GetComponent<ButtonHover>().normalColor;
            } else //Unpauses the game
            {
                if (!settingsMenu.enabled && !quitGameMenu.enabled && !restartPrompt.enabled)
                {
                    paused = false;
                    Time.timeScale = 1;
                    AudioListener.pause = false;
                    gamePausedMenu.enabled = false;
                    pauseButton.gameObject.SetActive(true);
                    pauseButton.color = pauseButton.GetComponent<ButtonHover>().normalColor;
                }
            }
        }
    }

    public void resumeGame()
    {
        if (!settingsMenu.enabled && !quitGameMenu.enabled)
        {
            if (audioSource)
            {
                if (buttonClick)
                {
                    audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                } else
                {
                    audioSource.volume = getVolumeData(true);
                    audioSource.Play();
                }
            }
            paused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
            gamePausedMenu.enabled = false;
            pauseButton.gameObject.SetActive(true);
            pauseButton.color = pauseButton.GetComponent<ButtonHover>().normalColor;
        }
    }

    public void restart()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        StartCoroutine(loadScene("Level " + PlayerPrefs.GetInt("IngameLevel")));
        PlayerPrefs.SetInt("Restarted", 1);
        PlayerPrefs.Save();
    }

    public void exitGame()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        Application.Quit();
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #endif
    }

    public void exitToMainMenu()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        StartCoroutine(loadScene("Main Menu"));
    }

    public void toNextLevel()
    {
        if (won && levelCompletedMenu.enabled)
        {
            int level = PlayerPrefs.GetInt("Level");
            if (audioSource)
            {
                if (buttonClick)
                {
                    audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                } else
                {
                    audioSource.volume = getVolumeData(true);
                    audioSource.Play();
                }
            }
            if (PlayerPrefs.GetInt("Level") < PlayerPrefs.GetInt("MaxLevels"))
            {
                StartCoroutine(loadScene("Level " + PlayerPrefs.GetInt("Level")));
            } else
            {
                StartCoroutine(loadScene("Ending"));
            }
            PlayerPrefs.DeleteKey("Restarted");
            PlayerPrefs.DeleteKey("WatchedAd");
        }
    }

    public void openCanvasFromClickSource(Canvas canvas)
    {
        if (canvas)
        {
            if (audioSource)
            {
                if (buttonClick)
                {
                    audioSource.PlayOneShot(buttonClick, getVolumeData(true));
                } else
                {
                    audioSource.volume = getVolumeData(true);
                    audioSource.Play();
                }
            }
            if (!canvas.enabled)
            {
                canvas.enabled = true;
                if (clickSource <= 1)
                {
                    gamePausedMenu.enabled = false;
                } else if (clickSource == 2)
                {
                    gameOverMenu.enabled = false;
                } else if (clickSource >= 3)
                {
                    levelCompletedMenu.enabled = false;
                }
            } else
            {
                canvas.enabled = false;
                if (clickSource <= 1)
                {
                    gamePausedMenu.enabled = true;
                } else if (clickSource == 2)
                {
                    gameOverMenu.enabled = true;
                } else if (clickSource >= 3)
                {
                    levelCompletedMenu.enabled = true;
                }
            }
        }
    }

    public void closeRevivePrompt()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick, getVolumeData(true));
            } else
            {
                audioSource.volume = getVolumeData(true);
                audioSource.Play();
            }
        }
        showRevivePrompt = false;
        revivePrompt.enabled = false;
        if (clickSource <= 1)
        {
            gamePausedMenu.enabled = true;
        } else if (clickSource == 2)
        {
            gameOverMenu.enabled = true;
        } else if (clickSource >= 3)
        {
            levelCompletedMenu.enabled = true;
        }
        canSetNewHighScore = true;
    }

    public void updatePlayerPosition(Vector3 newPosition, Quaternion newRotation)
    {
        playerPosition.position = newPosition;
        playerPosition.rotation = newRotation;
    }

    float getVolumeData(bool isSound)
    {
        float volume = 1;
        if (isSound)
        {
            if (PlayerPrefs.HasKey("SoundVolume")) volume = PlayerPrefs.GetFloat("SoundVolume");
        } else
        {
            if (PlayerPrefs.HasKey("MusicVolume")) volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        return volume;
    }

    IEnumerator loadScene(string scene)
    {
        if (!loading)
        {
            loading = true;
            AsyncOperation load = SceneManager.LoadSceneAsync(scene);
            while (!load.isDone)
            {
                loadingSlider.value = load.progress;
                loadingPercentage.text = Mathf.Floor(load.progress * 100) + "%";
                gameHUD.enabled = false;
                gamePausedMenu.enabled = false;
                gameOverMenu.enabled = false;
                levelCompletedMenu.enabled = false;
                settingsMenu.enabled = false;
                quitGameMenu.enabled = false;
                restartPrompt.enabled = false;
                yield return null;
            }
            loading = false;
            loadingSlider.value = 0;
            loadingPercentage.text = "0%";
        } else
        {
            StopCoroutine(loadScene(scene));
        }
    }
}
