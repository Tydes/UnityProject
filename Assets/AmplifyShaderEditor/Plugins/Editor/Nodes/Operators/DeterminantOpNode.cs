// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Determinant", "Matrix", "Scalar determinant of a square matrix" )]
	public sealed class DeterminantOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "determinant";
			m_validTypes = ( int ) WirePortDataType.OBJECT |
						  ( int ) WirePortDataType.FLOAT3x3 |
						  ( int ) WirePortDataType.FLOAT4x4;

			m_autoUpdateOutputPort = false;
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4x4, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT, false );

		}
	}
}
