using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 5f;
    private float moveSpeedMultiplier = 0.1f;

    void FixedUpdate()
    {
        characterController.Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * moveSpeed * moveSpeedMultiplier);
    }
}
