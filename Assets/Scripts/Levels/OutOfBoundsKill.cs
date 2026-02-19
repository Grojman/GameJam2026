using UnityEngine;

public class OutOfBoundsKill : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnTriggerEnter2D(Collider2D collider2D)
    {
        Debug.Log("Ha entrado\n");
        var player = collider2D.GetComponent<Player>();
        if(player != null)
        {
            player.Kill();
        }
    }
}
