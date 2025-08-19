Shader "Custom/DashedLine2D"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _DashSize ("Dash Length (UV)", Float) = 0.08
        _GapSize  ("Gap Length (UV)",  Float) = 0.06
        _Feather  ("Edge Feather",     Float) = 0.02
        _WidthFade("Width Fade (0-1)", Float) = 0.0
    }
    SubShader
    {
        Tags{ "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float  _DashSize;
            float  _GapSize;
            float  _Feather;
            float  _WidthFade;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0; // LineRenderer: x沿长度, y沿宽度
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 计算在一个“划+空”的周期中的位置
                float period = max(1e-4, _DashSize + _GapSize);
                float t = frac(i.uv.x / period); // 0..1 周期内

                // 划线区域阈值（0..dashFrac 显示）
                float dashFrac = saturate(_DashSize / period);

                // 羽化宽边（上下边缘）可选
                float width = abs(i.uv.y - 0.5) * 2.0; // 0 在中心，1 在边缘
                float widthMask = 1.0 - smoothstep(1.0 - _WidthFade, 1.0, width);

                // 计算划线的开关（带 Feather 的软裁切）
                float edge = smoothstep(dashFrac, dashFrac - _Feather, t); // t < dashFrac -> 1
                float alpha = edge * widthMask;

                // 丢弃空隙像素
                clip(alpha - 0.001);

                return fixed4(_Color.rgb, _Color.a * alpha);
            }
            ENDCG
        }
    }
    FallBack Off
}
