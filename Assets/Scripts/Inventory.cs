using System.Collections;
using System.Collections.Generic;

public class Inventory  {

    public int pistolAmmo, maxPistolAmmo;
    public int smgAmmo, maxsmgAmmo;
    public int assaultRifleAmmo, maxAssaultRifleAmmo;
    public int dmrAmmo, maxDmrAmmo;
    public int sniperAmmo, maxSniperAmmo;
    public int bandages, maxBandages;
    public int stimPacks, maxStims;
    public int royalJellies, maxRoyalJellies;
    
    public int armourLevel;

    public GunHolder gunOne ,gunTwo ;

    public int CheckAmmoAmmount(Gun.AmmoType type)
    {
        switch (type)
        {
            case Gun.AmmoType.pistolAmmo: return pistolAmmo;
            case Gun.AmmoType.smgAmmo:return smgAmmo;
            case Gun.AmmoType.assaultAmmo:return assaultRifleAmmo;
            case Gun.AmmoType.dmrAmmo:return dmrAmmo;
            case Gun.AmmoType.sniperAmmo:return sniperAmmo;
            default: return -1;
        }

            
    }

    public void ChangeAmmo(Gun.AmmoType type, int amount)
    {
        switch (type)
        {
            case Gun.AmmoType.pistolAmmo: pistolAmmo += amount; break;
            case Gun.AmmoType.smgAmmo: smgAmmo += amount; break;
            case Gun.AmmoType.assaultAmmo: assaultRifleAmmo += amount; break;
            case Gun.AmmoType.dmrAmmo: dmrAmmo += amount; break;
            case Gun.AmmoType.sniperAmmo: sniperAmmo += amount; break;
        }
    }


}
