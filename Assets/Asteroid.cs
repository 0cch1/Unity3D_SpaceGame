using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Asteroid : MonoBehaviour
{
    // inspector settings
    public Rigidbody rigidBody;
    public float splitScaleFactor = 0.5f;
    public float minSize = 0.05f;
    public float maxSize = 0.12f;

    // Flag to track if asteroid has already split
    public bool hasSplit = false;
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
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.RespawnPlayer();
        } else if (collision.gameObject.CompareTag("Bullet"))
        { 
            Destroy(collision.gameObject);

            if(transform.localScale.x > minSize * 2 && !hasSplit)
            {
                //if asteroid is large, split it into smaller asteroids
                SplitAsteroid();
            } else
            {
                // Destroy the asteroid if its small
                Destroy(gameObject);
            }
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
        hasSplit = true; //Indicate the asteroid has already split

        //Number of smaller asteroids to create
        int numFragments = 3;
        float newScale = transform.localScale.x * splitScaleFactor;

        // Ensure the new scale is not too small
        if(newScale < minSize)
        {
            Destroy(gameObject); // Destroy the original asteoid without split
            return;
        }

        for(int i = 0; i < numFragments; i++)
        {
            // Create a new asteroid with smaller scale
            GameObject smallerAsteroid = Instantiate(GameManager.instance.asteroidPrefab, transform.position, Quaternion.identity);
            smallerAsteroid.transform.localScale = new Vector3(newScale, newScale, newScale);

            // Give the smaller asteroid a random velocity
            Rigidbody rb = smallerAsteroid.GetComponent<Rigidbody>();
            rb.velocity = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
            rb.angularVelocity = Random.insideUnitSphere * 4f;

            // Mark the new asteroids so that they cannot split again
            Asteroid asteroidScript = smallerAsteroid.GetComponent<Asteroid>();
            asteroidScript.hasSplit = true;
        }
        Destroy(gameObject);
    }
}