using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupMini : MonoBehaviour
{
    [Header("Powerup Settings")]
    [SerializeField] float _powerupDuration = 5;

    [SerializeField] AudioClip _miniClip = null;

    [Header("Setup")]
    [SerializeField] GameObject _visualsToDeactivate = null;

    Collider _colliderToDeactivate = null;
    bool _poweredUp = false;
    
    private void Awake()
    {
        _colliderToDeactivate = GetComponent<Collider>();

        EnableObject();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerShip playerShip
            = other.gameObject.GetComponent<PlayerShip>();
        // if we have a valid player and not already powered up
        if (playerShip != null && _poweredUp == false)
        {
            // start powerup timer. Restart, if it's already started
            StartCoroutine(PowerupSequence(playerShip));
        }
    }

    IEnumerator PowerupSequence(PlayerShip playerShip)
    {
        // set boolean for detecting lockout
        _poweredUp = true;

        ActivatePowerup(playerShip);
        // simulate this object being disabled. We dont
        // REALLY want to disable it, because we still need
        // script behavior to continue functioning
        DisableObject();

        // wait for the required duration
        yield return new WaitForSeconds(_powerupDuration);
        // reset
        DeactivatePowerup(playerShip);
        EnableObject();

        // set boolean to release lockout
        _poweredUp = false;
    }

    void ActivatePowerup(PlayerShip playerShip)
    {
        if (playerShip != null)
        {
            // powerup player
            playerShip.setSize(0.5f);
            //Audio
            AudioHelper.PlayClip2D(_miniClip, 1);
        }
    }

    void DeactivatePowerup(PlayerShip playerShip)
    {
        // revert player powerup. -will subtract
        playerShip?.setSize(1f);
    }

    public void DisableObject()
    {
        // diable collider, so it can't be retriggered
        _colliderToDeactivate.enabled = false;
        // disable visuals, to simulate deactivated
        _visualsToDeactivate.SetActive(false);
        //TODO reactivate particle flash/audio

    }

    public void EnableObject()
    {
        // enable colider, so it can be retriggered
        _colliderToDeactivate.enabled = true;
        // enable visuals again, to drawe player attention
        _visualsToDeactivate.SetActive(true);
        //TODO reactivate particle flash/audio
    }
}
