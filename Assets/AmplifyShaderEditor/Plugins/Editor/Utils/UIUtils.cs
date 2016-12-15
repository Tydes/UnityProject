// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace AmplifyShaderEditor
{
	public enum AvailableSurfaceInputs
	{
		DEPTH = 0,
		UV_COORDS,
		UV2_COORDS,
		VIEW_DIR,
		COLOR,
		SCREEN_POS,
		WORLD_POS,
		WORLD_REFL,
		WORLD_NORMAL
	}

	public enum CustomStyle
	{
		NodeWindowOff = 0,
		NodeWindowOn,
		NodeTitle,
		NodeHeader,
		CommentaryHeader,
		ShaderLibraryTitle,
		ShaderLibraryAddToList,
		ShaderLibraryRemoveFromList,
		ShaderLibraryOpenListed,
		ShaderLibrarySelectionAsTemplate,
		ShaderLibraryItem,
		CommentaryTitle,
		PortEmptyIcon,
		PortFullIcon,
		InputPortlabel,
		OutputPortLabel,
		CommentaryResizeButton,
		CommentaryResizeButtonInv,
		CommentaryBackground,
		MinimizeButton,
		MaximizeButton,
		NodePropertiesTitle,
		ShaderModeTitle,
		MaterialModeTitle,
		ShaderNoMaterialModeTitle,
		PropertyValuesTitle,
		ShaderModeNoShader,
		MainCanvasTitle,
		ShaderBorder,
		MaterialBorder,
		SamplerTextureRef,
		SamplerTextureIcon
	}

	public struct NodeCastInfo
	{
		public int NodeId;
		public int PortId;
		public NodeCastInfo( int nodeId, int portId )
		{
			NodeId = nodeId;
			PortId = portId;
		}
		public override string ToString()
		{
			return NodeId.ToString()+PortId.ToString();
		}
	};

	public struct ButtonClickId
	{
		public const int LeftMouseButton = 0;
		public const int RightMouseButton = 1;
		public const int MiddleMouseButton = 2;
	}

	public enum ASESelectionMode
	{
		Shader = 0,
		Material
	}

	public enum DrawOrder
	{
		Background,
		Default
	}

	public enum NodeConnectionStatus
	{
		Not_Connected = 0,
		Connected,
		Error,
		Island
	}

	public enum InteractionMode
	{
		Target,
		Other,
		Both
	}

	public class UIUtils
	{
		public static bool DirtyMask = true;

		public static float HeaderMaxHeight;
		public static float CurrentHeaderHeight;
		public static GUISkin MainSkin;
		public static WireReference InputPortReference = new WireReference();
		public static WireReference OutputPortReference = new WireReference();
		public static Vector2 SnapPosition = Vector2.zero;
		public static bool SnapEnabled = false;
		public static WireReference SnapPort = new WireReference();
		public static AmplifyShaderEditorWindow CurrentWindow = null;
		public static MasterNodeDataCollector CurrentDataCollector = null;

		public static Vector2 PortsSize;
		public static Vector3 PortsDelta;
		public static Vector3 ScaledPortsDelta;


		public static bool InhibitMessages = false;


		//Label Vars

		private static TextAnchor m_alignment;
		private static TextClipping m_clipping;
		private static bool m_wordWrap;
		private static int m_fontSize;
		private static Color m_fontColor;
		private static FontStyle m_fontStyle;
		private static string m_latestOpenedFolder = string.Empty;
		private static Dictionary<string, Color> m_nodeCategoryToColor = new Dictionary<string, Color>() {{ "Master",                            new Color( 0.26f, 0.35f, 0.44f, 1.0f )},
																										{ "Default",                            new Color( 0.1f, 0.35f, 0.44f, 1.0f )},
																										{ "Vertex Data",                        new Color( 0.75f, 0.10f, 0.30f, 1.0f )},
																										{ "Operators",                          new Color( 0.10f, 0.27f, 0.45f, 1.0f) },
																										{ "Trigonometry",                        new Color( 0.8f, 0.07f, 0.18f, 1.0f)},
																										{ "Image Effects",                      new Color( 0.12f, 0.47f, 0.88f, 1.0f)},
																										{ "Misc",                               new Color( 0.49f, 0.32f, 0.60f, 1.0f)},
																										{ "Camera and Screen",                  new Color( 0.17f, 0.22f, 0.07f, 1.0f) },
																										{ "Constants",                          new Color( 0.42f, 0.70f, 0.22f, 1.0f) },
																										{ "Surface Standard Inputs",            new Color( 0.92f, 0.73f, 0.03f, 1.0f)},
																										{ "Transform",                          new Color( 0.09f, 0.43f, 0.2f, 1.0f) },
																										{ "Time",                               new Color( 0.89f, 0.59f, 0.0f, 1.0f) },
																										{ "Vector",                             new Color( 0.1f, 0.20f, 0.35f, 1.0f)},
																										{ "Debug",                              new Color( 0.78f, 0.05f, 0.43f, 1.0f)},
																										{ "Matrix",                             new Color( 0.45f, 0.9f, 0.20f, 1.0f) },
																										{ "Fog and Ambient",                    new Color( 0.35f, 0.35f, 0.35f, 1.0f)},
																										{ "Light",                              new Color( 1.0f, 0.9f, 0.0f, 1.0f) },
																										{ "Various",                            new Color( 0.8f, 0.8f, 0.8f, 0.8f) },
																										{ "Master Log",                         new Color( 0.80f, 0.22f, 0.22f, 1.0f )},
																										{ "Textures",                           new Color( 0.15f, 0.40f, 0.8f, 1.0f)},
																										{ "Commentary",                         new Color( 0.30f, 0.45f, 0.70f, 1.0f)},
																										{ "Vertex-lit",                         new Color( 0.15f, 0.4f, 0.49f, 1.0f)},
																										{ "Screen Space",                       new Color( 0.15f, 0.4f, 0.49f, 1.0f)},
																										{ "Forward Render",                     new Color( 0.15f, 0.4f, 0.49f, 1.0f)},
																										{ "Generic",                            new Color( 0.15f, 0.4f, 0.49f, 1.0f)},
																										{ "Vertex Transform",                   new Color( 0.15f, 0.4f, 0.49f, 1.0f)}};

		private static Dictionary<ToolButtonType, List<string>> m_toolButtonTooltips = new Dictionary<ToolButtonType, List<string>> {  { ToolButtonType.New,              new List<string>() { "Create new shader." } },
																																		{ ToolButtonType.Open,             new List<string>() { "Open existing shader." } },
																																		{ ToolButtonType.Save,             new List<string>() { "No changes to save.", "Save current changes." } },
																																		{ ToolButtonType.Library,          new List<string>() { "Lists custom shader selection." } },
																																		{ ToolButtonType.Options,          new List<string>() { "Open Options menu." } },
																																		{ ToolButtonType.Update,           new List<string>() { "Open or create a new shader first.", "Click to enable to update current shader.", "Shader up-to-date." } },
																																		{ ToolButtonType.Live,             new List<string>() { "Open or create a new shader first.", "Click to enable live shader preview", "Click to enable live shader and material preview." , "Live preview active, click to disable." } },
																																		{ ToolButtonType.CleanUnusedNodes, new List<string>() { "No unconnected nodes to clean.", "Remove all nodes not connected( directly or indirectly) to the master node." }},
																																		{ ToolButtonType.Help,             new List<string>() { "Show help window." } },
																																		{ ToolButtonType.FocusOnMasterNode,new List<string>() { "Focus on active master node." } },
																																		{ ToolButtonType.FocusOnSelection, new List<string>() { "Focus on selection fit to screen ( if none selected )." } }};

		private static Color[] m_dataTypeToColorMonoMode = { new Color( 0.5f, 0.5f, 0.5f, 1.0f ), Color.white };
		private static Dictionary<WirePortDataType, Color> m_dataTypeToColor = new Dictionary<WirePortDataType, Color>() {{  WirePortDataType.OBJECT, Color.white},
																															{ WirePortDataType.FLOAT, Color.gray},
																															{ WirePortDataType.FLOAT2, new Color(0f,0f,0.5f,1f)},
																															{ WirePortDataType.FLOAT3, new Color(0f,0f,0.8f,1f)},
																															{ WirePortDataType.FLOAT4, Color.blue},
																															{ WirePortDataType.FLOAT3x3, Color.red},
																															{ WirePortDataType.FLOAT4x4, Color.cyan},
																															{ WirePortDataType.COLOR, Color.yellow},
																															{ WirePortDataType.INT, Color.green}};


		private static Dictionary<WirePortDataType, string> m_dataTypeToName = new Dictionary<WirePortDataType, string>() {{ WirePortDataType.OBJECT,     "Generic Object"},
																															{ WirePortDataType.FLOAT,      "Float"},
																															{ WirePortDataType.FLOAT2,     "Vector2"},
																															{ WirePortDataType.FLOAT3,     "Vector3"},
																															{ WirePortDataType.FLOAT4,     "Vector4"},
																															{ WirePortDataType.FLOAT3x3,   "3x3 Matrix"},
																															{ WirePortDataType.FLOAT4x4,   "4x4 Matrix"},
																															{ WirePortDataType.COLOR,      "Color"},
																															{ WirePortDataType.INT,        "Int"}};


		private static Dictionary<AvailableSurfaceInputs, string> m_inputTypeDeclaration = new Dictionary<AvailableSurfaceInputs, string>() {{ AvailableSurfaceInputs.DEPTH, "float Depth : SV_Depth"},
																															{ AvailableSurfaceInputs.UV_COORDS, "float2 uv"},// texture uv must have uv or uv2 followed by the texture name
																															{ AvailableSurfaceInputs.UV2_COORDS, "float2 uv2"},
																															{ AvailableSurfaceInputs.VIEW_DIR, "float3 viewDir"},
																															{ AvailableSurfaceInputs.COLOR, "float4 color : COLOR"},
																															{ AvailableSurfaceInputs.SCREEN_POS, "float4 screenPos"},
																															{ AvailableSurfaceInputs.WORLD_POS, "float3 worldPos"},
																															{ AvailableSurfaceInputs.WORLD_REFL, "float3 worldRefl"},
																															{ AvailableSurfaceInputs.WORLD_NORMAL,"float3 worldNormal"}};


		private static Dictionary<AvailableSurfaceInputs, string> m_inputTypeName = new Dictionary<AvailableSurfaceInputs, string>() {   { AvailableSurfaceInputs.DEPTH, "Depth"},
																															{ AvailableSurfaceInputs.UV_COORDS, "uv"},// texture uv must have uv or uv2 followed by the texture name
																															{ AvailableSurfaceInputs.UV2_COORDS, "uv2"},
																															{ AvailableSurfaceInputs.VIEW_DIR, "viewDir"},
																															{ AvailableSurfaceInputs.COLOR, "color"},
																															{ AvailableSurfaceInputs.SCREEN_POS, "screenPos"},
																															{ AvailableSurfaceInputs.WORLD_POS, "worldPos"},
																															{ AvailableSurfaceInputs.WORLD_REFL, "worldRefl"},
																															{ AvailableSurfaceInputs.WORLD_NORMAL,"worldNormal"}};


		private static Dictionary<WirePortDataType, string> m_wirePortToCgType = new Dictionary<WirePortDataType, string>(){{WirePortDataType.FLOAT,"float"},
																															{WirePortDataType.FLOAT2,"float2"},
																															{WirePortDataType.FLOAT3,"float3"},
																															{WirePortDataType.FLOAT4,"float4"},
																															{WirePortDataType.FLOAT3x3,"float3x3"},
																															{WirePortDataType.FLOAT4x4,"float4x4"},
																															{WirePortDataType.COLOR,"float4"},
																															{WirePortDataType.INT,"int"}};

		private static Dictionary<KeyCode, string> m_keycodeToString = new Dictionary<KeyCode, string>(){   {KeyCode.Alpha0,"0" },
																											{KeyCode.Alpha1,"1" },
																											{KeyCode.Alpha2,"2" },
																											{KeyCode.Alpha3,"3" },
																											{KeyCode.Alpha4,"4" },
																											{KeyCode.Alpha5,"5" },
																											{KeyCode.Alpha6,"6" },
																											{KeyCode.Alpha7,"7" },
																											{KeyCode.Alpha8,"8" },
																											{KeyCode.Alpha9,"9" }};


		private static Dictionary<WireStatus, Color> m_wireStatusToColor = new Dictionary<WireStatus, Color>(){    {WireStatus.Default,new Color(0.7f,0.7f,0.7f,1.0f) },
																													{WireStatus.Highlighted,Color.yellow },
																													{WireStatus.Selected,Color.white}};


		private static Dictionary<string, bool> m_unityNativeShaderPaths = new Dictionary<string, bool> { { "Resources/unity_builtin_extra", true }, { "Library/unity default resources", true } };

		public static bool ValidReferences()
		{
			return ( InputPortReference.IsValid || OutputPortReference.IsValid );
		}

		public static void InvalidateReferences()
		{
			InputPortReference.Invalidate();
			OutputPortReference.Invalidate();
			SnapPort.Invalidate();
			SnapEnabled = false;
		}

		public static void ResetMainSkin()
		{
			if ( MainSkin != null )
			{
				CurrentHeaderHeight = HeaderMaxHeight;
				ScaledPortsDelta = PortsDelta;
				MainSkin.textField.fontSize = ( int ) ( Constants.TextFieldFontSize );
				MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.NodeTitle ].fontSize = ( int ) ( Constants.DefaultTitleFontSize );
				MainSkin.label.fontSize = ( int ) ( Constants.DefaultFontSize );
				MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.InputPortlabel ].fontSize = ( int ) ( Constants.DefaultFontSize );
				MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.OutputPortLabel ].fontSize = ( int ) ( Constants.DefaultFontSize );
				MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.CommentaryTitle ].fontSize = ( int ) ( Constants.DefaultFontSize );
			}
		}

		public static void InitMainSkin()
		{
			MainSkin = Resources.Load<GUISkin>( "GUISkins/MainSkin" );
			Texture2D portTex = CustomStyle( AmplifyShaderEditor.CustomStyle.PortEmptyIcon ).normal.background;
			PortsSize = new Vector2( portTex.width, portTex.height );
			PortsDelta = new Vector3( 0.5f * PortsSize.x, 0.5f * PortsSize.y );
			HeaderMaxHeight = MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.NodeHeader ].normal.background.height;
		}

		public static void UpdateMainSkin( DrawInfo drawInfo )
		{
			if ( MainSkin == null )
			{
				InitMainSkin();
			}

			CurrentHeaderHeight = HeaderMaxHeight * drawInfo.InvertedZoom;
			ScaledPortsDelta = drawInfo.InvertedZoom * PortsDelta;
			MainSkin.textField.fontSize = ( int ) ( Constants.TextFieldFontSize * drawInfo.InvertedZoom );
			MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.NodeTitle ].fontSize = ( int ) ( Constants.DefaultTitleFontSize * drawInfo.InvertedZoom );
			MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.PropertyValuesTitle ].fontSize = ( int ) ( Constants.PropertiesTitleFontSize * drawInfo.InvertedZoom );

			MainSkin.label.fontSize = ( int ) ( Constants.DefaultFontSize * drawInfo.InvertedZoom );
			MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.InputPortlabel ].fontSize = ( int ) ( Constants.DefaultFontSize * drawInfo.InvertedZoom );
			MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.OutputPortLabel ].fontSize = ( int ) ( Constants.DefaultFontSize * drawInfo.InvertedZoom );

			//MainSkin.customStyles[ (int)eCustomStyle.NodeHeader ].fontSize = ( int ) ( Constants.DefaultFontSize * drawInfo.InvertedZoom );
			//MainSkin.customStyles[ (int)eCustomStyle.NodeHeader ].overflow.right = ( int ) ( COMMENTARY_BOX_OVERFLOW * drawInfo.InvertedZoom );
			MainSkin.customStyles[ ( int ) AmplifyShaderEditor.CustomStyle.CommentaryTitle ].fontSize = ( int ) ( Constants.DefaultFontSize * drawInfo.InvertedZoom );
		}

		public static void CacheLabelVars()
		{
			m_alignment = GUI.skin.label.alignment;
			m_clipping = GUI.skin.label.clipping;
			m_wordWrap = GUI.skin.label.wordWrap;
			m_fontSize = GUI.skin.label.fontSize;
			m_fontStyle = GUI.skin.label.fontStyle;
			m_fontColor = GUI.skin.label.normal.textColor;
		}

		public static void RestoreLabelVars()
		{
			GUI.skin.label.alignment = m_alignment;
			GUI.skin.label.clipping = m_clipping;
			GUI.skin.label.wordWrap = m_wordWrap;
			GUI.skin.label.fontSize = m_fontSize;
			GUI.skin.label.fontStyle = m_fontStyle;
			GUI.skin.label.normal.textColor = m_fontColor;
		}

		public static string GetTooltipForToolButton( ToolButtonType toolButtonType, int state )
		{
			return m_toolButtonTooltips[ toolButtonType ][ state ];
		}

		public static string KeyCodeToString( KeyCode keyCode )
		{
			if ( m_keycodeToString.ContainsKey( keyCode ) )
				return m_keycodeToString[ keyCode ];

			return keyCode.ToString();
		}

		public static string WirePortToCgType( WirePortDataType type )
		{
			if ( type == WirePortDataType.OBJECT )
				return string.Empty;

			return m_wirePortToCgType[ type ];
		}

		public static Color GetColorForDataType( WirePortDataType dataType, bool monochromeMode = true, bool isInput = true )
		{
			if ( monochromeMode )
			{
				return isInput ? m_dataTypeToColorMonoMode[ 0 ] : m_dataTypeToColorMonoMode[ 1 ];
			}
			else
			{
				return m_dataTypeToColor[ dataType ];
			}
		}

		public static string GetNameForDataType( WirePortDataType dataType )
		{
			return m_dataTypeToName[ dataType ];
		}

		public static string GetInputDeclarationFromType( AvailableSurfaceInputs inputType )
		{
			return m_inputTypeDeclaration[ inputType ];
		}

		public static string GetInputValueFromType( AvailableSurfaceInputs inputType )
		{
			return m_inputTypeName[ inputType ];
		}

		public static string CreateLocalValueName( WirePortDataType dataType, string localOutputValue, string value )
		{
			return WirePortToCgType( dataType ) + " " + localOutputValue + "=" + value + ";";
		}

		public static string CastPortType( NodeCastInfo castInfo, object value, WirePortDataType oldType, WirePortDataType newType, string parameterName = null )
		{
			if ( oldType == newType )
			{
				return ( parameterName != null ) ? parameterName : value.ToString();
			}

			string localVarName = oldType + "To" + newType + castInfo.ToString();
			string result = string.Empty;
			bool useRealValue = ( parameterName == null );

			switch ( oldType )
			{
				case WirePortDataType.FLOAT:
				{
					switch ( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? value.ToString() : parameterName; break;
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT3:
						case WirePortDataType.COLOR:
						case WirePortDataType.FLOAT4:
						{
							string localVal = CreateLocalValueName( newType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							CurrentDataCollector.AddToLocalVariables( -1, localVal );
							result = localVarName;// "float4( " + localVarName + " , " + localVarName + " , " + localVarName + " , " + localVarName + " )";
						}
						break;
						case WirePortDataType.FLOAT3x3:
						{
							string localVal = CreateLocalValueName( newType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							CurrentDataCollector.AddToLocalVariables( -1, localVal );
							result = localVarName;
							//result = "float3x3( " + localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//							localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//							localVarName + " , " + localVarName + " , " + localVarName + " )";
						}
						break;
						case WirePortDataType.FLOAT4x4:
						{
							string localVal = CreateLocalValueName( newType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							CurrentDataCollector.AddToLocalVariables( -1, localVal );
							result = localVarName;
							//result = "float4x4( " + localVarName + " , " + localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//						localVarName + " , " + localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//						localVarName + " , " + localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//						localVarName + " , " + localVarName + " , " + localVarName + " , " + localVarName + " )";
						}
						break;
						case WirePortDataType.INT:
						{
							result = ( useRealValue ) ? ( ( int ) value ).ToString() : "(int)" + parameterName;
						}
						break;
					}
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					Vector2 vecVal = useRealValue ? ( Vector2 ) value : Vector2.zero;
					switch ( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? "float2( " + vecVal.x + " , " + vecVal.y + " )" : parameterName; break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? vecVal.x.ToString() : parameterName + ".x";
						}
						break;
						case WirePortDataType.FLOAT3:
						{
							result = ( useRealValue ) ? "float3( " + vecVal.x + " , " + vecVal.y + " , " + " 0.0 )" : "float3( " + parameterName + " ,  0.0 )";
						}
						break;
						case WirePortDataType.COLOR:
						case WirePortDataType.FLOAT4:
						{
							result = ( useRealValue ) ? "float4( " + vecVal.x + " , " + vecVal.y + " , " + " 0.0 , 0.0 )" : "float4( " + parameterName + ", 0.0 , 0.0 )";
						}
						break;
					}
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					Vector3 vecVal = useRealValue ? ( Vector3 ) value : Vector3.zero;
					switch ( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? "float3( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " )" : parameterName; break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? vecVal.x.ToString() : parameterName + ".x";
						}
						break;
						case WirePortDataType.FLOAT2:
						{
							result = ( useRealValue ) ? "float2( " + vecVal.x + " , " + vecVal.y + " )" : parameterName + ".xy";
						}
						break;
						case WirePortDataType.COLOR:
						case WirePortDataType.FLOAT4:
						{
							result = ( useRealValue ) ? "float4( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , 0.0 )" : "float4( " + parameterName + " , 0.0 )";
						}
						break;
						case WirePortDataType.FLOAT3x3:
						{
							if ( useRealValue )
							{
								result = "float3x3( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , " +
															vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , " +
															vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " )";
							}
							else
							{
								string localVal = CreateLocalValueName( newType, localVarName, parameterName );
								CurrentDataCollector.AddToLocalVariables( -1, localVal );
								result = "float3x3( " + localVarName + ".x , " + localVarName + ".y , " + localVarName + ".x , " +
													   localVarName + ".x , " + localVarName + ".y , " + localVarName + ".y , " +
													   localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z )";
							}
						}
						break;
						case WirePortDataType.FLOAT4x4:
						{
							if ( useRealValue )
							{
								result = "float4x4( " + vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , 0 , " +
														vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , 0 , " +
														vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , 0 , " +
														vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , 0 )";
							}
							else
							{
								string localVal = CreateLocalValueName( newType, localVarName, parameterName );
								CurrentDataCollector.AddToLocalVariables( -1, localVal );
								result = "float4x4( " + localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , 0 , " +
														localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , 0 , " +
														localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , 0 , " +
														localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , 0 )";
							}
						}
						break;
					}
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					Vector4 vecVal = useRealValue ? ( Vector4 ) value : Vector4.zero;
					switch ( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? "float4( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , " + vecVal.w + " )" : parameterName; break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? vecVal.x.ToString() : parameterName + ".x";
						}
						break;
						case WirePortDataType.FLOAT2:
						{
							result = ( useRealValue ) ? "float2( " + vecVal.x + " , " + vecVal.y + " )" : parameterName + ".xy";
						}
						break;
						case WirePortDataType.FLOAT3:
						{
							result = ( useRealValue ) ? "float3( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " )" : parameterName + ".xyz";
						}
						break;
						case WirePortDataType.FLOAT4x4:
						{
							if ( useRealValue )
							{
								result = "float4x4( " + vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , " + vecVal + ".w , " +
														vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , " + vecVal + ".w , " +
														vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , " + vecVal + ".w , " +
														vecVal + ".x , " + vecVal + ".y , " + vecVal + ".z , " + vecVal + ".w )";
							}
							else
							{
								string localVal = CreateLocalValueName( newType, localVarName, parameterName );
								CurrentDataCollector.AddToLocalVariables( -1, localVal );
								result = "float4x4( " + localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
														localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
														localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
														localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w )";
							}
						}
						break;
						case WirePortDataType.COLOR:
						{
							result = useRealValue ? "float4( " + vecVal.x + " , " + vecVal.y + " , " + vecVal.z + " , " + vecVal.w + " )" : parameterName;
						}
						break;
					}
				}
				break;
				case WirePortDataType.FLOAT3x3:
				{
					Matrix4x4 matrixVal = useRealValue ? ( Matrix4x4 ) value : Matrix4x4.identity;
					switch ( newType )
					{
						case WirePortDataType.OBJECT:
						case WirePortDataType.FLOAT4x4:
						{
							result = ( useRealValue ) ? "float4x4(" + matrixVal.m00 + " , " + matrixVal.m01 + " , " + matrixVal.m02 + " , " + matrixVal.m03 + " , " +
																		matrixVal.m10 + " , " + matrixVal.m11 + " , " + matrixVal.m12 + " , " + matrixVal.m10 + " , " +
																		matrixVal.m20 + " , " + matrixVal.m21 + " , " + matrixVal.m22 + " , " + matrixVal.m20 + " , " +
																		matrixVal.m30 + " , " + matrixVal.m31 + " , " + matrixVal.m32 + " , " + matrixVal.m30 + " )" : "float4x4(" + parameterName + ")";
						}
						break;
					}
				}
				break;
				case WirePortDataType.FLOAT4x4:
				{
					Matrix4x4 matrixVal = useRealValue ? ( Matrix4x4 ) value : Matrix4x4.identity;
					switch ( newType )
					{
						case WirePortDataType.OBJECT:
						{
							result = ( useRealValue ) ? "float4x4(" + matrixVal.m00 + " , " + matrixVal.m01 + " , " + matrixVal.m02 + " , " + matrixVal.m03 + " , " +
																		matrixVal.m10 + " , " + matrixVal.m11 + " , " + matrixVal.m12 + " , " + matrixVal.m10 + " , " +
																		matrixVal.m20 + " , " + matrixVal.m21 + " , " + matrixVal.m22 + " , " + matrixVal.m20 + " , " +
																		matrixVal.m30 + " , " + matrixVal.m31 + " , " + matrixVal.m32 + " , " + matrixVal.m30 + " )" : parameterName;
						}
						break;
					}
				}
				break;
				case WirePortDataType.COLOR:
				{
					Color colorValue = ( useRealValue ) ? ( Color ) value : Color.black;
					switch ( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? "float4( " + colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " )" : parameterName; break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? colorValue.r.ToString() : parameterName + ".r";
						}
						break;
						case WirePortDataType.FLOAT2:
						{
							result = ( useRealValue ) ? "float2( " + colorValue.r + " , " + colorValue.g + " )" : parameterName + ".rg";
						}
						break;
						case WirePortDataType.FLOAT3:
						{
							result = ( useRealValue ) ? "float3( " + colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " )" : parameterName + ".rgb";
						}
						break;
						case WirePortDataType.FLOAT4:
						{
							result = useRealValue ? "float4( " + colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " )" : parameterName;
						}
						break;
						case WirePortDataType.FLOAT4x4:
						{
							if ( useRealValue )
							{
								result = "float4x4( " + colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " , " +
														colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " , " +
														colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " , " +
														colorValue.r + " , " + colorValue.g + " , " + colorValue.b + " , " + colorValue.a + " )";
							}
							else
							{
								string localVal = CreateLocalValueName( newType, localVarName, parameterName );
								CurrentDataCollector.AddToLocalVariables( -1, localVal );

								result = "float4x4( " + localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
														localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
														localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w , " +
														localVarName + ".x , " + localVarName + ".y , " + localVarName + ".z , " + localVarName + ".w )";
							}
						}
						break;
					}
				}
				break;
				case WirePortDataType.INT:
				{
					switch ( newType )
					{
						case WirePortDataType.OBJECT: result = useRealValue ? value.ToString() : parameterName; break;
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT3:
						case WirePortDataType.COLOR:
						case WirePortDataType.FLOAT4:
						{
							string localVal = CreateLocalValueName( newType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							CurrentDataCollector.AddToLocalVariables( -1, localVal );
							result = localVarName;
						}
						break;
						case WirePortDataType.FLOAT3x3:
						{
							string localVal = CreateLocalValueName( oldType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							CurrentDataCollector.AddToLocalVariables( -1, localVal );
							result = localVarName;
							//result = "float3x3( " + localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//						localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//						localVarName + " , " + localVarName + " , " + localVarName + " )";
						}
						break;
						case WirePortDataType.FLOAT4x4:
						{
							string localVal = CreateLocalValueName( oldType, localVarName, ( ( useRealValue ) ? value.ToString() : parameterName ) );
							CurrentDataCollector.AddToLocalVariables( -1, localVal );
							result = localVarName;
							//result = "float4x4( " + localVarName + " , " + localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//						localVarName + " , " + localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//						localVarName + " , " + localVarName + " , " + localVarName + " , " + localVarName + " , " +
							//						localVarName + " , " + localVarName + " , " + localVarName + " , " + localVarName + " )";
						}
						break;
						case WirePortDataType.FLOAT:
						{
							result = ( useRealValue ) ? ( ( int ) value ).ToString() : "(float)" + parameterName;
						}
						break;
					}
				}
				break;

			}
			if ( result.Equals( string.Empty ) )
			{
				result = "0";
				string warningStr = string.Format( "Unable to cast from {0} to {1}. Generating dummy data ( {} )", oldType, newType, result );
				ShowMessage( warningStr, MessageSeverity.Warning );
			}
			return result;
		}

		public static bool CanCast( WirePortDataType from, WirePortDataType to )
		{
			if ( from == WirePortDataType.OBJECT || to == WirePortDataType.OBJECT || from == to )
				return true;

			switch ( from )
			{
				case WirePortDataType.FLOAT:
				{
					if ( to == WirePortDataType.INT )
						return true;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					return false;
				}
				case WirePortDataType.FLOAT3:
				{
					if ( to == WirePortDataType.COLOR ||
						to == WirePortDataType.FLOAT4 )
						return true;
				}
				break;
				case WirePortDataType.FLOAT4:
				{
					if ( to == WirePortDataType.FLOAT3 ||
						to == WirePortDataType.COLOR )
						return true;
				}
				break;
				case WirePortDataType.FLOAT3x3:
				{
					if ( to == WirePortDataType.FLOAT4x4 )
						return true;
				}
				break;
				case WirePortDataType.FLOAT4x4:
				{
					if ( to == WirePortDataType.FLOAT3x3 )
						return true;
				}
				break;
				case WirePortDataType.COLOR:
				{
					if ( to == WirePortDataType.FLOAT3 ||
						to == WirePortDataType.FLOAT4 )
						return true;

				}
				break;
				case WirePortDataType.INT:
				{
					if ( to == WirePortDataType.FLOAT )
						return true;
				}
				break;
			}

			return false;
		}

		public static int GetChannelsAmount( WirePortDataType type )
		{
			switch ( type )
			{
				case WirePortDataType.OBJECT: return 0;
				case WirePortDataType.FLOAT: return 1;
				case WirePortDataType.FLOAT2: return 2;
				case WirePortDataType.FLOAT3: return 3;
				case WirePortDataType.FLOAT4: return 4;
				case WirePortDataType.FLOAT3x3: return 9;
				case WirePortDataType.FLOAT4x4: return 16;
				case WirePortDataType.COLOR: return 4;
				case WirePortDataType.INT: return 1;
			}
			return 0;
		}

		public static string GeneratePropertyName( string name )
		{
			if ( string.IsNullOrEmpty( name ) )
				return name;

			name = UIUtils.RemoveInvalidCharacters( name );
			if ( name[ 0 ] != '_' )
			{
				name = '_' + name;
			}
			return name;
		}

		public static string RemoveInvalidCharacters( string originalString )
		{
			for ( int i = 0; i < Constants.OverallInvalidChars.Length; i++ )
			{
				originalString = originalString.Replace( Constants.OverallInvalidChars[ i ], string.Empty );
			}
			return originalString;
		}

		public static string RemoveShaderInvalidCharacters( string originalString )
		{
			for ( int i = 0; i < Constants.ShaderInvalidChars.Length; i++ )
			{
				originalString = originalString.Replace( Constants.ShaderInvalidChars[ i ], string.Empty );
			}
			return originalString;
		}

		public static void RecordObject( Object newObject )
		{
			bool performUndo = false;
			switch ( Event.current.type )
			{
				case EventType.MouseDown:
				{
					performUndo = true;
				}
				break;
				case EventType.MouseUp:
				{
					performUndo = true;
				}
				break;
				case EventType.MouseMove:
				{
					performUndo = true;
				}
				break;
				case EventType.MouseDrag:
				{
					performUndo = true;
				}
				break;
				case EventType.KeyDown:
				{
					performUndo = true;
				}
				break;
				case EventType.KeyUp:
				{
					performUndo = false;
				}
				break;
				case EventType.ScrollWheel:
				{
					performUndo = false;
				}
				break;
				case EventType.Repaint:
				case EventType.Layout:
				{
					performUndo = false;
				}
				break;
				case EventType.DragUpdated:
				case EventType.DragPerform:
				{
					performUndo = true;
				}
				break;
				case EventType.Ignore:
				{
					performUndo = false;
				}
				break;
				case EventType.Used:
				{
					performUndo = false;
				}
				break;
				case EventType.ValidateCommand:
				case EventType.ExecuteCommand:
				{
					performUndo = true;
				}
				break;
				case EventType.DragExited:
				{
					performUndo = false;
				}
				break;
				case EventType.ContextClick:
				{
					performUndo = false;
				}
				break;
			}

			//Debug.Log("RecordObject " + newObject);
			if ( performUndo )
			{
				Undo.RecordObject( newObject, Constants.NodeUndoId );
			}
		}

		public static void RegisterCreatedObjectUndo( Object newObject )
		{
			//Debug.Log("RegisterCreatedObjectUndo " + newObject);
			//Undo.RegisterCreatedObjectUndo(newObject, Constants.NodeCreateUndoId);
		}

		public static void DestroyObjectImmediate( Object newObject )
		{
			//Debug.Log("DestroyObjectImmediate " + newObject);
			//Undo.DestroyObjectImmediate(newObject);
			GameObject.DestroyImmediate( newObject );
		}

		public static bool IsUnityNativeShader( string path )
		{
			return m_unityNativeShaderPaths.ContainsKey( path );
		}

		public static string GetComponentForPosition( int pos, WirePortDataType type, bool addDot = false )
		{
			string result = addDot ? "." : string.Empty;
			switch ( pos )
			{
				case 0:
				{
					return ( ( type == WirePortDataType.COLOR ) ? ( result + "r" ) : ( result + "x" ) );
				}
				case 1:
				{
					return ( ( type == WirePortDataType.COLOR ) ? ( result + "g" ) : ( result + "y" ) );
				}
				case 2:
				{
					return ( ( type == WirePortDataType.COLOR ) ? ( result + "b" ) : ( result + "z" ) );
				}
				case 3:
				{
					return ( ( type == WirePortDataType.COLOR ) ? ( result + "a" ) : ( result + "w" ) );
				}
			}
			return string.Empty;
		}

		public static string InvalidParameter( ParentNode node )
		{
			ShowMessage( "Invalid entrance type on node" + node, MessageSeverity.Error );
			return "0";
		}

		public static string NoConnection( ParentNode node )
		{
			ShowMessage( "No Input connection on node" + node, MessageSeverity.Error );
			return "0";
		}


		public static string UnknownError( ParentNode node )
		{
			ShowMessage( "Unknown error on node" + node, MessageSeverity.Error );
			return "0";
		}


		public static void ActivateSnap( Vector2 position, WirePort port )
		{
			SnapPort.SetReference( port );
			SnapEnabled = true;
			SnapPosition = position;
		}

		public static void DeactivateSnap()
		{
			SnapEnabled = false;
			SnapPort.Invalidate();
		}

		public static string GetTex2DProperty( string name, TexturePropertyValues defaultValue )
		{
			return name + "(\"" + name + "\", 2D) = \"" + defaultValue + "\" {}";
		}

		public static string AddBrackets( string value )
		{
			return "( " + value + " )";
		}

		public static Color GetColorFromWireStatus( WireStatus status )
		{
			return m_wireStatusToColor[ status ];
		}

		public static Color GetColorFromCategory( string category )
		{
			if ( m_nodeCategoryToColor.ContainsKey( category ) )
				return m_nodeCategoryToColor[ category ];

			Debug.LogWarning( category + " category does not contain an associated color" );
			return m_nodeCategoryToColor[ "Default" ];
		}

		public static string LatestOpenedFolder
		{
			get { return m_latestOpenedFolder; }
			set { m_latestOpenedFolder = value; }
		}

		public static Shader CreateNewUnlit()
		{
			if ( CurrentWindow == null )
				return null;

			string shaderName;
			string pathName;
			Shader newShader = null;
			IOUtils.GetShaderName( out shaderName, out pathName, "MyUnlitShader", m_latestOpenedFolder );
			if ( !System.String.IsNullOrEmpty( shaderName ) && !System.String.IsNullOrEmpty( pathName ) )
			{
				CurrentWindow.CreateNewGraph( shaderName );
				CurrentWindow.PreMadeShadersInstance.FlatColorSequence.Execute();

				CurrentWindow.CurrentGraph.CurrentMasterNode.SetName( shaderName );
				newShader = CurrentWindow.CurrentGraph.FireMasterNode( pathName, true );
				AssetDatabase.Refresh();
			}
			return newShader;
		}

		public static Shader CreateNewEmpty( string customPath = null )
		{
			if ( CurrentWindow == null )
				return null;

			string shaderName;
			string pathName;
			Shader newShader = null;
			if ( string.IsNullOrEmpty( customPath ) )
			{
				IOUtils.GetShaderName( out shaderName, out pathName, "MyEmptyShader", m_latestOpenedFolder );
			}
			else
			{
				pathName = customPath;
				shaderName = "MyEmptyShader";
				while ( File.Exists( pathName + shaderName + ".shader" ) )
				{
					shaderName += "New";
				}
				pathName = pathName + shaderName + ".shader";
			}

			if ( !System.String.IsNullOrEmpty( shaderName ) && !System.String.IsNullOrEmpty( pathName ) )
			{
				m_latestOpenedFolder = pathName;
				CurrentWindow.CreateNewGraph( shaderName );
				CurrentWindow.CurrentGraph.CurrentMasterNode.SetName( shaderName );
				newShader = CurrentWindow.CurrentGraph.FireMasterNode( pathName, true );
				AssetDatabase.Refresh();
			}

			return newShader;
		}

		public static void SetDelayedMaterialMode( Material material )
		{
			if ( CurrentWindow == null )
				return;
			CurrentWindow.SetDelayedMaterialMode( material );
		}

		public static void CreateEmptyFromInvalid( Shader shader )
		{
			if ( CurrentWindow == null )
				return;

			CurrentWindow.CreateNewGraph( shader );
			CurrentWindow.ForceRepaint();
		}

		public static void DrawFloat( ref Rect propertyDrawPos, ref float value, float newLabelWidth = 8 )
		{
			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = newLabelWidth;
			value = EditorGUI.FloatField( propertyDrawPos, "  ", value, UIUtils.MainSkin.textField );
			EditorGUIUtility.labelWidth = labelWidth;
		}

		public static GUIStyle CustomStyle( CustomStyle style )
		{
			return MainSkin.customStyles[ ( int ) style ];
		}

		public static void OpenFile()
		{
			if ( CurrentWindow == null )
				return;
			string newShader = EditorUtility.OpenFilePanel( "Select Shader to open", m_latestOpenedFolder, "shader" );
			if ( !System.String.IsNullOrEmpty( newShader ) )
			{
				m_latestOpenedFolder = newShader.Substring( 0, newShader.LastIndexOf( '/' ) + 1 );
				int relFilenameId = newShader.IndexOf( Application.dataPath );
				if ( relFilenameId > -1 )
				{
					string relFilename = newShader.Substring( relFilenameId + Application.dataPath.Length - 6 );// -6 need to also copy the assets/ part
					CurrentWindow.LoadFromDisk( relFilename );
				}
				else
				{
					ShowMessage( "Can only load shaders\nfrom inside the projects folder", MessageSeverity.Error );
				}
			}
		}

		public static bool DetectNodeLoopsFrom( ParentNode node, Dictionary<int, int> currentNodes )
		{
			if ( currentNodes.ContainsKey( node.UniqueId ) )
			{
				currentNodes.Clear();
				currentNodes = null;
				return true;
			}

			currentNodes.Add( node.UniqueId, 1 );
			bool foundLoop = false;
			for ( int i = 0; i < node.InputPorts.Count; i++ )
			{
				if ( node.InputPorts[ i ].IsConnected )
				{
					ParentNode newNode = node.InputPorts[ i ].GetOutputNode();
					if ( newNode.InputPorts.Count > 0 )
					{
						Dictionary<int, int> newDict = new Dictionary<int, int>();
						foreach ( KeyValuePair<int, int> entry in currentNodes )
						{
							newDict.Add( entry.Key, entry.Value );
						}
						foundLoop = foundLoop || DetectNodeLoopsFrom( newNode, newDict );
						if ( foundLoop )
							break;
					}
				}
			}

			currentNodes.Clear();
			currentNodes = null;

			return foundLoop;
		}

		public static ParentNode CreateNode( System.Type type, bool registerUndo, Vector2 pos, int nodeId = -1, bool addLast = true )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.CreateNode( type, registerUndo, pos, nodeId, addLast );
			}
			return null;
		}

		public static void DestroyNode( int nodeId )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.DestroyNode( nodeId );
			}
		}

		public static void ShowMessage( string message, MessageSeverity severity = MessageSeverity.Normal )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.ShowMessage( message, severity );
			}
		}

		public static ParentNode GetNode( int nodeId )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.GetNode( nodeId );
			}
			return null;
		}

		public static void DeleteConnection( bool isInput, int nodeId, int portId, bool registerOnLog )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.DeleteConnection( isInput, nodeId, portId, registerOnLog );
			}
		}

		public static void ConnectInputToOutput( int inNodeId, int inPortId, int outNodeId, int outPortId )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.ConnectInputToOutput( inNodeId, inPortId, outNodeId, outPortId );
			}
		}

		public static Shader CreateNewGraph( string name )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CreateNewGraph( name );
			}
			return null;
		}
		public static void SetConnection( int InNodeId, int InPortId, int OutNodeId, int OutPortId )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.SetConnection( InNodeId, InPortId, OutNodeId, OutPortId );
			}
		}

		public static bool IsChannelAvailable( int channelId )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.IsChannelAvailable( channelId );
			}
			return false;
		}

		public static bool ReleaseUVChannel( int nodeId, int channelId )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.ReleaseUVChannel( nodeId, channelId );
			}
			return false;
		}

		public static bool RegisterUVChannel( int nodeId, int channelId, string name )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.RegisterUVChannel( nodeId, channelId, name );
			}
			return false;
		}

		public static void GetFirstAvailableName( int nodeId, WirePortDataType type, out string outProperty, out string outInspector, bool useCustomPrefix = false, string customPrefix = null )
		{
			outProperty = string.Empty;
			outInspector = string.Empty;
			if ( CurrentWindow != null )
			{
				CurrentWindow.DuplicatePrevBufferInstance.GetFirstAvailableName( nodeId, type, out outProperty, out outInspector, useCustomPrefix, customPrefix );
			}
		}

		public static bool RegisterUniformName( int nodeId, string name )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.RegisterUniformName( nodeId, name );
			}
			return false;
		}

		public static bool ReleaseUniformName( int nodeId, string name )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.ReleaseUniformName( nodeId, name );
			}
			return false;
		}

		public static bool IsUniformNameAvailable( string name )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.IsUniformNameAvailable( name );
			}
			return false;
		}

		public static string GetChannelName( int channelId )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.GetChannelName( channelId );
			}
			return string.Empty;
		}

		public static void SetChannelName( int channelId, string name )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.DuplicatePrevBufferInstance.SetChannelName( channelId, name );
			}
		}

		public static int RegisterFirstAvailableChannel( int nodeId, string name )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.DuplicatePrevBufferInstance.RegisterFirstAvailableChannel( nodeId, name );
			}
			return -1;
		}

		public static bool DisplayDialog( string shaderPath )
		{
			string value = System.String.Format( "Save changes to the shader {0} before closing?", shaderPath );
			return EditorUtility.DisplayDialog( "Load selected", value, "Yes", "No" );
		}

		public static void ForceUpdateFromMaterial()
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.Focus();
				CurrentWindow.ForceUpdateFromMaterial();
			}
		}

		public static void MarkToRepaint()
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.MarkToRepaint();
			}
		}

		public static string FloatToString( float value )
		{
			string floatStr = value.ToString();
			if ( value % 1 == 0 )
			{
				floatStr += ".0";
			}
			return floatStr;
		}

		public static int CurrentVersion()
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentVersion;
			}
			return -1;
		}


		public static int CurrentShaderVersion()
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.LoadedShaderVersion;
			}
			return -1;
		}


		public static bool IsProperty( PropertyType type )
		{
			return ( type == PropertyType.Property || type == PropertyType.InstancedProperty );
		}

		public static MasterNode CurrentMasterNode()
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.CurrentMasterNode;
			}
			return null;
		}

		public static void AddInstancePropertyCount()
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.AddInstancePropertyCount();
			}
		}

		public static bool IsInstancedShader()
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.IsInstancedShader;
			}
			return false;
		}

		public static void RemoveInstancePropertyCount()
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.RemoveInstancePropertyCount();
			}
		}

		public static void AddNormalDependentCount()
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.AddNormalDependentCount();
			}
		}

		public static void RemoveNormalDependentCount()
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.RemoveNormalDependentCount();
			}
		}

		public static bool IsNormalDependent()
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.IsNormalDependent;
			}
			return false;
		}

		public static void CopyValuesFromMaterial( Material mat )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.CopyValuesFromMaterial( mat );
			}

		}

		// Sampler Node

		public static void RegisterSamplerNode( SamplerNode node )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.AddSamplerNode( node );
			}
		}

		public static void UnregisterSamplerNode( SamplerNode node )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.RemoveSamplerNode( node );
			}
		}

		public static string[] SamplerNodeArr()
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.SamplerNodesArr;
			}
			return null;
		}


		public static SamplerNode GetSamplerNode( int idx )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.GetSamplerNode( idx );
			}
			return null;
		}

		// Screen Color Node

		public static void RegisterScreenColorNode( ScreenColorNode node )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.AddScreenColorNode( node );
			}
		}

		public static void UnregisterScreenColorNode( ScreenColorNode node )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.RemoveScreenColorNode( node );
			}
		}

		public static string[] ScreenColorNodeArr()
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.ScreenColorNodesArr;
			}
			return null;
		}


		public static ScreenColorNode GetScreenColorNode( int idx )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.GetScreenColorNode( idx );
			}
			return null;
		}


		// Local Var Node

		public static int RegisterLocalVarNode( RegisterLocalVarNode node )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.LocalVarNodes.AddNode( node );
			}
			return -1;
		}

		public static void UnregisterLocalVarNode( RegisterLocalVarNode node )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.LocalVarNodes.RemoveNode( node );
			}
		}

		public static string[] LocalVarNodeArr()
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.LocalVarNodes.NodesArr;
			}
			return null;
		}


		public static int LocalVarNodeAmount()
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.LocalVarNodes.NodesList.Count;
			}
			return 0;
		}


		public static int GetNodeRegisterId( int uniqueId )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.LocalVarNodes.GetNodeRegisterId( uniqueId ) ;
			}
			return -1;
		}

		public static RegisterLocalVarNode GetLocalVarNode( int idx )
		{
			if ( CurrentWindow != null )
			{
				return CurrentWindow.CurrentGraph.LocalVarNodes.GetNode( idx ) as RegisterLocalVarNode;
			}
			return null;
		}

		public static void UpdateLocalVarDataNode( int nodeIdx, string data )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.CurrentGraph.LocalVarNodes.UpdateDataOnNode( nodeIdx, data );
			}
		}
		///////////////////////////////////////////////////////////////////////////////////

		public static void FocusOnNode( ParentNode node, float zoom, bool selectNode )
		{
			if ( CurrentWindow != null )
			{
				CurrentWindow.FocusOnNode( node, zoom, selectNode );
			}
		}
	}
}
