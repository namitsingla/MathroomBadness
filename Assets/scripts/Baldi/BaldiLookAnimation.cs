using UnityEngine;
using UnityEngine.UI;

public class BaldiLookAnimation : MonoBehaviour
{
    public Image lookImage;
    public Animator animator;

    public void baldiLook() 
    {
        lookImage.enabled = true;
        animator.SetTrigger("baldiLook");
    }

    public void OnBaldiLookAnimationEnd()
    {
        lookImage.enabled = false;
    }
}
