using UnityEngine;

public class BaldiWarningHide : MonoBehaviour
{
    public void ShowWarning ()
    {
        gameObject.SetActive(true);
        Invoke("HideBaldiWarning", 5f);
    }

    public void HideBaldiWarning()
    {
        gameObject.SetActive(false);   // hide the canvas
    }
}
