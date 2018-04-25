using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public AudioClip destroySound;
    public AudioClip bossSound;
    public AudioClip menuSound;

    private AudioSource audioSource;

    public int score;
    public int slimesSold;
    public int wellHealth;
    public int wellHealthAmount;
    public bool gamePaused;
    public bool playerEaten = false;
    public bool gameEnded = false;
    public bool enterKeyEnabled = true;
    public bool attackOrdered = false;
    public bool onMainMenu = true;
    public bool gameStarted = false;

    public float time;
    public float fadeTime;
    public float fadeEndScreen;
    public float fadeMultiplier;
    public float slimeSpawnTimer;
    public float slimeSpawnMinTime;
    public float slimeSpawnMaxTime;

    public GameObject slimeCountText;
    public GameObject timeCountText;
    public GameObject wellHealthText;
    public GameObject pauseText;

    public GameObject player;
    public GameObject buyer;

    public GameObject mainMenu;
    public GameObject well;
    public GameObject smoke;
    public GameObject hole;
    public GameObject bossSlime;
    public GameObject fadeToBlack;
    public GameObject endScreen;
    public GameObject scoreText;
    public GameObject scoreTextEnd;

    public List<GameObject> redSlimes;  
    public List<GameObject> allSlimes;
    public List<GameObject> slimeSpawns;
    public List<GameObject> splashes;

    public int redSlimeCount;
    public int slimeCount;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void StartGame()
    {
        gameStarted = true;
        // Disable main menu
        mainMenu.SetActive(false);
        onMainMenu = false;

        // RESET BOOLS
        gameEnded = false;
        playerEaten = false;
        enterKeyEnabled = false;
        // RESET ACTORS
        player.transform.position = new Vector3(-6.5f, -1, 0f);
        player.GetComponent<Player>().disableInput = false;
        //
        buyer.transform.position = new Vector3(-11f, 0f, 0f);
        buyer.GetComponent<Buyer>().Arrive();
        //
        bossSlime.transform.position = new Vector3(0f, 0f, 0f);
        bossSlime.SetActive(false);
        //
        foreach (GameObject slime in allSlimes)
        {
           DestroyObject(slime);
        }
        // CLEAN SPLASHES
        foreach (GameObject splash in splashes)
        {
           DestroyObject(splash);
        }
        // RESET OBJS
        well.SetActive(true);
        wellHealth = wellHealthAmount;
        hole.SetActive(false);
        endScreen.SetActive(false);
        // RESET TIMERS
        slimeSpawnTimer = Random.Range(slimeSpawnMinTime, slimeSpawnMaxTime);   // Set duration for next spawn
        time = 0;
    }

    private void Update()
    {
        if (onMainMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                audioSource.PlayOneShot(menuSound, 1f);

                Debug.Log("ESC pressed for quit.");
                Application.Quit();
            }
            if (Input.GetKeyDown("enter") || Input.GetKeyDown("return") || Input.GetKey(KeyCode.KeypadEnter))
            {
                audioSource.PlayOneShot(menuSound, 1f);
                Debug.Log("Enter pressed.");
                onMainMenu = false;
                gamePaused = false;
                pauseText.SetActive(false);
                mainMenu.SetActive(false); // Enable main menu
                Time.timeScale = 1;

            }
        }
        if (!gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                audioSource.PlayOneShot(menuSound, 1f);
                Debug.Log("ESC pressed for main menu and pause");
                gamePaused = !gamePaused;

                if (gamePaused) {
                    mainMenu.SetActive(true); // Enable main menu
                    Time.timeScale = 0;
                    pauseText.SetActive(true);
                    onMainMenu = true;
                }
                else
                {
                    mainMenu.SetActive(false); // disable main menu
                    Time.timeScale = 1;
                    pauseText.SetActive(false);
                    onMainMenu = false;
                }            
            }
        }
        // START / RESTART BUTTON
        if (gameEnded && playerEaten)
        { 
            if (Input.GetKeyDown("enter") || Input.GetKeyDown("return") || Input.GetKey(KeyCode.KeypadEnter))
            {
                audioSource.PlayOneShot(menuSound, 1f);
                Debug.Log("Enter pressed for restart");
                StartGame();
            }
        }
        /*
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            EndGame();
        }
        */
        // WELL BREAKING MEANING GAME OVER
        if (wellHealth <= 0)  // Prevent looping
        {
            if (playerEaten)
                FadeToBlack();

            if (!gameEnded)
                EndGame();
        }
        // DO NORMAL STUFF 
        if (!gameEnded && !gamePaused && !onMainMenu)
        {
            // SCORES, TIME, SOLD SLIMES..
            HandleUI(); 

            FadeToWhite();  // Remove the curtains
            // SLIME SPAWNER
            if (slimeSpawnTimer > 0)
            {
                slimeSpawnTimer -= Time.deltaTime;
            }
            else
            {
                SpawnSlime();
            }
            // KEEP TRACK OF THE SLIMES
            redSlimeCount = redSlimes.Count;
            slimeCount = allSlimes.Count;
            // SEND ATTACK ORDER WHEN CRITICAL MASS OF RED SLIMES ACCUMULATED
            if (redSlimeCount >= 10) //  && !attackOrdered
            {
                foreach (GameObject slime in redSlimes)
                {
                    if (slime.GetComponent<Slime>().mode != Slime.Mode.Exploding) // Do not touch them
                    {
                        if (slime.GetComponent<Slime>().mode != Slime.Mode.Attacking)
                        slime.GetComponent<Slime>().mode = Slime.Mode.Attacking;
                    }
                }
                attackOrdered = true;
            }
            // CALL OFF ATTACK WHEN COUNT IS LOW
            else if (redSlimeCount < 10 ) // && attackOrdered
            {
                foreach (GameObject slime in redSlimes)
                {
                    if (slime.GetComponent<Slime>().mode == Slime.Mode.Attacking)
                    slime.GetComponent<Slime>().mode = Slime.Mode.None;
                }
                attackOrdered = false;
            }
        }
    }
    void SpawnSlime()
    {
        //Debug.Log("SpawnSlime called");
        int spawnLoc = Random.Range(0, slimeSpawns.Count); // Draw random spawn number
        for (int i = 0; i < slimeSpawns.Count; i++)
        {
            if (i == spawnLoc)  // Found match
            {
                //Debug.Log("SpawnSlime spawned Slime");
                slimeSpawns[i].GetComponent<SlimeSpawn>().SpawnSlime();  // Spawn from spawn
                slimeSpawnTimer = Random.Range(slimeSpawnMinTime, slimeSpawnMaxTime);   // Set duration for next spawn
                break;  // Break loop
            }
            //else
                //Debug.Log("Error in SpawnSlime! i was:"+i+ "spawnLoc was: "+ spawnLoc);
        }
    }
    void EndGame()
    {
        audioSource.PlayOneShot(destroySound, 1f);
        audioSource.PlayOneShot(bossSound, 1f);

        wellHealth = 0; // FOR DEBUG
        if (player.GetComponent<Player>().mode == Player.Mode.Carrying)
            player.GetComponent<Player>().DropSlime(player.GetComponent<Player>().pickedSlime);
        foreach (GameObject slime in allSlimes)
        {
            slime.GetComponent<Slime>().disableInput = true;
            slime.GetComponent<Slime>().Hold();
        }
        hole.SetActive(true);
        Instantiate(smoke, hole.transform.position, Quaternion.identity);
        bossSlime.SetActive(true);
        well.SetActive(false);

        player.GetComponent<Player>().disableInput = true;
        player.GetComponent<Player>().Hold();
        gameEnded = true;
        enterKeyEnabled = true;
    }
    void HandleUI()
    {
        time += Time.deltaTime;
        int roundedTime = Mathf.RoundToInt(time);
        score = roundedTime * slimesSold;
        slimeCountText.GetComponent<Text>().text = slimeCount.ToString();
        timeCountText.GetComponent<Text>().text = roundedTime.ToString();
        wellHealthText.GetComponent<Text>().text = wellHealth.ToString();
        scoreText.GetComponent<Text>().text = score.ToString();
        scoreTextEnd.GetComponent<Text>().text = score.ToString();

    }
    void FadeToWhite()
    {
        //Debug.Log("Fade to White.");
        fadeToBlack.SetActive(true);   
        if (fadeTime > 0)
        {
            fadeTime -= Time.deltaTime;
        }
        else
        {
        }
        fadeToBlack.GetComponent<Image>().color = new Color(0, 0, 0, fadeTime * fadeMultiplier);
        //Debug.Log(fadeToBlack.GetComponent<Image>().color.a);
    }
    void FadeToBlack()
    {
        //Debug.Log("Fade to Black.");
        fadeToBlack.SetActive(true);
        if (fadeTime < 1)
        {
            fadeTime += Time.deltaTime;
        }
        else {
            FadeEndScreen();
        }
        fadeToBlack.GetComponent<Image>().color = new Color(0, 0, 0, fadeTime * fadeMultiplier);
    }
    void FadeEndScreen()
    {
        //Debug.Log("Fade End Screen.");
        endScreen.SetActive(true);
        if (fadeEndScreen < 4)
        {
            fadeEndScreen += Time.deltaTime;
        }
        else if (fadeEndScreen >= 5 && !enterKeyEnabled)
        {
            Debug.Log("Enter key enabled and can start / restart game");
            enterKeyEnabled = true;
        }
        endScreen.GetComponent<Image>().color = new Color(1, 1, 1, fadeEndScreen * fadeMultiplier);
    }

}
