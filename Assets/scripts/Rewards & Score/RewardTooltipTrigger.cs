using UnityEngine;
using UnityEngine.EventSystems;

public class RewardTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // --- CHANGED: Now holds the whole reward object ---
    [HideInInspector] public Reward currentReward; 
    public RarityManager rarityManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentReward != null && !rarityManager.isSpinning)
        {
            // Pass the whole reward to the manager
            rarityManager.ShowTooltip(currentReward); 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rarityManager.HideTooltip();
    }
}