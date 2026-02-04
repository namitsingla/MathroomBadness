using UnityEngine;

public class WallRunner : MonoBehaviour
{
    public player_controller player_Controller;
    public BoostsHandler boostsHandler;
    private void OnTriggerEnter(Collider collision)
    {
        if (!boostsHandler.isWallRunner) return;
        if (collision.GetComponent<Collider>().CompareTag("Walls"))
            player_Controller.moveSpeed *= 1.5f;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (!boostsHandler.isWallRunner) return;
        if (collision.GetComponent<Collider>().CompareTag("Walls"))
            player_Controller.moveSpeed /= 1.5f;
    }
}
