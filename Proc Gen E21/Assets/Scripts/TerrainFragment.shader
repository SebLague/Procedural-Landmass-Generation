Shader "Custom/TerrainFragment" {

		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			Tags{"LightMode" = "LightweightForward"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma glsl
		#pragma multi_compile_fog

		#include "UnityCG.cginc"
		#include "Lighting.cginc"

	// compile shader into multiple variants, with and without shadows
	// (we don't care about any lightmaps yet, so skip these variants)
	#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
	// shadow helper functions and macros
	#include "AutoLight.cginc"

	const static int maxLayerCount = 8;
	const static float epsilon = 1E-4;

	int layerCount;
	float3 baseColors[maxLayerCount];
	float baseStartHeights[maxLayerCount];
	float baseBlends[maxLayerCount];
	float baseColorStrength[maxLayerCount];
	float baseTextureScales[maxLayerCount];

	float minHeight;
	float maxHeight;

	UNITY_DECLARE_TEX2DARRAY(baseTextures);

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float3 worldPos : TEXCOORD0;
		half3 worldNormal : TEXCOORD1;
		fixed4 diff : COLOR0; // diffuse lighting color
		fixed3 ambient : COLOR1;
		UNITY_FOG_COORDS(1)
	};

	float inverseLerp(float a, float b, float value) {

		return saturate((value - a) / (b - a));
	}

	float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {

		float3 scaledWorldPos = worldPos / scale;
		float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
		float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
		float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
		return xProjection + yProjection + zProjection;
	}

	float getDrawStrength(int index, float heightPercent) {

		float lerpA = -baseBlends[index] / 2.0 - epsilon;
		float lerpB = baseBlends[index] / 2.0;
		float lerpC = heightPercent - baseStartHeights[index];

		return  inverseLerp(lerpA, lerpB, lerpC);
	}

	v2f vert(float4 vertex : POSITION, float3 normal : NORMAL)
	{
		v2f o;
		o.worldPos = mul(unity_ObjectToWorld, vertex);
		o.vertex = UnityObjectToClipPos(vertex);
		o.worldNormal = UnityObjectToWorldNormal(normal);

		half nl = max(0, dot(o.worldNormal, _WorldSpaceLightPos0.xyz));
		// factor in the light color
		o.diff = nl * _LightColor0;
		o.ambient = ShadeSH9(half4(o.worldNormal, 1));
		TRANSFER_SHADOW(o)
		UNITY_TRANSFER_FOG(o, o.vertex);
		return o;
	}

	fixed4 _Color;
	fixed4 frag(v2f IN) : SV_Target
	{
		float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
		float3 blendAxes = abs(IN.worldNormal);
		blendAxes /= dot(blendAxes, 1.0);

		for (int i = 0; i < layerCount; i++) {
			float drawStrength = getDrawStrength(i, heightPercent);

			float3 baseColor = baseColors[i] * baseColorStrength[i];
			float3 textureColor = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColorStrength[i]);
			_Color.rgb = _Color.rgb * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;
		}

		// compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
		fixed shadow = SHADOW_ATTENUATION(IN);
		// darken light's illumination with shadow, keep ambient intact
		fixed3 lighting = IN.diff * shadow + IN.ambient;
		_Color.rgb *= lighting;
		UNITY_APPLY_FOG(i.fogCoord, _Color);

		// UNITY_APPLY_FOG(IN.fogCoord, _Color);
		return _Color;
	}
	ENDCG
}
UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
	}
		FallBack "Diffuse"
}
