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
    CameraController camController;

    
    [SerializeField]
    public Image healthBar;
    float healthbarOriginalWidth;

    [HideInInspector]
    public Inventory inv = new Inventory();
    public Text pistolAmmo, smgAmmo, arAmmo, dmrAmmo, sniperAmmo;

    public bool isCurrentlyHealing = false;
    public float timeLeftToHeal;

    public float royalJellyTime = 5;
    public float stimPackTime = 6;
    public float bandageTime = 3;

    public int royalJellyHealAmount = 10000;
    public int stimPackHealAmount = 50;
    public int bandageHealAmount = 10;

    public enum healthType { royalJelly, stimPack, bandage};
    healthType healUsing;

    GameObject usingMedsBar;
    Image medTimeImage;
    Text medTimeText, usingMedText;
    


    void Start()
    {
        if (!isLocalPlayer)
        {
            transform.GetComponent<GunController>().enabled = false;
            this.enabled = false;
        }
        cam = Camera.main;
        camController = cam.GetComponent<CameraController>();
        camController.target = this.transform;
        currentHealth = maxHealth;
        healthBar = GameObject.FindGameObjectWithTag("Healthbar").GetComponent<Image>();
        healthbarOriginalWidth = healthBar.rectTransform.sizeDelta.x;


        usingMedsBar = GameObject.FindGameObjectWithTag("UsingMeds");
        medTimeImage = GameObject.FindGameObjectWithTag("MedTimeImage").GetComponent<Image>();
        medTimeText = GameObject.FindGameObjectWithTag("MedTimeText").GetComponent<Text>();
        usingMedText = GameObject.FindGameObjectWithTag("UsingMedText").GetComponent<Text>();
        GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>().controller = this;


        usingMedsBar.SetActive(false);
        this.transform.tag = "Player";
        body = GetComponent<Rigidbody2D>();
        SetupInventory();
        
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
        HandleHealing();
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
            isCurrentlyHealing = false;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveSpeedX = Mathf.Lerp(moveSpeedX, -movementModifier, acceleration * Time.deltaTime);
            isCurrentlyHealing = false;
        }
        else
        {
            moveSpeedX = Mathf.Lerp(moveSpeedX, 0, deceleration * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveSpeedY = Mathf.Lerp(moveSpeedY, -movementModifier, acceleration * Time.deltaTime);
            isCurrentlyHealing = false;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveSpeedY = Mathf.Lerp(moveSpeedY, movementModifier, acceleration * Time.deltaTime);
            isCurrentlyHealing = false;
        }
        else
        {
            moveSpeedY = Mathf.Lerp(moveSpeedY, 0, deceleration * Time.deltaTime);
        }

    }

    void HandleUI()
    {
        healthBar.rectTransform.sizeDelta = new Vector2(((float)currentHealth / (float)maxHealth) * healthbarOriginalWidth, healthBar.rectTransform.sizeDelta.y);
        pistolAmmo.text = inv.pistolAmmo.ToString();
        if (inv.pistolAmmo == inv.maxPistolAmmo) { pistolAmmo.color = Color.red; } else { pistolAmmo.color = Color.white; }
        smgAmmo.text = inv.smgAmmo.ToString();
        if (inv.smgAmmo == inv.maxsmgAmmo) { smgAmmo.color = Color.red; } else { smgAmmo.color = Color.white; } 
        arAmmo.text = inv.assaultRifleAmmo.ToString();
        if (inv.assaultRifleAmmo == inv.maxAssaultRifleAmmo) { arAmmo.color = Color.red; } else { arAmmo.color = Color.white; }
        dmrAmmo.text = inv.dmrAmmo.ToString();
        if (inv.dmrAmmo == inv.maxDmrAmmo) { dmrAmmo.color = Color.red; } else { dmrAmmo.color = Color.white; }
        sniperAmmo.text = inv.sniperAmmo.ToString();
        if (inv.sniperAmmo == inv.maxSniperAmmo) { sniperAmmo.color = Color.red; } else { sniperAmmo.color = Color.white; }
        HandleHealthUI();
    }

    void HandleHealthUI()
    {
        if (isCurrentlyHealing)
        {
            if (!usingMedsBar.activeInHierarchy) { usingMedsBar.SetActive(true); }
            switch (healUsing)
            {
                case healthType.royalJelly: { medTimeImage.fillAmount = timeLeftToHeal / royalJellyTime; usingMedText.text = "Using Royal Jelly"; break; }
                case healthType.stimPack: { medTimeImage.fillAmount = timeLeftToHeal / stimPackTime; usingMedText.text = "Using Stim Pack"; break; }
                case healthType.bandage: { medTimeImage.fillAmount = timeLeftToHeal / bandageTime; usingMedText.text = "Using Bandage"; break; }
            }
            medTimeText.text = timeLeftToHeal.ToString("F1");
        }
        if (!isCurrentlyHealing) { if (usingMedsBar.activeInHierarchy) { usingMedsBar.SetActive(false); } }
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

    #region Character Setup

    void SetupInventory()
    {
        pistolAmmo = GameObject.Find("PistolAmmoText").GetComponent<Text>();
        smgAmmo = GameObject.Find("SMGAmmoText").GetComponent<Text>();
        arAmmo = GameObject.Find("ARAmmoText").GetComponent<Text>();
        dmrAmmo = GameObject.Find("DMRAmmoText").GetComponent<Text>();
        sniperAmmo = GameObject.Find("SniperAmmoText").GetComponent<Text>();


        inv.pistolAmmo = 40;
        inv.smgAmmo = 90;
        inv.assaultRifleAmmo = 80;
        inv.dmrAmmo = 30;
        inv.sniperAmmo = 15;

        inv.maxPistolAmmo = 40;
        inv.maxsmgAmmo = 90;
        inv.maxAssaultRifleAmmo = 80;
        inv.maxDmrAmmo = 30;
        inv.maxSniperAmmo = 15;
    }
    #endregion

    #region Handle Healing

    public void UseHealItem(healthType healer)
    {
        if (!isCurrentlyHealing)
        {
            switch (healer)
            {
                case healthType.royalJelly:
                    {
                        if (inv.royalJellies > 0)
                        {
                            timeLeftToHeal = royalJellyTime;
                            isCurrentlyHealing = true;
                        }
                        break;
                    }
                case healthType.stimPack:
                    {
                        if (inv.stimPacks > 0)
                        {
                            timeLeftToHeal = stimPackTime;
                            isCurrentlyHealing = true;
                        }
                        break;
                    }
                case healthType.bandage:
                    {
                        if (inv.bandages > 0)
                        {
                            timeLeftToHeal = bandageTime;
                            isCurrentlyHealing = true;
                        }
                        break;
                    }
                default: break;
            }
        }
        
    }

    void Heal(healthType healer)
    {
        switch (healer)
        {
            case healthType.royalJelly: currentHealth += royalJellyHealAmount; inv.royalJellies -= 1; break;
            case healthType.stimPack: currentHealth += stimPackHealAmount; inv.stimPacks -= 1; break;
            case healthType.bandage: currentHealth += bandageHealAmount; inv.bandages -= 1;  break;
            default:break;

        }
        isCurrentlyHealing = false;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
    
    void HandleHealing()
    {
        if (isCurrentlyHealing) { timeLeftToHeal -= Time.deltaTime; }

        if (Input.GetKey(KeyCode.Alpha8)) { healUsing = healthType.bandage; UseHealItem(healUsing); }
        if (Input.GetKey(KeyCode.Alpha9)) { healUsing = healthType.stimPack; UseHealItem(healUsing); }
        if (Input.GetKey(KeyCode.Alpha0)) { healUsing = healthType.royalJelly; UseHealItem(healUsing);}

        if (isCurrentlyHealing && timeLeftToHeal <= 0)
        {
            Heal(healUsing);
            isCurrentlyHealing = false;
        }
    }

    #endregion





}
