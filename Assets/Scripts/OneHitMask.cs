using UnityEngine;

public class OneHitMask : Mask
{
    protected override void OnGet(Player player)
    {
        player.AttackDamage = 3;
    }

    protected override void OnClose(Player player)
    {
        player.AttackDamage = 1;
    }
}
