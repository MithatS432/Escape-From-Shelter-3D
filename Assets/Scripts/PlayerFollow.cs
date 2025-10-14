using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 1, -1);
    private void LateUpdate()
    {
        transform.position = player.position + offset;
    }
}
