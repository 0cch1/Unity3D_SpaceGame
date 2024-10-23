using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using TMPro;

public enum GameState
{
    MENU,
    PLAYING
}

public class GameManager : MonoBehaviour
{
    // inspector settings
    public GameObject asteroidPrefab, spaceshipPrefab, fragmentPrefab;
    private GameObject playerInstance; // store reference to the player spaceship
    public GameObject menuCanvas, playingCanvas;
    public TMP_Text scoreText, highScoreText, livesText, asteroidText;
    // class-level statics
    public static GameManager instance;
    public static int currentGameLevel;
    public static Vector3 screenBottomLeft, screenTopRight;
    public static float screenWidth, screenHeight;

    private int score = 0, highScore = 0, playerLives = 3;
    public int activeAsteroids = 0;

    public GameState currentGameState = GameState.MENU;

    // Use this for initialization
    void Start()
    {
        instance = this;
        Camera.main.transform.position = new Vector3(0f, 30f, 0f);
        Camera.main.transform.LookAt(Vector3.zero, new Vector3(0f, 0f, 1f));
        
        UpdateUI(); 
        SwitchToMenu();
        
    }

    public static void StartNextLevel()
    {

        if(instance.currentGameState != GameState.PLAYING)
        {
            return;
        }

        Debug.Log("New level start");
        currentGameLevel++;

        // find (slightly expanded) screen corners and size, in world coordinates
        // for ViewportToWorldPoint, the z value specified is in world units from the camera

        screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, -0.1f, 30f));
        screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 1.1f, 30f));
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.z - screenBottomLeft.z;

        Debug.Log("BottomLeft: " + screenBottomLeft);
        Debug.Log("TopRight: " + screenTopRight);
        Debug.Log("Width: " + screenWidth);
        Debug.Log("Height: " + screenHeight);

        // instantiate some asteroids near the edges of the screen
        for (int i = 0; i < currentGameLevel * 2 + 3; i++)
        {
            GameObject go = Instantiate(instance.asteroidPrefab) as GameObject;
            float x, z;
            if (Random.Range(0f, 1f) < 0.5f)
                x = screenBottomLeft.x + Random.Range(0f, 0.15f) * screenWidth; 
            else
                x = screenTopRight.x - Random.Range(0f, 0.15f) * screenWidth;
            if (Random.Range(0f, 1f) < 0.5f)
                z = screenBottomLeft.z + Random.Range(0f, 0.15f) * screenHeight; 
            else
                z = screenTopRight.z - Random.Range(0f, 0.15f) * screenHeight; 

            go.transform.position = new Vector3(x, 0f, z);

            instance.activeAsteroids++;
        }
    }
    private void CreatePlayerSpaceship()
    {
        // instantiate the player's spaceship
        playerInstance = Instantiate(spaceshipPrefab);
        playerInstance.transform.position = Vector3.zero;
    }

    public void RespawnPlayer()
    {
        if(playerInstance != null)
        {
            Destroy(playerInstance);
        }
        if(playerLives > 0)
        {
            CreatePlayerSpaceship();
        } else
        {
            EndGame();
        }
        
        
    }

    public void StartNewGame()
    {
        currentGameState = GameState.PLAYING;
        currentGameLevel= 0;
        score = 0;
        playerLives = 3;

        UpdateUI();
        SwitchToPlay();

        StartNextLevel();
        CreatePlayerSpaceship();

    }

    private void SwitchToMenu()
    {
        currentGameState = GameState.MENU;
        menuCanvas.SetActive(true);
        playingCanvas.SetActive(false);
    }

    private void SwitchToPlay()
    {
        currentGameState= GameState.PLAYING;
        playingCanvas.SetActive(true);
        menuCanvas.SetActive(false);
    }
    public void EndGame()
    {
        currentGameState = GameState.MENU;
        DestroyAllAsteroids();
        SwitchToMenu();
    }

    private void DestroyAllAsteroids()
    {
        Asteroid[] asteroids = FindObjectsOfType<Asteroid>();
        foreach(Asteroid asteroid in asteroids)
        {
            Destroy(asteroid.gameObject);
        }
        activeAsteroids = 0;
    }

    public void AsteroidDestroyed()
    {
        activeAsteroids--;

        Debug.Log("Asteroids left: " + activeAsteroids);
        if(activeAsteroids <= 0 && currentGameState == GameState.PLAYING)
        {
            StartNextLevel();
        }
    }
    public void AddScore(int points)
    {
        score += points;
        if(score > highScore)
        {
            highScore = score;
        }
        UpdateUI();
    }
    public void LoseLife()
    {
        playerLives--;
        UpdateUI();

        if(playerLives <= 0)
        {
            EndGame();
            
        }

    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        highScoreText.text = "Highest Score: " + highScore;
        livesText.text = "Lives: " + playerLives;
        asteroidText.text = "Asteroid: " + activeAsteroids;
    }

}
