// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Local Vertex Pos", "Surface Standard Inputs", "Interpolated Vertex Position in Local Space" )]
	public sealed class LocalVertexPosNode : ParentNode
	{
		private const string VertexVarName = "localVertexPos";
		private readonly string CompleteVertexVarName = Constants.InputVarStr + "." + VertexVarName;

		[SerializeField]
		private bool m_addInstruction = false;

		public override void Reset()
		{
			base.Reset();
			m_addInstruction = true;
		}
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
		}
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_addInstruction )
			{
				dataCollector.AddToInput( m_uniqueId, "float3 " + VertexVarName, true );
				dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + "." + VertexVarName + " = " + Constants.VertexShaderInputStr + ".vertex.xyz ", m_uniqueId );
				m_addInstruction = false;
			}
			return GetOutputVectorItem( 0, outputId, CompleteVertexVarName );
		}
	}
}
