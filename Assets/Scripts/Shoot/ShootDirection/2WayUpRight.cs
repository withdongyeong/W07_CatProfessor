using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern2WayUpRight", menuName = "Mana System/Shoot Direction/2WayUpRight")]
public class DirectionPattern2WayUpRight : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        Vector2.up,
        Vector2.right
    };
}
