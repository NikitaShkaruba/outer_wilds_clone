using UnityEngine;

public class Player : MonoBehaviour
{
    private new Rigidbody rigidbody;

    private float verticalBodyRotation;
    private float horizontalBodyRotation;

    private const float Speed = 12000f;

    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        ProcessMovementKeys();
        ProcessCameraKeys();
    }

    private void ProcessMovementKeys()
    {
        bool wKeyPressed = Input.GetKey(KeyCode.W);
        bool aKeyPressed = Input.GetKey(KeyCode.A);
        bool sKeyPressed = Input.GetKey(KeyCode.S);
        bool dKeyPressed = Input.GetKey(KeyCode.D);
        bool isShiftPressed = Input.GetKey(KeyCode.LeftShift);
        bool isCtrlPressed = Input.GetKey(KeyCode.LeftControl);

        float forwardMotion = CalculateMotion(wKeyPressed, sKeyPressed);
        float sidewaysMotion = CalculateMotion(dKeyPressed, aKeyPressed);
        float yMotion = CalculateMotion(isShiftPressed, isCtrlPressed);
        
        Transform cachedTransform = transform;
        Vector3 playerMotion = cachedTransform.forward * forwardMotion +
                               cachedTransform.right * sidewaysMotion +
                               cachedTransform.up * yMotion;

        playerMotion *= Time.deltaTime;

        rigidbody.AddForce(playerMotion);
    }

    private static float CalculateMotion(bool oneDirectionKeyPressed, bool otherDirectionKeyPressed)
    {
        if (oneDirectionKeyPressed)
        {
            return Speed;
        } else if (otherDirectionKeyPressed)
        {
            return -Speed;
        }

        return 0f;
    }

    private void ProcessCameraKeys()
    {
        const float mouseSensitivity = 150f;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        verticalBodyRotation -= mouseY;
        horizontalBodyRotation += mouseX;

        // We don't want our player to roll over with the camera :)
        verticalBodyRotation = Mathf.Clamp(verticalBodyRotation, -90f, 90f);

        rigidbody.MoveRotation(Quaternion.Euler(verticalBodyRotation, horizontalBodyRotation, 0f));
    }
}