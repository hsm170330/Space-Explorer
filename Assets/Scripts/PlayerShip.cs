using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 12f;
    [SerializeField] float _turnSpeed = 3f;
    [SerializeField] GameObject ShipThruster;
    [SerializeField] GameObject LeftThruster;
    [SerializeField] GameObject RightThruster;
    Rigidbody _rb = null;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
    public void Start()
    {
        ShipThruster.GetComponent<ParticleSystem>().Stop();
        LeftThruster.GetComponent<ParticleSystem>().Stop();
        RightThruster.GetComponent<ParticleSystem>().Stop();
    }
    private void FixedUpdate()
    {
        MoveShip();
        TurnShip();
    }

    private void Update()
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
        this.gameObject.SetActive(false);
    }
}
