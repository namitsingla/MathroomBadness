using System.Collections;
using UnityEngine;

public class ShockwaveEffect : MonoBehaviour
{
    [Header("Shockwave Settings")]
    [Tooltip("How large the sphere will get before disappearing.")]
    public float maxRadius = 15f;
    
    [Tooltip("How long the effect lasts in seconds.")]
    public float duration = 0.5f;
    
    [Tooltip("Starting alpha (opacity) of the shockwave. 1 is solid, 0 is invisible.")]
    public float startingOpacity = 0.5f;

    private Material material;
    private Color baseColor;

    void Start()
    {
        // 1. Grab the material from the MeshRenderer
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            // By accessing .material, Unity creates an instance so we don't alter the original project asset
            material = rend.material;
            baseColor = material.color;
        }

        // 2. Start the sphere at zero scale so it bursts outward
        transform.localScale = Vector3.zero;

        // 3. Kick off the animation sequence
        StartCoroutine(AnimateShockwave());
    }

    private IEnumerator AnimateShockwave()
    {
        float elapsedTime = 0f;
        Vector3 initialScale = Vector3.zero;
        Vector3 finalScale = new Vector3(maxRadius, maxRadius, maxRadius);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            // Calculate a 0 to 1 value representing how far along we are
            float progress = elapsedTime / duration;

            // Expand the sphere smoothly
            transform.localScale = Vector3.Lerp(initialScale, finalScale, progress);

            // Fade the opacity out smoothly
            if (material != null)
            {
                float currentAlpha = Mathf.Lerp(startingOpacity, 0f, progress);
                material.color = new Color(baseColor.r, baseColor.g, baseColor.b, currentAlpha);
            }

            // Wait for the next frame
            yield return null;
        }

        // Once the animation is done, destroy this game object
        Destroy(gameObject);
    }
}