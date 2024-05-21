Shader "Unlit/Credits"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tex2 ("Texture", 2D) = "white" {}
        _Tex3 ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Tex2;
            sampler2D _Tex3;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _CreditWipe = 0.0f;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            	float2 offset = i.uv - float2(0.5, 0.5);
	            offset.y *= _MainTex_TexelSize.w / _MainTex_TexelSize.z; 
	            float radius = length(offset) * 4.0 - _CreditWipe + 0.5;
	            float zone = 0.0;
	            if (radius < -1.0)
		            zone = (-radius - 1.0) % 3.0;
	            else if (radius < -0.98)
		            return fixed4(0, 0, 0, 1);
	            else
	                return tex2D(_Tex3, i.uv); 
	            if (zone < 0.98)
		            return tex2D(_MainTex, i.uv);
	            else if (zone < 1.0)
		            return fixed4(0, 0, 0, 1);
	            else if (zone < 1.98)
		            return tex2D(_Tex2, i.uv);
	            else if (zone < 2.0)
		            return fixed4(0, 0, 0, 1);
	            else if (zone < 2.98)
		            return tex2D(_Tex3, i.uv);
	            else if (zone < 3.0)
		            return fixed4(0, 0, 0, 1);
                return tex2D(_Tex3, i.uv);


                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
