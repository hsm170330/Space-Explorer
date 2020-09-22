using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 12f;
    [SerializeField] float _turnSpeed = 3f;

    [Header("Feedback")]
    [SerializeField] TrailRenderer _trail = null;

    [SerializeField] GameObject ShipThruster = null;
    [SerializeField] GameObject LeftThruster = null;
    [SerializeField] GameObject RightThruster = null;

    [Header("Setup")]
    [SerializeField] GameObject _visualsToDeactivate = null;

    [SerializeField] AudioClip _explosion = null;
    [SerializeField] AudioClip _coin = null;

    Rigidbody _rb = null;

    private int count;
    private Boolean isDead;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject collectTextObject;
    public GameObject Respawning;
    public TextMeshProUGUI respawnText = null;
    private Vector3 spawn;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _trail.enabled = false;
    }
    public void Start()
    {
        ShipThruster.GetComponent<ParticleSystem>().Stop();
        LeftThruster.GetComponent<ParticleSystem>().Stop();
        RightThruster.GetComponent<ParticleSystem>().Stop();
        count = 0;
        isDead = false;

        SetCountText();
        winTextObject.SetActive(false);
        Respawning.SetActive(false);

        spawn = transform.position;
    }
    private void FixedUpdate()
    {
        if (!isDead)
        {
            MoveShip();
            TurnShip();
        }

        if (count >= 1)
        {
            collectTextObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isDead)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ShipThruster.GetComponent<ParticleSystem>().Play();
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                ShipThruster.GetComponent<ParticleSystem>().Stop();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                LeftThruster.GetComponent<ParticleSystem>().Play();
                RightThruster.GetComponent<ParticleSystem>().Play();
            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                LeftThruster.GetComponent<ParticleSystem>().Stop();
                RightThruster.GetComponent<ParticleSystem>().Stop();
            }
        }
        else if (isDead)
        {
            _rb.velocity = new Vector3(0, 0, 0);
            StartCoroutine(RespawnPlayer());
        }
        
    }
    // use forces to build momentum forward/backward
    void MoveShip()
    {
        // S/Down = -1, W/Up = 1, None = 0. Scale by moveSpeed
        float moveAmountThisFrame = Input.GetAxisRaw("Vertical") * _moveSpeed;
        // combine out direction with our calculated amount
        Vector3 moveDirection = transform.forward * moveAmountThisFrame;
        // apply the movement to the physics object
        _rb.AddForce(moveDirection);
    }
    void StopParticles()
    {
        ShipThruster.GetComponent<ParticleSystem>().Stop();
        LeftThruster.GetComponent<ParticleSystem>().Stop();
        RightThruster.GetComponent<ParticleSystem>().Stop();
    }

    // don't use forces for this. We want rotations to be precise
    void TurnShip()
    {
        // A/Left = -1, D/Right = 1, None = 0. Scale by turnSpeed
        float turnAmountThisFrame = Input.GetAxisRaw("Horizontal") * _turnSpeed;
        // specify an axis to apply our turn amount  (x, y, z) as a roatation
        Quaternion turnOffset = Quaternion.Euler(0, turnAmountThisFrame, 0);
        // spin the rigidbody
        _rb.MoveRotation(_rb.rotation * turnOffset);
    }
    public void Kill()
    {
        Debug.Log("Player has been killed!");
        _visualsToDeactivate.SetActive(false);
        AudioHelper.PlayClip2D(_explosion, 1);
        isDead = true;
    }

    IEnumerator RespawnPlayer()
    {
        Respawning.SetActive(true);
        respawnText = Respawning.GetComponent<TextMeshProUGUI>();

        respawnText.text = "Respawning...";
        yield return new WaitForSeconds(3);
        isDead = false;
        _visualsToDeactivate.SetActive(true);
        transform.position = spawn;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        StopParticles();
        
        Respawning.SetActive(false);
    }

    public void SetSpeed(float speedChange)
    {
        _moveSpeed += speedChange;
        //TODO audio/visuals
    }

    public void setSize(float size)
    {
        transform.localScale = new Vector3(size, size, size);
    }

    public void SetBoosters(bool activeState)
    {
        _trail.enabled = activeState; //Side Chesto
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
            AudioHelper.PlayClip2D(_coin, 1);
        }
    }
    void SetCountText()
    {
        countText.text = "Coins: " + count.ToString() + "/16";
        if (count >= 16)
        {
            winTextObject.SetActive(true);
        }
    }
}
