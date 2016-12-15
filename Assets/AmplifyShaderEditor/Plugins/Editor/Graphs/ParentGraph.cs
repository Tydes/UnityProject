// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ParentGraph : ISerializationCallbackReceiver
	{
		public delegate void EmptyGraphDetected( ParentGraph graph );
		public event EmptyGraphDetected OnEmptyGraphDetectedEvt;

		public delegate void NodeEvent( ParentNode node );
		public event NodeEvent OnNodeEvent;

		public event MasterNode.OnMaterialUpdated OnMaterialUpdatedEvent;
		public event MasterNode.OnMaterialUpdated OnShaderUpdatedEvent;

		private bool m_afterDeserializeFlag = true;
		[SerializeField]
		private int m_validNodeId;

		[SerializeField]
		private List<ParentNode> m_nodes;


		// Sampler Nodes registry
		[SerializeField]
		private List<SamplerNode> m_samplerNodes;

		[SerializeField]
		private string[] m_samplerNodesArr;
		
		// Screen Color Nodes registry
		[SerializeField]
		private List<ScreenColorNode> m_screenColorNodes;

		[SerializeField]
		private string[] m_screenColorNodesArr;

		[SerializeField]
		NodeUsageRegister m_localVarNodes;
		////////////////////////////////////////

		[ SerializeField]
		private int m_masterNodeId = Constants.INVALID_NODE_ID;

		[SerializeField]
		private bool m_isDirty;

		[SerializeField]
		private bool m_saveIsDirty = false;

		[SerializeField]
		private int m_nodeClicked;

		[SerializeField]
		private int m_loadedShaderVersion;

		[SerializeField]
		private int m_instancePropertyCount = 0;

		[SerializeField]
		protected int m_normalDependentCount = 0;
		
		private List<ParentNode> m_visibleNodes = new List<ParentNode>();

		private Dictionary<int, ParentNode> m_nodesDict;
		private List<ParentNode> m_selectedNodes;
		private List<WireReference> m_highlightedWires;
		private Type m_masterNodeDefaultType;

		private NodeGrid m_nodeGrid;

		private bool m_markedToDeSelect = false;
		private int m_markToSelect = -1;
		private bool m_markToReOrder = false;

		private bool m_hasUnConnectedNodes = false;

		private bool m_checkSelectedWireHighlights = false;

		public ParentGraph()
		{
			m_nodeGrid = new NodeGrid();
			m_nodes = new List<ParentNode>();
			m_samplerNodes = new List<SamplerNode>();
			m_screenColorNodes = new List<ScreenColorNode>();
			m_localVarNodes = new NodeUsageRegister();

			m_selectedNodes = new List<ParentNode>();
			m_highlightedWires = new List<WireReference>();
			m_nodesDict = new Dictionary<int, ParentNode>();
			m_validNodeId = 0;
			IsDirty = false;
			SaveIsDirty = false;
			m_masterNodeDefaultType = typeof( StandardSurfaceOutputNode );
		}

		public int GetValidId()
		{
			return m_validNodeId++;
		}

		void UpdateIdFromNode( ParentNode node )
		{
			if ( node.UniqueId >= m_validNodeId )
			{
				m_validNodeId = node.UniqueId + 1;
			}
		}

		public void CleanUnusedNodes()
		{
			List<ParentNode> unusedNodes = new List<ParentNode>();
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( m_nodes[ i ].ConnStatus == NodeConnectionStatus.Not_Connected )
				{
					unusedNodes.Add( m_nodes[ i ] );
				}
			}

			for ( int i = 0; i < unusedNodes.Count; i++ )
			{
				DestroyNode( unusedNodes[ i ] );
			}
			unusedNodes.Clear();
			unusedNodes = null;

			IsDirty = true;
		}

		public void CleanNodes()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].Destroy();
				GameObject.DestroyImmediate( m_nodes[ i ] );
			}

			m_masterNodeId = Constants.INVALID_NODE_ID;
			m_validNodeId = 0;

			m_nodes.Clear();
			m_samplerNodes.Clear();
			m_screenColorNodes.Clear();
			m_localVarNodes.Clear();
			m_selectedNodes.Clear();
		}

		public void ResetHighlightedWires()
		{
			for ( int i = 0; i < m_highlightedWires.Count; i++ )
			{
				m_highlightedWires[ i ].WireStatus = WireStatus.Default;
			}
			m_highlightedWires.Clear();
		}

		public void HighlightWiresStartingNode( ParentNode node )
		{
			for ( int outputIdx = 0; outputIdx < node.OutputPorts.Count; outputIdx++ )
			{
				for ( int extIdx = 0; extIdx < node.OutputPorts[ outputIdx ].ExternalReferences.Count; extIdx++ )
				{
					WireReference wireRef = node.OutputPorts[ outputIdx ].ExternalReferences[ extIdx ];
					ParentNode nextNode = GetNode( wireRef.NodeId );
					if ( nextNode && nextNode.ConnStatus == NodeConnectionStatus.Connected )
					{
						if ( nextNode.InputPorts[ wireRef.PortId ].ExternalReferences.Count == 0 || nextNode.InputPorts[ wireRef.PortId ].ExternalReferences[ 0 ].WireStatus == WireStatus.Highlighted )
						{
							// if even one wire is already highlighted then this tells us that this node was already been analysed
							return;
						}

						nextNode.InputPorts[ wireRef.PortId ].ExternalReferences[ 0 ].WireStatus = WireStatus.Highlighted;
						m_highlightedWires.Add( nextNode.InputPorts[ wireRef.PortId ].ExternalReferences[ 0 ] );
						HighlightWiresStartingNode( nextNode );
					}
				}
			}
		}

		void PropagateHighlightDeselection( ParentNode node, int portId = -1 )
		{
			if ( portId > -1 )
			{
				node.InputPorts[ portId ].ExternalReferences[ 0 ].WireStatus = WireStatus.Default;
			}

			if ( node.Selected )
				return;

			for ( int i = 0; i < node.InputPorts.Count; i++ )
			{
				if ( node.InputPorts[ i ].ExternalReferences.Count > 0 && node.InputPorts[ i ].ExternalReferences[ 0 ].WireStatus == WireStatus.Highlighted )
				{
					// even though node is deselected, it receives wire highlight from a previous one 
					return;
				}
			}

			for ( int outputIdx = 0; outputIdx < node.OutputPorts.Count; outputIdx++ )
			{
				for ( int extIdx = 0; extIdx < node.OutputPorts[ outputIdx ].ExternalReferences.Count; extIdx++ )
				{
					WireReference wireRef = node.OutputPorts[ outputIdx ].ExternalReferences[ extIdx ];
					ParentNode nextNode = GetNode( wireRef.NodeId );
					PropagateHighlightDeselection( nextNode, wireRef.PortId );
				}
			}
		}

		public void Destroy()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( m_nodes[ i ] != null )
				{
					m_nodes[ i ].Destroy();
					GameObject.DestroyImmediate( m_nodes[ i ] );
				}
			}

			m_masterNodeId = Constants.INVALID_NODE_ID;
			m_validNodeId = 0;

			m_nodeGrid.Destroy();
			m_nodeGrid = null;

			m_nodes.Clear();
			m_nodes = null;

			m_samplerNodes.Clear();
			m_samplerNodes = null;

			m_screenColorNodes.Clear();
			m_screenColorNodes = null;

			m_localVarNodes.Destroy();
			m_localVarNodes = null;
			
			m_selectedNodes.Clear();
			m_selectedNodes = null;

			m_nodesDict.Clear();
			m_nodesDict = null;

			IsDirty = true;

			OnNodeEvent = null;

			OnMaterialUpdatedEvent = null;
			OnShaderUpdatedEvent = null;
			OnEmptyGraphDetectedEvt = null;
		}

		void OnNodeChangeSizeEvent( ParentNode node )
		{
			m_nodeGrid.RemoveNodeFromGrid( node, true );
			m_nodeGrid.AddNodeToGrid( node );
		}

		void OnNodeFinishMoving( ParentNode node, bool testOnlySelected, InteractionMode interactionMode )
		{
			if ( OnNodeEvent != null )
				OnNodeEvent( node );

			m_nodeGrid.RemoveNodeFromGrid( node, true );
			m_nodeGrid.AddNodeToGrid( node );

			if ( testOnlySelected )
			{
				for ( int i = m_visibleNodes.Count - 1; i > -1; i-- )
				{
					if ( node.UniqueId != m_visibleNodes[ i ].UniqueId )
					{
						switch ( interactionMode )
						{
							case InteractionMode.Target:
							{
								node.OnNodeInteraction( m_visibleNodes[ i ] );
							}
							break;
							case InteractionMode.Other:
							{
								m_visibleNodes[ i ].OnNodeInteraction( node );
							}
							break;
							case InteractionMode.Both:
							{
								node.OnNodeInteraction( m_visibleNodes[ i ] );
								m_visibleNodes[ i ].OnNodeInteraction( node );
							}
							break;
						}
					}
				}
			}
			else
			{
				for ( int i = m_nodes.Count - 1; i > -1; i-- )
				{
					if ( node.UniqueId != m_nodes[ i ].UniqueId )
					{
						switch ( interactionMode )
						{
							case InteractionMode.Target:
							{
								node.OnNodeInteraction( m_nodes[ i ] );
							}
							break;
							case InteractionMode.Other:
							{
								m_nodes[ i ].OnNodeInteraction( node );
							}
							break;
							case InteractionMode.Both:
							{
								node.OnNodeInteraction( m_nodes[ i ] );
								m_nodes[ i ].OnNodeInteraction( node );
							}
							break;
						}
					}
				}
			}
		}


		public void OnNodeReOrderEvent( ParentNode node, int index )
		{
			if ( node.Depth < index )
			{
				Debug.LogWarning( "Reorder canceled: This is a specific method for when reordering needs to be done and a its original index is higher than the new one" );
			}
			else
			{
				m_nodes.Remove( node );
				m_nodes.Insert( index, node );
				m_markToReOrder = true;
			}
		}

		public void AddNode( ParentNode node, bool updateId = false, bool addLast = true )
		{
			if ( OnNodeEvent != null )
			{
				OnNodeEvent( node );
			}
			if ( updateId )
			{
				node.UniqueId = GetValidId();
			}
			else
			{
				UpdateIdFromNode( node );
			}

			node.SetMaterialMode( CurrentMaterial );

			if ( addLast )
			{
				m_nodes.Add( node );
				node.Depth = m_nodes.Count;
			}
			else
			{
				m_nodes.Insert( 0, node );
				node.Depth = 0;
			}

			if ( m_nodesDict.ContainsKey( node.UniqueId ) )
				m_nodesDict[ node.UniqueId ] = node;
			else
				m_nodesDict.Add( node.UniqueId, node );

			m_nodeGrid.AddNodeToGrid( node );
			node.OnNodeChangeSizeEvent += OnNodeChangeSizeEvent;
			node.OnNodeReOrderEvent += OnNodeReOrderEvent;
			IsDirty = true;
		}

		public void RemoveNode( ParentNode node )
		{
			m_nodes.Remove( node );
			m_nodesDict.Remove( node.UniqueId );
			if ( node.UniqueId == m_masterNodeId )
			{
				m_masterNodeId = -1;
			}
			node.Destroy();
			IsDirty = true;
			m_markToReOrder = true;
		}

		public ParentNode GetClickedNode()
		{
			if ( m_nodeClicked < 0 )
				return null;
			return GetNode( m_nodeClicked );
		}

		public ParentNode GetNode( int nodeId )
		{
			if ( m_nodesDict.Count != m_nodes.Count )
			{
				m_nodesDict.Clear();
				for ( int i = 0; i < m_nodes.Count; i++ )
				{
					if ( m_nodes[ i ] != null )
						m_nodesDict.Add( m_nodes[ i ].UniqueId, m_nodes[ i ] );
				}
			}

			if ( m_nodesDict.ContainsKey( nodeId ) )
				return m_nodesDict[ nodeId ];

			return null;
		}

		public void ForceReOrder()
		{
			m_nodes.Sort( ( x, y ) => x.Depth.CompareTo( y.Depth ) );
		}
		
		public bool Draw( DrawInfo drawInfo )
		{
			
			if ( m_afterDeserializeFlag )
			{
				m_afterDeserializeFlag = false;
				CleanCorruptedNodes();
				if ( m_nodes.Count == 0 )
				{
					UIUtils.CreateNewGraph( "Empty" );
					SaveIsDirty = true;
					if ( OnEmptyGraphDetectedEvt != null )
						OnEmptyGraphDetectedEvt( this );
				}
			}

			if ( m_markedToDeSelect )
				DeSelectAll();

			if ( m_markToSelect > -1 )
			{
				AddToSelectedNodes( GetNode( m_markToSelect ) );
				m_markToSelect = -1;
			}

			if ( m_markToReOrder )
			{
				m_markToReOrder = false;
				for ( int i = 0; i < m_nodes.Count; i++ )
				{
					m_nodes[ i ].Depth = i;
				}
			}

			m_visibleNodes.Clear();
			int nullCount = 0;
			m_hasUnConnectedNodes = false;
			bool repaint = false;
			MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
			Material currentMaterial = masterNode != null ? masterNode.CurrentMaterial : null;
			EditorGUI.BeginChangeCheck();
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( m_nodes[ i ] != null )
				{
					//repaint = repaint || _nodes[ i ].SafeDraw( drawInfo );
					if ( !m_nodes[ i ].IsOnGrid )
					{
						m_nodeGrid.AddNodeToGrid( m_nodes[ i ] );
					}

					bool restoreMouse = false;
					if ( Event.current.type == EventType.mouseDown && m_nodes[ i ].UniqueId != m_nodeClicked )
					{
						restoreMouse = true;
						Event.current.type = EventType.ignore;
					}

					m_nodes[ i ].Draw( drawInfo );

					if ( restoreMouse )
					{
						Event.current.type = EventType.mouseDown;
					}

					m_hasUnConnectedNodes = m_hasUnConnectedNodes ||
											( m_nodes[ i ].ConnStatus != NodeConnectionStatus.Connected && m_nodes[ i ].ConnStatus != NodeConnectionStatus.Island );

					if ( m_nodes[ i ].RequireMaterialUpdate && currentMaterial != null )
					{
						m_nodes[ i ].UpdateMaterial( currentMaterial );
						if ( currentMaterial == Selection.activeObject )
						{

						}
					}

					if ( m_nodes[ i ].IsVisible )
						m_visibleNodes.Add( m_nodes[ i ] );

					IsDirty = ( m_isDirty || m_nodes[ i ].IsDirty );
					SaveIsDirty = ( m_saveIsDirty || m_nodes[ i ].SaveIsDirty );
				}
				else
					nullCount += 1;
			}

			if ( m_checkSelectedWireHighlights )
			{
				m_checkSelectedWireHighlights = false;
				ResetHighlightedWires();
				for ( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					HighlightWiresStartingNode( m_selectedNodes[ i ] );
				}
			}

			if ( EditorGUI.EndChangeCheck() )
			{
				repaint = true;
				SaveIsDirty = true;
			}

			if ( nullCount == m_nodes.Count )
				m_nodes.Clear();

			return repaint;
		}


		public void DrawWires( Texture2D wireTex, DrawInfo drawInfo, bool contextPaletteActive, Vector3 contextPalettePos )
		{
			Handles.BeginGUI();
			// Draw connected node wires
			for ( int nodeIdx = 0; nodeIdx < m_nodes.Count; nodeIdx++ )
			{
				ParentNode node = m_nodes[ nodeIdx ];
				if ( node == null )
					return;

				for ( int inputPortIdx = 0; inputPortIdx < node.InputPorts.Count; inputPortIdx++ )
				{
					InputPort inputPort = node.InputPorts[ inputPortIdx ];
					if ( inputPort.ExternalReferences.Count > 0 )
					{
						bool cleanInvalidConnections = false;
						for ( int wireIdx = 0; wireIdx < inputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference reference = inputPort.ExternalReferences[ wireIdx ];
							if ( reference.NodeId != -1 && reference.PortId != -1 )
							{
								ParentNode outputNode = GetNode( reference.NodeId );
								if ( outputNode != null )
								{
									OutputPort outputPort = outputNode.GetOutputPortById( reference.PortId );
									Vector3 endPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
									Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );

									bool isVisible = node.IsVisible || outputNode.IsVisible;
									if ( !isVisible )
									{
										float x = ( startPos.x < endPos.x ) ? startPos.x : endPos.x;
										float y = ( startPos.y < endPos.y ) ? startPos.y : endPos.y;
										float width = Mathf.Abs( startPos.x - endPos.x );
										float height = Mathf.Abs( startPos.y - endPos.y );
										Rect portsBoundingBox = new Rect( x, y, width, height );
										isVisible = drawInfo.TransformedCameraArea.Overlaps( portsBoundingBox );
									}

									if ( isVisible )
									{
										DrawBezier( drawInfo.InvertedZoom, startPos, endPos, reference.WireStatus, wireTex );
									}
								}
								else
								{
									UIUtils.ShowMessage( "Detected Invalid connection from node " + node.UniqueId + " port " + inputPortIdx + " to Node " + reference.NodeId + " port " + reference.PortId, MessageSeverity.Error );
									cleanInvalidConnections = true;
									inputPort.ExternalReferences[ wireIdx ].Invalidate();
								}
							}
						}

						if ( cleanInvalidConnections )
						{
							inputPort.RemoveInvalidConnections();
						}
					}
				}
			}


			//Draw selected wire
			if ( UIUtils.ValidReferences() )
			{
				if ( UIUtils.InputPortReference.IsValid )
				{
					InputPort inputPort = GetNode( UIUtils.InputPortReference.NodeId ).GetInputPortById( UIUtils.InputPortReference.PortId );
					Vector3 endPos = Vector3.zero;
					if ( UIUtils.SnapEnabled )
					{
						Vector2 pos = ( UIUtils.SnapPosition + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
						endPos = new Vector3( pos.x, pos.y ) + UIUtils.ScaledPortsDelta;
					}
					else
					{
						endPos = contextPaletteActive ? contextPalettePos : new Vector3( Event.current.mousePosition.x, Event.current.mousePosition.y );
					}

					Vector3 startPos = new Vector3( inputPort.Position.x, inputPort.Position.y );
					DrawBezier( drawInfo.InvertedZoom, endPos, startPos, WireStatus.Default, wireTex );
				}

				if ( UIUtils.OutputPortReference.IsValid )
				{
					OutputPort outputPort = GetNode( UIUtils.OutputPortReference.NodeId ).GetOutputPortById( UIUtils.OutputPortReference.PortId );
					Vector3 endPos = Vector3.zero;
					if ( UIUtils.SnapEnabled )
					{
						Vector2 pos = ( UIUtils.SnapPosition + drawInfo.CameraOffset ) * drawInfo.InvertedZoom;
						endPos = new Vector3( pos.x, pos.y ) + UIUtils.ScaledPortsDelta;
					}
					else
					{
						endPos = contextPaletteActive ? contextPalettePos : new Vector3( Event.current.mousePosition.x, Event.current.mousePosition.y );
					}
					Vector3 startPos = new Vector3( outputPort.Position.x, outputPort.Position.y );
					DrawBezier( drawInfo.InvertedZoom, startPos, endPos, WireStatus.Default, wireTex );
				}
			}
			Handles.EndGUI();
		}

		void DrawBezier( float invertedZoom, Vector3 startPos, Vector3 endPos, WireStatus wireStatus, Texture2D wireTex )
		{
			startPos += UIUtils.ScaledPortsDelta;
			endPos += UIUtils.ScaledPortsDelta;

			//float wireMinTangent = 50;
			float wiresTickness =/* drawInfo.InvertedZoom * */Constants.WIRE_WIDTH;

			//float multiplier = Constants.WIRE_CONTROL_POINT_DIST;

			//float xMag = ( endPos.x - startPos.x );
			//if ( xMag < 0 )
			//{
			//	xMag = -xMag;
			//	multiplier = Constants.WIRE_CONTROL_POINT_DIST_INV;
			//}

			//if ( xMag < wireMinTangent )
			//{
			//	multiplier = Constants.WIRE_CONTROL_POINT_DIST_INV;
			//	xMag = wireMinTangent;
			//}

			float mag = Mathf.Min( ( endPos - startPos ).magnitude, 100 * invertedZoom );
			Vector3 startTangent = new Vector3( startPos.x + mag, startPos.y );
			Vector3 endTangent = new Vector3( endPos.x - mag, endPos.y );

			//Vector3 startTangent = new Vector3( startPos.x + multiplier * xMag, startPos.y );
			//Vector3 endTangent = new Vector3( endPos.x - multiplier * xMag, endPos.y );

			Handles.DrawBezier( startPos, endPos, startTangent, endTangent, UIUtils.GetColorFromWireStatus( wireStatus ), wireTex, wiresTickness );
			//Handles.DrawLine( startPos, startTangent );
			//Handles.DrawLine( endPos, endTangent );
		}


		public void MoveSelectedNodes( Vector2 delta )
		{
			for ( int i = 0; i < m_selectedNodes.Count; i++ )
			{
				if ( !m_selectedNodes[ i ].MovingInFrame )
					m_selectedNodes[ i ].Move( delta );
			}
			IsDirty = true;
		}

		public void SetConnection( int InNodeId, int InPortId, int OutNodeId, int OutPortId )
		{
			ParentNode inNode = GetNode( InNodeId );
			ParentNode outNode = GetNode( OutNodeId );
			InputPort inputPort = null;
			OutputPort outputPort = null;
			if ( inNode != null && outNode != null )
			{
				inputPort = inNode.GetInputPortById( InPortId );
				outputPort = outNode.GetOutputPortById( OutPortId );
				if ( inputPort != null && outputPort != null )
				{

					if ( inputPort.IsConnectedTo( OutNodeId, OutPortId ) ||
							outputPort.IsConnectedTo( InNodeId, InPortId )
						)
					{
						UIUtils.ShowMessage( "Node/Port already connected " + InNodeId, MessageSeverity.Error );
						return;
					}

					if ( inputPort.ConnectTo( OutNodeId, OutPortId, outputPort.DataType, false ) )
					{
						inNode.OnInputPortConnected( InPortId, OutNodeId, OutPortId );
					}


					if ( outputPort.ConnectTo( InNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked ) )
					{
						outNode.OnOutputPortConnected( OutPortId, InNodeId, InPortId );
					}
				}
				else if ( inputPort == null )
				{
					UIUtils.ShowMessage( "Input Port " + InPortId + " doesn't exist on node " + InNodeId, MessageSeverity.Error );
				}
				else
				{
					UIUtils.ShowMessage( "Output Port " + OutPortId + " doesn't exist on node " + OutNodeId, MessageSeverity.Error );
				}
			}
			else if ( inNode == null )
			{
				UIUtils.ShowMessage( "Input node " + InNodeId + " doesn't exist", MessageSeverity.Error );
			}
			else
			{
				UIUtils.ShowMessage( "Output node " + OutNodeId + " doesn't exist", MessageSeverity.Error );
			}
		}

		public void DeleteConnection( bool isInput, int nodeId, int portId, bool registerOnLog )
		{
			ParentNode node = GetNode( nodeId );
			if ( node == null )
				return;

			if ( isInput )
			{
				InputPort inputPort = node.GetInputPortById( portId );
				if ( inputPort.IsConnected )
				{

					if ( node.ConnStatus == NodeConnectionStatus.Connected )
					{
						inputPort.GetOutputNode().DeactivateNode( portId, false );
						m_checkSelectedWireHighlights = true;
					}

					for ( int i = 0; i < inputPort.ExternalReferences.Count; i++ )
					{
						WireReference inputReference = inputPort.ExternalReferences[ i ];
						GetNode( inputReference.NodeId ).GetOutputPortById( inputReference.PortId ).InvalidateConnection( inputPort.NodeId, inputPort.PortId );
					}
					inputPort.InvalidateAllConnections();
					node.OnInputPortDisconnected( portId );
				}
			}
			else
			{
				OutputPort outputPort = node.GetOutputPortById( portId );
				if ( outputPort.IsConnected )
				{
					for ( int i = 0; i < outputPort.ExternalReferences.Count; i++ )
					{
						WireReference outputReference = outputPort.ExternalReferences[ i ];
						ParentNode refNode = GetNode( outputReference.NodeId );
						if ( refNode.ConnStatus == NodeConnectionStatus.Connected )
						{
							node.DeactivateNode( portId, false );
							m_checkSelectedWireHighlights = true;
						}
						refNode.GetInputPortById( outputReference.PortId ).InvalidateConnection( outputPort.NodeId, outputPort.PortId );
					}
					outputPort.InvalidateAllConnections();
				}
			}
			IsDirty = true;
		}

		public void DeleteSelectedNodes()
		{
			bool invalidateMasterNode = false;
			for ( int nodeIdx = 0; nodeIdx < m_selectedNodes.Count; nodeIdx++ )
			{
				ParentNode node = m_selectedNodes[ nodeIdx ];
				if ( node.UniqueId == m_masterNodeId )
				{
					invalidateMasterNode = true;
				}
				else
				{
					DestroyNode( node );
				}
			}

			if ( invalidateMasterNode )
			{
				CurrentMasterNode.Selected = false;
			}
			//Clear all references
			m_selectedNodes.Clear();
			IsDirty = true;
		}

		public void DestroyNode( int nodeId )
		{
			ParentNode node = GetNode( nodeId );
			DestroyNode( node );
		}

		public void DestroyNode( ParentNode node )
		{
			if ( node == null )
			{
				UIUtils.ShowMessage( "Attempting to destroying a inexistant node ", MessageSeverity.Warning );
				return;
			}

			if ( node.ConnStatus == NodeConnectionStatus.Connected && !m_checkSelectedWireHighlights )
			{
				ResetHighlightedWires();
				m_checkSelectedWireHighlights = true;
			}

			if ( node.UniqueId != m_masterNodeId )
			{
				m_nodeGrid.RemoveNodeFromGrid( node, false );
				//Send Deactivation signal if active
				if ( node.ConnStatus == NodeConnectionStatus.Connected )
				{
					node.DeactivateNode( -1, true );
				}

				//Invalidate references
				//Invalidate input references
				for ( int inputPortIdx = 0; inputPortIdx < node.InputPorts.Count; inputPortIdx++ )
				{
					InputPort inputPort = node.InputPorts[ inputPortIdx ];
					if ( inputPort.IsConnected )
					{
						for ( int wireIdx = 0; wireIdx < inputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference inputReference = inputPort.ExternalReferences[ wireIdx ];
							GetNode( inputReference.NodeId ).GetOutputPortById( inputReference.PortId ).InvalidateConnection( inputPort.NodeId, inputPort.PortId );
						}
						inputPort.InvalidateAllConnections();
					}
				}

				//Invalidate output reference
				for ( int outputPortIdx = 0; outputPortIdx < node.OutputPorts.Count; outputPortIdx++ )
				{
					OutputPort outputPort = node.OutputPorts[ outputPortIdx ];
					if ( outputPort.IsConnected )
					{
						for ( int wireIdx = 0; wireIdx < outputPort.ExternalReferences.Count; wireIdx++ )
						{
							WireReference outputReference = outputPort.ExternalReferences[ wireIdx ];
							ParentNode outnode = GetNode( outputReference.NodeId );
							if ( outnode != null )
							{
								outnode.GetInputPortById( outputReference.PortId ).InvalidateConnection( outputPort.NodeId, outputPort.PortId );
								outnode.OnInputPortDisconnected( outputReference.PortId );
							}
						}
						outputPort.InvalidateAllConnections();
					}
				}

				//Remove node from main list
				m_nodes.Remove( node );
				m_nodesDict.Remove( node.UniqueId );
				node.Destroy();
				UIUtils.DestroyObjectImmediate( node );
				IsDirty = true;
				m_markToReOrder = true;
			}
			else
			{
				DeselectNode( node );
				UIUtils.ShowMessage( "Attempting to destroy a master node" );
			}
		}

		void AddToSelectedNodes( ParentNode node )
		{
			node.Selected = true;
			m_selectedNodes.Add( node );
			node.OnNodeStoppedMovingEvent += OnNodeFinishMoving;
			if ( node.ConnStatus == NodeConnectionStatus.Connected )
			{
				HighlightWiresStartingNode( node );
			}
		}

		void RemoveFromSelectedNodes( ParentNode node )
		{
			node.Selected = false;
			m_selectedNodes.Remove( node );
			node.OnNodeStoppedMovingEvent -= OnNodeFinishMoving;
		}

		public void SelectNode( ParentNode node, bool append, bool reorder )
		{

			if ( append )
			{
				if ( !m_selectedNodes.Contains( node ) )
				{
					AddToSelectedNodes( node );
				}
			}
			else
			{
				DeSelectAll();
				AddToSelectedNodes( node );
			}
			if ( reorder && !node.ReorderLocked )
			{
				m_nodes.Remove( node );
				m_nodes.Add( node );
				m_markToReOrder = true;
			}
		}

		public void MultipleSelection( Rect selectionArea, bool append, bool reorder )
		{
			if ( !append )
				DeSelectAll();

			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( !m_nodes[ i ].Selected && selectionArea.Overlaps( m_nodes[ i ].Position, true ) )
				{
					m_nodes[ i ].Selected = true;
					AddToSelectedNodes( m_nodes[ i ] );
				}
			}
			if ( reorder )
			{
				for ( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					if ( !m_selectedNodes[ i ].ReorderLocked )
					{
						m_nodes.Remove( m_selectedNodes[ i ] );
						m_nodes.Add( m_selectedNodes[ i ] );
						m_markToReOrder = true;
					}
				}
			}
		}

		public void SelectAll()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( !m_nodes[ i ].Selected )
					AddToSelectedNodes( m_nodes[ i ] );
			}
		}

		public void SelectMasterNode()
		{
			if ( m_masterNodeId != Constants.INVALID_NODE_ID )
			{
				SelectNode( CurrentMasterNode, false, false );
			}
		}

		public void DeselectNode( int nodeId )
		{
			ParentNode node = GetNode( nodeId );
			if ( node )
			{
				m_selectedNodes.Remove( node );
				node.Selected = false;
			}
		}

		public void DeselectNode( ParentNode node )
		{
			m_selectedNodes.Remove( node );
			node.Selected = false;
			PropagateHighlightDeselection( node );
		}



		public void DeSelectAll()
		{
			m_markedToDeSelect = false;
			for ( int i = 0; i < m_selectedNodes.Count; i++ )
			{
				m_selectedNodes[ i ].Selected = false;
				m_selectedNodes[ i ].OnNodeStoppedMovingEvent -= OnNodeFinishMoving;
			}
			m_selectedNodes.Clear();
			ResetHighlightedWires();
		}

		public void AssignMasterNode()
		{
			if ( m_selectedNodes.Count == 1 )
			{
				MasterNode newMasterNode = m_selectedNodes[ 0 ] as MasterNode;
				if ( newMasterNode != null )
				{
					if ( m_masterNodeId != Constants.INVALID_NODE_ID && m_masterNodeId != newMasterNode.UniqueId )
					{
						MasterNode oldMasterNode = GetNode( m_masterNodeId ) as MasterNode;
						if ( oldMasterNode != null )
							oldMasterNode.IsMainMasterNode = false;
					}
					m_masterNodeId = newMasterNode.UniqueId;
					newMasterNode.IsMainMasterNode = true;
					newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
					newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
				}
			}

			IsDirty = true;
		}

		public void AssignMasterNode( MasterNode node, bool onlyUpdateGraphId )
		{
			AssignMasterNode( node.UniqueId, onlyUpdateGraphId );
			node.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
			node.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
		}

		public void AssignMasterNode( int nodeId, bool onlyUpdateGraphId )
		{
			if ( nodeId < 0 || m_masterNodeId == nodeId )
				return;

			if ( m_masterNodeId > Constants.INVALID_NODE_ID )
			{
				MasterNode oldMasterNode = ( GetNode( nodeId ) as MasterNode );
				if ( oldMasterNode != null )
					oldMasterNode.IsMainMasterNode = false;
			}

			if ( onlyUpdateGraphId )
			{
				m_masterNodeId = nodeId;
			}
			else
			{
				MasterNode masterNode = ( GetNode( nodeId ) as MasterNode );
				if ( masterNode != null )
				{
					masterNode.IsMainMasterNode = true;
					m_masterNodeId = nodeId;
				}
			}

			IsDirty = true;
		}

		public void DrawGrid( DrawInfo drawInfo )
		{
			m_nodeGrid.DrawGrid( drawInfo );
		}

		public float MaxNodeDist
		{
			get { return m_nodeGrid.MaxNodeDist; }
		}

		public List<ParentNode> GetNodesInGrid( Vector2 transformedMousePos )
		{
			return m_nodeGrid.GetNodesOn( transformedMousePos );
		}

		public void FireMasterNode( Shader selectedShader )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).Execute( selectedShader );
		}

		public Shader FireMasterNode( string pathname, bool isFullPath )
		{
			return ( GetNode( m_masterNodeId ) as MasterNode ).Execute( pathname, isFullPath );
		}

		public void ForceSignalPropagationOnMasterNode()
		{
			( GetNode( m_masterNodeId ) as MasterNode ).GenerateSignalPropagation();
			List<ParentNode> localVarNodes = m_localVarNodes.NodesList;
			int count = localVarNodes.Count;
			for ( int i = 0; i < count; i++ )
			{
				SignalGeneratorNode node = localVarNodes[ i ] as SignalGeneratorNode;
				if ( node != null )
				{
					node.GenerateSignalPropagation();
				}
			}
		}

		public void UpdateShaderOnMasterNode( Shader newShader )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).UpdateFromShader( newShader );
		}

		public void CopyValuesFromMaterial( Material material )
		{
			Material currMaterial = CurrentMaterial;
			if ( currMaterial == material )
			{
				for ( int i = 0; i < m_nodes.Count; i++ )
				{
					m_nodes[ i ].ForceUpdateFromMaterial( material );
				}
			}
		}

		public void UpdateMaterialOnMasterNode( Material material )
		{
			( GetNode( m_masterNodeId ) as MasterNode ).UpdateMasterNodeMaterial( material );
		}

		public void SetMaterialModeOnGraph( Material mat )
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].SetMaterialMode( mat );
			}
		}

		public ParentNode CheckNodeAt( Vector3 pos, bool checkForRMBIgnore = false )
		{
			ParentNode selectedNode = null;

			// this is checked on the inverse order to give priority to nodes that are drawn on top  ( last on the list )
			for ( int i = m_nodes.Count - 1; i > -1; i-- )
			{
				if ( m_nodes[ i ].GlobalPosition.Contains( pos ) )
				{
					if ( checkForRMBIgnore )
					{
						if ( !m_nodes[ i ].RMBIgnore )
						{
							selectedNode = m_nodes[ i ];
							break;
						}
					}
					else
					{
						selectedNode = m_nodes[ i ];
						break;
					}
				}
			}
			return selectedNode;
		}

		public void ResetNodesLocalVariables()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].Reset();
				m_nodes[ i ].ResetOutputLocals();
			}
		}

		public override string ToString()
		{
			string dump = ( "Parent Graph \n" );
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				dump += ( m_nodes[ i ] + "\n" );
			}
			return dump;
		}

		public void WriteToString( ref string nodesInfo, ref string connectionsInfo )
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				m_nodes[ i ].WriteToString( ref nodesInfo, ref connectionsInfo );
				m_nodes[ i ].WriteInputDataToString( ref nodesInfo );
				IOUtils.AddLineTerminator( ref nodesInfo );
			}
		}

		public void Reset()
		{
			SaveIsDirty = false;
			IsDirty = false;
		}

		public void OnBeforeSerialize()
		{
			DeSelectAll();
		}

		public void OnAfterDeserialize()
		{
			m_afterDeserializeFlag = true;
		}

		public void CleanCorruptedNodes()
		{
			for ( int i = 0; i < m_nodes.Count; i++ )
			{
				if ( m_nodes[ i ] == null )
				{
					m_nodes.RemoveAt( i );
					CleanCorruptedNodes();
				}
			}
		}

		public ParentNode CreateNode( Type type, bool registerUndo, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = ScriptableObject.CreateInstance( type ) as ParentNode;
			if ( newNode )
			{
				if ( registerUndo )
					UIUtils.RegisterCreatedObjectUndo( newNode );
				newNode.UniqueId = nodeId;
				AddNode( newNode, nodeId < 0, addLast );
			}
			return newNode;
		}

		public ParentNode CreateNode( Type type, bool registerUndo, Vector2 pos, int nodeId = -1, bool addLast = true )
		{
			ParentNode newNode = CreateNode( type, registerUndo, nodeId, addLast );
			if ( newNode )
			{
				newNode.Vec2Position = pos;
			}
			return newNode;
		}

		public void CreateNewEmpty( string name )
		{
			CleanNodes();
			MasterNode newMasterNode = CreateNode( m_masterNodeDefaultType, false ) as MasterNode;
			newMasterNode.SetName( name );
			m_masterNodeId = newMasterNode.UniqueId;
			newMasterNode.OnMaterialUpdatedEvent += OnMaterialUpdatedEvent;
			newMasterNode.OnShaderUpdatedEvent += OnShaderUpdatedEvent;
			newMasterNode.IsMainMasterNode = true;
		}

		public Vector2 SelectedNodesCentroid
		{
			get
			{
				if ( m_selectedNodes.Count == 0 )
					return Vector2.zero;
				Vector2 pos = new Vector2( 0, 0 );
				for ( int i = 0; i < m_selectedNodes.Count; i++ )
				{
					pos += m_selectedNodes[ i ].Vec2Position;
				}

				pos /= m_selectedNodes.Count;
				return pos;
			}
		}

		public void AddInstancePropertyCount()
		{
			m_instancePropertyCount += 1;
		}

		public void RemoveInstancePropertyCount()
		{
			m_instancePropertyCount -= 1;
			if ( m_instancePropertyCount < 0 )
			{
				Debug.LogWarning( "Invalid property instance count" );
			}
		}

		public bool IsInstancedShader { get { return m_instancePropertyCount > 0; } }


		public void AddNormalDependentCount()
		{
			m_normalDependentCount += 1;
		}

		public void RemoveNormalDependentCount()
		{
			m_normalDependentCount -= 1;
			if ( m_normalDependentCount < 0 )
			{
				Debug.LogWarning( "Invalid normal dependentCount count" );
			}
		}

		public bool IsNormalDependent { get { return m_normalDependentCount > 0; } }

		public void MarkToDeselect()
		{
			m_markedToDeSelect = true;
		}

		public void MarkToSelect( int nodeId )
		{
			m_markToSelect = nodeId;
		}

		public void MarkWireHighlights()
		{
			m_checkSelectedWireHighlights = true;
		}


		// Sampler Nodes
		public void AddSamplerNode( SamplerNode node )
		{
			if ( !m_samplerNodes.Contains( node ) )
			{
				m_samplerNodes.Add( node );
				UpdateSamplerNodeArr();
			}
		}

		public void RemoveSamplerNode( SamplerNode node )
		{
			if ( m_samplerNodes.Contains( node ) )
			{
				m_samplerNodes.Remove( node );
				UpdateSamplerNodeArr();
			}
		}

		void UpdateSamplerNodeArr()
		{
			m_samplerNodesArr = new string[ m_samplerNodes.Count ];
			int count = m_samplerNodesArr.Length;
			for ( int i = 0; i < count; i++ )
			{
				m_samplerNodesArr[ i ] = m_samplerNodes[ i ].PropertyName;
			}
		}

		public SamplerNode GetSamplerNode(int idx)
		{
			if ( idx > -1 && idx < m_samplerNodes.Count )
			{
				return m_samplerNodes[ idx ];
			}
			return null;
		}

		// Screen Color Nodes
		public void AddScreenColorNode( ScreenColorNode node )
		{
			if ( !m_screenColorNodes.Contains( node ) )
			{
				m_screenColorNodes.Add( node );
				UpdateScreenColorNodeArr();
			}
		}

		public void RemoveScreenColorNode( ScreenColorNode node )
		{
			if ( m_screenColorNodes.Contains( node ) )
			{
				m_screenColorNodes.Remove( node );
				UpdateScreenColorNodeArr();
			}
		}

		void UpdateScreenColorNodeArr()
		{
			m_screenColorNodesArr = new string[ m_screenColorNodes.Count ];
			int count = m_screenColorNodesArr.Length;
			for ( int i = 0; i < count; i++ )
			{
				m_screenColorNodesArr[ i ] = m_screenColorNodes[ i ].PropertyName;
			}
		}

		public ScreenColorNode GetScreenColorNode( int idx )
		{
			if ( idx > -1 && idx < m_screenColorNodes.Count )
			{
				return m_screenColorNodes[ idx ];
			}
			return null;
		}
		
		//

		public List<ParentNode> SelectedNodes
		{
			get { return m_selectedNodes; }
		}

		public int CurrentMasterNodeId
		{
			get { return m_masterNodeId; }
		}

		public Shader CurrentShader
		{
			get
			{
				MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
				if ( masterNode != null )
					return masterNode.CurrentShader;
				return null;
			}
		}

		public Material CurrentMaterial
		{
			get
			{
				MasterNode masterNode = GetNode( m_masterNodeId ) as MasterNode;
				if ( masterNode != null )
					return masterNode.CurrentMaterial;
				return null;
			}
		}

		public MasterNode CurrentMasterNode
		{
			get { return GetNode( m_masterNodeId ) as MasterNode; }
		}

		public List<ParentNode> AllNodes
		{
			get { return m_nodes; }
		}

		public int NodeCount { get { return m_nodes.Count; } }

		public List<ParentNode> VisibleNodes
		{
			get { return m_visibleNodes; }
		}

		public int NodeClicked
		{
			set { m_nodeClicked = value; }
			get { return m_nodeClicked; }
		}

		public bool IsDirty
		{
			set { m_isDirty = value && UIUtils.DirtyMask; }
			get
			{
				bool value = m_isDirty;
				m_isDirty = false;
				return value;
			}
		}

		public bool SaveIsDirty
		{
			set { m_saveIsDirty = value && UIUtils.DirtyMask; }
			get
			{
				bool value = m_saveIsDirty;
				m_saveIsDirty = false;
				return value;
			}
		}
		public int LoadedShaderVersion
		{
			get { return m_loadedShaderVersion; }
			set { m_loadedShaderVersion = value; }
		}

		public bool HasUnConnectedNodes { get { return m_hasUnConnectedNodes; } }
		public string[] SamplerNodesArr { get { return m_samplerNodesArr; } }
		public string[] ScreenColorNodesArr { get { return m_screenColorNodesArr; } }
		public NodeUsageRegister LocalVarNodes { get { return m_localVarNodes; } }
	}
}
