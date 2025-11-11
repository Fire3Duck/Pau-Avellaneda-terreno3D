using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player; 
    public float height = 50f; 

    void LateUpdate()
    {
        if (player == null) return;

        
        Vector3 newPosition = player.position;
        newPosition.y += height;
        transform.position = newPosition;

        
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
