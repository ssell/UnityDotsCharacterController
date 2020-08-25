Shader "VertexFragment/CheckerGrid"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0.4, 0.4, 0.4, 1.0)
        _ColorB ("Color B", Color) = (0.2, 0.2, 0.2, 1.0)
        
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _ColorA;
        fixed4 _ColorB;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        float2 CheckerIntegral(float2 x)
        {
            x /= 2.0;
            return floor(x) + max(2.0 * frac(x) - 1.0, 0.0);
        }

        float Checkers(in float2 p)
        {
            float2 fw = max(abs(ddx(p)), abs(ddy(p))); 
            float w = max(fw.x, fw.y);

            float2 i = (CheckerIntegral(p + 0.5 * w) - CheckerIntegral(p - 0.5 * w)) / w;
            
            float c = i.x + i.y - (2.0 * i.x * i.y); 

            return c;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float checker = Checkers(IN.worldPos.xz);
            float3 color = lerp(_ColorA, _ColorB, checker);

            o.Albedo = color;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
