Shader "Custom/VertexColor" {
	SubShader{
	CGPROGRAM

	#pragma surface surf Lambert
	
	struct Input{
		float4 vertColor : COLOR;
	};

	void surf(Input input, inout SurfaceOutput o)
	{
	o.Albedo = input.vertColor;
	}

	ENDCG
	}

	FallBack "Diffuse"
}
