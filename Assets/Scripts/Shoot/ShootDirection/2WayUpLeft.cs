using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern2WayUpLeft", menuName = "Mana System/Shoot Direction/2WayUpLeft")]
public class DirectionPattern2WayUpLeft : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        Vector2.up,
        Vector2.left
    };
}
