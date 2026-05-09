using UnityEngine;

public class PointCube : MonoBehaviour
{
    public int pointValue = 1;

    // This triggers the exact moment another collider touches this object
    void OnTriggerEnter(Collider other)
    {
        // Check if the thing touching it is the Player
        if (other.CompareTag("Player"))
        {
            // Find the GameManager and run the AddScore function
            FindObjectOfType<GameManager>().AddScore(pointValue);

            // Destroy this cube so it vanishes from the screen
            Destroy(gameObject);
        }
    }
}