// ChipStripes.shader
// Diagonal hatched stripes that scroll continuously. Drive visibility
// through the material color alpha (or CanvasGroup.alpha on the Image).
//
// Create a Material that uses this shader, assign it to the Stripes Image.
// The Image's source sprite can be the default UI sprite (or any — the
// shader ignores the texture and uses screen-UV stripes, so a 1x1 white
// sprite works fine).
//
// Exposed props:
//   _TintColor     (Color) — stripe color (default red-ish)
//   _StripeWidth   (Float) — px-ish width of one stripe
//   _StripeGap     (Float) — spacing between stripes
//   _Angle         (Float) — stripe angle in radians (default ~45°)
//   _Speed         (Float) — scroll speed (UV units / sec)

Shader "UI/ChipStripes"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _TintColor   ("Tint",         Color) = (1,1,1,1)
        _StripeWidth ("Stripe Width", Float) = 0.10
        _StripeGap   ("Stripe Gap",   Float) = 0.10
        _Angle       ("Angle (rad)",  Float) = 0.7854   // 45°
        _Speed       ("Speed",        Float) = 0.35

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil      ("Stencil ID", Float) = 0
        _StencilOp    ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask  ("Stencil Read Mask", Float) = 255
        _ColorMask    ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _TintColor;
            float  _StripeWidth;
            float  _StripeGap;
            float  _Angle;
            float  _Speed;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex   = UnityObjectToClipPos(v.vertex);
                o.color    = v.color;
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Rotate UVs, then mod by (width+gap) to make stripe bands.
                float2 uv = i.texcoord;
                float  s = sin(_Angle), c = cos(_Angle);
                float  u = uv.x * c - uv.y * s;

                // Scroll
                u += _Time.y * _Speed;

                float period = _StripeWidth + _StripeGap;
                float f      = fmod(u, period);
                if (f < 0) f += period;

                // antialias the stripe edge a touch
                float aa = fwidth(f) * 1.5;
                float a  = smoothstep(0.0, aa, f) * (1.0 - smoothstep(_StripeWidth - aa, _StripeWidth, f));

                fixed4 col = _TintColor * i.color;
                col.a *= a;
                return col;
            }
            ENDCG
        }
    }
}
