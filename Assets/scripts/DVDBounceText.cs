using UnityEngine;
using TMPro;

public class DVDBounceText : MonoBehaviour
{
    public float speed = 100f;

    private RectTransform rect;
    private TextMeshProUGUI text;
    private Vector2 direction;

    private static DVDBounceText[] allTexts;

    private float collisionCooldown = 0.1f;
    private float collisionTimer = 0f;

    private Vector2 screenBounds;

    [Header("Audio")]
    public AudioClip hitSound;
    private AudioSource audioSource;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<TextMeshProUGUI>();

        // Get the AudioSource, or add one if it doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; 
    }

    void Start()
    {
        // 1. Force TMP to calculate its size first
        text.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();

        // Random normalized direction
        direction = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        // Cache all bouncing texts
        allTexts = FindObjectsOfType<DVDBounceText>();

        // Cache screen bounds once
        RectTransform canvasRect = rect.root.GetComponent<RectTransform>();
        screenBounds = canvasRect.rect.size / 2f;

        Vector2 size = rect.rect.size;

        // 2. Add a small buffer so it never spawns mathematically ON the edge
        float spawnPadding = 5f; 

        float minX = -screenBounds.x + (size.x / 2f) + spawnPadding;
        float maxX = screenBounds.x - (size.x / 2f) - spawnPadding;
        float minY = -screenBounds.y + (size.y / 2f) + spawnPadding;
        float maxY = screenBounds.y - (size.y / 2f) - spawnPadding;

        // Safety check in case the screen is smaller than the text
        if (minX > maxX) { minX = 0; maxX = 0; }
        if (minY > maxY) { minY = 0; maxY = 0; }

        rect.anchoredPosition = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );
    }

    void Update()
    {
        collisionTimer -= Time.unscaledDeltaTime;

        rect.anchoredPosition += (Vector2)(direction * speed * Time.unscaledDeltaTime);

        CheckScreenBounds();
        CheckTextCollision();
    }

    void CheckScreenBounds()
    {
        Vector2 pos = rect.anchoredPosition;
        Vector2 size = rect.rect.size;
        bool bounced = false;

        if (pos.x + size.x / 2 > screenBounds.x)
        {
            direction.x *= -1;
            pos.x = screenBounds.x - size.x / 2; // Clamp it back inside
            bounced = true;
        }
        else if (pos.x - size.x / 2 < -screenBounds.x)
        {
            direction.x *= -1;
            pos.x = -screenBounds.x + size.x / 2; // Clamp it back inside
            bounced = true;
        }

        if (pos.y + size.y / 2 > screenBounds.y)
        {
            direction.y *= -1;
            pos.y = screenBounds.y - size.y / 2; // Clamp it back inside
            bounced = true;
        }
        else if (pos.y - size.y / 2 < -screenBounds.y)
        {
            direction.y *= -1;
            pos.y = -screenBounds.y + size.y / 2; // Clamp it back inside
            bounced = true;
        }

        if (bounced)
        {
            rect.anchoredPosition = pos; // Apply clamped position
            ChangeColor();
            PlayHitSound();
        }
    }

    void CheckTextCollision()
    {
        foreach (DVDBounceText other in allTexts)
        {
            if (other == this) continue;

            if (collisionTimer <= 0f && RectOverlaps(rect, other.rect))
            {
                // Swap directions
                Vector2 temp = direction;
                direction = other.direction;
                other.direction = temp;

                collisionTimer = collisionCooldown;

                rect.anchoredPosition += direction * 2f;
                other.rect.anchoredPosition += other.direction * 2f;

                ChangeColor();
                other.ChangeColor();
                PlayHitSound();
            }
        }
    }

    bool RectOverlaps(RectTransform a, RectTransform b)
    {
        Vector2 posA = (Vector2)a.anchoredPosition;
        Vector2 posB = (Vector2)b.anchoredPosition;

        Rect rectA = new Rect(
            posA - a.rect.size / 2f,
            a.rect.size);

        Rect rectB = new Rect(
            posB - b.rect.size / 2f,
            b.rect.size);

        return rectA.Overlaps(rectB);
    }

    void ChangeColor()
    {
        text.color = Color.HSVToRGB(Random.value, 1f, 1f);
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
}