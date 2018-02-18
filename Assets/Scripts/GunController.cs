using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GunController : NetworkBehaviour {

    #region Public Variables
    public Gun gun;
    public GameObject bullet;
    public Transform bulletSpawn;

    public Text ammoText;

    #endregion

    #region Private Variables
    float timeToNextShot;
    int currentAmmo;

    bool isReloading = false;
    float currentReloadTime;


    float currentSpreadInDegrees;
    float currentSpreadCooldown;
    #endregion

    #region MonoBehaviour Functions
    // Use this for initialization
    void Start () {
        currentReloadTime = gun.reloadTime;
        ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(gun.isAutomatic);
        if (timeToNextShot > 0) { timeToNextShot -= Time.deltaTime;}
        if (gun.isAutomatic)
        {
            if (Input.GetMouseButton(0))
            {
                
                if (CheckIfCanFire())
                {
                    isReloading = false;
                    CmdFire();
                }
            }
        }
        else if (!gun.isAutomatic)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (CheckIfCanFire())
                {
                    isReloading = false;
                    CmdFire();
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            isReloading = true;
        }
        HandleReloading();
        HandleShotSpread();
        HandleUI();
    }
    #endregion

    #region Handling UI

    void HandleUI()
    {
        ammoText.text = currentAmmo + "/" + gun.clipSize;
    }

    #endregion

    #region Handling Firing


    [Command]
    void CmdFire()
    {

        GameObject b = Instantiate(bullet, bulletSpawn.position + gun.bulletSpawnOffset, bulletSpawn.rotation);
        BulletController bScript = b.GetComponent<BulletController>();
        bScript.maxDamage = gun.bulletMaxDamage;
        bScript.bulletDropoff = gun.bulletDropoff;
        bScript.originatingPlayer = this.transform.gameObject;
        bScript.life = gun.bulletLife;
        GetShotSpread(b);
        b.GetComponent<Rigidbody2D>().velocity = b.transform.right * gun.bulletSpeed;
        NetworkServer.Spawn(b);
        Destroy(b, gun.bulletLife);
        currentAmmo -= 1;
        timeToNextShot = 1/ gun.fireRate;
        currentSpreadCooldown = gun.spreadCooldown;
        currentSpreadInDegrees = gun.spreadPerShot;
    }
    
    bool CheckIfCanFire()
    {

        if (timeToNextShot <= 0 && currentAmmo > 0)
        {
            
            return true;
        }
        return false;

    }

    void HandleReloading()
    {

        if (!isReloading)
        {
            currentReloadTime = gun.reloadTime;
        }
        if (isReloading)
        {
            currentReloadTime -= Time.deltaTime;
        }
        if (currentReloadTime <= 0)
        {
            currentAmmo = gun.clipSize;
            isReloading = false;
        }
    }

    void HandleShotSpread()
    {
        if (currentSpreadInDegrees > gun.maxSpreadInDegrees)
        {
            currentSpreadInDegrees = gun.maxSpreadInDegrees;
        }
        currentSpreadCooldown -= Time.deltaTime;
        if (currentSpreadCooldown <= 0)
        {
            currentSpreadInDegrees = 0;
        }

    }

    void GetShotSpread(GameObject bullet)
    {

        bullet.transform.Rotate(0, 0, Random.Range(-currentSpreadInDegrees / 2, currentSpreadInDegrees / 2));
        
    }
    #endregion

    #region Handling Zoom

    #endregion

    #region Handling Changing Gun

    #endregion


}
