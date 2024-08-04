using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public float yOffset = 10f; // The height of the minimap camera above the player

    private void Update()
    {
        if (player != null)
        {
            // Update the position of the minimap camera to follow the player
            Vector3 newPosition = new Vector3(player.position.x, yOffset, player.position.z);
            transform.position = newPosition;

            // Rotate the minimap camera to match the player's rotation
            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}
