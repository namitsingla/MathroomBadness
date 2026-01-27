using UnityEngine;

public class BaldiStickStrike : MonoBehaviour
{
    public BaldiEnemy baldiEnemy;
    public AudioSource stickSound;
   public void StickStrike ()
    {
        baldiEnemy.ResumeMoving();
        stickSound.Play();
    }
}
