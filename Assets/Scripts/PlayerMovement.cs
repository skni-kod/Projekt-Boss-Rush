using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotationSpeed = 10f;
    private Camera _camera;
    private Transform body;
    private float rotationSpeedMultiplier = 20f;
    private float moveSpeedMultiplier = 0.1f;

    void Start()
    {
        _camera = transform.Find("Main Camera").GetComponent<Camera>();
        body = transform.Find("Body");
    }

    void FixedUpdate()
    {
        // player movement
        characterController.Move(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * moveSpeed * moveSpeedMultiplier * Time.deltaTime);

        // rotate player relative to mouse position
        Vector3 playerPosOnScreen = _camera.WorldToScreenPoint(transform.position); // project Player's position to camera's 2d view
        playerPosOnScreen.z = 0;
        Vector3 direction = new Vector3(body.forward.x, body.forward.z, 0);
        Vector3 newDirection = Input.mousePosition - playerPosOnScreen; // get direction vector relative to player's on screen position
        float angle = Vector3.Angle(direction, newDirection);
        if (angle > 3) // rotation threshold
        {
            Vector3 cross = Vector3.Cross(direction, newDirection);
            if (cross.z < 0)
            {
                body.Rotate(0, rotationSpeedMultiplier * rotationSpeed * Time.deltaTime, 0);
            }
            else
            {
                body.Rotate(0, -rotationSpeedMultiplier * rotationSpeed * Time.deltaTime, 0);
            }
        }
    }
}
