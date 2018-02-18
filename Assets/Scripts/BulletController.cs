using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Animations;

public class BulletController : NetworkBehaviour {

    
    public GameObject originatingPlayer;

    public AnimationCurve bulletDropoff;
    public float life, timeAlive = 0, maxDamage;

	
	
	// Update is called once per frame
	void Update () {
        timeAlive += Time.deltaTime;
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" && col.gameObject != originatingPlayer)
        {
            int damage = (int)(bulletDropoff.Evaluate(timeAlive / life) * maxDamage);
            col.GetComponent<PlayerController>().currentHealth -= damage;
            Destroy(this.gameObject);
        }
        else if (col.tag == "Obstacle")
        {
            Destroy(this.gameObject);
        }
    }

}
