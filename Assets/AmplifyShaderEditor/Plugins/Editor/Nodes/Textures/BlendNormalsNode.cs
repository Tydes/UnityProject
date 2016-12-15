// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Blend Normals", "Textures", "Blend Normals" )]
	public class BlendNormalsNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "A" );
			AddInputPort( WirePortDataType.FLOAT3, false, "B" );
			AddOutputPort( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
			m_useInternalPortData = true;
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			dataCollector.AddToIncludes( m_uniqueId, Constants.UnityStandardUtilsLibFuncs );
			string _inputA = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar, 0, true );
			string _inputB = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar, 0, true );
			string result = "BlendNormals( " + _inputA + " , " + _inputB + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
