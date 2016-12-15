// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Parallax Offset", "Generic", "Calculates UV offset for parallax normal mapping" )]
	public sealed class ParallaxOffsetHlpNode : HelperParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_funcType = "ParallaxOffset";
			m_inputPorts[ 0 ].ChangeProperties( "h", WirePortDataType.FLOAT, false );
			AddInputPort( WirePortDataType.FLOAT, false, "height" );
			AddInputPort( WirePortDataType.FLOAT3, false, "viewDir" );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2, false );
		}
	}
}
