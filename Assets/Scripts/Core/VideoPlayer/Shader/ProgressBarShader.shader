Shader "Unlit/ProgressBarShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 0.5
        _Color ("Color", Color) = (1, 1, 1, 1)
        _BgColor ("Background Color", Color) = (0.2, 0.2, 0.2, 0)
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
        ZWrite Off
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };
            
            sampler2D _MainTex;
            float _Progress;
            fixed4 _Color;
            fixed4 _BgColor;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
			fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);

                float2 pos = i.uv - 0.5;
                float2 pos0 = i.uv;

                float distx = abs(pos0.x - _Progress);
                
                if (pos0.x > _Progress)
                {
                    tex.rgb *= 1.6 - 1.0 / (0.65 + exp(-30 * distx));
                }
                
                tex.a *= i.color.a;
                
                return tex;
            }
            ENDCG
        }
    }
}
