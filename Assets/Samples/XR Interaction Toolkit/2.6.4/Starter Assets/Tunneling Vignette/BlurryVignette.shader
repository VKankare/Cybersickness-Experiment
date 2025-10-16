Shader "VR/BlurryVignette"
{
    Properties
    {
        _ApertureSize("Aperture Size", Range(0,1)) = 0.7
        _FeatheringEffect("Feathering Effect", Range(0,1)) = 0.2
        _BlurSize("Blur Size", Range(0,10)) = 4.0
        _Iterations("Blur Iterations", Range(1,4)) = 2
        _Downsample("Downsample Factor", Range(1,4)) = 2
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            // Opaque texture (scene color buffer)
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);
            float4 _CameraOpaqueTexture_TexelSize;

            float _ApertureSize;
            float _FeatheringEffect;
            float _BlurSize;
            int _Iterations;
            int _Downsample;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);

                // Normalized screen-space UVs (0–1)
                float4 clipPos = o.pos / o.pos.w;
                o.uv = clipPos.xy * 0.5 + 0.5;

                return o;
            }

            float4 Blur1D(float2 uv, float2 dir)
            {
                float2 offset = dir * _CameraOpaqueTexture_TexelSize.xy * _BlurSize * _Downsample;
                float w[5] = {0.204,0.304,0.093,0.023,0.004};
                float4 col = 0;
                [unroll]
                for(int i=-4; i<=4; i++)
                {
                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv + offset*i) * w[abs(i)];
                }
                return col;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float4 original = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv);
                float4 blurred = original;

                for(int it=0; it<_Iterations; it++)
                {
                    blurred = Blur1D(uv, float2(1,0));
                    blurred = Blur1D(uv, float2(0,1));
                }

                // --- Radial mask ---
                float2 center = float2(0.5, 0.5);
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 scaledUV = uv - center;
                scaledUV.x *= aspect;
                float dist = length(scaledUV);

                float radius = _ApertureSize * 0.5;
                float feather = _FeatheringEffect * 0.5;
                float mask = saturate((dist - radius) / (feather + 1e-5));

                return lerp(original, blurred, mask);
            }
            ENDHLSL
        }
    }
}