using UnityEngine;

public class LockableWall : MonoBehaviour
{
    public void ActivateWall()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateWall()
    {
        gameObject.SetActive(false);
    }
}
