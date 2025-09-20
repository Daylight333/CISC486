using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float speed = 2.0f;
    private Rigidbody rb;
    private float movex = 0f;
    private float movez = 0f;
    private Vector3 move = new Vector3(0,0,0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movex = 0f;
        movez = 0f;
        move = new Vector3(0,0,0);
        if (Input.GetKey("w")){
            movez = 1f;
        }
        if (Input.GetKey("a")){
            movex = -1f;
        }
        if (Input.GetKey("s")){
            movez = -1f;
        }
        if (Input.GetKey("d")){
            movex = 1f;
        }

        move = new Vector3(movex, 0f, movez).normalized;

        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);

    }
}
