using DistilledGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    private Rigidbody2D rig;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier = 1.5f;  // multiplier for sprinting

    private Vector3 currentMoveDirection = Vector3.zero;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    #region Movement

    private void MovePlayer()
    {
        // Move using physics to allow collisions etc.
        float currentSpeed = PlayerInputHandler.Instance.Sprint ? moveSpeed * sprintMultiplier : moveSpeed; // Adjust the speed based on sprinting state
        rig.MovePosition(transform.position + (currentSpeed * Time.deltaTime * currentMoveDirection));
    }

    public void OnMove(Vector2 input)
    {
        currentMoveDirection = new Vector3(input.x, input.y, 0);
    }

    #endregion
}
