// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public enum TexReferenceType
	{
		Object,
		Instance
	}

	public enum TexturePropertyValues
	{
		white,
		black,
		gray,
		bump
	}

	public enum TextureType
	{
		Texture2D,
		Texture3D,
		Cube
	}

	public enum AutoCastType
	{
		Auto,
		LockedToTexture2D,
		LockedToTexture3D,
		LockedToCube
	}

	[Serializable]
	[NodeAttributes( "Texture Sample", "Textures", "Textures", KeyCode.T, true, typeof( Texture ), typeof( Texture2D ), typeof( Texture3D ), typeof( Cubemap ) )]
	public sealed class SamplerNode : PropertyNode
	{
		private const int OriginalFontSizeUpper = 9;
		private const int OriginalFontSizeLower = 9;

		private const string DefaultTextureStr = "Default value";
		private const string AutoCastModeStr = "Auto-Cast Mode";

		private const string DefaultTextureUseSematicsStr = "Use Semantics";
		private const string DefaultTextureIsNormalMapsStr = "Is Normal Map";

		private const string AutoUnpackNormalsStr = "Normal";
		private const string NormalScaleStr = "Normal Scale";


		private const float PreviewerSizeX = 110;
		private const float PreviewerSizeY = 110;

		private const float PreviewerNormalSizeX = 88;
		private const float PreviewerNormalSizeY = 88;

		private readonly Color ReferenceHeaderColor = new Color( 2.67f, 1.0f, 0.5f, 1.0f );

		[SerializeField]
		private Texture m_defaultValue;

		[SerializeField]
		private Texture m_materialValue;

		[SerializeField]
		private TexturePropertyValues m_defaultTextureValue;

		[SerializeField]
		private int m_textureCoordSet = 0;

		[SerializeField]
		private bool m_useSemantics;

		[SerializeField]
		private bool m_isNormalMap;

		[SerializeField]
		private Type m_textureType;

		[SerializeField]
		private string m_samplerType;

		[SerializeField]
		private bool m_isTextureFetched;

		[SerializeField]
		private string m_textureFetchedValue;

		[SerializeField]
		private string m_normalMapUnpackMode;

		//[SerializeField]
		//private string _metafileId = "2700000";

		[SerializeField]
		private TextureType m_currentType = TextureType.Texture2D;

		[SerializeField]
		private AutoCastType m_autocastMode = AutoCastType.Auto;

		[SerializeField]
		private bool m_autoUnpackNormals = false;

		[SerializeField]
		private TexReferenceType m_referenceType = TexReferenceType.Object;

		[SerializeField]
		private int m_referenceId = -1;

		private float m_deltaPreviewer = 0;
		private SamplerNode m_referenceNode = null;

		[SerializeField]
		private GUIStyle m_referenceStyle = null;

		[SerializeField]
		private GUIStyle m_referenceIconStyle = null;

		[SerializeField]
		private GUIContent m_referenceContent = null;

		[SerializeField]
		private float m_referenceWidth = -1;


		public SamplerNode() : base() { }
		public SamplerNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_defaultTextureValue = TexturePropertyValues.white;
			m_fixedSize.x = 170;
			m_fixedSize.y = 10;
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddInputPort( WirePortDataType.FLOAT, false, "Normal Scale" );
			m_inputPorts[ 1 ].Visible = m_autoUnpackNormals;
			m_inputPorts[ 1 ].FloatInternalData = 1.0f;
			AddOutputColorPorts( WirePortDataType.COLOR, "RGBA" );
			m_currentParameterType = PropertyType.Property;
			m_useCustomPrefix = true;
			m_customPrefix = "Texture Sample ";
			m_referenceContent = new GUIContent( string.Empty );
			m_freeType = false;
			m_useSemantics = true;
			m_textLabelWidth = 100;
			ConfigTextureData( TextureType.Texture2D );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			if ( m_referenceType == TexReferenceType.Object )
				UIUtils.RegisterSamplerNode( this );
		}

		public void ConfigTextureData( TextureType type )
		{
			switch ( m_autocastMode )
			{
				case AutoCastType.Auto:
				{
					m_currentType = type;
				}
				break;
				case AutoCastType.LockedToTexture2D:
				{
					m_currentType = TextureType.Texture2D;
				}
				break;
				case AutoCastType.LockedToTexture3D:
				{
					m_currentType = TextureType.Texture3D;
				}
				break;
				case AutoCastType.LockedToCube:
				{
					m_currentType = TextureType.Cube;
				}
				break;
			}

			switch ( m_currentType )
			{
				case TextureType.Texture2D:
				{
					m_textureType = typeof( Texture2D );
					m_samplerType = "tex2D";
					//	_metafileId = "2800000";
				}
				break;
				case TextureType.Texture3D:
				{
					m_textureType = typeof( Texture3D );
					m_samplerType = "tex3D";
					//_metafileId = "2700000";
				}
				break;
				case TextureType.Cube:
				{
					m_textureType = typeof( Cubemap );
					m_samplerType = "texCUBE";
					//_metafileId = "2700000";
				}
				break;
			}
		}

		// Cube
		public string GetCubePropertyValue()
		{
			return m_propertyName + "(\"" + m_propertyInspectorName + "\", CUBE) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetCubeUniformValue()
		{
			return "uniform samplerCUBE " + m_propertyName + ";";
		}

		// Texture2D
		public string GetTexture2DPropertyValue()
		{
			return m_propertyName + "(\"" + m_propertyInspectorName + "\", 2D) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetTexture2DUniformValue()
		{
			return "uniform sampler2D " + m_propertyName + ";";
		}

		//Texture3D
		public string GetTexture3DPropertyValue()
		{
			return m_propertyName + "(\"" + m_propertyInspectorName + "\", 3D) = \"" + m_defaultTextureValue + "\" {}";
		}

		public string GetTexture3DUniformValue()
		{
			return "uniform sampler3D " + m_propertyName + ";";
		}
		//

		public override void DrawSubProperties()
		{
			m_textureCoordSet = EditorGUILayout.IntPopup( Constants.AvailableUVSetsLabel, m_textureCoordSet, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );
			m_defaultTextureValue = ( TexturePropertyValues ) EditorGUILayout.EnumPopup( DefaultTextureStr, m_defaultTextureValue );
			AutoCastType newAutoCast = ( AutoCastType ) EditorGUILayout.EnumPopup( AutoCastModeStr, m_autocastMode );
			if ( newAutoCast != m_autocastMode )
			{
				m_autocastMode = newAutoCast;
				if ( m_autocastMode != AutoCastType.Auto )
				{
					ConfigTextureData( m_currentType );
				}
			}

			//if ( _isNormalMap )
			{
				bool autoUnpackNormals = EditorGUILayout.Toggle( AutoUnpackNormalsStr, m_autoUnpackNormals );
				if ( m_autoUnpackNormals != autoUnpackNormals )
				{
					AutoUnpackNormals = autoUnpackNormals;

					ConfigurePorts();
				}
			}

			if ( m_autoUnpackNormals && !m_inputPorts[ 1 ].IsConnected )
			{
				m_inputPorts[ 1 ].FloatInternalData = EditorGUILayout.FloatField( NormalScaleStr, m_inputPorts[ 1 ].FloatInternalData );
			}

			EditorGUI.BeginChangeCheck();
			m_defaultValue = ( Texture ) EditorGUILayout.ObjectField( Constants.DefaultValueLabel, m_defaultValue, m_textureType, false );
			if ( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
			}
		}

		public override void DrawMaterialProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_materialValue = ( Texture ) EditorGUILayout.ObjectField( Constants.MaterialValueLabel, m_materialValue, m_textureType, false );
			if ( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
			}
		}

		void ConfigurePortsFromReference()
		{
			bool float4Mode = !m_referenceNode.AutoUnpackNormals;
			m_inputPorts[ 1 ].Visible = m_referenceNode.AutoUnpackNormals;
			m_outputPorts[ 4 ].Visible = float4Mode;
			if ( float4Mode )
			{
				m_deltaPreviewer = 0;
				m_outputPorts[ 0 ].ChangeProperties( "RGBA", WirePortDataType.FLOAT4, false );
				m_outputPorts[ 1 ].ChangeProperties( "R", WirePortDataType.FLOAT, false );
				m_outputPorts[ 2 ].ChangeProperties( "G", WirePortDataType.FLOAT, false );
				m_outputPorts[ 3 ].ChangeProperties( "B", WirePortDataType.FLOAT, false );
				m_outputPorts[ 4 ].ChangeProperties( "A", WirePortDataType.FLOAT, false );

			}
			else
			{
				m_deltaPreviewer = 70;
				m_outputPorts[ 0 ].ChangeProperties( "XYZ", WirePortDataType.FLOAT3, false );
				m_outputPorts[ 1 ].ChangeProperties( "X", WirePortDataType.FLOAT, false );
				m_outputPorts[ 2 ].ChangeProperties( "Y", WirePortDataType.FLOAT, false );
				m_outputPorts[ 3 ].ChangeProperties( "Z", WirePortDataType.FLOAT, false );
			}
			m_sizeIsDirty = true;
		}
		
		void ConfigurePorts()
		{
			bool float4Mode = !m_autoUnpackNormals;
			m_inputPorts[ 1 ].Visible = m_autoUnpackNormals;
			m_outputPorts[ 4 ].Visible = float4Mode;
			if ( float4Mode )
			{
				m_deltaPreviewer = 0;
				m_outputPorts[ 0 ].ChangeProperties( "RGBA", WirePortDataType.FLOAT4, false );
				m_outputPorts[ 1 ].ChangeProperties( "R", WirePortDataType.FLOAT, false );
				m_outputPorts[ 2 ].ChangeProperties( "G", WirePortDataType.FLOAT, false );
				m_outputPorts[ 3 ].ChangeProperties( "B", WirePortDataType.FLOAT, false );
				m_outputPorts[ 4 ].ChangeProperties( "A", WirePortDataType.FLOAT, false );

			}
			else
			{
				m_deltaPreviewer = 70;
				m_outputPorts[ 0 ].ChangeProperties( "XYZ", WirePortDataType.FLOAT3, false );
				m_outputPorts[ 1 ].ChangeProperties( "X", WirePortDataType.FLOAT, false );
				m_outputPorts[ 2 ].ChangeProperties( "Y", WirePortDataType.FLOAT, false );
				m_outputPorts[ 3 ].ChangeProperties( "Z", WirePortDataType.FLOAT, false );
			}
			m_sizeIsDirty = true;
		}

		void CheckTextureImporter( bool setAutoUnpack )
		{
			m_requireMaterialUpdate = true;
			Texture texture = m_materialMode ? m_materialValue : m_defaultValue;
			TextureImporter importer = AssetImporter.GetAtPath( AssetDatabase.GetAssetPath( texture ) ) as TextureImporter;
			if ( importer != null )
			{
				m_isNormalMap = importer.normalmap;
				if ( setAutoUnpack )
					AutoUnpackNormals = m_isNormalMap;
			}

			ConfigurePorts();

			if ( ( texture as Texture2D ) != null )
			{
				ConfigTextureData( TextureType.Texture2D );
			}
			else if ( ( texture as Texture3D ) != null )
			{
				ConfigTextureData( TextureType.Texture3D );
			}
			else if ( ( texture as Cubemap ) != null )
			{
				ConfigTextureData( TextureType.Cube );
			}
		}

		public override void OnObjectDropped( UnityEngine.Object obj )
		{
			base.OnObjectDropped( obj );
			ConfigFromObject( obj );
		}

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			base.SetupFromCastObject( obj );
			ConfigFromObject( obj );
		}

		private void ConfigFromObject( UnityEngine.Object obj )
		{
			Texture texture = obj as Texture;
			if ( texture )
			{
				m_materialValue = texture;
				m_defaultValue = texture;
				CheckTextureImporter( true );
			}
		}

		void UpdateHeaderColor()
		{
			m_headerColorModifier = ( m_referenceType == TexReferenceType.Object ) ? Color.white : ReferenceHeaderColor;
		}

		protected override void ChangeSizeFinished()
		{
			if ( m_referenceType == TexReferenceType.Instance )
			{
				m_position.width += 20;
			}
		}

		public override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_referenceType = ( TexReferenceType ) EditorGUILayout.EnumPopup( Constants.ReferenceTypeStr, m_referenceType );
			if ( EditorGUI.EndChangeCheck() )
			{
				m_sizeIsDirty = true;
				if ( m_referenceType == TexReferenceType.Object )
				{
					UIUtils.RegisterSamplerNode( this );
					m_content.text = m_propertyInspectorName;
					m_additionalContent.text = string.Format( Constants.PropertyValueLabel, GetPropertyValStr() );
					ConfigurePorts();
				}
				else
				{
					UIUtils.UnregisterSamplerNode( this );
				}
				UpdateHeaderColor();
			}

			if ( m_referenceType == TexReferenceType.Object )
			{
				EditorGUI.BeginChangeCheck();
				base.DrawProperties();
				if ( EditorGUI.EndChangeCheck() )
				{
					OnPropertyNameChanged();
				}
			}
			else
			{
				string[] arr = UIUtils.SamplerNodeArr();
				bool guiEnabledBuffer = GUI.enabled;
				if ( arr != null && arr.Length > 0 )
				{
					GUI.enabled = true;
				}
				else
				{
					m_referenceId = -1;
					GUI.enabled = false;
				}

				m_referenceId = EditorGUILayout.Popup( Constants.AvailableReferenceStr, m_referenceId, arr );
				GUI.enabled = guiEnabledBuffer;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			EditorGUI.BeginChangeCheck();
			base.Draw( drawInfo );
			if ( EditorGUI.EndChangeCheck() )
			{
				OnPropertyNameChanged();
			}
			
			if ( m_isVisible )
			{
				Rect newPos = m_globalPosition;
				newPos.x += ( 60 + m_deltaPreviewer ) * drawInfo.InvertedZoom;
				newPos.y += 37 * drawInfo.InvertedZoom;
				

				if ( SoftValidReference )
				{
					float previewSizeX = m_referenceNode.AutoUnpackNormals ? PreviewerNormalSizeX : PreviewerSizeX;
					float previewSizeY = m_referenceNode.AutoUnpackNormals ? PreviewerNormalSizeY : PreviewerSizeY;
					newPos.width = previewSizeX * drawInfo.InvertedZoom;
					newPos.height = previewSizeY * drawInfo.InvertedZoom;


					m_content.text = m_referenceNode.TitleContent.text + Constants.InstancePostfixStr;
					m_additionalContent.text = m_referenceNode.AdditonalTitleContent.text;
					if ( m_referenceStyle == null )
					{
						m_referenceStyle = UIUtils.CustomStyle( CustomStyle.SamplerTextureRef );
					}

					if ( m_referenceIconStyle == null )
					{
						m_referenceIconStyle = UIUtils.CustomStyle( CustomStyle.SamplerTextureIcon );
					}

					Rect iconPos = m_globalPosition;
					iconPos.width = m_referenceIconStyle.normal.background.width * drawInfo.InvertedZoom;
					iconPos.height = m_referenceIconStyle.normal.background.height * drawInfo.InvertedZoom;

					iconPos.y += 6 * drawInfo.InvertedZoom;
					iconPos.x += m_globalPosition.width - iconPos.width - 7 * drawInfo.InvertedZoom;

					if ( GUI.Button( newPos, string.Empty, UIUtils.CustomStyle( CustomStyle.SamplerTextureRef )/* m_referenceStyle */) ||
						GUI.Button( iconPos, string.Empty, m_referenceIconStyle )
						)
					{
						UIUtils.FocusOnNode( m_referenceNode, 1, true );
					}

					newPos.x += 3 * drawInfo.InvertedZoom;
					newPos.y += 3 * drawInfo.InvertedZoom;

					newPos.width *= 0.94f;
					newPos.height *= 0.94f;

					if ( m_referenceContent != null && m_referenceContent.image != null )
						EditorGUI.DrawPreviewTexture( newPos, m_referenceContent.image );
				}
				else
				{
					float previewSizeX = m_autoUnpackNormals ? PreviewerNormalSizeX : PreviewerSizeX;
					float previewSizeY = m_autoUnpackNormals ? PreviewerNormalSizeY : PreviewerSizeY;
					newPos.width = previewSizeX * drawInfo.InvertedZoom;
					newPos.height = previewSizeY * drawInfo.InvertedZoom;


					//Lower portion
					int fontSizeUpper = GUI.skin.customStyles[ 265 ].fontSize;
					int fontSizeLower = GUI.skin.customStyles[ 266 ].fontSize;

					GUI.skin.customStyles[ 265 ].fontSize = ( int ) ( OriginalFontSizeUpper * drawInfo.InvertedZoom );
					GUI.skin.customStyles[ 266 ].fontSize = ( int ) ( OriginalFontSizeLower * drawInfo.InvertedZoom );

					EditorGUI.BeginChangeCheck();
					if ( m_materialMode )
					{
						m_materialValue = ( Texture ) EditorGUI.ObjectField( newPos, m_materialValue, m_textureType, false );
					}
					else
					{
						m_defaultValue = ( Texture ) EditorGUI.ObjectField( newPos, m_defaultValue, m_textureType, false );
					}

					if ( EditorGUI.EndChangeCheck() )
					{
						CheckTextureImporter( true );
						BeginDelayedDirtyProperty();
					}

					GUI.skin.customStyles[ 265 ].fontSize = fontSizeUpper;
					GUI.skin.customStyles[ 266 ].fontSize = fontSizeLower;
				}
			}
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			//if ( SoftValidReference )
			//{
			//	if ( !m_referenceNode.IsConnected )
			//		m_referenceNode.GenerateShaderForOutput( outputId,inputType, ref dataCollector, ignoreLocalVar );
			//}

			string propertyName = CurrentPropertyReference;
			OnPropertyNameChanged();

			if ( m_autoUnpackNormals )
			{
				bool isScaledNormal = false;
				if ( m_inputPorts[ 1 ].IsConnected )
				{
					isScaledNormal = true;
				}
				else
				{
					if ( m_inputPorts[ 1 ].FloatInternalData != 1 )
					{
						isScaledNormal = true;
					}
				}
				if ( isScaledNormal )
				{
					string scaleValue = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, inputType, ignoreLocalVar );
					dataCollector.AddToIncludes( m_uniqueId, Constants.UnityStandardUtilsLibFuncs );
					m_normalMapUnpackMode = "UnpackScaleNormal( {0} ," + scaleValue + " )";
				}
				else
				{
					m_normalMapUnpackMode = "UnpackNormal( {0} )";
				}
			}

			base.GenerateShaderForOutput( outputId, inputType, ref dataCollector, ignoreLocalVar );
			if ( !m_inputPorts[ 0 ].IsConnected )
			{
				string uvChannelDeclaration = IOUtils.GetUVChannelDeclaration( propertyName, -1, m_textureCoordSet );
				dataCollector.AddToInput( m_uniqueId, uvChannelDeclaration, true );

			}
			string valueName = SetFetchedData( ref dataCollector, ignoreLocalVar, inputType );
			return GetOutputColorItem( 0, outputId, valueName );
		}

		public string SetFetchedData( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar, WirePortDataType inputType )
		{
			string propertyName = CurrentPropertyReference;

			CheckTextureImporter( false );
			if ( ignoreLocalVar )
			{
				string samplerValue = m_samplerType + "( " + propertyName + "," + GetUVCoords( ref dataCollector, ignoreLocalVar ) + ")";
				AddNormalMapTag( ref samplerValue );
				return samplerValue;
			}

			if ( m_isTextureFetched )
				return m_textureFetchedValue;

			string samplerOp = m_samplerType + "( " + propertyName + "," + GetUVCoords( ref dataCollector, ignoreLocalVar ) + ")";
			AddNormalMapTag( ref samplerOp );

			int connectedPorts = 0;
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
				{
					connectedPorts += 1;
					if ( connectedPorts > 1 || m_outputPorts[ i ].ConnectionCount > 1 || ( i > 0 && inputType != WirePortDataType.FLOAT )/*if some cast is going to happen the its better to save fetch*/ )
					{
						// Create common local var and mark as fetched
						m_textureFetchedValue = m_samplerType + "Node" + m_uniqueId;
						m_isTextureFetched = true;

						dataCollector.AddToLocalVariables( m_uniqueId, ( ( /*m_isNormalMap && */m_autoUnpackNormals ) ? "float3 " : "float4 " ) + m_textureFetchedValue + " = " + samplerOp + ";" );
						m_outputPorts[ 0 ].SetLocalValue( m_textureFetchedValue );
						m_outputPorts[ 1 ].SetLocalValue( m_textureFetchedValue + ".r" );
						m_outputPorts[ 2 ].SetLocalValue( m_textureFetchedValue + ".g" );
						m_outputPorts[ 3 ].SetLocalValue( m_textureFetchedValue + ".b" );
						m_outputPorts[ 4 ].SetLocalValue( m_textureFetchedValue + ".a" );
						return m_textureFetchedValue;
					}
				}
			}
			return samplerOp;
		}

		public override void ResetOutputLocals()
		{
			base.ResetOutputLocals();
			m_isTextureFetched = false;
			m_textureFetchedValue = string.Empty;
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) )
			{
				OnPropertyNameChanged();
				if ( mat.HasProperty( m_propertyName ) )
				{
					mat.SetTexture( m_propertyName, m_materialValue );
				}
			}
		}

		public override void SetMaterialMode( Material mat )
		{
			base.SetMaterialMode( mat );
			if ( m_materialMode && UIUtils.IsProperty( m_currentParameterType ) )
			{
				if ( mat.HasProperty( m_propertyName ) )
				{
					m_materialValue = mat.GetTexture( m_propertyName );
					CheckTextureImporter( false );
				}
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
			{
				m_materialValue = material.GetTexture( m_propertyName );
				CheckTextureImporter( false );
			}
		}

		public override bool UpdateShaderDefaults( ref Shader shader, ref TextureDefaultsDataColector defaultCol/* ref string metaStr */)
		{
			if ( m_defaultValue != null )
			{
				defaultCol.AddValue( m_propertyName, m_defaultValue );
				//string data2 = AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( _defaultValue ) );
				//metaStr += ( "  - " + _propertyName + ": {fileID: " + _metafileId + ", guid: " + data2 + ", type: 3}\n" );
			}
			//else
			//{
			//	metaStr += ( "  - " + _propertyName + ": {instanceID: 0}\n" );
			//}
			return true;
		}

		private void AddNormalMapTag( ref string value )
		{
			if ( /*m_isNormalMap && */m_autoUnpackNormals )
			{
				value = string.Format( m_normalMapUnpackMode, value );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			string textureName = GetCurrentParam( ref nodeParams );
			m_defaultValue = AssetDatabase.LoadAssetAtPath<Texture>( textureName );
			m_useSemantics = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_textureCoordSet = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_isNormalMap = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_defaultTextureValue = ( TexturePropertyValues ) Enum.Parse( typeof( TexturePropertyValues ), GetCurrentParam( ref nodeParams ) );
			m_autocastMode = ( AutoCastType ) Enum.Parse( typeof( AutoCastType ), GetCurrentParam( ref nodeParams ) );
			AutoUnpackNormals = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			if ( UIUtils.CurrentShaderVersion() > 12 )
			{
				m_referenceType = ( TexReferenceType ) Enum.Parse( typeof( TexReferenceType ), GetCurrentParam( ref nodeParams ) );
				m_referenceId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				if ( m_referenceType == TexReferenceType.Instance )
				{
					UIUtils.UnregisterSamplerNode( this );
				}
				UpdateHeaderColor();
			}

			ConfigurePorts();
			ConfigFromObject( m_defaultValue );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_defaultValue != null ) ? AssetDatabase.GetAssetPath( m_defaultValue ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_useSemantics.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_textureCoordSet.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_isNormalMap.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultTextureValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autocastMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoUnpackNormals );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_referenceType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_referenceId );
		}

		public string GetUVCoords( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				if ( m_currentType != TextureType.Texture2D )
				{
					return m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar, 0, true );
				}
				else
				{
					return m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, ignoreLocalVar, 0, true );
				}
			}
			else
			{
				string propertyName = CurrentPropertyReference;
				string uvChannelName = IOUtils.GetUVChannelName( propertyName, m_textureCoordSet );
				string uvCoord = Constants.InputVarStr + "." + uvChannelName;
				if ( m_currentType != TextureType.Texture2D )
				{
					return string.Format( "float3({0},0.0)", uvCoord ); ;
				}
				else
				{
					return uvCoord;
				}
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_defaultValue = null;
			m_materialValue = null;
			m_referenceNode = null;
			m_referenceStyle = null;
			m_referenceContent = null;
			if ( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.UnregisterSamplerNode( this );
			}
		}

		public override string GetPropertyValStr()
		{
			return m_materialMode ? ( m_materialValue != null ? m_materialValue.name : IOUtils.NO_TEXTURES ) : ( m_defaultValue != null ? m_defaultValue.name : IOUtils.NO_TEXTURES );
		}

		public override string GetPropertyValue()
		{
			if ( SoftValidReference )
			{
				if ( m_referenceNode.IsConnected )
					return string.Empty;

				return m_referenceNode.GetPropertyValue();
			}

			switch ( m_currentType )
			{
				case TextureType.Texture2D:
				{
					return GetTexture2DPropertyValue();
				}
				case TextureType.Texture3D:
				{
					return GetTexture3DPropertyValue();
				}
				case TextureType.Cube:
				{
					return GetCubePropertyValue();
				}
			}
			return string.Empty;
		}

		public override string GetUniformValue()
		{
			if ( SoftValidReference )
			{
				if ( m_referenceNode.IsConnected )
					return string.Empty;

				return m_referenceNode.GetUniformValue();
			}

			switch ( m_currentType )
			{
				case TextureType.Texture2D:
				{
					return GetTexture2DUniformValue();
				}
				case TextureType.Texture3D:
				{
					return GetTexture3DUniformValue();
				}
				case TextureType.Cube:
				{
					return GetCubeUniformValue();
				}
			}

			return string.Empty;
		}

		public string CurrentPropertyReference
		{
			get
			{
				string propertyName = string.Empty;
				if ( m_referenceType == TexReferenceType.Instance && m_referenceId > -1 )
				{
					SamplerNode node = UIUtils.GetSamplerNode( m_referenceId );
					propertyName = ( node != null ) ? node.PropertyName : m_propertyName;
				}
				else
				{
					propertyName = m_propertyName;
				}
				return propertyName;
			}
		}

		public bool SoftValidReference
		{
			get
			{
				if ( m_referenceType == TexReferenceType.Instance && m_referenceId > -1 )
				{
					m_referenceNode = UIUtils.GetSamplerNode( m_referenceId );

					if ( m_referenceContent == null )
						m_referenceContent = new GUIContent();


					if ( m_referenceNode != null )
					{
						m_referenceContent.image = m_referenceNode.Value;
						if ( m_referenceWidth != m_referenceNode.Position.width )
						{
							m_referenceWidth = m_referenceNode.Position.width;
							m_sizeIsDirty = true;
						}
						if ( m_referenceNode.OutputPorts[ 4 ].Visible != m_outputPorts[ 4 ].Visible )
						{
							ConfigurePortsFromReference();
						}

					}
					else
					{
						m_referenceId = -1;
						m_referenceWidth = -1;
					}

					return m_referenceNode != null;
				}
				return false;
			}
		}

		public Texture Value
		{
			get { return m_materialMode ? m_materialValue : m_defaultValue; }
			set
			{
				if ( m_materialMode )
				{
					m_materialValue = value;
				}
				else
				{
					m_defaultValue = value;
				}
			}
		}

		private bool AutoUnpackNormals
		{
			get { return m_autoUnpackNormals; }
			set
			{
				m_autoUnpackNormals = value;
				m_defaultTextureValue = value ? TexturePropertyValues.bump : TexturePropertyValues.white;
			}
		}
	}
}
