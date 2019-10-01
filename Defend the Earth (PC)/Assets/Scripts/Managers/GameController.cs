using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [Header("Game Settings")]
    [SerializeField] private long maxWaves = 2;
    public int enemiesLeft = 8;
    [SerializeField] private int maxAliensReached = 15;
    [SerializeField] private Vector2 enemySpawnTime = new Vector2(3.75f, 4);
    [SerializeField] private Vector2 asteroidSpawnTime = new Vector2(7.5f, 8);
    public float bossInitialYPosition = 16;
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
    [SerializeField] private Text levelCount = null;
    [SerializeField] private Text scoreCount = null;
    [SerializeField] private Text waveCount = null;
    [SerializeField] private Text moneyCount = null;
    [SerializeField] private Text bossName = null;
    [SerializeField] private Slider bossHealthBar = null;
    [SerializeField] private Text bossHealthText = null;
    [SerializeField] private Text newHighScoreText = null;
    [SerializeField] private Text deathMessage = null;
    public RectTransform controllerShootIcon = null;
    [SerializeField] private GameObject loadingScreen = null;
    [SerializeField] private Slider loadingSlider = null;
    [SerializeField] private Text loadingPercentage = null;
    [SerializeField] private GameObject anyKeyPrompt = null;
    [SerializeField] private Text loadingTip = null;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClick = null;
    [SerializeField] private AudioClip loseJingle = null;
    [SerializeField] private AudioClip winJingle = null;

    [Header("Miscellaneous")]
    public bool isCampaignLevel = true;
    [SerializeField] private AudioClip[] randomMusic = new AudioClip[0];
    [Tooltip("The amount of enemies that reached the bottom.")] public int aliensReached = 0;
    public GameObject currentBoss;
    [Tooltip("Used for handling consecutive enemy health and damage increases in Endless.")] public long wavesCleared = 0;
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
    [SerializeField] private AudioMixer audioMixer = null;

    private AudioSource audioSource;
    private Controls input;
    private long wave = 1;
    private long score = 0;
    private long endlessMoneyReward = 150;
    private int enemyAmount = 0; //Stores the amount of enemies
    private long bossMaxHealth = 0; //If the value is above 0, the boss health bar's max value is not updated
    private bool reachedNextWave = false; //Checks if the player just reached the next wave, preventing wave skyrocketing
    private bool canWin = false; //Checks if the player is on the last wave, thus allowing the player to win
    private long endlessWavesClearedForEnemyAmount = 0;
    private long endlessWavesClearedForMoneyReward = 0;
    private bool canSetNewHighScore = true;
    private bool playedLoseSound = false, playedWinSound = false;
    private int clickSource = 1; //1 is game paused menu, 2 is game over menu, 3 is level completed menu
    private long storedMaxWaves = 2;
    private string currentLoadingTip = "";
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
        audioSource = GetComponent<AudioSource>();
        input = new Controls();
        if (audioSource) audioSource.ignoreListenerPause = true;
        if (maxWaves < 2) maxWaves = 2;
        gameOver = false;
        won = false;
        paused = false;
        currentBoss = null;
        bossMaxHealth = 0;
        if (enemiesLeft < 6) enemiesLeft = 6; //Checks if the starting amount of enemies left is less than 6
        enemyAmount = enemiesLeft;
        aliensReached = 0;
        if (maxAliensReached < 7) maxAliensReached = 7; //Checks if maximum aliens reached is less than 7
        wavesCleared = 0;
        deathMessageToShow = "";
        storedMaxWaves = maxWaves;
        currentLoadingTip = "";
        loading = false;
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
            maxAliensReached -= 1;
        }
        if (!PlayerPrefs.HasKey("SoundVolume"))
        {
            PlayerPrefs.SetFloat("SoundVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            audioMixer.SetFloat("SoundVolume", Mathf.Log10(PlayerPrefs.GetFloat("SoundVolume")) * 20);
        }
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.Save();
        } else
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume")) * 20);
        }
        if (PlayerPrefs.GetString("Spaceship") == "SpaceFighter")
        {
            Instantiate(playerShips[0], new Vector3(0, -7, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "AlienMower")
        {
            Instantiate(playerShips[1], new Vector3(0, -6.5f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "BlazingRocket")
        {
            Instantiate(playerShips[2], new Vector3(0, -6.75f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "QuadShooter")
        {
            Instantiate(playerShips[3], new Vector3(0, -6.75f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "PointVoidBreaker")
        {
            Instantiate(playerShips[4], new Vector3(0, -6.5f, 0), Quaternion.Euler(-90, 0, 0));
        } else if (PlayerPrefs.GetString("Spaceship") == "Annihilator")
        {
            Instantiate(playerShips[5], new Vector3(0, -6.5f, 0), Quaternion.Euler(-90, 0, 0));
        } else //Instantiates Space Fighter if the currently used spaceship is invalid
        {
            PlayerPrefs.SetString("Spaceship", "SpaceFighter");
            PlayerPrefs.Save();
            Instantiate(playerShips[0], new Vector3(0, -7, 0), Quaternion.Euler(-90, 0, 0));
        }
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
        newHighScoreText.enabled = false;
        StartCoroutine(spawnWaves());
        StartCoroutine(spawnAsteroids());
        AnalyticsEvent.LevelStart(SceneManager.GetActiveScene().name, new Dictionary<string, object>());
    }

    void OnEnable()
    {
        input.Enable();
        input.Gameplay.Fullscreen.performed += context => toggleFullscreen();
        input.Gameplay.Pause.performed += context => pause();
        input.Gameplay.Resume.performed += context => resumeGame(false);
        input.Gameplay.Restart.performed += context => restartForController();
        input.Menu.CloseMenu.performed += context => closeMenu();

        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        input.Debug.NextWave.performed += context => nextWave();
        input.Debug.SkipToBoss.performed += context => skipToBoss();
        #endif
    }

    void OnDisable()
    {
        input.Disable();
        input.Gameplay.Fullscreen.performed -= context => toggleFullscreen();
        input.Gameplay.Pause.performed -= context => pause();
        input.Gameplay.Resume.performed -= context => resumeGame(false);
        input.Gameplay.Restart.performed -= context => restartForController();
        input.Menu.CloseMenu.performed -= context => closeMenu();

        #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
        input.Debug.NextWave.performed -= context => nextWave();
        input.Debug.SkipToBoss.performed -= context => skipToBoss();
        #endif
    }

    void Update()
    {
        if (storedMaxWaves > 2)
        {
            maxWaves = storedMaxWaves;
        } else
        {
            maxWaves = 2;
        }
        if (!gameOver && !won && aliensReached >= maxAliensReached)
        {
            gameOver = true;
            deathMessageToShow = "You failed to protect the Earth!";
        }
        if (gameOver)
        {
            clickSource = 2;
            if (!quitGameMenu.enabled && !loading) gameOverMenu.enabled = true;
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
                audioSource.PlayOneShot(loseJingle);
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
                            audioSource.PlayOneShot(winJingle);
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
                ++wavesCleared;
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

        if (isCampaignLevel)
        {
            levelCount.transform.parent.gameObject.SetActive(true);
            scoreCount.transform.parent.gameObject.SetActive(false);
            if (PlayerPrefs.GetInt("IngameLevel") > 0)
            {
                levelCount.text = PlayerPrefs.GetInt("IngameLevel").ToString();
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
            if (controllerShootIcon)
            {
                if (isCampaignLevel)
                {
                    controllerShootIcon.anchoredPosition = new Vector2(-20, 20);
                } else
                {
                    controllerShootIcon.anchoredPosition = new Vector2(-20, 45);
                }
            }
            bossMaxHealth = 0;
            bossName.gameObject.SetActive(false);
        } else
        {
            if (bossMaxHealth <= 0) bossMaxHealth = currentBoss.GetComponent<EnemyHealth>().health;
            if (controllerShootIcon)
            {
                if (isCampaignLevel)
                {
                    controllerShootIcon.anchoredPosition = new Vector2(-20, 70);
                } else
                {
                    controllerShootIcon.anchoredPosition = new Vector2(-20, 105);
                }
            }
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
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player) player.SetActive(true);
            }
            foreach (GameObject background in GameObject.FindGameObjectsWithTag("Background"))
            {
                if (background)
                {
                    background.transform.position = new Vector3(0, background.transform.position.y, 5);
                    background.GetComponent<BackgroundScroll>().enabled = true;
                }
            }
            loadingScreen.SetActive(false);
            loadingTip.text = "";
        } else
        {
            Camera.main.transform.position = new Vector3(500, 0, -10);
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player) player.SetActive(false);
            }
            foreach (GameObject background in GameObject.FindGameObjectsWithTag("Background"))
            {
                if (background)
                {
                    background.transform.position = new Vector3(500, background.transform.position.y, 5);
                    background.GetComponent<BackgroundScroll>().enabled = false;
                }
            }
            loadingScreen.SetActive(true);
            loadingTip.text = currentLoadingTip;
        }
        if (PlayerPrefs.GetInt("Level") > PlayerPrefs.GetInt("MaxLevels")) //Checks if current level is more than the maximum amount
        {
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Levels"));
        } else if (PlayerPrefs.GetInt("Level") < 1) //Checks if current level is less than 1
        {
            PlayerPrefs.SetInt("Level", 1);
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

        if (wave > maxWaves) wave = maxWaves; //Checks if current wave is more than max waves
        if (maxAliensReached < 5) maxAliensReached = 5; //Checks if maximum aliens reached is less than 5
        if (limitedEnemySpawns < 1) limitedEnemySpawns = 1; //Checks if limited enemy spawns are less than 1
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("Difficulty");
        PlayerPrefs.DeleteKey("Restarted");
    }

    #region Input Functions
    void toggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    void closeMenu()
    {
        if (paused)
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
    }
    #endregion

    #region Input Debug Functions
    #if (UNITY_EDITOR || DEVELOPMENT_BUILD)
    void nextWave()
    {
        if (!gameOver && !won && !paused && wave < maxWaves) enemiesLeft = 0;
    }

    void skipToBoss()
    {
        if (!gameOver && !won && !paused && boss && !currentBoss)
        {
            enemiesLeft = 0;
            wave = maxWaves;
        }
    }
    #endif
    #endregion

    #region Main Functions
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
                                if (PlayerPrefs.GetInt("Difficulty") < 2) //Easy
                                {
                                    aliensReached -= 2;
                                } else //Normal, Hard and Nightmare
                                {
                                    --aliensReached;
                                }
                                if (aliensReached < 0) aliensReached = 0;
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
                                    if (PlayerPrefs.GetInt("Difficulty") < 2) //Easy
                                    {
                                        aliensReached -= 2;
                                    } else //Normal, Hard and Nightmare
                                    {
                                        --aliensReached;
                                    }
                                    if (aliensReached < 0) aliensReached = 0;
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
                                        if (PlayerPrefs.GetInt("Difficulty") < 2) //Easy
                                        {
                                            aliensReached -= 2;
                                        } else //Normal, Hard and Nightmare
                                        {
                                            --aliensReached;
                                        }
                                        if (aliensReached < 0) aliensReached = 0;
                                        reachedNextWave = false;
                                    }
                                } else
                                {
                                    yield return new WaitForSeconds(3);
                                    if (!gameOver && !won && !paused)
                                    {
                                        GameObject enemy = Instantiate(boss, new Vector3(0, bossInitialYPosition, 0), Quaternion.Euler(bossRotation.x, bossRotation.y, bossRotation.z));
                                        enemy.GetComponent<EnemyHealth>().invulnerable = true;
                                        enemy.name = boss.name;
                                        currentBoss = enemy;
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
                        aliensReached = 0;
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

    IEnumerator showNewHighScore()
    {
        for (int i = 0; i < 6; i++)
        {
            newHighScoreText.enabled = true;
            yield return new WaitForSeconds(0.5f);
            newHighScoreText.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
        newHighScoreText.enabled = false;
    }
    #endregion

    #region Menu Functions
    void pause()
    {
        if (!gameOver && !won && !gameOverMenu.enabled && !levelCompletedMenu.enabled)
        {
            if (!paused) //Pauses the game
            {
                clickSource = 1;
                paused = true;
                Time.timeScale = 0;
                AudioListener.pause = true;
                gamePausedMenu.enabled = true;
            } else //Unpauses the game
            {
                if (!settingsMenu.enabled && !quitGameMenu.enabled && !restartPrompt.enabled)
                {
                    paused = false;
                    Time.timeScale = 1;
                    AudioListener.pause = false;
                    gamePausedMenu.enabled = false;
                }
            }
        }
    }

    public void resumeGame(bool wasClicked)
    {
        if (!settingsMenu.enabled && !quitGameMenu.enabled && !restartPrompt.enabled)
        {
            if (audioSource && wasClicked)
            {
                if (buttonClick)
                {
                    audioSource.PlayOneShot(buttonClick);
                } else
                {
                    audioSource.Play();
                }
            }
            paused = false;
            Time.timeScale = 1;
            AudioListener.pause = false;
            gamePausedMenu.enabled = false;
        }
    }

    public void toNextLevel()
    {
        if (isCampaignLevel && won && levelCompletedMenu.enabled)
        {
            if (audioSource)
            {
                if (buttonClick)
                {
                    audioSource.PlayOneShot(buttonClick);
                } else
                {
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
        }
    }

    public void restart(bool wasClicked)
    {
        if (audioSource && wasClicked)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick);
            } else
            {
                audioSource.Play();
            }
        }
        if (isCampaignLevel)
        {
            PlayerPrefs.SetInt("Restarted", 1);
            PlayerPrefs.Save();
        }
        StartCoroutine(loadScene(SceneManager.GetActiveScene().name));
    }

    void restartForController()
    {
        if (restartPrompt.enabled) restart(false);
    }

    public void exitGame()
    {
        if (quitGameMenu.enabled)
        {
            if (audioSource)
            {
                if (buttonClick)
                {
                    audioSource.PlayOneShot(buttonClick);
                } else
                {
                    audioSource.Play();
                }
            }
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    public void exitToMainMenu()
    {
        if (audioSource)
        {
            if (buttonClick)
            {
                audioSource.PlayOneShot(buttonClick);
            } else
            {
                audioSource.Play();
            }
        }
        StartCoroutine(loadScene("Main Menu"));
    }

    public void openCanvasFromClickSource(Canvas canvas)
    {
        if (canvas)
        {
            if (audioSource)
            {
                if (buttonClick)
                {
                    audioSource.PlayOneShot(buttonClick);
                } else
                {
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
    #endregion

    IEnumerator loadScene(string scene)
    {
        if (!loading)
        {
            loading = true;
            AsyncOperation load = SceneManager.LoadSceneAsync(scene);
            if (LoadingTipArray.instance && LoadingTipArray.instance.tips.Length > 0 && PlayerPrefs.GetInt("Tips") >= 1) currentLoadingTip = LoadingTipArray.instance.tips[Random.Range(0, LoadingTipArray.instance.tips.Length)];
            if (Camera.main.GetComponent<AudioSource>()) Camera.main.GetComponent<AudioSource>().Stop();
            while (!load.isDone)
            {
                Time.timeScale = 0;
                AudioListener.pause = true;
                if (load.progress < 0.9f)
                {
                    load.allowSceneActivation = false;
                    loadingSlider.value = load.progress;
                    loadingPercentage.text = Mathf.Floor(load.progress * 100) + "%";
                    anyKeyPrompt.SetActive(false);
                } else
                {
                    if (PlayerPrefs.GetInt("Tips") >= 1)
                    {
                        if (Input.anyKeyDown) load.allowSceneActivation = true;
                        loadingSlider.value = 1;
                        loadingPercentage.text = "100%";
                        anyKeyPrompt.SetActive(true);
                    } else
                    {
                        load.allowSceneActivation = true;
                        loadingSlider.value = 1;
                        loadingPercentage.text = "100%";
                        anyKeyPrompt.SetActive(false);
                    }
                }
                gameHUD.enabled = false;
                gamePausedMenu.enabled = false;
                gameOverMenu.enabled = false;
                levelCompletedMenu.enabled = false;
                settingsMenu.enabled = false;
                quitGameMenu.enabled = false;
                restartPrompt.enabled = false;
                yield return null;
            }
        }
    }
}