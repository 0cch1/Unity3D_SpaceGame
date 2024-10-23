using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Asteroid : MonoBehaviour
{
    // inspector settings
    public Rigidbody rigidBody;
    public float splitScaleFactor = 0.3f;
    public float minSize = 0.05f;
    public float maxSize = 0.12f;

    
    public bool hasSplit = false; // Flag to track if asteroid has already split
    private bool isDestroyed = false; // flag to prevent multiple destruction
    // Use this for initialization
    void Start()
    {
        // randomise size+mass
        float randomScale = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomScale,randomScale,randomScale);
        rigidBody.mass = transform.localScale.x * transform.localScale.y *
       transform.localScale.z;
        // randomise velocity
        rigidBody.velocity = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f,
       10f));
        rigidBody.angularVelocity = new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f),
       Random.Range(-4f, 4f));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(isDestroyed) return;

        //Handle collision with player
        if (collision.gameObject.CompareTag("Player"))
        {
            
            GameManager.instance.LoseLife();
            GameManager.instance.RespawnPlayer();
            
            // hanlde collision with bullet
        } else if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Asteroid hit by bullet");
            Destroy(collision.gameObject); // Destroy bullet

            //If asteroid is large split it into smaller asteroids
            if(transform.localScale.x > minSize * 2 && !hasSplit)
            {
                SplitAsteroid();
            }
            else
            {
                // Destroy the asteroid if its small
                DestroyAsteroid();
            }
            // Add score when asteroid is destroyed
            GameManager.instance.AddScore(50);
        }
        // spawn fragments (the fragment prefab has no collider and has no gameplay effect)
        float colSpeed = collision.relativeVelocity.magnitude;
        int num = Mathf.Clamp(Mathf.RoundToInt(colSpeed), 2, 8);
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(GameManager.instance.fragmentPrefab);
            go.transform.position = collision.contacts[0].point;
            Rigidbody r = go.GetComponent<Rigidbody>();
            r.velocity = new Vector3(Random.Range(-colSpeed, colSpeed), 0f, Random.Range(-
            colSpeed, colSpeed));
            r.angularVelocity = new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f),
            Random.Range(-4f, 4f));
        }
    }

    private void SplitAsteroid()
    {
        if(isDestroyed) return;

        hasSplit = true; //Indicate the asteroid has already split

        //Number of smaller asteroids to create
        int numFragments = 2;
        float newScale = transform.localScale.x * splitScaleFactor;

        // Ensure the new scale is not too small
        if(newScale < minSize)
        {
            DestroyAsteroid(); // Destroy the original asteoid without split
            return;
        }

        for(int i = 0; i <= numFragments; i++)
        {
            // Increase active asteroid count for each new fragmen
            GameManager.instance.activeAsteroids++;
            // Create a new asteroid with smaller scale
            GameObject smallerAsteroid = Instantiate(GameManager.instance.asteroidPrefab);
            smallerAsteroid.transform.position = transform.position;
            smallerAsteroid.transform.localScale = new Vector3(newScale, newScale, newScale);

            // Give the smaller asteroid a random velocity
            Rigidbody rb = smallerAsteroid.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
            rb.angularVelocity = Random.insideUnitSphere * 4f;

            // Mark the new asteroids so that they cannot split again
            Asteroid asteroidScript = smallerAsteroid.GetComponent<Asteroid>();
            asteroidScript.hasSplit = true;
            
        }
        DestroyAsteroid();
    }

    private void DestroyAsteroid()
    {
        if(isDestroyed) return;
        isDestroyed= true;
        //Decrease the active asteroid count
        GameManager.instance.AsteroidDestroyed();
        //Destroy the asteroid game object
        Destroy(gameObject);
    }
}