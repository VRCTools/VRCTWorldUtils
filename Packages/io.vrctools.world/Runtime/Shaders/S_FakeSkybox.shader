// Copyright 2025 .start <https://dotstart.tv>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
Shader ".start/World Utilities/Fake Skybox"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Cubemap (HDR)", Cube) = "grey" {}
        _Tint ("Tint", Color) = (.5, .5, .5, 1)
        [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
        _Rotation ("Rotation", Range(0, 360)) = 0

        [Enum(Off,0,On,1)] _ZWrite ("Z-Write", Float) = 1.0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("Z-Test", Float) = 4
        [Enum(UnityEngine.Rendering.CullMode)] _Culling ("Culling", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            Cull [_Culling]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_FOG_COORDS(1)
            };

            samplerCUBE _MainTex;
            half4 _MainTex_HDR;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(half4, _Tint)
                UNITY_DEFINE_INSTANCED_PROP(half, _Exposure)
                UNITY_DEFINE_INSTANCED_PROP(float, _Rotation)
            UNITY_INSTANCING_BUFFER_END(Props)

            float3 rotate_around_y_in_degrees(float3 vertex, float degrees)
            {
                float alpha = degrees * UNITY_PI / 180.0;
                float sina, cosa;
                sincos(alpha, sina, cosa);
                float2x2 m = float2x2(cosa, -sina, sina, cosa);
                return float3(mul(m, vertex.xz), vertex.y).xzy;
            }

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);

                UNITY_TRANSFER_FOG(o, o.vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half4 tint = UNITY_ACCESS_INSTANCED_PROP(Props, _Tint);
                half exposure = UNITY_ACCESS_INSTANCED_PROP(Props, _Exposure);
                float rotation = UNITY_ACCESS_INSTANCED_PROP(Props, _Rotation);

                half3 world_view_dir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                world_view_dir = rotate_around_y_in_degrees(world_view_dir, rotation);

                half4 tex = texCUBE(_MainTex, -world_view_dir);
                half3 c = DecodeHDR(tex, _MainTex_HDR);
                c = c * tint.rgb * unity_ColorSpaceDouble.rgb;
                c *= exposure;

                half4 color = half4(c, 1);
                UNITY_APPLY_FOG(i.fogCoord, color);
                return color;
            }
            ENDCG
        }
    }
    CustomEditor "VRCTools.World.Editor.Shaders.FakeSkyboxShaderEditor"
}