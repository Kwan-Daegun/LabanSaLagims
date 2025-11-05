using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    [Header("throw settings")]
    public GameObject eggPrefab;
    public Transform throwPoint;
    public float throwForce = 10f;

    [Header("aiming settings")]
    public float aimSpeed = 90f;
    public float maxAngle = 75f;
    public float minAngle = -15f;

    [Header("movement settings")]
    public float moveSpeed = 5f;

    [Header("movement control")]
    public bool stopPlayerMovement = false;

    [Header("shoot control")]
    public bool canShoot = false; // shooting enabled only after trigger/cutscene

    [Header("ui settings")]
    public TextMeshProUGUI angleText;
    public TextMeshProUGUI eggsLeftText;

    [Header("throw limits")]
    public int maxEggs = 15;
    private int eggsThrown = 0;

    [Header("animation")]
    [SerializeField] private Animator animator;

    [Header("arrow aiming")]
    public GameObject arrowPrefab;       // assign arrow prefab (SpriteRenderer) in Inspector
    private GameObject arrowInstance;    // runtime arrow

    private float currentAngle;
    private bool goingUp = true;
    private Rigidbody2D eggRBTemplate;
    private Rigidbody2D rb;
    private GameObject currentEgg;
    private Vector3 originalScale;

    void Start()
    {
        Time.timeScale = 1;
        rb = GetComponent<Rigidbody2D>();

        // store whatever scale was set in the editor
        originalScale = transform.localScale;

        // cache rigidbody from prefab
        eggRBTemplate = eggPrefab.GetComponent<Rigidbody2D>();

        // start angle at midpoint
        currentAngle = (maxAngle + minAngle) * 0.5f;

        UpdateEggsUI();

        // hide ui until cutscene ends
        if (angleText != null) angleText.gameObject.SetActive(false);

        // spawn arrow but keep hidden until aiming
        if (arrowPrefab != null)
        {
            arrowInstance = Instantiate(arrowPrefab, throwPoint.position, Quaternion.identity, transform);
            arrowInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (!canShoot) return;

        HandleAiming();

        // throw input
        if (Input.GetKeyDown(KeyCode.Space) && eggsThrown < maxEggs && currentEgg == null)
        {
            animator.SetTrigger("ThrowTrigger"); // animation will call ThrowEgg
        }
    }

    void FixedUpdate()
    {
        if (!stopPlayerMovement) HandleMovement();
    }

    void HandleMovement()
    {
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        // flip using localscale, keeps size consistent
        if (move > 0.01f)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (move < -0.01f)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        animator.SetBool("IsWalking", Mathf.Abs(move) > 0.01f);
    }

    void HandleAiming()
    {
        // enable ui when cutscene ends
        if (angleText != null && !angleText.gameObject.activeSelf) angleText.gameObject.SetActive(true);

        if (arrowInstance != null && !arrowInstance.activeSelf)
            arrowInstance.SetActive(true);

        // bounce angle
        currentAngle += (goingUp ? 1 : -1) * aimSpeed * Time.deltaTime;
        if (currentAngle >= maxAngle) { currentAngle = maxAngle; goingUp = false; }
        else if (currentAngle <= minAngle) { currentAngle = minAngle; goingUp = true; }

        // update angle text above throwpoint
        if (angleText != null)
        {
            Vector3 offset = new Vector3(0f, 0.5f, 0);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(throwPoint.position + offset);
            angleText.rectTransform.position = screenPos;
            angleText.rectTransform.rotation = Quaternion.identity;
            angleText.text = Mathf.RoundToInt(currentAngle) + "°";
        }

        // update arrow position and rotation
        if (arrowInstance != null)
        {
            Vector2 dir = GetThrowDirection();
            arrowInstance.transform.position = throwPoint.position;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

            
            Vector3 scale = arrowInstance.transform.localScale;
            scale.x = (transform.localScale.x < 0) ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            arrowInstance.transform.localScale = scale;
        }
    }

    Vector2 GetThrowDirection()
    {
        float adjustedAngle = (transform.localScale.x < 0)
            ? 180f - currentAngle  // mirrored
            : currentAngle;

        return (Quaternion.Euler(0, 0, adjustedAngle) * Vector2.right).normalized;
    }

    void ThrowEgg()
    {
        if (currentEgg != null || eggsThrown >= maxEggs) return;

        currentEgg = Instantiate(eggPrefab, throwPoint.position, Quaternion.identity);

        if (currentEgg.TryGetComponent(out Rigidbody2D eggRB))
        {
            Physics2D.IgnoreCollision(currentEgg.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            eggRB.velocity = GetThrowDirection() * throwForce;
        }

        // tell camera to follow
        FindObjectOfType<CamControl>()?.FollowBullet(currentEgg);

        eggsThrown++;
        UpdateEggsUI();
        StartCoroutine(WatchEggDestroyed(currentEgg));
    }

    IEnumerator WatchEggDestroyed(GameObject egg)
    {
        while (egg != null) yield return null;
        currentEgg = null;
    }

    void UpdateEggsUI()
    {
        if (eggsLeftText != null)
            eggsLeftText.text = $"Bawang Left: {maxEggs - eggsThrown}";
    }
}
