using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GunController : NetworkBehaviour {

    #region Public Variables
    public GameObject bullet;
    public Transform bulletSpawn;

    public Text ammoText;

    Inventory inv;
    GunHolder currentGun;

    //TEST GUNS//

    public Gun g1, g2;
    #endregion

    #region Private Variables
    float timeToNextShot;

    bool isReloading = false;
    float currentReloadTime;


    float currentSpreadInDegrees;
    float currentSpreadCooldown;
    #endregion

    #region MonoBehaviour Functions
    // Use this for initialization
    void Start () {
       
        ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<Text>();
        inv = GetComponent<PlayerController>().inv;


        //TESTING REMOVE PLS//

        inv.gunOne = new GunHolder(true,g1,0);
        inv.gunTwo = new GunHolder(true,g2,0);
        currentReloadTime = inv.gunOne.gun.reloadTime;

        currentGun = inv.gunOne;
	}
	
	// Update is called once per frame
	void Update () {
        if (timeToNextShot > 0) { timeToNextShot -= Time.deltaTime;}
        if (currentGun.hasGun)
        {
            if (currentGun.gun.isAutomatic)
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
            else if (!currentGun.gun.isAutomatic)
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

            if (Input.GetKey(KeyCode.R) && currentGun.currentAmmo < currentGun.gun.clipSize && inv.CheckAmmoAmmount(currentGun.gun.ammo) > 0)
            {
                isReloading = true;
            }
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            isReloading = false;
            SwitchGun(1);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            isReloading = false;
            SwitchGun(2);
        }


        HandleReloading();
        HandleShotSpread();
        HandleUI();
    }
    #endregion

    #region Handling UI

    void HandleUI()
    {
        ammoText.text = currentGun.currentAmmo + "/" + currentGun.gun.clipSize;
    }

    #endregion

    #region Handling Firing


    [Command]
    void CmdFire()
    {

        GameObject b = Instantiate(bullet, bulletSpawn.position + currentGun.gun.bulletSpawnOffset, bulletSpawn.rotation);
        BulletController bScript = b.GetComponent<BulletController>();
        bScript.maxDamage = currentGun.gun.bulletMaxDamage;
        bScript.bulletDropoff = currentGun.gun.bulletDropoff;
        bScript.originatingPlayer = this.transform.gameObject;
        bScript.life = currentGun.gun.bulletLife;
        GetShotSpread(b);
        b.GetComponent<Rigidbody2D>().velocity = b.transform.right * currentGun.gun.bulletSpeed;
        NetworkServer.Spawn(b);
        Destroy(b, currentGun.gun.bulletLife);
        currentGun.currentAmmo -= 1;
        timeToNextShot = 1/ currentGun.gun.fireRate;
        currentSpreadCooldown = currentGun.gun.spreadCooldown;
        currentSpreadInDegrees = currentGun.gun.spreadPerShot;
    }
    
    bool CheckIfCanFire()
    {

        if (timeToNextShot <= 0 && currentGun.currentAmmo > 0)
        {
            
            return true;
        }
        return false;

    }

    void HandleReloading()
    {

        if (!isReloading)
        {
            currentReloadTime = currentGun.gun.reloadTime;
        }
        else
        {
            currentReloadTime -= Time.deltaTime;
        }
        if (currentReloadTime <= 0)
        {
            int ammoLeft = inv.CheckAmmoAmmount(currentGun.gun.ammo);
            if (ammoLeft >= (currentGun.gun.clipSize - currentGun.currentAmmo))
            {

                inv.ChangeAmmo(currentGun.gun.ammo, -(currentGun.gun.clipSize - currentGun.currentAmmo));
                currentGun.currentAmmo = currentGun.gun.clipSize;
            }
            else 
            {
                currentGun.currentAmmo += ammoLeft;
                inv.ChangeAmmo(currentGun.gun.ammo, -ammoLeft);
            }
            isReloading = false;
        }
    }

    void HandleShotSpread()
    {
        if (currentSpreadInDegrees > currentGun.gun.maxSpreadInDegrees)
        {
            currentSpreadInDegrees = currentGun.gun.maxSpreadInDegrees;
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

    void SwitchGun(int whichGun)
    {
        switch (whichGun)
        {
            case 1:   currentGun = inv.gunOne;  break;
            case 2:  currentGun = inv.gunTwo;   break;
        }
    }

    #endregion


}
