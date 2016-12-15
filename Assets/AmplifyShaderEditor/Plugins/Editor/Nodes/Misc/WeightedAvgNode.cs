// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Channel Weighted Avg", "Misc", "Mix all channels by a certain amount", null, KeyCode.None, false )]
	public sealed class WeightedAvgNode : ParentNode
	{
		private const string IsLayerStr = "Layered";
		private const string LayerFactorStr = "Layer Factor";
		private string[] AmountsStr = { "[0] Amount", "[1] Amount", "[2] Amount", "[3] Amount" };
		[SerializeField]
		private bool m_isLayered = false;

		[SerializeField]
		private float[] m_defaultAmounts = { 1, 1, 1, 1 };

		[SerializeField]
		private float m_defaultLayerFactor = 1;

		private int m_activePorts = 1;
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.OBJECT, false, "Input" );
			AddInputPort( WirePortDataType.FLOAT, true, AmountsStr[ 0 ] );
			AddInputPort( WirePortDataType.FLOAT, true, AmountsStr[ 1 ] );
			AddInputPort( WirePortDataType.FLOAT, true, AmountsStr[ 2 ] );
			AddInputPort( WirePortDataType.FLOAT, true, AmountsStr[ 3 ] );
			AddInputPort( WirePortDataType.FLOAT, true, LayerFactorStr );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_inputPorts[ 5 ].Visible = m_isLayered;
			UpdateConnection( false );
		}

		//public override void DrawProperties()
		//{
		//	base.DrawProperties();
		//	EditorGUI.BeginChangeCheck();
		//	_isLayered = EditorGUILayout.ToggleLeft( IsLayerStr, _isLayered );
		//	if ( EditorGUI.EndChangeCheck() )
		//	{
		//		_inputPorts[ 5 ].Visible = _isLayered;
		//		if ( !_inputPorts[ 5 ].Visible && _inputPorts[ 5 ].IsConnected )
		//		{
		//			UIUtils.CurrentWindow.DeleteConnection( true, _uniqueId, 5, true );
		//		}
		//		_sizeIsDirty = true;
		//	}

		//	if ( _isLayered )
		//	{
		//		_defaultLayerFactor = EditorGUILayout.FloatField( LayerFactorStr, _defaultLayerFactor );
		//	}

		//	for ( int i = 0; i < _activePorts; i++ )
		//	{
		//		_defaultAmounts[ i ] = EditorGUILayout.FloatField( AmountsStr[ i ], _defaultAmounts[ i ] );
		//	}
		//}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			if ( inputPortId == 0 )
				UpdateConnection( true );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			if ( portId == 0 )
				UpdateConnection( true );
		}

		void UpdateConnection( bool invalidateConnections )
		{
			m_inputPorts[ 0 ].MatchPortToConnection();
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.OBJECT:
				{
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = false;
					m_inputPorts[ 3 ].Visible = false;
					m_inputPorts[ 4 ].Visible = false;
					m_activePorts = 0;
				}
				break;
				case WirePortDataType.FLOAT:
				{
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = false;
					m_inputPorts[ 3 ].Visible = false;
					m_inputPorts[ 4 ].Visible = false;
					m_activePorts = 1;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = true;
					m_inputPorts[ 3 ].Visible = false;
					m_inputPorts[ 4 ].Visible = false;
					m_activePorts = 2;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = true;
					m_inputPorts[ 3 ].Visible = true;
					m_inputPorts[ 4 ].Visible = false;
					m_activePorts = 3;
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = true;
					m_inputPorts[ 3 ].Visible = true;
					m_inputPorts[ 4 ].Visible = true;
					m_activePorts = 4;
				}
				break;
				case WirePortDataType.FLOAT3x3:
				{
					m_inputPorts[ 1 ].Visible = false;
					m_inputPorts[ 2 ].Visible = false;
					m_inputPorts[ 3 ].Visible = false;
					m_inputPorts[ 4 ].Visible = false;
					m_activePorts = 0;
				}
				break;
				case WirePortDataType.FLOAT4x4:
				{
					m_inputPorts[ 1 ].Visible = false;
					m_inputPorts[ 2 ].Visible = false;
					m_inputPorts[ 3 ].Visible = false;
					m_inputPorts[ 4 ].Visible = false;
					m_activePorts = 0;
				}
				break;
				case WirePortDataType.COLOR:
				{
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = true;
					m_inputPorts[ 3 ].Visible = true;
					m_inputPorts[ 4 ].Visible = true;
					m_activePorts = 4;
				}
				break;
				case WirePortDataType.INT:
				{
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = false;
					m_inputPorts[ 3 ].Visible = false;
					m_inputPorts[ 4 ].Visible = false;
					m_activePorts = 1;
				}
				break;
			}
			if ( invalidateConnections )
			{
				for ( int i = m_activePorts + 1; i < 5; i++ )
				{
					UIUtils.DeleteConnection( true, m_uniqueId, i, true );
				}
			}
			m_sizeIsDirty = true;
		}
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string result = string.Empty;

			string colorTarget = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );

			WirePortDataType dataType = m_inputPorts[ 0 ].ConnectionType();
			if ( dataType != WirePortDataType.COLOR && dataType != WirePortDataType.FLOAT4 )
			{
				colorTarget = UIUtils.CastPortType( new NodeCastInfo( m_uniqueId, outputId ), null, dataType, WirePortDataType.FLOAT4, colorTarget );
			}

			string localVarName = "ChannelBlendNode" + m_uniqueId;
			string localVar = "float4 " + localVarName + " = " + colorTarget + ";";

			dataCollector.AddToLocalVariables( m_uniqueId, localVar );
			m_inputPorts[ 1 ].InternalData = m_inputPorts[ 1 ].IsConnected ? m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar ) : m_defaultAmounts[ 0 ].ToString();
			m_inputPorts[ 2 ].InternalData = m_inputPorts[ 2 ].IsConnected ? m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar ) : m_defaultAmounts[ 1 ].ToString();
			m_inputPorts[ 3 ].InternalData = m_inputPorts[ 3 ].IsConnected ? m_inputPorts[ 3 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar ) : m_defaultAmounts[ 2 ].ToString();
			m_inputPorts[ 4 ].InternalData = m_inputPorts[ 4 ].IsConnected ? m_inputPorts[ 4 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar ) : m_defaultAmounts[ 3 ].ToString();

			if ( m_isLayered )
			{
				string layerMask = m_inputPorts[ 4 ].IsConnected ? m_inputPorts[ 4 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar ) : m_defaultLayerFactor.ToString();
				WirePortDataType dataType4 = m_inputPorts[ 4 ].ConnectionType();
				if ( dataType4 != WirePortDataType.FLOAT )
				{
					layerMask = UIUtils.CastPortType( new NodeCastInfo( m_uniqueId, outputId ), null, dataType4, WirePortDataType.FLOAT, layerMask );
				}

				if ( m_activePorts == 0 )
				{
					result = colorTarget;
				}
				else if ( m_activePorts == 1 )
				{
					result += "lerp( " + layerMask + "," + m_inputPorts[ 1 ].InternalData + " , " + localVarName + ".r )";
				}
				else
				{
					result = layerMask;
					for ( int i = 0; i < m_activePorts; i++ )
					{
						result = "lerp( " + result + " , " + m_inputPorts[ i + 1 ].InternalData + " , " + localVarName + UIUtils.GetComponentForPosition( i, m_inputPorts[ 0 ].DataType, true ) + " )";
					}
				}
			}
			else
			{
				if ( m_activePorts == 0 )
				{
					result = colorTarget;
				}
				else if ( m_activePorts == 1 )
				{
					result += localVarName + "*" + m_inputPorts[ 1 ].InternalData;
				}
				else
				{
					for ( int i = 1; i < ( m_activePorts + 1 ); i++ )
					{
						result += localVarName + UIUtils.GetComponentForPosition( i - 1, m_inputPorts[ 0 ].DataType, true ) + "*" + m_inputPorts[ i ].InternalData;
						if ( i != m_activePorts )
						{
							result += " + ";
						}
					}
				}
			}
			result = UIUtils.AddBrackets( result );
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_isLayered );
			for ( int i = 0; i < m_defaultAmounts.Length; i++ )
				IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultAmounts[ i ] );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultLayerFactor );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_isLayered = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			for ( int i = 0; i < m_defaultAmounts.Length; i++ )
				m_defaultAmounts[ i ] = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_defaultLayerFactor = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
		}
	}
}
