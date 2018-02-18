using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunController : NetworkBehaviour {


    public Gun gun;
    public GameObject bullet;
    public Transform bulletSpawn;

    float timeToNextShot;
    int currentAmmo;

    bool isReloading = false;
    float currentReloadTime;


    float currentSpreadtInDegrees;
    float currentSpreadCooldown;

    // Use this for initialization
    void Start () {
        currentReloadTime = gun.reloadTime;
	}
	
	// Update is called once per frame
	void Update () {
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
    }

    [Command]
    void CmdFire()
    {

        GameObject b = Instantiate(bullet, bulletSpawn.position + gun.bulletSpawnOffset, bulletSpawn.rotation);
        BulletController bScript = b.GetComponent<BulletController>();
        b.GetComponent<Rigidbody2D>().velocity = b.transform.right * gun.bulletSpeed;
        bScript.maxDamage = gun.bulletMaxDamage;
        bScript.bulletDropoff = gun.bulletDropoff;
        bScript.originatingPlayer = this.transform.gameObject;
        bScript.life = gun.bulletLife;
        NetworkServer.Spawn(b);
        Destroy(b, gun.bulletLife);
        currentAmmo -= 1;
        timeToNextShot = 1/ gun.fireRate;
        currentSpreadCooldown = gun.spreadCooldown;
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

    Quaternion GetShotSpread()
    {



        return bulletSpawn.rotation;
    }

}
