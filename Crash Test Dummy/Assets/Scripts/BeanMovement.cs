using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanMovement : MonoBehaviour
{
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 6.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;

    [SerializeField] bool lockCursor = true;

    float cameraPitch = 0.0f;
    float velocityY = 0.0f;
    CharacterController controller = null;

    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    int doubleJumpNum;
    public float jumpForce = 1.5f;
    Vector3 playerVelocity;
    public float gravityValue = -9.81f;

    void Start() {
        controller = GetComponent<CharacterController>();
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        doubleJumpNum = 2;
    }

    void Update() {
        UpdateMouseLook();
        UpdateMovement();
    }

    void UpdateMouseLook() {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement() {
        float sprintingMultiplier = 1.5f;
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        if(controller.isGrounded) {
            doubleJumpNum = 2;
        }   
        if(Input.GetKey(KeyCode.LeftShift)) {
            sprintingMultiplier++;
        }

        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;
        controller.Move(velocity * sprintingMultiplier * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && (controller.isGrounded || doubleJumpNum > 0))
        {
            playerVelocity.y = 0;
            playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
            doubleJumpNum--;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        if(playerVelocity.y < 0 && controller.isGrounded) {
            playerVelocity.y = 0;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
