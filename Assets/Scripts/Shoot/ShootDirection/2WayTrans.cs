using UnityEngine;

[CreateAssetMenu(fileName = "DirectionPattern2WayTrans", menuName = "Mana System/Shoot Direction/2WayTrans")]
public class DirectionPattern2WayTrans : ShootDirectionConfig
{
    public override Vector2[] GetShootDirections() => new[]
    {
        new Vector2(1, 1).normalized,      
        new Vector2(-1, 1).normalized
    };
}
