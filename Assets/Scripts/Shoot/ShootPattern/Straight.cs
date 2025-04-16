using UnityEngine;

[CreateAssetMenu(fileName = "ShootPattern1", menuName = "Mana System/Shoot Pattern/Straight")]
public class ShootPattern1 : ShootPatternConfig
{
    public override void ApplyPattern(Mana mana)
    {
        // 방향을 정규화하여 직선 발사
        mana.direction = mana.direction.normalized;
    }
}
