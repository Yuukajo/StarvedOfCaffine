using NUnit.Framework;
using PurrNet;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Look Settings")] 
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("References")]
    [SerializeField] private NetworkAnimator playerAnimator;
    [SerializeField] private List<Renderer> playerRenderers;
    [SerializeField] private Transform itemPoint;
    
    private Camera _playerCamera;
    private CharacterController characterController;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    public static PlayerMovement localPlayerMovement;

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;

        if (!isOwner)
           return;
        
        localPlayerMovement = this;
        _playerCamera = Camera.main;
        characterController = GetComponent<CharacterController>();
        SetShadowOnly();
        _playerCamera.transform.SetParent(transform);
        _playerCamera.transform.localPosition = cameraOffset;
        itemPoint.SetParent(_playerCamera.transform); 
        if (_playerCamera == null)
        {
            enabled = false;
            return;
        }
    }

    private void SetShadowOnly()
    {
        foreach(var renderer in playerRenderers)
        {
            renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }

    protected override void OnDespawned()
    {
        base.OnDespawned();

        if (!isOwner)
            return;

        localPlayerMovement = null;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        bool isGrounded = IsGrounded();
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        playerAnimator.SetFloat("Forward", vertical);
        playerAnimator.SetFloat("Right", horizontal);
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        _playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.03f, Vector3.down, groundCheckDistance);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.03f, Vector3.down * groundCheckDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.TransformPoint(cameraOffset), 0.1f);
    }
#endif
}