Shader "Custom/StereoSplit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off // Render both sides to support inside-out meshes like cylinders

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing // Essential for Single Pass Instanced on Quest

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                // unity_StereoEyeIndex is 0 for Left Eye, 1 for Right Eye
                // We assume the texture is Side-by-Side (SBS)
                // Left half of texture for Left Eye
                // Right half of texture for Right Eye

                float2 sbsUV = i.uv;

                // Map [0,1] to [0, 0.5] if Left Eye
                // Map [0,1] to [0.5, 1.0] if Right Eye
                sbsUV.x = sbsUV.x * 0.5 + unity_StereoEyeIndex * 0.5;

                fixed4 col = tex2D(_MainTex, sbsUV);
                return col;
            }
            ENDCG
        }
    }
}
