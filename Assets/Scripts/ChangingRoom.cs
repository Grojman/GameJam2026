using UnityEngine;

public class ChangingRoom : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        var player = coll.GetComponent<Player>();
        if(player != null)
        {
            player.canChange = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        var player = coll.GetComponent<Player>();
        if(player != null)
        {
            Debug.Log("NO PUede\n");

            player.canChange = false;
        }
    }
}
