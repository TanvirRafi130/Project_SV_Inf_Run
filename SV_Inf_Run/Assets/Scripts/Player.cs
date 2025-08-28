using UnityEngine;
using NaughtyAttributes;

public class Player : MonoBehaviour
{



    [Header("Movement")]
    [SerializeField] Rigidbody rb;

    [Header("Animation")]
    [SerializeField] Animator animator;

    [SerializeField, AnimatorParam("animator")] string runTrigger;
    [SerializeField, AnimatorParam("animator")] string jumpTrigger;
    [SerializeField, AnimatorParam("animator")] string fallTrigger;
    [SerializeField, AnimatorParam("animator")] string runAnimationSpeedVariable;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 7f;
    [SerializeField] float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayer;

    private Vector2 touchStartPos;
    [SerializeField, ReadOnly] private bool isGrounded;

    bool isGameStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Check for first input to start the game
#if UNITY_IOS || UNITY_ANDROID
        if (!isGameStarted && Input.touchCount > 0)
            isGameStarted = true;
#else
        if (!isGameStarted && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
            isGameStarted = true;
#endif

        if (!isGameStarted) return;

#if UNITY_IOS || UNITY_ANDROID
        HandleTouchInput();
#else
        HandleKeyboardInput();
#endif
        CheckGrounded();
    }

    void HandleKeyboardInput()
    {
        float move = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            move = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            move = 1f;

        if (move != 0f)
            rb.linearVelocity = new Vector3(move * moveSpeed, rb.linearVelocity.y, 0);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
            Jump();

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            PullDown();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            touchStartPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            Vector2 delta = touch.position - touchStartPos;
            if (delta.magnitude < 50f) return; // Ignore small swipes

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                // Horizontal swipe
                if (delta.x > 0)
                    rb.linearVelocity = new Vector3(moveSpeed, rb.linearVelocity.y, 0); // Right
                else
                    rb.linearVelocity = new Vector3(-moveSpeed, rb.linearVelocity.y, 0); // Left
            }
            else
            {
                // Vertical swipe
                if (delta.y > 0 && isGrounded)
                    Jump();
                else if (delta.y < 0)
                    PullDown();
            }
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0);
        if (animator) animator.SetTrigger(jumpTrigger);
    }

    void PullDown()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -jumpForce, 0);
    }

    void CheckGrounded()
    {
        Vector3 pos = transform.position;
        pos.y += 0.1f;
        Ray ray = new Ray(pos, Vector3.down);
        Debug.DrawRay(pos, Vector3.down * groundCheckDistance, Color.red);
        bool wasGrounded = isGrounded;
        isGrounded = Physics.Raycast(ray, groundCheckDistance, groundLayer);

        if (isGrounded && !wasGrounded) // just landed
        {
            if (animator) animator.SetTrigger(runTrigger);
        }
    }

}
