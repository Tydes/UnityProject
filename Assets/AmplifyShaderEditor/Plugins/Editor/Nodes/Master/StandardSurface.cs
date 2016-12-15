// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{

	public enum StandardShaderLightModel
	{
		Standard,
		StandardSpecular,
		Lambert,
		BlinnPhong
	}

	//public enum eReservedPorts
	//{
	//	DiscardOpPort = 0,
	//	VertexDisplacementPort,
	//	CustomPerVertexDataPort,
	//	CustomLightModelPort,
	//	StandardLightModelPortBegin
	//}

	public enum CullMode
	{
		Back,
		Front,
		Off
	}

	public enum ZWriteMode
	{
		On,
		Off
	}

	public enum ZTestMode
	{
		Less,
		Greater,
		LEqual,
		GEqual,
		Equal,
		NotEqual,
		Always
	}

	public enum AlphaMode
	{
		Opaque = 0,
		Masked,
		Fade,
		Transparent
	}

	public enum RenderType
	{
		Opaque,
		Transparent,
		TransparentCutout,
		Background,
		Overlay,
		TreeOpaque,
		TreeTransparentCutout,
		TreeBillboard,
		Grass,
		GrassBillboard
	}

	public enum RenderQueue
	{
		Background,
		Geometry,
		AlphaTest,
		Transparent,
		Overlay
	}

	[Serializable]
	public class NodeCache
	{
		public int TargetNodeId = -1;
		public int TargetPortId = -1;

		public NodeCache( int targetNodeId, int targetPortId )
		{
			SetData( targetNodeId, targetPortId );
		}

		public void SetData( int targetNodeId, int targetPortId )
		{
			TargetNodeId = targetNodeId;
			TargetPortId = targetPortId;
		}

		public void Invalidate()
		{
			TargetNodeId = -1;
			TargetPortId = -1;
		}

		public bool IsValid
		{
			get { return ( TargetNodeId >= 0 ); }
		}

		public override string ToString()
		{
			return "TargetNodeId " + TargetNodeId + " TargetPortId " + TargetPortId;
		}
	}

	[Serializable]
	public class CacheNodeConnections
	{
		public Dictionary<string, NodeCache> NodeCacheArray;

		public CacheNodeConnections()
		{
			NodeCacheArray = new Dictionary<string, NodeCache>();
		}

		public void Add( string key, NodeCache value )
		{
			NodeCacheArray.Add( key, value );
		}

		public NodeCache Get( string key )
		{
			if ( NodeCacheArray.ContainsKey( key ) )
				return NodeCacheArray[ key ];
			return null;
		}

		public void Clear()
		{
			NodeCacheArray.Clear();
		}
	}

	[Serializable]
	[NodeAttributes( "Standard Surface Output", "Master", "Surface shader generator output", null, KeyCode.None, false )]
	public sealed class StandardSurfaceOutputNode : MasterNode, ISerializationCallbackReceiver
	{
		private const string LightModelStr = "Light Model";
		private const string CullModeStr = "Cull Mode";
		private const string ZWriteModeStr = "ZWrite Mode";
		private const string ZTestModeStr = "ZTest Mode";
		private const string ShaderNameStr = "Shader Name";

		private const string DiscardStr = "Opacity Mask";
		private const string VertexDisplacementStr = "Local Vertex Offset";
		private const string PerVertexDataStr = "Per Vertex Data";
		private const string CustomLightModelStr = "C. Light Model";
		private const string AlbedoStr = "Albedo";
		private const string NormalStr = "Normal";
		private const string EmissionStr = "Emission";
		private const string MetallicStr = "Metallic";
		private const string SmoothnessStr = "Smoothness";
		private const string OcclusionStr = "Occlusion";
		private const string AlphaStr = "Opacity";
		private const string AlphaDataStr = "Alpha";
		private const string DebugStr = "Debug";
		private const string SpecularStr = "Specular";
		private const string GlossStr = "Gloss";
		private const string AlphaModeStr = "Blend Mode";
		private const string OpacityMaskClipValueStr = "Mask Clip Value";
		private const string CastShadowsStr = "Cast Shadows";
		private const string KeepAlphaStr = "Keep Alpha";
		private const string QueueIndexStr = "Queue Index";

		private const string RenderTypeStr = "Render Type";
		private const string RenderQueueStr = "Render Queue";

		private GUIContent m_shaderNameContent;
		//private const string _currentShaderStr = "Current Shader:";

		//private const string _shaderCategoryStr = "Shader Category";
		//private const string _isHiddenStr = "Is Hidden";
		//private const string _pickMaterialStr = "Material from Selected";
		//private const string _materialStr = "Current material";
		//private const string _shaderNameTextfieldControlName = "ShaderName";
		private const string DefaultShaderName = "MyNewShader";
		//private const string _codeGenerationTitle = "Code Generation Options";
		//private const string _materialProperties = "Material Properties";

		// label sizes
		//private float _lightModelLen = -1;
		//private float _cullModeLen = -1;
		//private float _zWriteModeLen = -1;
		//private float _zTestModeLen = -1;
		//private float _shaderNameLen = -1;
		//private float _materialLen = -1;
		//private float _currentShaderLen = -1;

		//private readonly string[] ZTestModeStr = { "Less", "Greater", "Less or Equal", "Greater or Equal", "Equal", "Not Equal", "Always" };

		//[SerializeField]
		//private string _shaderCategory = String.Empty;

		//[SerializeField]
		//private bool _isHidden = false;

		//[SerializeField]
		//private string _materialName = Constants.NoMaterialStr;

		[SerializeField]
		private StandardShaderLightModel m_currentLightModel;

		[SerializeField]
		private StandardShaderLightModel m_lastLightModel;

		[SerializeField]
		private CullMode m_cullMode = CullMode.Back;

		[SerializeField]
		private ZWriteMode m_zWriteMode = ZWriteMode.On;

		[SerializeField]
		private ZTestMode m_zTestMode = ZTestMode.LEqual;

		[SerializeField]
		private AlphaMode m_alphaMode = AlphaMode.Opaque;

		[SerializeField]
		private RenderType m_renderType = RenderType.Opaque;

		[SerializeField]
		private RenderQueue m_renderQueue = RenderQueue.Geometry;

		[SerializeField]
		private bool m_customBlendMode = false;

		[SerializeField]
		private float m_opacityMaskClipValue = 0.5f;

		[SerializeField]
		private int m_discardPortId = -1;

		[SerializeField]
		private bool m_keepAlpha = true;

		[SerializeField]
		private bool m_castShadows = true;

		[SerializeField]
		private int m_queueOrder = 0;

		[SerializeField]
		private List<CodeGenerationData> m_codeGenerationDataList;

		[SerializeField]
		private CacheNodeConnections m_cacheNodeConnections = new CacheNodeConnections();

		//private CustomMaterialEditor _materialEditor;

		//private GUIStyle _materialLabelStyle;

		protected override void CommonInit( int uniqueId )
		{
			_shaderTypeLabel += "Surface Shader";

			m_currentLightModel = m_lastLightModel = StandardShaderLightModel.Standard;
			//_materialEditor = null;
			m_codeGenerationDataList = new List<CodeGenerationData>();
			m_codeGenerationDataList.Add( new CodeGenerationData( "Exclude Deferred", "exclude_path:deferred" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Exclude Forward", "exclude_path:forward" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Exclude Legacy Deferred", "exclude_path:prepass" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable shadows", "noshadow" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable Ambient Light", "noambient" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable Per Vertex Light", "novertexlights " ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable all lightmaps", "nolightmap " ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable dynamic global GI", "nodynlightmap" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable directional lightmaps", "nodirlightmap " ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable built-in fog", "nofog" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Don't generate meta", "nometa" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( "Disable Add Pass", "noforwardadd " ) );
			m_textLabelWidth = 97;

			base.CommonInit( uniqueId );

			m_shaderNameContent = new GUIContent( ShaderNameStr, string.Empty );
		}

		public override void AddMasterPorts()
		{
			base.AddMasterPorts();



			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr );
					AddInputPort( WirePortDataType.FLOAT, false, MetallicStr );
					AddInputPort( WirePortDataType.FLOAT, false, SmoothnessStr );
					AddInputPort( WirePortDataType.FLOAT, false, OcclusionStr );
				}
				break;
				case StandardShaderLightModel.StandardSpecular:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr );
					AddInputPort( WirePortDataType.FLOAT3, false, SpecularStr );
					AddInputPort( WirePortDataType.FLOAT, false, SmoothnessStr );
					AddInputPort( WirePortDataType.FLOAT, false, OcclusionStr );
				}
				break;
				case StandardShaderLightModel.Lambert:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr );
					AddInputPort( WirePortDataType.FLOAT, false, SpecularStr );
					AddInputPort( WirePortDataType.FLOAT, false, GlossStr );
				}
				break;
				case StandardShaderLightModel.BlinnPhong:
				{
					AddInputPort( WirePortDataType.FLOAT3, false, AlbedoStr, 1 );
					AddInputPort( WirePortDataType.FLOAT3, false, NormalStr, 0 );
					AddInputPort( WirePortDataType.FLOAT3, false, EmissionStr );
					AddInputPort( WirePortDataType.FLOAT, false, SpecularStr );
					AddInputPort( WirePortDataType.FLOAT, false, GlossStr );
				}
				break;
			}

			AddInputPort( WirePortDataType.FLOAT, false, AlphaStr );
			m_inputPorts[ m_inputPorts.Count - 1 ].DataName = AlphaDataStr;

			AddInputPort( WirePortDataType.OBJECT, false, DiscardStr );
			m_discardPortId = m_inputPorts.Count - 1;

			AddInputPort( WirePortDataType.OBJECT, false, VertexDisplacementStr );

			AddInputPort( WirePortDataType.OBJECT, false, PerVertexDataStr );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;

			AddInputPort( WirePortDataType.OBJECT, false, CustomLightModelStr );
			m_inputPorts[ m_inputPorts.Count - 1 ].Locked = true;

			AddInputPort( WirePortDataType.FLOAT3, false, DebugStr );


			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				m_inputPorts[ i ].CustomColor = Color.white;
			}
			m_sizeIsDirty = true;
		}

		public override void SetName( string name )
		{
			ShaderName = name;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_buttonStyle.fixedWidth = 200;
			m_buttonStyle.fixedHeight = 50;
			//if ( _lightModelLen < 0 )
			//{
			//	_lightModelLen = GUI.skin.label.CalcSize( new GUIContent( _lightModelStr ) ).x;
			//	_cullModeLen = GUI.skin.label.CalcSize( new GUIContent( _cullModeStr ) ).x;
			//	_zWriteModeLen = GUI.skin.label.CalcSize( new GUIContent( _zWriteModeStr ) ).x;
			//	_zTestModeLen = GUI.skin.label.CalcSize( new GUIContent( _zTestModeStr ) ).x;
			//	_shaderNameLen = GUI.skin.label.CalcSize( new GUIContent( _shaderNameStr ) ).x;
			//	//_materialLen = GUI.skin.label.CalcSize( new GUIContent( _materialStr ) ).x;
			//	//_currentShaderLen = GUI.skin.label.CalcSize( new GUIContent( _currentShaderStr ) ).x;
			//}

			//float labelWidth = EditorGUIUtility.labelWidth;
			//EditorGUIUtility.labelWidth = _shaderNameLen;
			EditorGUILayout.BeginVertical();
			{

				EditorGUILayout.Separator();

				//GUI.SetNextControlName( _shaderNameTextfieldControlName ); // Give a name to the textfield control so we can know when the player is typing on it
				EditorGUI.BeginChangeCheck();

				string newShaderName = EditorGUILayout.TextField( m_shaderNameContent, m_shaderName, m_textfieldStyle );
				//if ( Event.current.isKey && GUI.GetNameOfFocusedControl().Equals( _shaderNameTextfieldControlName ) ) //if player is typing on this specific textfield
				if ( EditorGUI.EndChangeCheck() )
				{
					if ( newShaderName.Length > 0 )
					{
						newShaderName = UIUtils.RemoveShaderInvalidCharacters( newShaderName );
					}
					else
					{
						newShaderName = DefaultShaderName;
					}
					ShaderName = newShaderName;
				}
				m_shaderNameContent.tooltip = m_shaderName;

				//EditorGUILayout.Separator();

				//EditorGUIUtility.labelWidth = _currentShaderLen;
				//EditorGUI.BeginChangeCheck();
				//_currentShader = EditorGUILayout.ObjectField( _currentShaderStr, _currentShader, typeof( Shader ), false ) as Shader;
				//if ( EditorGUI.EndChangeCheck() )
				//{
				//	if ( _currentShader != null )
				//	{
				//		UIUtils.CurrentWindow.OnValidShaderFound( _currentShader, null );
				//	}
				//}

				EditorGUILayout.Separator();
				//EditorGUIUtility.labelWidth = _lightModelLen;
				m_currentLightModel = ( StandardShaderLightModel ) EditorGUILayout.EnumPopup( LightModelStr, m_currentLightModel );


				EditorGUILayout.Separator();
				//EditorGUIUtility.labelWidth = _cullModeLen;
				m_cullMode = ( CullMode ) EditorGUILayout.EnumPopup( CullModeStr, m_cullMode );
				//EditorGUILayout.Separator();
				//_zWriteMode = ( eZWriteMode ) EditorGUILayout.EnumPopup( _zWriteModeStr, _zWriteMode );
				//EditorGUILayout.Separator();
				//_zTestMode = ( eZTestMode ) EditorGUILayout.Popup( _zTestModeStr, ( int ) _zTestMode, ZTestModeStr );

				EditorGUILayout.Separator();
				EditorGUI.BeginChangeCheck();
				m_alphaMode = ( AlphaMode ) EditorGUILayout.EnumPopup( AlphaModeStr, m_alphaMode );
				if ( EditorGUI.EndChangeCheck() )
				{
					m_customBlendMode = false;
					UpdateFromBlendMode();
				}

				EditorGUILayout.Separator();

				EditorGUI.BeginChangeCheck();

				m_renderType = ( RenderType ) EditorGUILayout.EnumPopup( RenderTypeStr, m_renderType );

				EditorGUILayout.Separator();

				m_renderQueue = ( RenderQueue ) EditorGUILayout.EnumPopup( RenderQueueStr, m_renderQueue );

				if ( EditorGUI.EndChangeCheck() )
				{
					m_customBlendMode = true;
				}

				EditorGUILayout.Separator();
				int queueOrder = EditorGUILayout.IntField( QueueIndexStr, m_queueOrder );
				m_queueOrder = ( queueOrder < 0 ) ? 0 : queueOrder;

				bool bufferedEnabled = GUI.enabled;

				GUI.enabled = ( m_alphaMode == AlphaMode.Masked && !m_customBlendMode );
				m_inputPorts[ m_discardPortId ].Locked = !GUI.enabled;
				EditorGUILayout.Separator();
				m_opacityMaskClipValue = EditorGUILayout.FloatField( OpacityMaskClipValueStr, m_opacityMaskClipValue );

				GUI.enabled = bufferedEnabled;

				EditorGUILayout.Separator();
				m_keepAlpha = EditorGUILayout.Toggle( KeepAlphaStr, m_keepAlpha );

				EditorGUILayout.Separator();
				m_castShadows = EditorGUILayout.Toggle( CastShadowsStr, m_castShadows );

				//EditorGUILayout.Separator();
				//EditorGUILayout.LabelField( _shaderCategoryStr );

				//EditorGUI.BeginChangeCheck();
				//_shaderCategory = EditorGUILayout.TextField( _shaderCategory, _textfieldStyle );
				//if ( EditorGUI.EndChangeCheck() )
				//{
				//	if ( _shaderCategory.Length > 0 )
				//		_shaderCategory = UIUtils.RemoveInvalidCharacters( _shaderCategory );
				//}

				//EditorGUILayout.Separator();
				//_isHidden = EditorGUILayout.ToggleLeft( _isHiddenStr, _isHidden );


				//EditorGUILayout.Separator();
				//EditorGUIUtility.labelWidth = _materialLen;
				//bool updateMaterial = false;
				//EditorGUI.BeginChangeCheck();
				//_currentMaterial = ( Material ) EditorGUILayout.ObjectField( _materialStr, _currentMaterial, typeof( Material ), false );
				//if ( EditorGUI.EndChangeCheck() )
				//{
				//	updateMaterial = true;
				//}

				//EditorGUILayout.Separator();
				//EditorGUIUtility.labelWidth = labelWidth;
				//if ( GUILayout.Button( _pickMaterialStr, _buttonStyle ) )
				//{
				//	if ( Selection.activeGameObject != null )
				//	{
				//		Renderer renderer = Selection.activeGameObject.GetComponent<Renderer>();

				//		if ( renderer )
				//		{
				//			_currentMaterial = renderer.sharedMaterial;
				//			UpdateMaterialEditor();
				//		}
				//	}
				//}

				//if ( _currentMaterial != null )
				//{
				//	if ( /*_materialEditor == null ||*/ updateMaterial )
				//	{
				//		UpdateMaterialEditor();
				//	}
				//}

				//EditorGUILayout.Separator();

				//for ( int i = 0; i < _codeGenerationDataList.Count; i++ )
				//{
				//	_codeGenerationDataList[ i ].IsActive = EditorGUILayout.ToggleLeft( _codeGenerationDataList[ i ].Name, _codeGenerationDataList[ i ].IsActive );
				//}

				//if ( _materialEditor != null )
				//{
				//	if ( _materialLabelStyle == null )
				//	{
				//		_materialLabelStyle = new GUIStyle( UIUtils.CurrentWindow.CustomStylesInstance.Label );
				//		_materialLabelStyle.fontStyle = FontStyle.Bold;
				//		_materialLabelStyle.fontSize = 15;
				//	}
				//	EditorGUILayout.LabelField( Constants.LineSeparator );
				//	EditorGUILayout.Separator();
				//	EditorGUILayout.LabelField( _materialProperties, _materialLabelStyle, GUILayout.Height( 20 ) );
				//	EditorGUILayout.Separator();
				//	_materialEditor.OnInspectorGUI();
				//}
			}
			EditorGUILayout.EndVertical();

			if ( m_currentLightModel != m_lastLightModel )
			{
				CacheCurrentSettings();
				m_lastLightModel = m_currentLightModel;
				DeleteAllInputConnections( true );
				AddMasterPorts();
				ConnectFromCache();
			}
		}

		private void CacheCurrentSettings()
		{
			m_cacheNodeConnections.Clear();
			for ( int portId = 0; portId < m_inputPorts.Count; portId++ )
			{
				if ( m_inputPorts[ portId ].IsConnected )
				{
					WireReference connection = m_inputPorts[ portId ].GetConnection();
					m_cacheNodeConnections.Add( m_inputPorts[ portId ].Name, new NodeCache( connection.NodeId, connection.PortId ) );
				}
			}
		}

		private void ConnectFromCache()
		{
			for ( int i = 0; i < m_inputPorts.Count; i++ )
			{
				NodeCache cache = m_cacheNodeConnections.Get( m_inputPorts[ i ].Name );
				if ( cache != null )
				{
					UIUtils.SetConnection( m_uniqueId, i, cache.TargetNodeId, cache.TargetPortId );
				}
			}
		}

		public override void UpdateMasterNodeMaterial( Material material )
		{
			m_currentMaterial = material;
			UpdateMaterialEditor();
		}

		void UpdateMaterialEditor()
		{
			//_materialEditor = ( CustomMaterialEditor ) Editor.CreateEditor( _currentMaterial, typeof( CustomMaterialEditor ) );
			FireMaterialChangedEvt();
		}

		//public void AnaliseNode( ParentNode node, int outputTargetId, eWirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		//{
		//	dataCollector.AddInstructions( node.GetValueFromOutputStr( outputTargetId, inputPortType, ref dataCollector, ignoreLocalVar ) );
		//}

		public void CreateInstructionsForPort( InputPort port, string portName, bool addCustomDelimiters = false, string customDelimiterIn = null, string customDelimiterOut = null, bool ignoreLocalVar = false )
		{
			WireReference connection = port.GetConnection();
			ParentNode node = UIUtils.GetNode( connection.NodeId );

			string newInstruction = node.GetValueFromOutputStr( connection.PortId, port.DataType, ref UIUtils.CurrentDataCollector, ignoreLocalVar );
			if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
			{
				UIUtils.CurrentDataCollector.AddInstructions( UIUtils.CurrentDataCollector.SpecialLocalVariables );
				UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
			}
			if ( UIUtils.CurrentDataCollector.ForceNormal )
			{
				UIUtils.CurrentDataCollector.AddToStartInstructions( "\t\t\t" + Constants.OutputVarStr + ".Normal = float3(0,0,1);\n" );
				UIUtils.CurrentDataCollector.ForceNormal = false;
			}

			UIUtils.CurrentDataCollector.AddInstructions( addCustomDelimiters ? customDelimiterIn : ( "\t\t\t" + portName + " = " ) );
			UIUtils.CurrentDataCollector.AddInstructions( newInstruction );
			UIUtils.CurrentDataCollector.AddInstructions( addCustomDelimiters ? customDelimiterOut : ";\n" );
		}

		public void AddLocalVarInstructions()
		{
			List<ParentNode> localVarNodes = UIUtils.CurrentWindow.CurrentGraph.LocalVarNodes.NodesList;
			int count = localVarNodes.Count;

			List<RegisterLocalVarNode> sortedList = new List<RegisterLocalVarNode>( count );
			for ( int i = 0; i < count; i++ )
			{
				RegisterLocalVarNode node = localVarNodes[ i ] as RegisterLocalVarNode;
				sortedList.Add( node );
			}
			sortedList.Sort( ( x, y ) => { return x.OrderIndex.CompareTo( y.OrderIndex ); } );

			for ( int i = 0; i < count; i++ )
			{
				string newInstruction = sortedList[ i ].CreateLocalVariable( 0, WirePortDataType.FLOAT, ref UIUtils.CurrentDataCollector, false );
				if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
				{
					UIUtils.CurrentDataCollector.AddInstructions( UIUtils.CurrentDataCollector.SpecialLocalVariables );
					UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
				}
				UIUtils.CurrentDataCollector.AddInstructions( newInstruction );
			}
		}

		public override Shader Execute( string pathname, bool isFullPath )
		{
			base.Execute( pathname, isFullPath );

			bool isInstancedShader = UIUtils.IsInstancedShader();

			UIUtils.CurrentDataCollector = new MasterNodeDataCollector();

			string tags = "\"RenderType\" = \"{0}\"  \"Queue\" = \"{1}\"";
			tags = string.Format( tags, m_renderType, ( m_renderQueue + "+" + m_queueOrder ) );
			//if ( !m_customBlendMode )
			{
				if ( m_alphaMode == AlphaMode.Fade || m_alphaMode == AlphaMode.Transparent )
				{
					tags += " \"IgnoreProjector\" = \"True\"";
				}
			}

			tags = "Tags{ " + tags + " }";

			string outputStruct = "";
			switch ( m_currentLightModel )
			{
				case StandardShaderLightModel.Standard: outputStruct = "SurfaceOutputStandard"; break;
				case StandardShaderLightModel.StandardSpecular: outputStruct = "SurfaceOutputStandardSpecular"; break;
				case StandardShaderLightModel.Lambert:
				case StandardShaderLightModel.BlinnPhong: outputStruct = "SurfaceOutput"; break;
			}

			outputStruct += " " + Constants.OutputVarStr;
			// Register Local variables
			AddLocalVarInstructions();


			if ( m_inputPorts[ m_inputPorts.Count - 1 ].IsConnected )
			{
				//Debug Port active
				InputPort debugPort = m_inputPorts[ m_inputPorts.Count - 1 ];
				CreateInstructionsForPort( debugPort, Constants.OutputVarStr + ".Emission", false, null, null, UIUtils.IsNormalDependent() );
			}
			else
			{
				// Custom Light Model
				//TODO: Create Custom Light behaviour
				SortedList<int, InputPort> sortedPorts = new SortedList<int, InputPort>();
				for ( int i = 0; i < m_inputPorts.Count; i++ )
				{
					sortedPorts.Add( m_inputPorts[ i ].OrderId, m_inputPorts[ i ] );
				}
				//Collect data from standard nodes
				for ( int i = 0; i < sortedPorts.Count; i++ )
				{
					if ( sortedPorts[ i ].IsConnected )
					{
						if ( i == 0 )// Normal Map is Connected
						{
							UIUtils.CurrentDataCollector.DirtyNormal = true;
						}

						if ( m_inputPorts[ i ].Name.Equals( DiscardStr ) )
						{
							//Discard Op Node
							string opacityValue = "0.0";
							switch ( m_inputPorts[ i ].ConnectionType() )
							{
								case WirePortDataType.INT:
								case WirePortDataType.FLOAT:
								{
									opacityValue = IOUtils.MaskClipValueName;//UIUtils.FloatToString( m_opacityMaskClipValue );
								}
								break;

								case WirePortDataType.FLOAT2:
								{
									opacityValue = string.Format( "( {0} ).xx", IOUtils.MaskClipValueName );
								}
								break;

								case WirePortDataType.FLOAT3:
								{
									opacityValue = string.Format( "( {0} ).xxx", IOUtils.MaskClipValueName );
								}
								break;

								case WirePortDataType.FLOAT4:
								{
									opacityValue = string.Format( "( {0} ).xxxx", IOUtils.MaskClipValueName );
								}
								break;
							}
							CreateInstructionsForPort( sortedPorts[ i ], Constants.OutputVarStr + "." + sortedPorts[ i ].DataName, true, "\t\t\tclip( ", " - " + opacityValue + " );\n" );
						}
						else if ( m_inputPorts[ i ].Name.Equals( VertexDisplacementStr ) )
						{
							//Vertex displacement and per vertex custom data
							WireReference connection = m_inputPorts[ i ].GetConnection();
							ParentNode node = UIUtils.GetNode( connection.NodeId );

							string vertexInstructions = node.GetValueFromOutputStr( connection.PortId, m_inputPorts[ i ].DataType, ref UIUtils.CurrentDataCollector, true );

							if ( UIUtils.CurrentDataCollector.DirtySpecialLocalVariables )
							{
								UIUtils.CurrentDataCollector.AddVertexInstruction( UIUtils.CurrentDataCollector.SpecialLocalVariables, m_uniqueId, false );
								UIUtils.CurrentDataCollector.ClearSpecialLocalVariables();
							}

							UIUtils.CurrentDataCollector.AddToVertexDisplacement( vertexInstructions );
						}
						else
						{
							// if working on normals and have normal dependent node then ignore local var generation
							bool ignoreLocalVar = ( i == 0 && UIUtils.IsNormalDependent() );
							CreateInstructionsForPort( sortedPorts[ i ], Constants.OutputVarStr + "." + sortedPorts[ i ].DataName, false, null, null, ignoreLocalVar );
						}
					}
				}
			}

			for ( int i = 0; i < 4; i++ )
			{
				if ( UIUtils.CurrentDataCollector.GetChannelUsage( i ) == TextureChannelUsage.Required )
				{
					string channelName = UIUtils.GetChannelName( i );
					UIUtils.CurrentDataCollector.AddToProperties( -1, UIUtils.GetTex2DProperty( channelName, TexturePropertyValues.white ), -1 );
				}
			}

			UIUtils.CurrentDataCollector.AddToProperties( -1, IOUtils.DefaultASEDirtyCheckProperty, -1 );
			if ( m_alphaMode == AlphaMode.Masked && !m_customBlendMode )
			{
				UIUtils.CurrentDataCollector.AddToProperties( -1, string.Format(IOUtils.MaskClipValueProperty , OpacityMaskClipValueStr,m_opacityMaskClipValue ), -1 );
				UIUtils.CurrentDataCollector.AddToUniforms( -1, string.Format( IOUtils.MaskClipValueUniform, m_opacityMaskClipValue ));
			}

			//UIUtils.CurrentDataCollector.AddToUniforms( -1, IOUtils.DefaultASEDirtyCheckUniform );

			if ( !UIUtils.CurrentDataCollector.DirtyInputs )
				UIUtils.CurrentDataCollector.AddToInput( m_uniqueId, "fixed filler", true );

			UIUtils.CurrentDataCollector.CloseInputs();
			UIUtils.CurrentDataCollector.CloseProperties();
			UIUtils.CurrentDataCollector.ClosePerVertexHeader();

			//build Shader Body
			//string ShaderBody = "Shader \"" + ( _isHidden ? "Hidden/" : String.Empty ) + ( _shaderCategory.Length > 0 ? ( _shaderCategory + "/" ) : String.Empty ) + _shaderName + "\"\n{\n";
			string ShaderBody = "Shader \"" + m_shaderName + "\"\n{\n";
			{
				//set properties
				if ( UIUtils.CurrentDataCollector.DirtyProperties )
				{
					//ShaderBody += UIUtils.CurrentDataCollector.Properties + '\n';
					ShaderBody += UIUtils.CurrentDataCollector.BuildPropertiesString();
				}
				//set subshader
				ShaderBody += "\n\tSubShader\n\t{\n";
				{
					//Add SubShader tags
					ShaderBody += "\t\t" + tags + '\n';
					ShaderBody += "\t\tCull " + m_cullMode + '\n';
					//ShaderBody += "\t\tZWrite " + _zWriteMode + '\n';
					//ShaderBody += "\t\tZTest " + _zTestMode + '\n';


					//Add GrabPass
					if ( UIUtils.CurrentDataCollector.DirtyGrabPass )
					{
						ShaderBody += UIUtils.CurrentDataCollector.GrabPass;
					}

					//add cg program
					ShaderBody += "\t\tCGPROGRAM\n";
					{
						//Add Includes
						if ( UIUtils.CurrentDataCollector.DirtyIncludes )
							ShaderBody += UIUtils.CurrentDataCollector.Includes;

						//define as surface shader and specify lighting model
						ShaderBody += IOUtils.PragmaTargetHeader;
						if ( isInstancedShader )
						{
							ShaderBody += IOUtils.InstancedPropertiesHeader;
						}

						// build optional parameters
						string OptionalParameters = string.Empty;
						//if ( !m_customBlendMode )
						{
							switch ( m_alphaMode )
							{
								case AlphaMode.Opaque:
								case AlphaMode.Masked: break;
								case AlphaMode.Fade:
								{
									OptionalParameters += "alpha:fade" + Constants.OptionalParametersSep;
								}
								break;
								case AlphaMode.Transparent:
								{
									OptionalParameters += "alpha:premul" + Constants.OptionalParametersSep;
								}
								break;
							}
						}

						if ( m_keepAlpha )
						{
							OptionalParameters += "keepalpha" + Constants.OptionalParametersSep;
						}

						OptionalParameters += ( ( m_castShadows ) ? "addshadow" + Constants.OptionalParametersSep + "fullforwardshadows" : "noshadow" ) + Constants.OptionalParametersSep;
						ShaderBody += "\t\t#pragma surface surf " + m_currentLightModel.ToString() + Constants.OptionalParametersSep + OptionalParameters;

						//Add code generation options
						for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
						{
							if ( m_codeGenerationDataList[ i ].IsActive )
							{
								ShaderBody += m_codeGenerationDataList[ i ].Value + Constants.OptionalParametersSep;
							}
						}
						//Check if Custom Vertex is being used and add tag
						ShaderBody += UIUtils.CurrentDataCollector.DirtyPerVertexData ? "vertex:" + Constants.VertexDataFunc + "\n" : "\n";

						// Add Input struct
						if ( UIUtils.CurrentDataCollector.DirtyInputs )
							ShaderBody += UIUtils.CurrentDataCollector.Inputs + "\n\n";

						//Add Uniforms
						if ( UIUtils.CurrentDataCollector.DirtyUniforms )
							ShaderBody += UIUtils.CurrentDataCollector.Uniforms + "\n";


						//Add Instanced Properties
						if ( isInstancedShader && UIUtils.CurrentDataCollector.DirtyInstancedProperties )
						{
							UIUtils.CurrentDataCollector.SetupInstancePropertiesBlock( UIUtils.RemoveInvalidCharacters( ShaderName ) );
							ShaderBody += UIUtils.CurrentDataCollector.InstancedProperties + "\n";
						}

						if ( UIUtils.CurrentDataCollector.DirtyFunctions )
							ShaderBody += UIUtils.CurrentDataCollector.Functions + "\n";

						//Add Custom Vertex Data
						if ( UIUtils.CurrentDataCollector.DirtyPerVertexData )
						{
							ShaderBody += UIUtils.CurrentDataCollector.VertexData;
						}

						//Add Surface Shader body
						ShaderBody += "\t\tvoid surf( Input " + Constants.InputVarStr + " , inout " + outputStruct + " )\n\t\t{\n";
						{
							//add local vars
							if ( UIUtils.CurrentDataCollector.DirtyLocalVariables )
								ShaderBody += UIUtils.CurrentDataCollector.LocalVariables;

							//add nodes ops
							ShaderBody += UIUtils.CurrentDataCollector.Instructions;
						}
						ShaderBody += "\t\t}\n";
					}
					ShaderBody += "\n\t\tENDCG\n";
				}
				ShaderBody += "\t}\n";
				ShaderBody += "\tFallback \"Diffuse\"\n";
			}
			ShaderBody += "}\n";

			// Generate Graph info
			ShaderBody += UIUtils.CurrentWindow.GenerateGraphInfo();

			//TODO: Remove current SaveDebugShader and uncomment SaveToDisk as soon as pathname is editable
			if ( !String.IsNullOrEmpty( pathname ) )
			{
				IOUtils.StartSaveThread( ShaderBody, ( isFullPath ? pathname : ( IOUtils.dataPath + pathname ) ) );
				//IOUtils.SaveTextfileToDisk( ShaderBody, ( isFullPath ? pathname : ( IOUtils.dataPath + pathname ) ) );
			}
			else
			{
				IOUtils.StartSaveThread( ShaderBody, Application.dataPath + "/AmplifyShaderEditor/Samples/Shaders/" + m_shaderName + ".shader" );
				//IOUtils.SaveTextfileToDisk( ShaderBody, Application.dataPath + "/AmplifyShaderEditor/Samples/Shaders/" + m_shaderName + ".shader" );
			}

			// Load new shader into material

			if ( CurrentShader == null )
			{
				AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
				CurrentShader = Shader.Find( ShaderName );
			}
			else
			{
				// need to always get asset datapath because a user can change and asset location from the project window 
				AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentShader ) );
			}

			if ( m_currentShader != null )
			{
				//bool setShaderDefaults = false;
				if ( m_currentMaterial != null )
				{
					m_currentMaterial.shader = m_currentShader;
					UIUtils.CurrentDataCollector.UpdateMaterialOnPropertyNodes( m_currentMaterial );
					UpdateMaterialEditor();
					// need to always get asset datapath because a user can change and asset location from the project window
					AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( m_currentMaterial ) );

					//setShaderDefaults = ( UIUtils.CurrentWindow.CurrentSelection == ASESelectionMode.Shader );
				}
				//else
				//{
				//	setShaderDefaults = true;
				//}

				//if ( setShaderDefaults )
				UIUtils.CurrentDataCollector.UpdateShaderOnPropertyNodes( ref m_currentShader );
			}
			//AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );

			UIUtils.CurrentDataCollector.Destroy();
			UIUtils.CurrentDataCollector = null;

			return m_currentShader;
		}

		public override void UpdateFromShader( Shader newShader )
		{
			if ( m_currentMaterial != null )
			{
				m_currentMaterial.shader = newShader;
			}
			CurrentShader = newShader;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_codeGenerationDataList.Clear();
			m_codeGenerationDataList = null;
		}

		public override void OnBeforeSerialize()
		{
			base.OnBeforeSerialize();
			//if ( _materialEditor != null )
			//{
			//	//Editor.DestroyImmediate( _materialEditor );
			//	_materialEditor = null;
			//}
		}
		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			//_materialEditor = null;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			try
			{
				base.ReadFromString( ref nodeParams );
				m_currentLightModel = ( StandardShaderLightModel ) Enum.Parse( typeof( StandardShaderLightModel ), GetCurrentParam( ref nodeParams ) );
				//_isHidden = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				//_shaderCategory = GetCurrentParam( ref nodeParams );
				//if ( _shaderCategory.Length > 0 )
				//	_shaderCategory = UIUtils.RemoveInvalidCharacters( _shaderCategory );
				ShaderName = GetCurrentParam( ref nodeParams );
				if ( m_shaderName.Length > 0 )
					ShaderName = UIUtils.RemoveShaderInvalidCharacters( ShaderName );

				//_materialName = GetCurrentParam( ref nodeParams );
				//_currentMaterial = AssetDatabase.LoadAssetAtPath<Material>( _materialName );
				for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
				{
					m_codeGenerationDataList[ i ].IsActive = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				}

				m_cullMode = ( CullMode ) Enum.Parse( typeof( CullMode ), GetCurrentParam( ref nodeParams ) );
				m_zWriteMode = ( ZWriteMode ) Enum.Parse( typeof( ZWriteMode ), GetCurrentParam( ref nodeParams ) );
				m_zTestMode = ( ZTestMode ) Enum.Parse( typeof( ZTestMode ), GetCurrentParam( ref nodeParams ) );
				m_alphaMode = ( AlphaMode ) Enum.Parse( typeof( AlphaMode ), GetCurrentParam( ref nodeParams ) );
				m_opacityMaskClipValue = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
				m_keepAlpha = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_castShadows = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_queueOrder = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				if ( UIUtils.CurrentShaderVersion() > 11 )
				{
					m_customBlendMode = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					m_renderType = ( RenderType ) Enum.Parse( typeof( RenderType ), GetCurrentParam( ref nodeParams ) );
					m_renderQueue = ( RenderQueue ) Enum.Parse( typeof( RenderQueue ), GetCurrentParam( ref nodeParams ) );
				}
				m_lastLightModel = m_currentLightModel;
				DeleteAllInputConnections( true );
				AddMasterPorts();
				m_customBlendMode = TestCustomBlendMode();
			}
			catch ( Exception e )
			{
				Debug.Log( e );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentLightModel );
			//IOUtils.AddFieldValueToString( ref nodeInfo, _isHidden );
			//IOUtils.AddFieldValueToString( ref nodeInfo, _shaderCategory );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderName );
			//IOUtils.AddFieldValueToString( ref nodeInfo, ( _currentMaterial != null ) ? AssetDatabase.GetAssetPath( _currentMaterial ) : _materialName );
			for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_codeGenerationDataList[ i ].IsActive );
			}

			IOUtils.AddFieldValueToString( ref nodeInfo, m_cullMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zWriteMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_zTestMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_alphaMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_opacityMaskClipValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_keepAlpha );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_castShadows );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_queueOrder );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customBlendMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_renderQueue );
		}

		private bool TestCustomBlendMode()
		{
			switch ( m_alphaMode )
			{
				case AlphaMode.Opaque:
				{
					if ( m_renderType == RenderType.Opaque && m_renderQueue == RenderQueue.Geometry )
						return false;
				}
				break;
				case AlphaMode.Masked:
				{
					if ( m_renderType == RenderType.TransparentCutout && m_renderQueue == RenderQueue.AlphaTest )
						return false;
				}
				break;
				case AlphaMode.Fade:
				case AlphaMode.Transparent:
				{
					if ( m_renderType == RenderType.Transparent && m_renderQueue == RenderQueue.Transparent )
						return false;
				}
				break;
			}
			return true;
		}
		private void UpdateFromBlendMode()
		{
			switch ( m_alphaMode )
			{
				case AlphaMode.Opaque:
				{
					m_renderType = RenderType.Opaque;
					m_renderQueue = RenderQueue.Geometry;
				}
				break;
				case AlphaMode.Masked:
				{
					m_renderType = RenderType.TransparentCutout;
					m_renderQueue = RenderQueue.AlphaTest;
				}
				break;
				case AlphaMode.Fade:
				case AlphaMode.Transparent:
				{
					m_renderType = RenderType.Transparent;
					m_renderQueue = RenderQueue.Transparent;
				}
				break;
			}
		}
	}
}
