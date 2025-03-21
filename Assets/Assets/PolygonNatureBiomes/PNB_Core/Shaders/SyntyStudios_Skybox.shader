Shader "SyntyStudios/SkyboxUnlitOptimized"
{
    Properties
    {
        _ColorTop("Color Top", Color) = (0,1,0.7517242,0)
        _ColorBottom("Color Bottom", Color) = (0.8161765,0.9087222,1,0)
        _Offset("Offset", Float) = 0
        _Distance("Distance", Float) = 0
        _Falloff("Falloff", Range(0.001, 100)) = 0.001
        _UV_Based_Toggle("UV Based Toggle", Float) = 0  // Объявляем свойство
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+0" "IsEmissive" = "true" }
        Cull Back

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Unlit keepalpha noshadow

        struct Input
        {
            float3 worldPos;
            float2 uv_texcoord;
        };

        uniform float4 _ColorBottom;
        uniform float4 _ColorTop;
        uniform float _Offset;
        uniform float _Distance;
        uniform float _Falloff;
        uniform float _UV_Based_Toggle; // Используем этот параметр в коде

        inline half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
        {
            return half4(0, 0, 0, s.Alpha);
        }

        void surf(Input i, inout SurfaceOutput o)
        {
            float3 worldPos = i.worldPos;
            float clampValue = clamp((_Offset + worldPos.y) / _Distance, 0.0, 1.0);
            float interpolation = saturate(pow(clampValue, _Falloff));
            
            // Определение цвета с использованием одного lerp
            float4 color = lerp(_ColorBottom, _ColorTop, interpolation);

            // Используем значение _UV_Based_Toggle для выбора цвета
            o.Emission = (_UV_Based_Toggle == 1.0) ? lerp(_ColorBottom, _ColorTop, i.uv_texcoord.y).rgb : color.rgb;
            o.Alpha = 1;
        }

        ENDCG
    }
    CustomEditor "ASEMaterialInspector"
}
