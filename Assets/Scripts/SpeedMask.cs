using UnityEngine;

public class SpeedMask : Mask
{
    protected override void OnGet(Player player)
    {
        player.Speed += 8;
    }

    protected override void OnClose(Player player)
    {
        player.Speed -= 8;
    }
}
