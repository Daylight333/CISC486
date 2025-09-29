using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 2.0f;
    public Rigidbody rb;
    private Vector3 moveDir = new Vector3(0,0,0);
    private Vector3 viewCamDir;
    private float horizontal;
    private float vertical;
    private Vector3 camForward;
    private Vector3 camRight;

    public Transform virtualCamera;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get the direction the camera is looking
        viewCamDir = virtualCamera.forward;

        // Flatten it so the character only rotates on the XZ plane
        viewCamDir.y = 0;

        // Only rotate if direction is valid
        if (viewCamDir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(viewCamDir);
        }

         // A/D or Left/Right arrows
        horizontal = Input.GetAxis("Horizontal");

        // W/S or Up/Down arrows
        vertical   = Input.GetAxis("Vertical");

        // Get camera forward and right directions (flattened to XZ plane)
        camForward = new Vector3(virtualCamera.forward.x, 0, virtualCamera.forward.z).normalized;
        camRight   = new Vector3(virtualCamera.right.x, 0, virtualCamera.right.z).normalized;

        // Combine input with camera directions
        moveDir = (camForward * vertical + camRight * horizontal).normalized;

        // Move player
        rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
    }
}
