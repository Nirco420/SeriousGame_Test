// Weissenhorn Toon-Shader fuer die Built-in Render Pipeline (klassisches 3D-Template)
// Nachbau des Blender-Stils "Toon weich": 3 Helligkeitsstufen, getoente Schatten,
// Lichtfarbe, Randlicht, optionale Farbpalette und Kontur (Inverted Hull).
// Anwenden: Material anlegen -> Shader "Weissenhorn/Toon Built-in" waehlen -> Textur in "Textur" ziehen.
Shader "Weissenhorn/Toon Built-in"
{
    Properties
    {
        [Header(Grundfarbe)]
        _MainTex ("Textur", 2D) = "white" {}
        _Color ("Farbe (wird multipliziert)", Color) = (1,1,1,1)

        [Header(Toon Schattierung)]
        _ShadowColor ("Schattenfarbe", Color) = (0.55,0.45,0.58,1)
        _ShadowStrength ("Schattenstaerke", Range(0,1)) = 0.7
        _MidLevel ("Helligkeit Mittelton", Range(0,1)) = 0.5
        _Threshold1 ("Grenze Schatten zu Mittelton", Range(0,1)) = 0.4
        _Threshold2 ("Grenze Mittelton zu Licht", Range(0,1)) = 0.8
        _Softness ("Weichheit der Kanten", Range(0.001,0.2)) = 0.02
        _LightTint ("Lichtfarbe", Color) = (1,0.94,0.82,1)
        _LightTintStrength ("Lichtfarbe Staerke", Range(0,1)) = 0.3
        [ToggleUI] _UseLightColor ("Farbe der Szenen-Sonne nutzen", Float) = 1
        _Ambient ("Umgebungslicht", Range(0,1)) = 0.2
        _ReceiveShadowAmount ("Schatten empfangen", Range(0,1)) = 1

        [Header(Feste Lichtrichtung wie in Blender)]
        [ToggleUI] _FixedLight ("Feste Lichtrichtung statt Sonne", Float) = 0
        _FixedLightDir ("Richtung (Welt, zeigt zum Licht)", Vector) = (0.4,0.8,0.45,0)

        [Header(Randlicht)]
        _RimColor ("Randlicht Farbe", Color) = (1,0.9,0.75,1)
        _RimStrength ("Randlicht Staerke", Range(0,1)) = 0.15
        _RimPower ("Randlicht Schaerfe", Range(0.5,8)) = 3

        [Header(Farbpalette  0 bedeutet Originalfarben)]
        _PaletteAmount ("Einfaerbung", Range(0,1)) = 0
        _P1 ("Palette 1 (dunkel)", Color) = (0.22,0.14,0.10,1)
        _P2 ("Palette 2", Color) = (0.50,0.25,0.17,1)
        _P3 ("Palette 3", Color) = (0.72,0.38,0.24,1)
        _P4 ("Palette 4", Color) = (0.86,0.74,0.52,1)
        _P5 ("Palette 5 (hell)", Color) = (0.95,0.92,0.85,1)

        [Header(Kontur  Breite 0 bedeutet aus)]
        _OutlineWidth ("Konturbreite (Meter)", Range(0,0.2)) = 0.03
        _OutlineColor ("Konturfarbe", Color) = (0.16,0.10,0.08,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }

        CGINCLUDE
        #include "UnityCG.cginc"
        sampler2D _MainTex; float4 _MainTex_ST;
        fixed4 _Color, _ShadowColor, _LightTint, _RimColor, _OutlineColor;
        fixed4 _P1, _P2, _P3, _P4, _P5;
        float4 _FixedLightDir;
        half _ShadowStrength, _MidLevel, _Threshold1, _Threshold2, _Softness;
        half _LightTintStrength, _UseLightColor, _Ambient, _ReceiveShadowAmount, _FixedLight;
        half _RimStrength, _RimPower, _PaletteAmount;
        float _OutlineWidth;
        ENDCG

        // ---------- Hauptpass: Toon-Beleuchtung ----------
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="ForwardBase" }
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct v2f
            {
                float4 pos  : SV_POSITION;
                float2 uv   : TEXCOORD0;
                float3 wpos : TEXCOORD1;
                float3 wn   : TEXCOORD2;
                SHADOW_COORDS(3)
                UNITY_FOG_COORDS(4)
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos  = UnityObjectToClipPos(v.vertex);
                o.uv   = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.wn   = UnityObjectToWorldNormal(v.normal);
                TRANSFER_SHADOW(o);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            half3 PaletteRamp (half l)
            {
                half t = saturate(l) * 4.0;
                half3 c = lerp(_P1.rgb, _P2.rgb, saturate(t));
                c = lerp(c, _P3.rgb, saturate(t - 1.0));
                c = lerp(c, _P4.rgb, saturate(t - 2.0));
                c = lerp(c, _P5.rgb, saturate(t - 3.0));
                return c;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half3 albedo = (tex2D(_MainTex, i.uv) * _Color).rgb;
                half lum = dot(albedo, half3(0.2126, 0.7152, 0.0722));
                #ifndef UNITY_COLORSPACE_GAMMA
                lum = pow(saturate(lum), 1.0 / 2.2);
                #endif
                albedo = lerp(albedo, PaletteRamp(lum), _PaletteAmount);

                float3 N = normalize(i.wn);
                float3 L = (_FixedLight > 0.5) ? normalize(_FixedLightDir.xyz) : normalize(_WorldSpaceLightPos0.xyz);
                half atten = lerp(1.0, SHADOW_ATTENUATION(i), _ReceiveShadowAmount);
                half x = saturate(dot(N, L) * 0.5 + 0.5) * atten;
                half band = _MidLevel * smoothstep(_Threshold1 - _Softness, _Threshold1 + _Softness, x)
                          + (1.0 - _MidLevel) * smoothstep(_Threshold2 - _Softness, _Threshold2 + _Softness, x);

                half3 shadowTint = lerp(half3(1,1,1), _ShadowColor.rgb * 0.6, _ShadowStrength);
                half3 lightTint  = lerp(half3(1,1,1), _LightTint.rgb, _LightTintStrength);
                half3 lightCol   = lerp(half3(1,1,1), _LightColor0.rgb, _UseLightColor);

                half3 col = albedo * lerp(shadowTint, lightTint, band) * lightCol;
                col += albedo * ShadeSH9(float4(N, 1)) * _Ambient;

                float3 V = normalize(_WorldSpaceCameraPos.xyz - i.wpos);
                half rim = pow(1.0 - saturate(dot(N, V)), _RimPower) * _RimStrength * band;
                col += _RimColor.rgb * rim;

                fixed4 c = fixed4(col, 1);
                UNITY_APPLY_FOG(i.fogCoord, c);
                return c;
            }
            ENDCG
        }

        // ---------- Kontur (Inverted Hull) ----------
        Pass
        {
            Name "OUTLINE"
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            struct v2f
            {
                float4 pos : SV_POSITION;
                UNITY_FOG_COORDS(0)
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 wn   = normalize(UnityObjectToWorldNormal(v.normal));
                wpos += wn * _OutlineWidth;
                o.pos = mul(UNITY_MATRIX_VP, float4(wpos, 1));
                if (_OutlineWidth <= 0.0001) o.pos = float4(0,0,0,0);   // Kontur aus
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = _OutlineColor;
                UNITY_APPLY_FOG(i.fogCoord, c);
                return c;
            }
            ENDCG
        }

        // ---------- Schatten werfen ----------
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    FallBack "Diffuse"
}
