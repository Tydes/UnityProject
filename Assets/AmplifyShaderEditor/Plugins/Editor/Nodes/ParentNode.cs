// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ParentNode : ScriptableObject, ISerializationCallbackReceiver
	{
		private const int MOVE_COUNT_BUFFER = 3;// When testing for stopped movement we need to take Layout and Repaint into account for them not to interfere with tests

		public delegate void OnNodeEvent( ParentNode node, bool testOnlySelected, InteractionMode interactionMode );
		public delegate void OnNodeChangeSize( ParentNode node );
		public delegate void OnNodeReOrder( ParentNode node, int index );

		[SerializeField]
		protected InteractionMode m_defaultInteractionMode = InteractionMode.Other;

		[SerializeField]
		public event OnNodeEvent OnNodeStoppedMovingEvent;

		[SerializeField]
		public OnNodeChangeSize OnNodeChangeSizeEvent;

		[SerializeField]
		public event OnNodeReOrder OnNodeReOrderEvent;

		[SerializeField]
		protected int m_uniqueId;

		[SerializeField]
		protected Rect m_position;

		[SerializeField]
		protected GUIContent m_content;

		[SerializeField]
		protected GUIContent m_additionalContent;

		[SerializeField]
		protected bool m_initialized;

		[SerializeField]
		protected NodeConnectionStatus m_connStatus;
		protected bool m_selfPowered = false;

		[SerializeField]
		private int m_activeConnections;

		[SerializeField]
		protected Type m_activeType;

		[SerializeField]
		protected int m_activePort;

		[SerializeField]
		protected int m_activeNode;

		protected NodeRestrictions m_restictions;

		[SerializeField]
		protected Color m_statusColor;

		[SerializeField]
		protected Rect m_propertyDrawPos;

		// Ports
		[SerializeField]
		protected List<InputPort> m_inputPorts;

		[SerializeField]
		protected List<OutputPort> m_outputPorts;

		[SerializeField]
		protected Rect m_globalPosition;

		[SerializeField]
		protected Rect m_headerPosition;

		private Vector2 m_tooltipOffset;

		[SerializeField]
		protected bool m_sizeIsDirty = false;

		[SerializeField]
		protected Vector2 m_fixedSize;

		[SerializeField]
		protected float m_fontHeight;

		[SerializeField]
		protected NodeAttributes m_nodeAttribs;

		// Editor State save on Play Button
		[SerializeField]
		protected bool m_isDirty;

		[SerializeField]
		private int m_isMoving = 0;
		[SerializeField]
		private Rect m_lastPosition;

		// Live Shader Gen
		[SerializeField]
		private bool m_saveIsDirty;

		[SerializeField]
		protected bool m_requireMaterialUpdate = false;

		[SerializeField]
		protected bool m_isCommentaryParented = false;

		[SerializeField]
		protected int m_depth = -1;

		[SerializeField]
		protected bool m_materialMode = false;

		[SerializeField]
		protected int m_titleLineAdjust = 1;

		protected GUIStyle m_buttonStyle;
		protected GUIStyle m_labelStyle;
		protected GUIStyle m_toggleStyle;
		protected GUIStyle m_textfieldStyle;
		//protected GUIStyle _portAreaStyle;
		protected GUIStyle m_empty = new GUIStyle();
		protected bool m_isVisible;
		protected bool m_selected;
		protected bool m_rmbIgnore;
		private GUIContent m_sizeContentAux;

		private uint m_currentReadParamIdx = 1;
		protected bool m_reorderLocked = false;

		protected Rect m_cachedPos;
		private bool m_isOnGrid = false;
		protected bool m_useInternalPortData = false;
		protected DrawOrder m_drawOrder = DrawOrder.Default;

		private bool m_movingInFrame = false;
		private float m_anchorAdjust = -1;

		protected Color m_headerColor;

		protected Color m_headerColorModifier = Color.white;

		private bool m_infiniteLoopDetected = false;
		protected int m_textLabelWidth = -1;


		public ParentNode()
		{
			m_position = new Rect( 0, 0, 0, 0 );
			m_content = new GUIContent( GUIContent.none );
			m_additionalContent = new GUIContent( GUIContent.none );
			CommonInit( -1 );
		}

		public ParentNode( int uniqueId, float x, float y, float width, float height )
		{
			m_position = new Rect( x, y, width, height );
			m_content = new GUIContent( GUIContent.none );
			m_additionalContent = new GUIContent( GUIContent.none );
			CommonInit( uniqueId );
		}

		public virtual void OnEnable()
		{
			//	_depth = UnityEngine.Random.Range( -100, 100 );
			hideFlags = HideFlags.DontSave;
		}

		protected virtual void CommonInit( int uniqueId )
		{
			m_uniqueId = uniqueId;
			m_isOnGrid = false;
			ConnStatus = NodeConnectionStatus.Not_Connected;
			m_inputPorts = new List<InputPort>();
			m_outputPorts = new List<OutputPort>();

			System.Reflection.MemberInfo info = this.GetType();
			m_nodeAttribs = info.GetCustomAttributes( true )[ 0 ] as NodeAttributes;
			if ( m_nodeAttribs != null )
			{
				m_content.text = m_nodeAttribs.Name;
				m_content.tooltip = m_nodeAttribs.Description;
				m_selected = false;
				m_headerColor = UIUtils.GetColorFromCategory( m_nodeAttribs.Category );
			}

			m_sizeContentAux = new GUIContent();
			m_fixedSize = new Vector2( 50, 10 );
			m_sizeIsDirty = true;
			m_initialized = true;
			m_restictions = new NodeRestrictions();

			m_tooltipOffset = new Vector2( 0, 10 );

			m_propertyDrawPos = new Rect();

		}

		public virtual void Destroy()
		{
			OnNodeStoppedMovingEvent = null;
			OnNodeChangeSizeEvent = null;
			OnNodeReOrderEvent = null;
			m_restictions.Destroy();
			m_restictions = null;
			int inputCount = m_inputPorts.Count;
			for ( int i = 0; i < inputCount; i++ )
			{
				m_inputPorts[ i ].Destroy();
			}

			int outputCount = m_outputPorts.Count;
			for ( int i = 0; i < outputCount; i++ )
			{
				m_outputPorts[ i ].Destroy();
			}

			m_inputPorts.Clear();
			m_inputPorts = null;

			m_outputPorts.Clear();
			m_outputPorts = null;

			m_buttonStyle = null;
			m_labelStyle = null;
			m_toggleStyle = null;
			m_textfieldStyle = null;
			//_portAreaStyle = null;
		}

		public virtual void Move( Vector2 delta )
		{
			if ( m_isMoving == 0 )
			{
				m_cachedPos = m_position;
			}

			m_isMoving = MOVE_COUNT_BUFFER;
			m_position.x += delta.x;
			m_position.y += delta.y;
			m_movingInFrame = true;
		}

		public virtual void UpdateMaterial( Material mat )
		{
			m_requireMaterialUpdate = false;
		}

		public virtual void SetMaterialMode( Material mat )
		{
			m_materialMode = ( mat != null );
		}

		public virtual bool UpdateShaderDefaults( ref Shader shader, ref TextureDefaultsDataColector defaultCol ) { return false; }
		public virtual void ForceUpdateFromMaterial( Material material ) { }
		public void SetSaveIsDirty()
		{
			if ( m_connStatus == NodeConnectionStatus.Connected )
			{
				SaveIsDirty = true;
			}
		}

		public void ActivateNodeReordering( int index )
		{
			if ( OnNodeReOrderEvent != null )
				OnNodeReOrderEvent( this, index );
		}

		// Manually add Ports 
		public void AddInputPort( WirePortDataType type, bool typeLocked, string name, int orderId = -1 )
		{
			m_inputPorts.Add( new InputPort( m_uniqueId, m_inputPorts.Count, type, name, typeLocked, ( orderId != -1 ? orderId : m_inputPorts.Count ) ) );
			SetSaveIsDirty();
		}

		public void AddOutputPort( WirePortDataType type, string name )
		{
			m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, type, name ) );
			SetSaveIsDirty();
		}

		public void AddOutputVectorPorts( WirePortDataType type, string name )
		{
			m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, type, name ) );
			switch ( type )
			{
				case WirePortDataType.FLOAT2:
				{
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "X" ) );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Y" ) );
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "X" ) );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Y" ) );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Z" ) );
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "X" ) );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Y" ) );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "Z" ) );
					m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "W" ) );
				}
				break;
			}
			SetSaveIsDirty();
		}

		public string GetOutputVectorItem( int vectorPortId, int currentPortId, string result )
		{
			if ( m_outputPorts[ 0 ].DataType == WirePortDataType.COLOR )
			{
				switch ( currentPortId - vectorPortId )
				{
					case 1: result += ".r"; break;
					case 2: result += ".g"; break;
					case 3: result += ".b"; break;
					case 4: result += ".a"; break;
				}
			}
			else
			{
				switch ( currentPortId - vectorPortId )
				{
					case 1: result += ".x"; break;
					case 2: result += ".y"; break;
					case 3: result += ".z"; break;
					case 4: result += ".w"; break;
				}
			}
			return result;
		}

		public void AddOutputColorPorts( WirePortDataType type, string name )
		{
			m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, type, name ) );
			if ( type == WirePortDataType.COLOR )
			{
				m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "R" ) );
				m_outputPorts[ 1 ].CustomColor = Color.red;
				m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "G" ) );
				m_outputPorts[ 2 ].CustomColor = Color.green;
				m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "B" ) );
				m_outputPorts[ 3 ].CustomColor = Color.blue;
				m_outputPorts.Add( new OutputPort( m_uniqueId, m_outputPorts.Count, WirePortDataType.FLOAT, "A" ) );
				m_outputPorts[ 4 ].CustomColor = Color.white;
			}
		}

		public void ConvertFromVectorToColorPorts()
		{
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.COLOR, false );

			m_outputPorts[ 1 ].Name = "R";
			m_outputPorts[ 1 ].CustomColor = Color.red;

			m_outputPorts[ 2 ].Name = "G";
			m_outputPorts[ 2 ].CustomColor = Color.green;

			m_outputPorts[ 3 ].Name = "B";
			m_outputPorts[ 3 ].CustomColor = Color.blue;

			m_outputPorts[ 4 ].Name = "A";
			m_outputPorts[ 4 ].CustomColor = Color.white;
		}


		public string GetOutputColorItem( int vectorPortId, int currentPortId, string result )
		{
			switch ( currentPortId - vectorPortId )
			{
				case 1: result += ".r"; break;
				case 2: result += ".g"; break;
				case 3: result += ".b"; break;
				case 4: result += ".a"; break;
			}
			return result;
		}

		public void ChangeOutputType( WirePortDataType type, bool invalidateConnections )
		{
			int outputCount = m_outputPorts.Count;
			for ( int i = 0; i < outputCount; i++ )
			{
				m_outputPorts[ i ].ChangeType( type, invalidateConnections );
			}
		}

		public void ChangeInputType( WirePortDataType type, bool invalidateConnections )
		{
			int inputCount = m_inputPorts.Count;
			for ( int i = 0; i < inputCount; i++ )
			{
				m_inputPorts[ i ].ChangeType( type, invalidateConnections );
			}
		}

		public void ChangeOutputProperties( int outputID, string newName, WirePortDataType newType, bool invalidateConnections = true )
		{
			if ( outputID < m_outputPorts.Count )
			{
				m_outputPorts[ outputID ].ChangeProperties( newName, newType, invalidateConnections );
				IsDirty = true;
				m_sizeIsDirty = true;
				SetSaveIsDirty();
			}
		}

		public void ChangeOutputName( int outputID, string newName )
		{
			if ( outputID < m_outputPorts.Count )
			{
				m_outputPorts[ outputID ].Name = newName;
				IsDirty = true;
				m_sizeIsDirty = true;
			}
		}

		public InputPort CheckInputPortAt( Vector3 pos )
		{
			int count = m_inputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				//if ( m_inputPorts[ i ].Position.Contains( pos ) )
				if ( m_inputPorts[ i ].InsideActiveArea( pos ) )
					return m_inputPorts[ i ];
			}
			return null;
		}

		public InputPort GetFirstInputPortOfType( WirePortDataType dataType, bool countObjectTypeAsValid )
		{
			int count = m_inputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( ( m_inputPorts[ i ].DataType == dataType ) || ( countObjectTypeAsValid && m_inputPorts[ i ].DataType == WirePortDataType.OBJECT ) )
					return m_inputPorts[ i ];
			}
			return null;
		}

		public OutputPort CheckOutputPortAt( Vector3 pos )
		{
			int count = m_outputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				//if ( m_outputPorts[ i ].Position.Contains( pos ) )
				if ( m_outputPorts[ i ].InsideActiveArea( pos ) )
					return m_outputPorts[ i ];
			}
			return null;
		}

		public OutputPort GetFirstOutputPortOfType( WirePortDataType dataType, bool checkForCasts )
		{
			int count = m_outputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( ( m_outputPorts[ i ].DataType == dataType ) || ( checkForCasts && UIUtils.CanCast( dataType, m_outputPorts[ i ].DataType ) ) )
					return m_outputPorts[ i ];
			}
			return null;
		}

		virtual protected void ChangeSizeFinished() { }
		void ChangeSize()
		{
			m_cachedPos = m_position;
			UIUtils.ResetMainSkin();

			string maxInString = string.Empty;
			string maxOutString = string.Empty;
			int inputCount = 0;
			int inputSize = m_inputPorts.Count;
			for ( int i = 0; i < inputSize; i++ )
			{
				if ( m_inputPorts[ i ].Visible )
				{
					if ( m_inputPorts[ i ].Name.Length > maxInString.Length )
					{
						maxInString = m_inputPorts[ i ].Name;
					}

					inputCount += 1;
				}
			}

			int outputCount = 0;
			int outputSize = m_outputPorts.Count;
			for ( int o = 0; o < outputSize; o++ )
			{
				if ( m_outputPorts[ o ].Visible )
				{
					if ( m_outputPorts[ o ].Name.Length > maxOutString.Length )
					{
						maxOutString = m_outputPorts[ o ].Name;
					}
					outputCount += 1;
				}
			}

			Vector2 inSize = Vector2.zero;
			Vector2 outSize = Vector2.zero;
			if ( maxInString.Length > 0 )
			{
				m_sizeContentAux.text = maxInString;
				inSize = UIUtils.CustomStyle( CustomStyle.InputPortlabel ).CalcSize( m_sizeContentAux );
				inSize.x += UIUtils.PortsSize.x;

				if ( UIUtils.PortsSize.y > inSize.y )
					inSize.y = UIUtils.PortsSize.y;
			}

			if ( maxOutString.Length > 0 )
			{
				m_sizeContentAux.text = maxOutString;
				outSize = UIUtils.CustomStyle( CustomStyle.OutputPortLabel ).CalcSize( m_sizeContentAux );
				outSize.x += UIUtils.PortsSize.x + Constants.PORT_INITIAL_X;

				if ( UIUtils.PortsSize.y > outSize.y )
					outSize.y = UIUtils.PortsSize.y;
			}

			m_fontHeight = Mathf.Max( inSize.y, outSize.y );
			//string auxText = ( _nodeAttribs.Name.Length > _stringAuxBuffer.Length ) ? _nodeAttribs.Name : _stringAuxBuffer;
			string auxText = ( m_content.text.Length > m_additionalContent.text.Length ) ? m_content.text : m_additionalContent.text;

			if ( auxText.Length > ( maxOutString.Length + maxInString.Length ) )
			{
				m_sizeContentAux.text = auxText;
				m_position.width = Mathf.Max( 20 + UIUtils.CustomStyle( CustomStyle.NodeTitle ).CalcSize( m_sizeContentAux ).x, m_fixedSize.x + inSize.x + outSize.x );
			}
			else
			{
				m_position.width = m_fixedSize.x + inSize.x + outSize.x;
			}

			m_position.height = Constants.NODE_HEADER_HEIGHT + m_fixedSize.y + Mathf.Max( inputCount, outputCount ) * ( m_fontHeight + Constants.INPUT_PORT_DELTA_Y ) + Constants.INPUT_PORT_DELTA_Y;

			if ( OnNodeChangeSizeEvent != null )
			{
				OnNodeChangeSizeEvent( this );
			}
			ChangeSizeFinished();
		}
		public virtual void Reset() { }
		public virtual void OnOutputPortConnected( int portId, int otherNodeId, int otherPortId ) { }

		public virtual void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			if ( activateNode && m_connStatus == NodeConnectionStatus.Connected )
			{
				m_inputPorts[ portId ].GetOutputNode().ActivateNode( m_activeNode, m_activePort, m_activeType );
			}
			SetSaveIsDirty();
		}

		public virtual void OnInputPortDisconnected( int portId ) { }

		public void ActivateNode( int signalGenNodeId, int signalGenPortId, Type signalGenNodeType )
		{
			if ( m_selfPowered )
				return;

			ConnStatus = m_restictions.GetRestiction( signalGenNodeType, signalGenPortId ) ? NodeConnectionStatus.Error : NodeConnectionStatus.Connected;
			m_activeConnections += 1;

			m_activeType = signalGenNodeType;
			m_activeNode = signalGenNodeId;
			m_activePort = signalGenPortId;

			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected )
				{
					m_inputPorts[ i ].GetOutputNode().ActivateNode( signalGenNodeId, signalGenPortId, signalGenNodeType );
				}
			}
			SetSaveIsDirty();
		}

		public void DeactivateNode( int deactivatedPort, bool forceComplete )
		{
			if ( m_selfPowered )
				return;

			SetSaveIsDirty();
			m_activeConnections -= 1;
			if ( forceComplete || m_activeConnections <= 0 )
			{
				m_activeConnections = 0;
				ConnStatus = NodeConnectionStatus.Not_Connected;
				for ( int i = 0; i < m_inputPorts.Count; i++ )
				{
					if ( m_inputPorts[ i ].IsConnected )
					{
						m_inputPorts[ i ].GetOutputNode().DeactivateNode( deactivatedPort == -1 ? m_inputPorts[ i ].PortId : deactivatedPort, false );
					}
				}
			}
		}

		public Rect GlobalToLocalPosition( DrawInfo drawInfo )
		{
			float width = m_globalPosition.width / drawInfo.InvertedZoom;
			float height = m_globalPosition.height / drawInfo.InvertedZoom;

			float x = m_globalPosition.x / drawInfo.InvertedZoom - drawInfo.CameraOffset.x;
			float y = m_globalPosition.y / drawInfo.InvertedZoom - drawInfo.CameraOffset.y;
			return new Rect( x, y, width, height );
		}

		protected void CalculatePositionAndVisibility( DrawInfo drawInfo )
		{
			m_movingInFrame = false;
			m_globalPosition = m_position;
			m_globalPosition.x = drawInfo.InvertedZoom * ( m_globalPosition.x + drawInfo.CameraOffset.x );
			m_globalPosition.y = drawInfo.InvertedZoom * ( m_globalPosition.y + drawInfo.CameraOffset.y );
			m_globalPosition.width *= drawInfo.InvertedZoom;
			m_globalPosition.height *= drawInfo.InvertedZoom;

			m_isVisible = ( m_globalPosition.x + m_globalPosition.width > 0 ) &&
							( m_globalPosition.x < drawInfo.CameraArea.width ) &&
							( m_globalPosition.y + m_globalPosition.height > 0 ) &&
							( m_globalPosition.y < drawInfo.CameraArea.height );


			if ( m_isMoving > 0 && drawInfo.CurrentEventType != EventType.mouseDrag )
			{
				float deltaX = Mathf.Abs( m_lastPosition.x - m_position.x );
				float deltaY = Mathf.Abs( m_lastPosition.y - m_position.y );
				if ( deltaX < 0.01f && deltaY < 0.01f )
				{
					m_isMoving -= 1;
					if ( m_isMoving == 0 )
					{
						OnSelfStoppedMovingEvent();
					}
				}
				else
				{
					m_isMoving = MOVE_COUNT_BUFFER;
				}
				m_lastPosition = m_position;
			}

			if ( m_isVisible )
			{
				//if ( _globalPosition.Contains( drawInfo.MousePosition ) )
				//{
				//	UIUtils.RecordObject( this );
				//}

				if ( !( drawInfo.MousePosition.x < m_globalPosition.x ||
					drawInfo.MousePosition.x > ( m_globalPosition.x + m_globalPosition.width ) ||
					 drawInfo.MousePosition.y < m_globalPosition.y ||
					drawInfo.MousePosition.y > ( m_globalPosition.y + m_globalPosition.height ) ) )
				{
					UIUtils.RecordObject( this );
				}
			}
		}

		public void FireStoppedMovingEvent( bool testOnlySelected, InteractionMode interactionMode )
		{
			if ( OnNodeStoppedMovingEvent != null )
				OnNodeStoppedMovingEvent( this, testOnlySelected, interactionMode );
		}

		public virtual void OnSelfStoppedMovingEvent()
		{
			FireStoppedMovingEvent( true, m_defaultInteractionMode );
		}

		public virtual void Draw( DrawInfo drawInfo )
		{
			if ( m_initialized )
			{
				if ( m_sizeIsDirty )
				{
					m_sizeIsDirty = false;
					ChangeSize();
				}

				CalculatePositionAndVisibility( drawInfo );
				Color colorBuffer = GUI.color;

				// Background
				if ( m_isVisible )
				{
					GUI.color = m_infiniteLoopDetected ? Constants.InfiniteLoopColor : Constants.NodeBodyColor;
					GUI.Box( m_globalPosition, string.Empty, UIUtils.CustomStyle( CustomStyle.NodeWindowOff ) );

					m_headerPosition = m_globalPosition;
					m_headerPosition.height = UIUtils.CurrentHeaderHeight + m_titleLineAdjust * drawInfo.InvertedZoom;

					// Header
					GUI.color = m_headerColor * m_headerColorModifier;
					GUI.Box( m_headerPosition, string.Empty, UIUtils.CustomStyle( CustomStyle.NodeHeader ) );
					GUI.color = m_infiniteLoopDetected ? Constants.InfiniteLoopColor : colorBuffer;
					// Selection Box
					if ( m_selected )
					{
						GUI.Box( m_globalPosition, string.Empty, UIUtils.CustomStyle( CustomStyle.NodeWindowOn ) );
					}
					GUI.color = colorBuffer;

					// Title
					Rect titlePos = m_globalPosition;
					titlePos.y += ( -m_titleLineAdjust + 9 ) * drawInfo.InvertedZoom;
					GUI.Label( titlePos, m_content, UIUtils.CustomStyle( CustomStyle.NodeTitle ) );

					if ( m_titleLineAdjust != 0 )
					{
						titlePos.y += ( 1.1f * m_titleLineAdjust + 9 ) * drawInfo.InvertedZoom;
						GUI.Label( titlePos, m_additionalContent, UIUtils.CustomStyle( CustomStyle.PropertyValuesTitle ) );
					}
				}

				if ( m_anchorAdjust < 0 )
				{
					m_anchorAdjust = UIUtils.CustomStyle( CustomStyle.PortEmptyIcon ).normal.background.width;
				}

				m_globalPosition.y += m_titleLineAdjust * drawInfo.InvertedZoom;
				//Render Ports
				//Input ports
				{
					Rect currInputPortPos = m_globalPosition;
					currInputPortPos.width = drawInfo.InvertedZoom * UIUtils.PortsSize.x;
					currInputPortPos.height = drawInfo.InvertedZoom * UIUtils.PortsSize.y;

					currInputPortPos.x += drawInfo.InvertedZoom * Constants.PORT_INITIAL_X;
					currInputPortPos.y += drawInfo.InvertedZoom * Constants.PORT_INITIAL_Y;
					int inputCount = m_inputPorts.Count;
					for ( int i = 0; i < inputCount; i++ )
					{
						if ( m_inputPorts[ i ].Visible )
						{
							// Button
							m_inputPorts[ i ].Position = currInputPortPos;

							//Label
							Rect textPos = currInputPortPos;
							float deltaX = 1f * drawInfo.InvertedZoom * ( UIUtils.PortsSize.x + Constants.PORT_TO_LABEL_SPACE_X );
							textPos.x += deltaX;

							if ( m_isVisible )
							{
								UIUtils.CustomStyle( CustomStyle.InputPortlabel ).normal.textColor = Constants.PortTextColor;
								GUI.Label( textPos, m_inputPorts[ i ].Name, UIUtils.CustomStyle( CustomStyle.InputPortlabel ) );
							}
							m_sizeContentAux.text = m_inputPorts[ i ].Name;
							if ( drawInfo.ZoomChanged )
								m_inputPorts[ i ].LabelSize = UIUtils.CustomStyle( CustomStyle.InputPortlabel ).CalcSize( m_sizeContentAux );

							GUIStyle style = m_inputPorts[ i ].IsConnected ? UIUtils.CustomStyle( CustomStyle.PortFullIcon ) : UIUtils.CustomStyle( CustomStyle.PortEmptyIcon );
							if ( m_inputPorts[ i ].Locked )
							{
								if ( m_isVisible )
								{
									GUI.color = Constants.LockedPortColor;
									GUI.Box( currInputPortPos, "", style );
								}
							}
							else
							{
								Rect portPos = currInputPortPos;
								portPos.x -= Constants.PORT_X_ADJUST;
								portPos.y -= 5;
								portPos.width += ( m_inputPorts[ i ].LabelSize.x + deltaX + Constants.PORT_X_ADJUST );
								portPos.height += 10;
								m_inputPorts[ i ].ActivePortArea = portPos;
								if ( m_isVisible )
								{
									GUI.color = m_inputPorts[ i ].HasCustomColor ? m_inputPorts[ i ].CustomColor : UIUtils.GetColorForDataType( m_inputPorts[ i ].DataType, true, true );
									GUI.Box( currInputPortPos, string.Empty, style );
								}

								if ( m_isVisible && GUI.RepeatButton( portPos, string.Empty, m_empty ) && drawInfo.LeftMouseButtonPressed )
								{
									// need to put the mouse button on a hot state so it will detect the Mouse Up event correctly on the Editor Window
									int controlID = GUIUtility.GetControlID( FocusType.Passive );
									GUIUtility.hotControl = controlID;

									bool saveReference = true;
									if ( m_inputPorts[ i ].IsConnected )
									{
										//if ( AppyModifierToPort( _inputPorts[ i ], true ) )
										//{
										//saveReference = false;
										//}
										if ( !AppyModifierToPort( m_inputPorts[ i ], true ) )
										{
											PickInput( m_inputPorts[ i ] );
										}
										saveReference = false;
									}

									if ( saveReference && !UIUtils.InputPortReference.IsValid )
									//if ( !modifierApplied && !UIUtils.InputPortReference.IsValid )
									{
										UIUtils.InputPortReference.SetReference( m_uniqueId, m_inputPorts[ i ].PortId, m_inputPorts[ i ].DataType, m_inputPorts[ i ].TypeLocked );
									}

									//GUI.Box( portPos, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );
									IsDirty = true;
								}
							}
							GUI.color = colorBuffer;
							currInputPortPos.y += drawInfo.InvertedZoom * ( m_fontHeight + Constants.INPUT_PORT_DELTA_Y );
						}
					}
				}

				//Output Ports
				{
					Rect currOutputPortPos = m_globalPosition;
					currOutputPortPos.width = drawInfo.InvertedZoom * UIUtils.PortsSize.x;
					currOutputPortPos.height = drawInfo.InvertedZoom * UIUtils.PortsSize.y;

					currOutputPortPos.x += ( m_globalPosition.width - drawInfo.InvertedZoom * ( Constants.PORT_INITIAL_X + m_anchorAdjust ) );
					currOutputPortPos.y += drawInfo.InvertedZoom * Constants.PORT_INITIAL_Y;
					int outputCount = m_outputPorts.Count;
					for ( int i = 0; i < outputCount; i++ )
					{
						if ( m_outputPorts[ i ].Visible )
						{
							//Button
							m_outputPorts[ i ].Position = currOutputPortPos;

							//Label
							Rect textPos = currOutputPortPos;
							float deltaX = 1f * drawInfo.InvertedZoom * ( UIUtils.PortsSize.x + Constants.PORT_TO_LABEL_SPACE_X );
							textPos.x -= deltaX;

							if ( m_isVisible )
							{
								UIUtils.CustomStyle( CustomStyle.OutputPortLabel ).normal.textColor = Constants.PortTextColor;
								GUI.Label( textPos, m_outputPorts[ i ].Name, UIUtils.CustomStyle( CustomStyle.OutputPortLabel ) );
							}
							m_sizeContentAux.text = m_outputPorts[ i ].Name;
							if ( drawInfo.ZoomChanged )
								m_outputPorts[ i ].LabelSize = UIUtils.CustomStyle( CustomStyle.OutputPortLabel ).CalcSize( m_sizeContentAux );

							GUIStyle style = m_isVisible ? ( m_outputPorts[ i ].IsConnected ? UIUtils.CustomStyle( CustomStyle.PortFullIcon ) : UIUtils.CustomStyle( CustomStyle.PortEmptyIcon ) ) : null;

							if ( m_outputPorts[ i ].Locked )
							{
								if ( m_isVisible )
								{
									GUI.color = Constants.LockedPortColor;
									GUI.Box( currOutputPortPos, "", style );
								}
							}
							else
							{
								Rect portPos = currOutputPortPos;
								portPos.y -= 5;
								portPos.x -= ( m_outputPorts[ i ].LabelSize.x + deltaX );
								portPos.width += m_outputPorts[ i ].LabelSize.x + deltaX + Constants.PORT_X_ADJUST;
								portPos.height += 10;
								m_outputPorts[ i ].ActivePortArea = portPos;
								if ( m_isVisible )
								{
									GUI.color = m_outputPorts[ i ].HasCustomColor ? m_outputPorts[ i ].CustomColor : UIUtils.GetColorForDataType( m_outputPorts[ i ].DataType, true, false );
									GUI.Box( currOutputPortPos, string.Empty, style );
								}

								if ( m_isVisible && GUI.RepeatButton( portPos, string.Empty, m_empty ) && drawInfo.LeftMouseButtonPressed )
								{
									// need to put the mouse button on a hot state so it will detect the Mouse Up event correctly on the Editor Window
									int controlID = GUIUtility.GetControlID( FocusType.Passive );
									GUIUtility.hotControl = controlID;
									bool saveReference = true;
									if ( m_outputPorts[ i ].IsConnected )
									{
										if ( AppyModifierToPort( m_outputPorts[ i ], false ) )
										{
											saveReference = false;
										}
									}

									if ( saveReference && !UIUtils.OutputPortReference.IsValid )
									{
										UIUtils.OutputPortReference.SetReference( m_uniqueId, m_outputPorts[ i ].PortId, m_outputPorts[ i ].DataType, false );
									}
									//GUI.Box( portPos, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box );
									IsDirty = true;
								}

							}

							GUI.color = colorBuffer;
							currOutputPortPos.y += drawInfo.InvertedZoom * ( m_fontHeight + Constants.INPUT_PORT_DELTA_Y );
						}
					}
				}
				GUI.color = colorBuffer;
			}
		}

		public bool SafeDraw( DrawInfo drawInfo )
		{
			EditorGUI.BeginChangeCheck();
			Draw( drawInfo );
			if ( EditorGUI.EndChangeCheck() )
			{
				SaveIsDirty = true;
				return true;
			}
			return false;
		}

		public void ShowTooltip()
		{
			Rect globalTooltipPos = m_globalPosition;
			globalTooltipPos.x += m_tooltipOffset.x;
			globalTooltipPos.y += m_tooltipOffset.y;
			globalTooltipPos.height *= 1.2f;
			GUI.Label( globalTooltipPos, GUI.tooltip );
		}

		public virtual bool SafeDrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			DrawProperties();
			if ( EditorGUI.EndChangeCheck() )
			{
				//UIUtils.RecordObject(this);
				return true;
			}
			return false;
		}

		virtual public void DrawProperties()
		{
			if ( m_buttonStyle == null )
			{
				m_buttonStyle = new GUIStyle( UIUtils.CurrentWindow.CustomStylesInstance.Button );
			}

			if ( m_labelStyle == null )
			{
				m_labelStyle = UIUtils.CurrentWindow.CustomStylesInstance.Label;
			}

			if ( m_toggleStyle == null )
			{
				m_toggleStyle = new GUIStyle( UIUtils.CurrentWindow.CustomStylesInstance.Toggle );
			}

			if ( m_textfieldStyle == null )
			{
				m_textfieldStyle = UIUtils.CurrentWindow.CustomStylesInstance.Textfield;
			}

			if ( m_useInternalPortData )
			{
				for ( int i = 0; i < m_inputPorts.Count; i++ )
				{
					if ( !m_inputPorts[ i ].IsConnected )
					{
						m_inputPorts[ i ].ShowInternalData();
					}
				}
			}
		}

		protected void PickInput( WirePort port )
		{
			WireReference connection = port.GetConnection( 0 );
			UIUtils.OutputPortReference.SetReference( connection.NodeId, connection.PortId, connection.DataType, connection.TypeLocked );
			UIUtils.DeleteConnection( true, UniqueId, port.PortId, true );
			IsDirty = true;
			SetSaveIsDirty();
		}

		protected bool AppyModifierToPort( WirePort port, bool isInput )
		{
			bool modifierApplied = false;
			switch ( Event.current.modifiers )
			{
				case EventModifiers.Alt:
				{
					UIUtils.DeleteConnection( isInput, UniqueId, port.PortId, true );
					modifierApplied = true;
				}
				break;
				case EventModifiers.Control:
				{
					//WireReference connection = port.GetConnection( 0 );
					//if ( isInput )
					//{
					//	UIUtils.OutputPortReference.SetReference( connection.NodeId, connection.PortId, connection.DataType, connection.TypeLocked );
					//}
					//else
					//{
					//	UIUtils.InputPortReference.SetReference( connection.NodeId, connection.PortId, connection.DataType, connection.TypeLocked );
					//}

					//UIUtils.DeleteConnection( isInput, UniqueId, port.PortId, true );
					//modifierApplied = true;

					if ( !isInput )
					{
						WireReference connection = port.GetConnection( 0 );
						UIUtils.InputPortReference.SetReference( connection.NodeId, connection.PortId, connection.DataType, connection.TypeLocked );
						UIUtils.DeleteConnection( isInput, UniqueId, port.PortId, true );
						modifierApplied = true;
					}
				}
				break;
			}

			if ( modifierApplied )
			{
				IsDirty = true;
				SetSaveIsDirty();
			}
			return modifierApplied;
		}

		public void DeleteAllInputConnections( bool alsoDeletePorts )
		{
			int count = m_inputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_inputPorts[ i ].IsConnected )
					UIUtils.DeleteConnection( true, UniqueId, m_inputPorts[ i ].PortId, false );
			}
			if ( alsoDeletePorts )
				m_inputPorts.Clear();
			SetSaveIsDirty();
		}

		public void DeleteAllOutputConnections( bool alsoDeletePorts )
		{
			int count = m_outputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
					UIUtils.DeleteConnection( false, UniqueId, m_outputPorts[ i ].PortId, false );
			}

			if ( alsoDeletePorts )
				m_outputPorts.Clear();
			SetSaveIsDirty();
		}

		public void DeleteInputPort( int portIdx )
		{
			if ( portIdx >= m_inputPorts.Count )
				return;

			UIUtils.DeleteConnection( true, UniqueId, m_inputPorts[ portIdx ].PortId, false );
			m_inputPorts.RemoveAt( portIdx );

		}

		public void DeleteOutputPort( int portIdx )
		{
			if ( portIdx >= m_outputPorts.Count )
				return;

			UIUtils.DeleteConnection( false, UniqueId, m_outputPorts[ portIdx ].PortId, false );
			m_outputPorts.RemoveAt( portIdx );
		}

		public InputPort GetInputPortById( int id )
		{
			if ( id < m_inputPorts.Count )
				return m_inputPorts[ id ];

			return null;
		}

		public OutputPort GetOutputPortById( int id )
		{
			if ( id < m_outputPorts.Count )
				return m_outputPorts[ id ];

			return null;
		}

		public override string ToString()
		{
			string dump = "";
			dump += ( "Type: " + GetType() );
			dump += ( " Unique Id: " + UniqueId + "\n" );
			dump += ( " Inputs: \n" );

			int inputCount = m_inputPorts.Count;
			int outputCount = m_outputPorts.Count;

			for ( int inputIdx = 0; inputIdx < inputCount; inputIdx++ )
			{
				dump += ( m_inputPorts[ inputIdx ] + "\n" );
			}
			dump += ( "Outputs: \n" );
			for ( int outputIdx = 0; outputIdx < outputCount; outputIdx++ )
			{
				dump += ( m_outputPorts[ outputIdx ] + "\n" );
			}
			return dump;
		}

		public string GetValueFromOutputStr( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( ignoreLocalvar )
			{
				return GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			}

			if ( m_outputPorts[ outputId ].IsLocalValue )
			{
				if ( m_outputPorts[ outputId ].DataType != WirePortDataType.OBJECT && m_outputPorts[ outputId ].DataType != inputPortType )
				{
					return UIUtils.CastPortType( new NodeCastInfo( m_uniqueId, outputId ), null, m_outputPorts[ outputId ].DataType, inputPortType, m_outputPorts[ outputId ].LocalValue );
				}
				else
				{
					return m_outputPorts[ outputId ].LocalValue;
				}
			}

			string result = GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			result = CreateOutputLocalVariable( outputId, result, ref dataCollector );

			if ( m_outputPorts[ outputId ].DataType != WirePortDataType.OBJECT && m_outputPorts[ outputId ].DataType != inputPortType )
			{
				result = UIUtils.CastPortType( new NodeCastInfo( m_uniqueId, outputId ), null, m_outputPorts[ outputId ].DataType, inputPortType, result );
			}
			return result;
		}

		public virtual string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			return string.Empty;
		}

		protected virtual void OnUniqueIDAssigned() { }
		public virtual void ReleaseUniqueIdData() { }

		public string CreateOutputLocalVariable( int outputId, string value, ref MasterNodeDataCollector dataCollector )
		{
			if ( dataCollector.DirtyNormal )
				return value;

			if ( m_outputPorts[ outputId ].IsLocalValue )
				return m_outputPorts[ outputId ].LocalValue;

			if ( m_outputPorts[ outputId ].ConnectionCount > 1 )
			{
				string localVar = m_outputPorts[ outputId ].ConfigOutputLocalValue( value );
				dataCollector.AddToLocalVariables( m_uniqueId, localVar );
				return m_outputPorts[ outputId ].LocalValue;
			}

			return value;
		}

		public void InvalidateConnections()
		{
			int inputCount = m_inputPorts.Count;
			int outputCount = m_outputPorts.Count;

			for ( int i = 0; i < inputCount; i++ )
			{
				m_inputPorts[ i ].InvalidateAllConnections();
			}

			for ( int i = 0; i < outputCount; i++ )
			{
				m_outputPorts[ i ].InvalidateAllConnections();
			}
		}

		public virtual void ResetOutputLocals()
		{
			int outputCount = m_outputPorts.Count;
			for ( int i = 0; i < outputCount; i++ )
			{
				m_outputPorts[ i ].ResetLocalValue();
			}
		}

		public int UniqueId
		{
			get { return m_uniqueId; }

			set
			{
				m_uniqueId = value;
				int inputCount = m_inputPorts.Count;
				int outputCount = m_outputPorts.Count;

				for ( int inputIdx = 0; inputIdx < inputCount; inputIdx++ )
				{
					m_inputPorts[ inputIdx ].NodeId = value;
				}

				for ( int outputIdx = 0; outputIdx < outputCount; outputIdx++ )
				{
					m_outputPorts[ outputIdx ].NodeId = value;
				}
				OnUniqueIDAssigned();
			}
		}

		public Rect Position { get { return m_position; } }

		public Vector2 CenterPosition { get { return new Vector2( m_position.x + m_position.width * 0.5f, m_position.y + m_position.height * 0.5f ); ; } }

		public Rect GlobalPosition { get { return m_globalPosition; } }

		public Vector2 Corner { get { return new Vector2( m_position.x + m_position.width, m_position.y + m_position.height ); } }
		public Vector2 Vec2Position
		{
			get { return new Vector2( m_position.x, m_position.y ); }

			set
			{
				m_position.x = value.x;
				m_position.y = value.y;
			}
		}

		public bool Selected
		{
			get { return m_selected; }
			set
			{
				//if ( value )
				//{
				//	for ( int i = 0; i < m_inputPorts.Count; i++ )
				//	{
				//		Debug.Log( string.Format( "Input[{0}] - {1}", i, m_inputPorts[ i ].DataType ));
				//	}
				//	for ( int i = 0; i < m_outputPorts.Count; i++ )
				//	{
				//		Debug.Log( string.Format( "Output[{0}] - {1}", i, m_outputPorts[ i ].DataType ) );
				//	}
				//}
				m_infiniteLoopDetected = false;
				m_selected = value;
			}
		}

		public List<InputPort> InputPorts { get { return m_inputPorts; } }

		public List<OutputPort> OutputPorts
		{
			get { return m_outputPorts; }
		}

		public bool IsConnected { get { return m_connStatus == NodeConnectionStatus.Connected; } }
		public NodeConnectionStatus ConnStatus
		{
			get { return m_connStatus; }
			set
			{
				if ( m_selfPowered )
				{
					m_connStatus = NodeConnectionStatus.Connected;
				}
				else
				{
					m_connStatus = value;
				}

				switch ( m_connStatus )
				{
					case NodeConnectionStatus.Island:
					case NodeConnectionStatus.Not_Connected: m_statusColor = Constants.NodeDefaultColor; break;
					case NodeConnectionStatus.Connected: m_statusColor = Constants.NodeConnectedColor; break;
					case NodeConnectionStatus.Error: m_statusColor = Constants.NodeErrorColor; break;
				}

			}
		}

		public bool SelfPowered
		{
			set
			{
				m_selfPowered = value;
				if ( value )
				{
					ConnStatus = NodeConnectionStatus.Connected;
				}
			}
		}

		// This is also called when recording on Undo
		public virtual void OnBeforeSerialize() { }
		public virtual void OnAfterDeserialize()
		{
			m_selected = false;
			m_isOnGrid = false;
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].ResetWireReferenceStatus();
			}
			//	OnNodeStoppedMovingEvent = null;
		}
		//Inherited classes must call this base method in order to setup id and position
		public virtual void ReadFromString( ref string[] nodeParams )
		{
			m_currentReadParamIdx = IOUtils.NodeTypeId + 1;

			UniqueId = Convert.ToInt32( nodeParams[ m_currentReadParamIdx++ ] );

			string[] posCoordinates = nodeParams[ m_currentReadParamIdx++ ].Split( IOUtils.VECTOR_SEPARATOR );

			m_position.x = Convert.ToSingle( posCoordinates[ 0 ] );
			m_position.y = Convert.ToSingle( posCoordinates[ 1 ] );
		}

		//should be called last, after ReadFromString
		public void ReadInputDataFromString( ref string[] nodeParams )
		{
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].InternalData = nodeParams[ m_currentReadParamIdx++ ];
			}
		}

		protected string GetCurrentParam( ref string[] nodeParams )
		{
			if ( m_currentReadParamIdx < nodeParams.Length )
			{
				return nodeParams[ m_currentReadParamIdx++ ];
			}

			UIUtils.ShowMessage( "Invalid params number in node " + m_uniqueId + " of type " + GetType(), MessageSeverity.Error );
			return string.Empty;
		}

		public virtual void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			IOUtils.AddTypeToString( ref nodeInfo, IOUtils.NodeParam );
			IOUtils.AddFieldValueToString( ref nodeInfo, GetType() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_uniqueId );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_position.x.ToString() + IOUtils.VECTOR_SEPARATOR + m_position.y.ToString() ) );

			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].WriteToString( ref connectionsInfo );
			}
		}

		public virtual void WriteInputDataToString( ref string nodeInfo )
		{
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].InternalData );
			}
		}

		public virtual string GetIncludes() { return string.Empty; }
		public virtual void OnObjectDropped( UnityEngine.Object obj ) { }
		public virtual void SetupFromCastObject( UnityEngine.Object obj ) { }
		public virtual bool OnNodeInteraction( ParentNode node ) { return false; }
		public virtual void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type ) { }
		public virtual void OnConnectedInputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type ) { }

		public Rect CachedPos { get { return m_cachedPos; } }

		public bool IsOnGrid
		{
			set { m_isOnGrid = value; }
			get { return m_isOnGrid; }
		}

		public uint CurrentReadParamIdx
		{
			get { return m_currentReadParamIdx++; }
			set { m_currentReadParamIdx = value; }
		}

		public Dictionary<string, InputPort> InputPortsDict
		{
			get
			{
				Dictionary<string, InputPort> dict = new Dictionary<string, InputPort>();
				for ( int i = 0; i < m_inputPorts.Count; i++ )
				{
					dict.Add( m_inputPorts[ i ].Name, m_inputPorts[ i ] );
				}
				return dict;
			}
		}

		public bool IsDirty
		{
			set
			{
				m_isDirty = value && UIUtils.DirtyMask;
			}
			get
			{
				bool value = m_isDirty;
				m_isDirty = false;
				return value;
			}
		}

		public virtual string DataToArray { get { return string.Empty; } }

		public bool SaveIsDirty
		{
			set
			{
				m_saveIsDirty = value && UIUtils.DirtyMask;
			}
			get
			{
				bool value = m_saveIsDirty;
				m_saveIsDirty = false;
				return value;
			}
		}
		public GUIContent TitleContent { get { return m_content; } }
		public GUIContent AdditonalTitleContent { get { return m_additionalContent; } }
		public bool IsVisible { get { return m_isVisible; } }
		public NodeAttributes Attributes { get { return m_nodeAttribs; } }
		public bool ReorderLocked { get { return m_reorderLocked; } }
		public bool RequireMaterialUpdate { get { return m_requireMaterialUpdate; } }
		public bool RMBIgnore { get { return m_rmbIgnore; } }
		public float TextLabelWidth { get { return m_textLabelWidth; } }
		public bool MovingInFrame { get { return m_movingInFrame; } }
		public bool SizeIsDirty { get { return m_sizeIsDirty; } }

		public bool IsCommentaryParented
		{
			get { return m_isCommentaryParented; }
			set { m_isCommentaryParented = value; }
		}

		public int Depth
		{
			get { return m_depth; }
			set { m_depth = value; }
		}

	}
}
