using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 direction; // This vector stores where we want to move
    public float forwardSpeed = 10f; // Speed going forward

    // Lane Management
    private int desiredLane = 1; // 0 = Left, 1 = Middle, 2 = Right
    public float laneDistance = 3f; // The distance between two lanes
    public float laneChangeSpeed = 15f; // How fast we switch lanes smoothly

    // Gravity and Jumping
    public float jumpForce = 8f;
    public float gravity = -20f;

    // Sliding
    private bool isSliding = false;

    void Start()
    {
        // Grab the Character Controller we attached in Step 2
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. Move Forward Constantly
        direction.z = forwardSpeed;

        // 2. Handle Jumping and Gravity
        if (controller.isGrounded)
        {
            // Keep the player pushed slightly into the ground so 'isGrounded' stays true
            direction.y = -1f;

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                direction.y = jumpForce;
            }
        }
        else
        {
            // If we are in the air, apply gravity manually over time
            direction.y += gravity * Time.deltaTime;
        }

        // 3. Handle Sliding
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!isSliding)
            {
                StartCoroutine(Slide());
            }
        }

        // 4. Handle Lane Switching Inputs
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            desiredLane++;
            if (desiredLane == 3) desiredLane = 2; // Prevent going past the right lane
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            desiredLane--;
            if (desiredLane == -1) desiredLane = 0; // Prevent going past the left lane
        }

        // 5. Calculate Target Lane Position (X Axis)
        float targetXPosition = 0f;
        if (desiredLane == 0) targetXPosition = -laneDistance;
        else if (desiredLane == 2) targetXPosition = laneDistance;

        // 6. Calculate smooth left/right movement
        float movementX = targetXPosition - transform.position.x;
        direction.x = movementX * laneChangeSpeed;

        // 7. Finally, apply all this math to move the Character Controller
        controller.Move(direction * Time.deltaTime);
    }

    // Slide Coroutine (A mini-program that runs over time alongside the main update loop)
    private IEnumerator Slide()
    {
        isSliding = true;

        // Shrink the player's collision box so they can fit under obstacles
        controller.height = 1f;
        controller.center = new Vector3(0, 0.5f, 0);

        // Wait for 1 second in real-time
        yield return new WaitForSeconds(1f);

        // Grow the player back to normal size
        controller.height = 2f;
        controller.center = new Vector3(0, 1f, 0);

        isSliding = false;
    }
}