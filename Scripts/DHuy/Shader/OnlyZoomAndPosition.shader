Shader "Custom/OnlyZoomAndPosition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ZoomAmount ("Zoom Amount", Float) = 1.0
        _PanOffset ("Pan Offset", Vector) = (0.5,0.5,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            float4 _MainTex_ST;
            float _ZoomAmount;
            float2 _PanOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Điều chỉnh tọa độ UV dựa trên tâm phóng to và offset
                float2 uv = _PanOffset + _ZoomAmount * (i.uv - _PanOffset);

                // Kiểm tra xem tọa độ UV có nằm trong khoảng [0,1] không để tránh lấy mẫu ngoài texture
               uv = frac(uv);

                // Lấy mẫu texture tại tọa độ UV đã điều chỉnh
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
