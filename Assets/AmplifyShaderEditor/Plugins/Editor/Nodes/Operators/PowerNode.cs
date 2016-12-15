// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Power", "Operators", "A to the b-th power of scalars and vectors", null, KeyCode.E )]
	public sealed class PowerNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "A" );
			AddInputPort( WirePortDataType.FLOAT, false, "B" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_textLabelWidth = 15;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnections( portId );
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnections( inputPortId );
		}
		
		void UpdateConnections( int inputPort )
		{
			m_inputPorts[ inputPort ].MatchPortToConnection();
			if ( inputPort == 0 )
			{
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string x = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar );
			string y = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar, 0, true );
			string result = "pow( " + x + " , " + y + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
