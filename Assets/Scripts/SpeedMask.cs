using UnityEngine;

public class SpeedMask : Mask
{
    protected override void OnGet(Player player)
    {
        player.Speed += 20;
    }

    protected override void OnClose(Player player)
    {
        player.Speed -= 20;
    }
}
