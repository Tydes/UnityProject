// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Multiply", "Operators", "Simple multiplication of two variables", null, KeyCode.M )]
	public sealed class SimpleMultiplyOpNode : DynamicTypeNode
	{
		public override string BuildResults( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.BuildResults( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			if ( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT3x3 ||
				m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT4x4 ||
				m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT3x3 ||
				m_inputPorts[ 1 ].DataType == WirePortDataType.FLOAT4x4 )
			{
				return "mul( " + m_inputA + " , " + m_inputB + " )";
			}
			else
			{
				return "( " + m_inputA + " * " + m_inputB + " )";
			}
		}
	}
}
