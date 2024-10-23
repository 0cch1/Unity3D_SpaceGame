using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spaceship : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Rigidbody rigid;
    private bool isFiring = false;
    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            rigid.AddForce(transform.up * (rigid.mass * Time.fixedDeltaTime * 1000f));
        // we're using an Angular Drag of 15.0 on the rigid body, so need a lot of torque here
        if (Input.GetKey(KeyCode.LeftArrow))
            rigid.AddTorque(-Vector3.up * (rigid.mass * Time.fixedDeltaTime * 4000f));
        else if (Input.GetKey(KeyCode.RightArrow))
            rigid.AddTorque(Vector3.up * (rigid.mass * Time.fixedDeltaTime * 4000f));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isFiring)
        {
            isFiring = true;
            InvokeRepeating("FireBullet", 0f, 0.25f); // Fire bullet immediately, every 0.25 seconds
        }
        if (Input.GetKeyUp(KeyCode.Space) && isFiring)
        {
            isFiring = false;
            CancelInvoke("FireBullet");
        }
    }

    void FireBullet()
    {
        Vector3 bulletSpwanPos = transform.position + transform.up * 1.5f;
        GameObject bullet = Instantiate(bulletPrefab,bulletSpwanPos,transform.rotation);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = transform.up * 50f;
    }

}