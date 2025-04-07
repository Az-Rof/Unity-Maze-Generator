using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    private Rigidbody rb;
    PlayerController_UI pui;

    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float mouseSensitivity = 2f;

    private bool isGrounded;
    private float xRotation = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pui = GetComponent<PlayerController_UI>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked; 
    }

    void Update()
    {
        MovePlayer();
        LookAround();
        Jump();
    }

    void MovePlayer()
    {
        switch (Input.GetKey(KeyCode.LeftShift))
        {
            case true:
                walkSpeed = 3.5f;
                pui.currentStamina -= Time.deltaTime * 2f;
                break;
            case false:
                walkSpeed = 1.5f;
                break;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;
        Vector3 newVelocity = new Vector3(move.x * walkSpeed, rb.velocity.y, move.z * walkSpeed);
        rb.velocity = newVelocity;
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Batas rotasi vertikal

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    

}
