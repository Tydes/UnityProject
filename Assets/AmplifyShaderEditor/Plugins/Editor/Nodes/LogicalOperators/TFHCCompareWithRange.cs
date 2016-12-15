// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Compare With Range
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Compare With Range", "Logical Operators", "Check if A is in the range between Range Min and Range Max. If true return value of True else return value of False", null, KeyCode.None )]
	public sealed class TFHCCompareWithRange : DynamicTypeNode
	{

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = "A";
			m_inputPorts[ 1 ].Name = "Range Min";
			AddInputPort( WirePortDataType.FLOAT, false, "Range Max" );
			AddInputPort( WirePortDataType.FLOAT, false, "True" );
			AddInputPort( WirePortDataType.FLOAT, false, "False" );
			m_textLabelWidth = 100;
			m_useInternalPortData = true;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnections();
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnections();
		}

		void UpdateConnections()
		{
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_inputPorts[ 1 ].ChangeType( InputPorts[ 0 ].DataType, false );
			m_inputPorts[ 2 ].ChangeType( InputPorts[ 0 ].DataType, false );
			m_inputPorts[ 3 ].MatchPortToConnection();
			m_inputPorts[ 4 ].MatchPortToConnection();
			m_outputPorts[ 0 ].MatchPortToConnection();
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{

			//Check if VALUE is in range between MIN and MAX. If true return VALUE IF TRUE else VALUE IF FALSE"
			string a = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string b = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string c = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string d = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string e = m_inputPorts[ 4 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string strout = " ( ( " + a + " >= " + b + " && " + a + " <= " + c + " ) ? " + d + " :  " + e + " ) ";
			//Debug.Log(strout);
			return CreateOutputLocalVariable( 0, strout, ref dataCollector );

		}
	}
}
