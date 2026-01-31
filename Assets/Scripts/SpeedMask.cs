using UnityEngine;

public class SpeedMask : Mask
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnGet(Player player)
    {
        player.Speed += 5;
    }

    protected override void OnClose(Player player)
    {
        player.Speed -= 5;
    }
}
