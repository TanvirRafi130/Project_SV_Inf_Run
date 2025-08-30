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
    public bool IsGameStarted => isGameStarted;

    private static Player _instance;
    public static Player Instance => _instance;
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

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

        if (GameManager.Instance.GameState != GameState.Playing) return;

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
            MoveHorizontal(move);
        else
            StopHorizontal();

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
            Jump();

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            PullDown();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            StopHorizontal(); // No touch, so stop movement
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            touchStartPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended)
        {
            Vector2 delta = touch.position - touchStartPos;
            if (delta.magnitude < 50f)
            {
                StopHorizontal();
                return; // Ignore small swipes
            }

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                // Horizontal swipe
                if (delta.x > 0)
                    MoveHorizontal(1f); // Right
                else
                    MoveHorizontal(-1f); // Left
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

    // Movement functions
    void MoveHorizontal(float direction)
    {
        rb.linearVelocity = new Vector3(direction * moveSpeed, rb.linearVelocity.y, 0);
    }

    void StopHorizontal()
    {
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0);
        PlayJumpAnimation();
    }

    void PullDown()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -jumpForce*1.5f, 0);
    }
    bool isJumpAutoCalled = false;
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
            isJumpAutoCalled = false;
            PlayRunAnimation();
        }
        if (!isGrounded && !isJumpAutoCalled)
        {
            isJumpAutoCalled = true;
            PlayJumpAnimation();
        }
    }

    // Animation trigger methods
    public void PlayRunAnimation()
    {
        if (animator)
        {
            animator.ResetTrigger(jumpTrigger);
            animator.SetTrigger(runTrigger);
        }
    }

    public void PlayJumpAnimation()
    {
        if (animator)
        {
            animator.SetTrigger(jumpTrigger);
        }
    }

    public void PlayFallAnimation()
    {
        if (animator)
        {
            animator.SetTrigger(fallTrigger);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Obstacle>(out Obstacle obstacle))
        {
            // Handle collision with obstacle
            PlayFallAnimation();
            GameManager.Instance.SetGameState(GameState.GameOver);
        }
    }

}
