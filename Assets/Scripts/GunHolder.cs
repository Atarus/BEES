using System.Collections;
using System.Collections.Generic;

public class GunHolder  {


   public bool hasGun = false;
   public Gun gun;
   public int currentAmmo = 0;

    public GunHolder(bool hg, Gun g, int ca)
    {
        hasGun = hg;
        gun = g;
        currentAmmo = ca;
    }
}
