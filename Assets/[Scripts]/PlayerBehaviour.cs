using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public Transform bulletSpawn;
    public GameObject bullet;
    public PauseScript pauseObj;
    public int fireRate;
    public float force;
    bool startTimer = false;
    float timer = 0.0f;
    public BulletManager bulletManager;

    [Header("Movement")]
    public float speed;
    public bool isGrounded;
   

    public RigidBody3D body;
    public CubeBehaviour cube;
    public Camera playerCam;

    void start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if(!pauseObj.isPaused)
        {
            _Fire();
            _Move();
        }
    }

    private void _Move()
    {
        if (isGrounded)
        {
            startTimer = false;
            timer = 0.0f;
            if (Input.GetAxisRaw("Horizontal") > 0.0f)
            {
                // move right
                body.velocity = playerCam.transform.right * speed * Time.deltaTime;
            }

            if (Input.GetAxisRaw("Horizontal") < 0.0f)
            {
                // move left
                body.velocity = -playerCam.transform.right * speed * Time.deltaTime;
            }

            if (Input.GetAxisRaw("Vertical") > 0.0f)
            {
                // move forward
                body.velocity = playerCam.transform.forward * speed * Time.deltaTime;
            }

            if (Input.GetAxisRaw("Vertical") < 0.0f) 
            {
                // move Back
                body.velocity = -playerCam.transform.forward * speed * Time.deltaTime;
            }

            body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, 0.9f);
            body.velocity = new Vector3(body.velocity.x, 0.0f, body.velocity.z); // remove y
            

            if (Input.GetAxisRaw("Jump") > 0.0f)
            {
                body.velocity = transform.up * speed * 0.1f * Time.deltaTime;
                startTimer = true;
            }


            transform.position += body.velocity;
        }
        else if(!isGrounded && timer >= 0.2f)
        {
            if (Input.GetAxisRaw("Horizontal") > 0.0f && Input.GetAxisRaw("Jump") > 0.0f)
            {
                // move right
                body.velocity = (playerCam.transform.right - transform.up / 1.5f) * 3.0f * Time.deltaTime;
            }

            if (Input.GetAxisRaw("Horizontal") < 0.0f && Input.GetAxisRaw("Jump") > 0.0f)
            {
                // move left
                body.velocity = (-playerCam.transform.right - transform.up / 1.5f) * 3.0f * Time.deltaTime;
            }
            if (Input.GetAxisRaw("Vertical") > 0.0f && Input.GetAxisRaw("Jump") > 0.0f)
            {
                // move forward
                body.velocity = (playerCam.transform.forward - transform.up/ 1.5f) * 3.0f * Time.deltaTime;
            }
            if (Input.GetAxisRaw("Vertical") < 0.0f && Input.GetAxisRaw("Jump") > 0.0f)
            {
                // move Back
                body.velocity = (-playerCam.transform.forward - transform.up / 1.5f) * 3.0f * Time.deltaTime;
            }
            if (Input.GetAxisRaw("Horizontal") > 0.0f)
            {
                // move right
                body.velocity = (playerCam.transform.right - transform.up / 1.5f) * 3.0f * Time.deltaTime;
            }

            if (Input.GetAxisRaw("Horizontal") < 0.0f )
            {
                // move left
                body.velocity = (-playerCam.transform.right - transform.up / 1.5f) * 3.0f * Time.deltaTime;
            }
            if (Input.GetAxisRaw("Vertical") > 0.0f)
            {
                // move forward
                body.velocity = (playerCam.transform.forward - transform.up / 1.5f) * 3.0f * Time.deltaTime;
            }
            if (Input.GetAxisRaw("Vertical") < 0.0f)
            {
                // move Back
                body.velocity = (-playerCam.transform.forward - transform.up / 1.5f) * 3.0f * Time.deltaTime;
            }
        }

        if(startTimer)
        {
            timer += 0.5f * Time.deltaTime;
        }
        Debug.Log(timer);
    }


    private void _Fire()
    {
        if (Input.GetAxisRaw("Fire1") > 0.0f)
        {
            // delays firing
            if (Time.frameCount % fireRate == 0)
            {

                var tempBullet = bulletManager.GetBullet(bulletSpawn.position, bulletSpawn.forward);
                tempBullet.transform.SetParent(bulletManager.gameObject.transform);
            }
        }
    }

    void FixedUpdate()
    {
        GroundCheck();
    }

    private void GroundCheck()
    {
        isGrounded = cube.isGrounded;
    }

}
