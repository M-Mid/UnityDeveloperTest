using UnityEngine;

public class KillZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // If the thing that touched this zone is the Player...
        if (other.CompareTag("Player"))
        {
            // Tell the GameManager that the player fell!
            FindObjectOfType<GameManager>().PlayerFell();
        }
    }
}