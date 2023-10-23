using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;
using UnityEngine.Windows;

public class BearController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier = 1.5f;  // multiplier for sprinting

    private bool isSprinting = false;  // Track if the bear is currently sprinting
    private Vector3 currentMoveDirection = Vector3.zero;

    void Update()
    {
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;  // Adjust the speed based on sprinting state
        transform.position += currentSpeed * Time.deltaTime * currentMoveDirection;
    }

    public void OnMove(Vector2 input)
    {
        currentMoveDirection = new Vector3(input.x, input.y, 0);
    }

    public void Moving(bool sprint)
    {
        isSprinting = sprint;
    }

    public void StopMove()
    {
        currentMoveDirection = Vector3.zero;
    }
}

