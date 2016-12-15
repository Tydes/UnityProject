// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class DynamicTypeNode : ParentNode
	{
		protected string m_inputA = string.Empty;
		protected string m_inputB = string.Empty;
		protected bool m_dynamicOutputType = true;

		[UnityEngine.SerializeField]
		protected WirePortDataType m_mainDataType = WirePortDataType.OBJECT;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_useInternalPortData = true;
			m_textLabelWidth = 15;
			AddPorts();
		}

		protected virtual void AddPorts()
		{
			AddInputPort( WirePortDataType.OBJECT, false, "A" );
			AddInputPort( WirePortDataType.OBJECT, false, "B" );
			AddOutputPort( WirePortDataType.OBJECT, Constants.EmptyPortValue );
		}

		public int GetPriority( WirePortDataType type )
		{
			switch ( type )
			{
				case WirePortDataType.FLOAT: return 1;
				case WirePortDataType.INT: return 1;
				case WirePortDataType.FLOAT2: return 2;
				case WirePortDataType.FLOAT3: return 3;
				case WirePortDataType.FLOAT4: return 4;
				case WirePortDataType.COLOR: return 4;
				case WirePortDataType.FLOAT3x3: return -1;
				case WirePortDataType.FLOAT4x4: return -1;
				case WirePortDataType.OBJECT: return -1;
			}
			return -1;
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			UpdateConnection( inputPortId );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnection( portId );
		}

		public override void OnInputPortDisconnected( int portId )
		{
			UpdateDisconnectedConnection( portId );
		}

		void UpdateDisconnectedConnection( int portId )
		{
			if ( m_inputPorts[ 0 ].DataType != m_inputPorts[ 1 ].DataType )
			{
				int otherPortId = ( portId + 1 ) % 2;
				if ( m_inputPorts[ otherPortId ].IsConnected )
				{
					m_mainDataType = m_inputPorts[ otherPortId ].DataType;
					m_inputPorts[ portId ].ChangeType( m_mainDataType, false );
					if ( m_dynamicOutputType )
						m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
				}
				else
				{
					if ( GetPriority( m_inputPorts[ 0 ].DataType ) > GetPriority( m_inputPorts[ 1 ].DataType ) )
					{
						m_mainDataType = m_inputPorts[ 0 ].DataType;
						m_inputPorts[ 1 ].ChangeType( m_mainDataType, false );
					}
					else
					{
						m_mainDataType = m_inputPorts[ 1 ].DataType;
						m_inputPorts[ 0 ].ChangeType( m_mainDataType, false );
					}

					if ( m_dynamicOutputType )
					{
						if ( m_mainDataType != m_outputPorts[ 0 ].DataType )
						{
							m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
						}
					}
				}
			}
		}

		void UpdateConnection( int portId )
		{
			m_inputPorts[ portId ].MatchPortToConnection();
			int otherPortId = ( portId + 1 ) % 2;
			if ( !m_inputPorts[ otherPortId ].IsConnected )
			{
				m_inputPorts[ otherPortId ].ChangeType( m_inputPorts[ portId ].DataType, false );
			}

			if ( m_inputPorts[ 0 ].DataType == m_inputPorts[ 1 ].DataType )
			{
				m_mainDataType = m_inputPorts[ 0 ].DataType;
				if ( m_dynamicOutputType )
					m_outputPorts[ 0 ].ChangeType( InputPorts[ 0 ].DataType, false );
			}
			else
			{
				if ( GetPriority( m_inputPorts[ 0 ].DataType ) > GetPriority( m_inputPorts[ 1 ].DataType ) )
				{
					m_mainDataType = m_inputPorts[ 0 ].DataType;
				}
				else
				{
					m_mainDataType = m_inputPorts[ 1 ].DataType;
				}

				if ( m_dynamicOutputType )
				{
					if ( m_mainDataType != m_outputPorts[ 0 ].DataType )
					{
						m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
					}
				}
			}
		}

		public virtual string BuildResults( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			SetInputData( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			return string.Empty;
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string result = BuildResults( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		protected void SetInputData( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			m_inputA = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			if ( m_inputPorts[ 0 ].DataType != m_mainDataType )
			{
				m_inputA = UIUtils.CastPortType( new NodeCastInfo( m_uniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, m_mainDataType, m_inputA );
			}
			m_inputB = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );
			if ( m_inputPorts[ 1 ].DataType != m_mainDataType )
			{
				m_inputB = UIUtils.CastPortType( new NodeCastInfo( m_uniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, m_mainDataType, m_inputB );
			}
		}

	}
}
