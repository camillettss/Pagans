using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/new torch")]
public class Torch : ItemBase
{
    public float brightness = 0.3f;
    public float radius = 0.5f;

    public override void onEquip()
    {
        // apply this rules to light
        var player = Player.i;
        player.torchLight.intensity = 0;
        player.torchLight.pointLightInnerRadius = radius;
        player.torchLight.pointLightOuterRadius = radius + 3f;
    }

    public override void Use(Player player)
    {
        // toggle light
        if (player.torchLight.intensity > 0)
            player.torchLight.intensity = 0;
        else
            player.torchLight.intensity = brightness;
    }
}