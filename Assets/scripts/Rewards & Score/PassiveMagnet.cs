using UnityEngine;

public class PassiveMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float magnetRadius = 25.0f;
    public float pullSpeed = 25.0f;
    public BoostsHandler boostsHandler;
    
    // We will store the layer mask here automatically
    private int collectibleLayerMask;
    private Vector3 collectPosition;

    void Start()
    {
        // This automatically converts your string name into the correct layer mask number
        collectibleLayerMask = LayerMask.GetMask("Collectibles");
        
        if (collectibleLayerMask == 0)
        {
            Debug.LogWarning("Layer 'Collectibles' not found! Make sure it's spelled exactly right in your Tags & Layers.");
        }
    }

    void Update()
    {
        if (!boostsHandler.isPassiveMagnetOn) return;

        collectPosition = transform.position + new Vector3(0f, -3f, 0f);

        // Step 1: Sweep the area, ONLY looking at the Collectibles layer
        Collider[] itemsInRadius = Physics.OverlapSphere(collectPosition, magnetRadius, collectibleLayerMask);

        // Step 2 & 3: Pull them in
        foreach (Collider item in itemsInRadius)
        {
            float step = pullSpeed * Time.deltaTime;
            
            // Move the item directly toward the Player's exact position
            item.transform.position = Vector3.MoveTowards(item.transform.position, collectPosition, step);
        }
    }
}