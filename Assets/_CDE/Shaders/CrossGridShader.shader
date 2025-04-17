Shader "Custom/Grid/Cross"
{
     Properties
    {
        _Color ("Dot Color", Color) = (1,1,1,1)
        _Spacing ("Dot Spacing", Float) = 1
        _Radius ("Dot Radius", Float) = 0.1
        _XOffset ("Dot X Offset", Float) = 0.0
        _YOffset ("Dot Y Offset", Float) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            fixed4 _Color;
            float _Spacing;
            float _Radius;
            float _XOffset;
            float _YOffset;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 cellPos = frac((i.worldPos.xy + float2(_XOffset, _YOffset)) / _Spacing);
                float2 offsetFromCenter = cellPos - 0.5;

                float thickness = _Radius; // 도트의 '두께'

                // 십자가 모양 만들기: 가로줄 또는 세로줄
                float horizontalBar = step(abs(offsetFromCenter.y), thickness);
                float verticalBar = step(abs(offsetFromCenter.x), thickness);

                float dot = max(horizontalBar, verticalBar); // 둘 중 하나라도 true면 십자가 도트

                fixed4 finalColor = _Color;
                finalColor.a *= dot;

                return finalColor;
            }
            ENDCG
        }
    }
}