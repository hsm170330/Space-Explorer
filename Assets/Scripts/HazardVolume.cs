using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // detect if it's the plate
        PlayerShip playerShip = other.gameObject.GetComponent<PlayerShip>();
        // if we found something valid continue
        if (playerShip != null)
        {
            // do something!
            playerShip.Kill();
        }
    }    
}
