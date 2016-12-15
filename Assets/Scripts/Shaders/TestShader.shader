// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyCollection/TestShader"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_TextureSample0;
		};

		uniform sampler2D _TextureSample0;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			output.Albedo = tex2D( _TextureSample0,input.uv_TextureSample0).xyz;
			output.Smoothness = 0.0;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=21
1148;100;526;728;-1.899933;457.7;1.7;False;False
Node;AmplifyShaderEditor.RangedFloatNode;1;301.8998,167.8;Constant;_Float0;Float 0;-1;0;0;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;562.6001,40.4;True;Standard;MyCollection/TestShader;False;False;False;False;False;False;False;False;False;False;False;False;Back;On;LEqual;Opaque;0.5;True;True;0;False;Opaque;Geometry;0,0,0;0,0,0;0,0,0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0.0;0,0,0
Node;AmplifyShaderEditor.SamplerNode;2;131.3998,-41.19997;Property;_TextureSample0;Texture Sample 0;-1;None;True;0;False;white;Auto;False;Object;-1;0,0;1.0
WireConnection;0;0;2;0
WireConnection;0;4;1;0
ASEEND*/
//CHKSM=3FCB8DF6EBC53D44DFEF7001CBF2C71CDC682E10