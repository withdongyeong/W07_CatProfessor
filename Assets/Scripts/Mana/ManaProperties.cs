using System.Collections.Generic;
using UnityEngine;

public static class ManaProperties
{
    public enum ManaType
    {
        None,
        Flame,
        Aqua,
        BLUEYELLOW,
        Volt,
        DesginMist,     
        DesginEmber,    
        REDYELLOW,      
        BLUERED,        
        DesginStorm,    
        DesginBio,      
        Neutral     // 중립 속성
    }

    public static readonly Dictionary<ManaType, Color> ManaColors = new Dictionary<ManaType, Color>
    {
        { ManaType.None, Color.white },
        { ManaType.Flame, new Color(1f, 0.23f, 0.1f) },   // 🔴 강한 붉은색
        { ManaType.Aqua, new Color(0.16f, 0.58f, 1f) },   // 🔵 깊은 푸른색
        { ManaType.BLUEYELLOW, new Color(0.2f, 0.8f, 0.2f) }, // 파랑 + 노랑 = 초록  
        { ManaType.Volt, new Color(1f, 0.9f, 0.16f) },    // 🟡 강렬한 노란색

        // 합성 속성 색상 추가
        { ManaType.DesginMist, new Color(0.7f, 0.7f, 1f) }, 
        { ManaType.DesginEmber, new Color(0.86f, 0.55f, 0f) },
        { ManaType.REDYELLOW, new Color(1f, 0.5f, 0f) },  // 빨강 + 노랑 = 주황
        { ManaType.BLUERED, new Color(0.5f, 0f, 0.5f) },  // 파랑 + 빨강 = 보라
        { ManaType.DesginStorm, new Color(0.12f, 0.78f, 0.78f) },
        { ManaType.DesginBio, new Color(0.7f, 1f, 0.2f) },

        // ✅ 중립 속성 색상 추가
        { ManaType.Neutral, new Color(1f, 1f, 1f) }
    };



    public static Color GetColor(ManaType type)
    {
        return ManaColors.TryGetValue(type, out Color color) ? color : Color.gray;
    }

    private static readonly Dictionary<(ManaType, ManaType), ManaType> ManaCombinations = new Dictionary<(ManaType, ManaType), ManaType>
    {
        { (ManaType.Flame, ManaType.Aqua), ManaType.BLUERED },
        { (ManaType.Aqua, ManaType.Flame), ManaType.BLUERED },

        //{ (ManaType.Flame, ManaType.Nature), ManaType.Ember },
        //{ (ManaType.Nature, ManaType.Flame), ManaType.Ember },

        { (ManaType.Flame, ManaType.Volt), ManaType.REDYELLOW },
        { (ManaType.Volt, ManaType.Flame), ManaType.REDYELLOW },

        //{ (ManaType.Aqua, ManaType.Nature), ManaType.Mud },
        //{ (ManaType.Nature, ManaType.Aqua), ManaType.Mud },

        { (ManaType.Aqua, ManaType.Volt), ManaType.BLUEYELLOW },
        { (ManaType.Volt, ManaType.Aqua), ManaType.BLUEYELLOW },

        //{ (ManaType.Nature, ManaType.Volt), ManaType.Bio },
        //{ (ManaType.Volt, ManaType.Nature), ManaType.Bio },

        // ✅ 중립 속성 변환 규칙 추가
        { (ManaType.Neutral, ManaType.Flame), ManaType.Flame },
        { (ManaType.Neutral, ManaType.Aqua), ManaType.Aqua },
        { (ManaType.Neutral, ManaType.Volt), ManaType.Volt },

        { (ManaType.Flame, ManaType.Neutral), ManaType.Flame },
        { (ManaType.Aqua, ManaType.Neutral), ManaType.Aqua },
        { (ManaType.Volt, ManaType.Neutral), ManaType.Volt },
    };

    public static ManaType CombineManaTypes(ManaType type1, ManaType type2)
    {
        if (type1 == type2) return type1; // 같은 속성끼리는 변화 없음

        // ✅ 중립 속성과 만나면 상대 속성을 반환
        if (type1 == ManaType.Neutral) return type2;
        if (type2 == ManaType.Neutral) return type1;

        return ManaCombinations.TryGetValue((type1, type2), out ManaType result) ? result : ManaType.None;
    }
}
