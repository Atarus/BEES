using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "ScriptableGun", menuName = "Gun", order = 1)]
public class Gun : ScriptableObject {

    public enum AmmoType
    {
        pistolAmmo, shotgunAmmo, smgAmmo, assaultAmmo, dmrAmmo, sniperAmmo
    }

    public AmmoType ammo;

    public Sprite gunSprite;
    public string gunName;
    public bool isAutomatic;
    public int  clipSize,  zoomRate;
    public float fireRate, reloadTime, maxSpreadInDegrees, spreadPerShot, spreadCooldown;
    public float bulletLife, bulletSpeed, bulletMaxDamage;

    public Vector3 bulletSpawnOffset;

    public AnimationCurve bulletDropoff;
}


