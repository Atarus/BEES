using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{

    float moveSpeedX = 0, moveSpeedY = 0, walkSpeed = 100, acceleration = 3, deceleration = 8;
    public int maxHealth = 100, currentHealth;
    Rigidbody2D body;
    Camera cam;

    Animator animator;
    

    [SerializeField]
    public Image healthBar;
    float healthbarOriginalWidth;


    void Start()
    {
        if (!isLocalPlayer)
        {
            transform.GetComponent<GunController>().enabled = false;
            this.enabled = false;
        }
        cam = Camera.main;
        currentHealth = maxHealth;
        healthBar = GameObject.FindGameObjectWithTag("Healthbar").GetComponent<Image>();
        healthbarOriginalWidth = healthBar.rectTransform.sizeDelta.x;
        this.transform.tag = "Player";
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.blue;
    }

    void Update()
    {
        CheckIfDead();
        LookToMouse();
        HandleMovement();
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Shoot");
        }
        HandleAnimations();
        HandleUI();
    }

    void FixedUpdate()
    {
        Vector2 movement = new Vector2(moveSpeedY, moveSpeedX);
        body.velocity = movement * walkSpeed * Time.deltaTime;
    }

    void HandleMovement()
    {
        float movementModifier = 1;
        if (Input.GetKey(KeyCode.LeftShift)) { movementModifier = 1.5f; }
        if (Input.GetKey(KeyCode.W))
        {
            moveSpeedX = Mathf.Lerp(moveSpeedX, movementModifier, acceleration * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveSpeedX = Mathf.Lerp(moveSpeedX, -movementModifier, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeedX = Mathf.Lerp(moveSpeedX, 0, deceleration * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveSpeedY = Mathf.Lerp(moveSpeedY, -movementModifier, acceleration * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveSpeedY = Mathf.Lerp(moveSpeedY, movementModifier, acceleration * Time.deltaTime);
        }
        else
        {
            moveSpeedY = Mathf.Lerp(moveSpeedY, 0, deceleration * Time.deltaTime);
        }

    }

    void HandleUI()
    {
        healthBar.rectTransform.sizeDelta = new Vector2(((float)currentHealth/(float)maxHealth) * healthbarOriginalWidth, healthBar.rectTransform.sizeDelta.y);
        
    }

    void HandleAnimations()
    {
        if (body.velocity != new Vector2(0, 0) && !animator.GetBool("isMoving"))
        {
            animator.SetBool("isMoving", true);
        }
        else if (animator.GetBool("isMoving"))
        {
            animator.SetBool("isMoving", false);
        }
    }

    void CheckIfDead()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Dead");
        }
    }
    
    void LookToMouse()
    {
        // Distance from camera to object.  We need this to get the proper calculation.
        float camDis = cam.transform.position.y - transform.position.y;

        // Get the mouse position in world space. Using camDis for the Z axis.
        Vector3 mouse = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDis));

        float AngleRad = Mathf.Atan2(mouse.y - transform.position.y, mouse.x - transform.position.x);
        float angle = (180 / Mathf.PI) * AngleRad;

        body.rotation = angle;
    }

    
}
