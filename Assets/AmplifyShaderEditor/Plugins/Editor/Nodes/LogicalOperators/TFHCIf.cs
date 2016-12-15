// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node If
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "If", "Logical Operators", "Compare A with B. If A is greater than B output the value of A > B port. If A is equal to B output the value of A == B port. If A is lower than B output the value of A < B port. Equal Threshold parameter will be used to check A == B adding and subtracting this value to A.", null, KeyCode.None )]
	public sealed class TFHCIf : DynamicTypeNode
	{

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = "A";
			m_inputPorts[ 1 ].Name = "B";
			AddInputPort( WirePortDataType.FLOAT, false, "A > B" );
			AddInputPort( WirePortDataType.FLOAT, false, "A == B" );
			AddInputPort( WirePortDataType.FLOAT, false, "A < B" );
			AddInputPort( WirePortDataType.FLOAT, false, "Equal Threshold" );
			m_textLabelWidth = 100;
			m_useInternalPortData = true;
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_inputPorts[ 1 ].ChangeType( InputPorts[ 0 ].DataType, false );
			m_inputPorts[ 2 ].MatchPortToConnection();
			m_inputPorts[ 3 ].MatchPortToConnection();
			m_inputPorts[ 4 ].MatchPortToConnection();
			m_inputPorts[ 5 ].ChangeType( InputPorts[ 0 ].DataType, false );
			m_outputPorts[ 0 ].MatchPortToConnection();
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{

			string a = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string b = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string r1 = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string r2 = m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string r3 = m_inputPorts[ 4 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			string tr = m_inputPorts[ 5 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );

			// No Equal Threshold parameter
			//(a > b ? r1 : a == b ? r2 : r3 )      
			//string strout = " ( " + a + " > " + b  + " ? " + r1 + " : " + a + " == " + b + " ? " + r2 + " : " +  r3 + " ) ";

			// With Equal Threshold parameter
			// ( a - tr > b ? r1 : a - tr <= b && a + tr >= b ? r2 : r3 )
			string strout = " ( " + a + " - " + tr + " > " + b + " ? " + r1 + " : " + a + " - " + tr + " <= " + b + " && " + a + " + " + tr + " >= " + b + " ? " + r2 + " : " + r3 + " ) ";

			Debug.Log( strout );
			return CreateOutputLocalVariable( 0, strout, ref dataCollector );
		}
	}
}
