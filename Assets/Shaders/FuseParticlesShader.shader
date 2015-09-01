Shader "Custom/FuseParticlesShader" {
	Properties {
		_MappingDomain ("mapping domain", Float) = 10.0
		_SphereRadius ("sphere radius", Float) = 5.0
		_DiffuseTex ("diffuse texture", 2D) = "white" {}
	}
	SubShader {
        /*
        so apparently it's not possible to write an additively blended
        surface shader. sigh...
        */

		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }	

        Pass {
            Cull Off
            ZWrite Off
            Blend SrcAlpha One

            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            
            float 		_MappingDomain;
            float		_SphereRadius;
            float4x4	_LocalToWorld;
            
            // general idea: sphere is "texture-mapped with vertices"
            
            // assumption: view matrix does only translate on z-axis
            
            static const float PI = 3.14159265;
            
            float3 Warp(float3 pos) {
                if(pos.x > _MappingDomain) pos.x -= 2.0 * _MappingDomain;
                if(pos.y > _MappingDomain) pos.y -= 2.0 * _MappingDomain;
                if(pos.x < -_MappingDomain) pos.x += 2.0 * _MappingDomain;
                if(pos.y < -_MappingDomain) pos.y += 2.0 * _MappingDomain;
                return pos;
            }
            
            float3 MapOnSphere(float3 pos) {
                float zOff = pos.z;
                
                float2 pos2 = (1.0 / _MappingDomain) * pos.xy;
                
                float r = min(length(pos2), 1.0);
                
                float phi = atan2(pos2.y, pos2.x);
                float theta = PI * r;
                
                return (_SphereRadius + zOff) * float3(
                    sin(theta) * cos(phi), sin(theta) * sin(phi), -cos(theta));
            }

            struct VS_Input {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct VS_Output {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };
            
            VS_Output vert(VS_Input IN) {
                float3 pos = mul(_LocalToWorld, float4(IN.vertex.xyz, 1.0)).xyz;
                pos = MapOnSphere(Warp(pos));

                pos.xy += 0.25 * IN.texcoord.xy;

                VS_Output output;
                output.vertex = mul(UNITY_MATRIX_MVP, float4(pos, 1.0));
                output.texcoord = 0.5 * (float2(1.0, 1.0) + IN.texcoord);
                output.color = IN.color;
                return output;
            }
            
            sampler2D   _DiffuseTex;
            
            float4 frag(VS_Output IN) : SV_Target {
                return IN.color * tex2D(_DiffuseTex, IN.texcoord);
            }
            
            ENDCG
        }
    }
	Fallback "Unlit/Color "
}
