Shader "Unlit/IcesoilgrassShader"
{
	Properties
	{
		_PointLightColor("Point Light Color", Color) = (0, 0, 0)
		_PointLightPosition("Point Light Position", Vector) = (0.0, 0.0, 0.0)
	}
		SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

    uniform float3 _PointLightColor;
    uniform float3 _PointLightPosition;

	struct vertIn
	{
		float4 vertex : POSITION;
		float4 normal : NORMAL;
		float4 color : COLOR;
	};

	struct vertOut
	{
		float4 vertex : SV_POSITION;
		float4 color : COLOR;
	};

	// Implementation of the vertex shader
	vertOut vert(vertIn v)
	{
		vertOut o;

		// Convert Vertex position and corresponding normal into world coords
		// Note that we have to multiply the normal by the transposed inverse of the world 
		// transformation matrix (for cases where we have non-uniform scaling; we also don't
		// care about the "fourth" dimension, because translations don't affect the normal) 
		float4 worldVertex = mul(_Object2World, v.vertex);
		float3 worldNormal = normalize(mul(transpose((float3x3)_World2Object), v.normal.xyz));

		// Calculate ambient RGB intensities
		float Ka = 0.5;
		float3 amb = v.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

		// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
		// (when calculating the reflected ray in our specular component)
		float fAtt = 1;
		float Kd = 0.5;
		float3 L = normalize(_PointLightPosition - worldVertex.xyz);
		float LdotN = dot(L, worldNormal.xyz);
		float3 dif = fAtt * _PointLightColor.rgb * Kd * v.color.rgb * saturate(LdotN);

		// Calculate specular reflections
		float Ks = 0.5;
		float specN = 5; // Values>>1 give tighter highlights
		float3 V = normalize(_WorldSpaceCameraPos - worldVertex.xyz);
		//float3 R = float3(0.0, 0.0, 0.0);
		float3 R = 2 * LdotN * normalize(worldNormal.xyz) - L;
		float3 spe = fAtt * _PointLightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);

		// Combine Phong illumination model components
		o.color.rgb = amb.rgb + dif.rgb + spe.rgb;
		o.color.a = v.color.a;

		// Transform vertex in world coordinates to camera coordinates
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

		return o;
	}

	// Implementation of the fragment shader
	fixed4 frag(vertOut v) : SV_Target
	{
		return v.color;
	}
		ENDCG
	}
	}
}

