using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern1WayRight", menuName = "Mana System/Shoot Direction/1WayRight")]
public class DirectionPattern1WayRight : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        Vector2.right
    };
}
