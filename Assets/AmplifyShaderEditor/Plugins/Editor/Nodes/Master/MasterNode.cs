// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class MasterNode : SignalGeneratorNode
	{
		public delegate void OnMaterialUpdated( MasterNode masterNode );
		public event OnMaterialUpdated OnMaterialUpdatedEvent;
		public event OnMaterialUpdated OnShaderUpdatedEvent;

		
		[SerializeField]
		protected Shader m_currentShader;

		[SerializeField]
		protected Material m_currentMaterial;

		[SerializeField]
		private bool m_isMainMasterNode = false;

		[SerializeField]
		private Rect m_masterNodeIconCoords;

		[SerializeField]
		protected string m_shaderName = "MyNewShader";

		private Texture2D m_masterNodeOnTex;
		private Texture2D m_masterNodeOffTex;

		private Texture2D m_gpuInstanceOnTex;
		private Texture2D m_gpuInstanceOffTex;

		public MasterNode() : base() { CommonInit(); }
		public MasterNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { CommonInit(); }

		protected string _shaderTypeLabel = "Shader Type: ";
		void CommonInit()
		{
			m_currentMaterial = null;
			m_masterNodeIconCoords = new Rect( 0, 0, 64, 64 );
			m_isMainMasterNode = false;
			m_connStatus = NodeConnectionStatus.Connected;
			m_activeType = GetType();
			AddMasterPorts();
		}

		public virtual void AddMasterPorts() { }

		public virtual void UpdateMasterNodeMaterial( Material material ) { }

		public virtual void SetName( string name ) { }

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( m_isMainMasterNode )
			{
				if ( m_masterNodeOnTex == null )
				{
					m_masterNodeOnTex = UIUtils.CurrentWindow.MasterNodeOnTexture;
				}

				if ( m_masterNodeOffTex == null )
				{
					m_masterNodeOffTex = UIUtils.CurrentWindow.MasterNodeOffTexture;
				}

				if ( m_gpuInstanceOnTex == null )
				{
					m_gpuInstanceOnTex = UIUtils.CurrentWindow.GPUInstancedOnTexture;
				}

				if ( m_gpuInstanceOffTex == null )
				{
					m_gpuInstanceOffTex = UIUtils.CurrentWindow.GPUInstancedOffTexture;
				}

				m_masterNodeIconCoords = m_globalPosition;
				m_masterNodeIconCoords.x += m_globalPosition.width - m_masterNodeOffTex.width * drawInfo.InvertedZoom;
				m_masterNodeIconCoords.y += m_globalPosition.height - m_masterNodeOffTex.height * drawInfo.InvertedZoom;
				m_masterNodeIconCoords.width = m_masterNodeOffTex.width * drawInfo.InvertedZoom;
				m_masterNodeIconCoords.height = m_masterNodeOffTex.height * drawInfo.InvertedZoom;

				GUI.DrawTexture( m_masterNodeIconCoords, m_masterNodeOffTex );


				if ( m_gpuInstanceOnTex == null )
				{
					m_gpuInstanceOnTex = UIUtils.CurrentWindow.GPUInstancedOnTexture;
				}

				if ( UIUtils.IsInstancedShader() )
				{
					m_masterNodeIconCoords = m_globalPosition;
					m_masterNodeIconCoords.x += m_globalPosition.width - 5 - m_gpuInstanceOffTex.width * drawInfo.InvertedZoom;
					m_masterNodeIconCoords.y += m_headerPosition.height;
					m_masterNodeIconCoords.width = m_gpuInstanceOffTex.width * drawInfo.InvertedZoom;
					m_masterNodeIconCoords.height = m_gpuInstanceOffTex.height * drawInfo.InvertedZoom;
					GUI.DrawTexture( m_masterNodeIconCoords, m_gpuInstanceOffTex );
				}
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.LabelField( _shaderTypeLabel );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_isMainMasterNode );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_isMainMasterNode = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			if ( m_isMainMasterNode )
			{
				UIUtils.CurrentWindow.CurrentGraph.AssignMasterNode( this, true );
			}
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			if ( activateNode )
				m_inputPorts[ portId ].GetOutputNode().ActivateNode( m_uniqueId, portId, m_activeType );
		}

		public void FireMaterialChangedEvt()
		{
			if ( OnMaterialUpdatedEvent != null )
			{
				OnMaterialUpdatedEvent( this );
			}
		}

		public void FireShaderChangedEvt()
		{
			if ( OnShaderUpdatedEvent != null )
				OnShaderUpdatedEvent( this );
		}

		// What operation this node does
		public virtual void Execute( Shader selectedShader )
		{
			Execute( AssetDatabase.GetAssetPath( selectedShader ), false );
		}

		public virtual Shader Execute( string pathname, bool isFullPath )
		{
			UIUtils.CurrentWindow.CurrentGraph.ResetNodesLocalVariables();
			return null;
		}

		public virtual void UpdateFromShader( Shader newShader ) { }

		public bool IsMainMasterNode
		{
			get { return m_isMainMasterNode; }
			set
			{
				if ( value != m_isMainMasterNode )
				{
					m_isMainMasterNode = value;
					if ( m_isMainMasterNode )
					{
						GenerateSignalPropagation();
					}
					else
					{
						GenerateSignalInibitor();
						OnShaderUpdatedEvent = null;
						OnMaterialUpdatedEvent = null;
					}
				}
			}
		}
		
		public Material CurrentMaterial
		{
			get { return m_currentMaterial; }
		}

		public Shader CurrentShader
		{
			set
			{
				if ( value != null )
				{
					SetName( value.name );
				}

				m_currentShader = value;
				FireShaderChangedEvt();
			}
			get { return m_currentShader; }
		}

		public override void Destroy()
		{
			base.Destroy();
			OnMaterialUpdatedEvent = null;
			OnShaderUpdatedEvent = null;
			m_masterNodeOnTex = null;
			m_masterNodeOffTex = null;
			m_gpuInstanceOnTex = null;
			m_gpuInstanceOffTex = null;

	}
		
		public string ShaderName
		{
			//get { return ( ( _isHidden ? "Hidden/" : string.Empty ) + ( String.IsNullOrEmpty( _shaderCategory ) ? "" : ( _shaderCategory + "/" ) ) + _shaderName ); }
			get { return m_shaderName; }
			set
			{
				m_shaderName = value;
				m_content.text = value;
				m_sizeIsDirty = true;
			}
		}
	}
}
