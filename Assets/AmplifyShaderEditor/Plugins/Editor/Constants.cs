// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
namespace AmplifyShaderEditor
{
	public struct Constants
	{
		public readonly static string ReferenceTypeStr = "Type";
		public readonly static string AvailableReferenceStr = "Reference";
		public readonly static string InstancePostfixStr = " (Instance) ";

		public readonly static string HelpURL = "http://amplify.pt/";
		public readonly static string ASEMenuName = "Amplify Shader";

		public readonly static string UnityShaderVariables = "UnityShaderVariables.cginc";
		public readonly static string UnityCgLibFuncs = "UnityCG.cginc";
		public readonly static string UnityStandardUtilsLibFuncs = "UnityStandardUtils.cginc";

		public readonly static Color InfiniteLoopColor = Color.red;

		public readonly static Color NodeBodyColor = new Color( 1f, 1f, 1f, 1.0f );

		public readonly static Color ModeTextColor = new Color( 1f, 1f, 1f, 0.25f );
		public readonly static Color ModeIconColor = new Color( 1f, 1f, 1f, 0.75f );

		public readonly static Color PortTextColor = new Color( 1f, 1f, 1f, 0.5f );
		public readonly static Color BoxSelectionColor = new Color( 1f, 1f, 1f, 0.5f );

		public readonly static Color NodeSelectedColor = new Color( 0.5f, 0.5f, 1f, 1f );
		public readonly static Color NodeDefaultColor = new Color( 1f, 1f, 1f, 1f );
		public readonly static Color NodeConnectedColor = new Color( 1.0f, 1f, 0.0f, 1f );
		public readonly static Color NodeErrorColor = new Color( 1f, 0.5f, 0.5f, 1f );
		public readonly static string NoSpecifiedCategoryStr = "<None>";

		public readonly static int MINIMIZE_WINDOW_LOCK_SIZE = 750;

		public readonly static float SNAP_SQR_DIST = 200f;
		public readonly static int INVALID_NODE_ID = -1;
		public readonly static float WIRE_WIDTH = 7f;
		public readonly static float WIRE_CONTROL_POINT_DIST = 0.7f;
		public readonly static float WIRE_CONTROL_POINT_DIST_INV = 1.7f;

		public readonly static float TextFieldFontSize = 11f;
		public readonly static float DefaultFontSize = 15f;
		public readonly static float DefaultTitleFontSize = 13f;
		public readonly static float PropertiesTitleFontSize = 11f;
		public readonly static float MessageFontSize = 40f;
		public readonly static float SelectedObjectFontSize = 30f;

		public readonly static float PORT_X_ADJUST = 10;
		public readonly static float PORT_INITIAL_X = 10;

		public readonly static float PORT_INITIAL_Y = 40;
		public readonly static float INPUT_PORT_DELTA_Y = 5;
		public readonly static float PORT_TO_LABEL_SPACE_X = 5;

		public readonly static float NODE_HEADER_HEIGHT = 32;

		public readonly static float MULTIPLE_SELECION_BOX_ALPHA = 0.5f;
		public readonly static float RMB_CLICK_DELTA_TIME = 0.1f;
		public readonly static float RMB_SCREEN_DIST = 10f;

		public readonly static float CAMERA_MAX_ZOOM = 2f;
		public readonly static float CAMERA_MIN_ZOOM = 1f;
		public readonly static float CAMERA_ZOOM_SPEED = 0.1f;

		public readonly static object INVALID_VALUE = null;

		//public readonly static Vector2 PortsSize = new Vector2( 15, 15 );
		//public readonly static Vector3 PortsDelta = new Vector3( 0.5f * PortsSize.x, 0.5f * PortsSize.y );

		public readonly static string LocalVarIdentation = "\t\t\t";
		public readonly static string SimpleLocalValueDec = LocalVarIdentation + "{0} {1};\n";
		public readonly static string LocalValueDecWithoutIdent = "{0} {1} = {2};";
		public readonly static string LocalValueDec = LocalVarIdentation + LocalValueDecWithoutIdent + '\n';
		public readonly static string LocalValueDef = LocalVarIdentation + "{0} = {1};\n";

		public readonly static string PropertyValueLabel = "Value( {0} )";
		public readonly static string ConstantsValueLabel = "Const( {0} )";

		public readonly static string PropertyFloatFormatLabel = "0.###";
		public readonly static string PropertyBigFloatFormatLabel = "0.###e+0";

		public readonly static string PropertyIntFormatLabel = "0";
		public readonly static string PropertyBigIntFormatLabel = "0e+0";


		public readonly static string PropertyVectorFormatLabel = "0.##";
		public readonly static string PropertyBigVectorFormatLabel = "0.##e+0";


		public readonly static string PropertyMatrixFormatLabel = "0.#";
		public readonly static string PropertyBigMatrixFormatLabel = "0.#e+0";

		public readonly static string NoPropertiesLabel = "No assigned properties";

		public readonly static string ValueLabel = "Value";
		public readonly static string DefaultValueLabel = "Default Value";
		public readonly static string MaterialValueLabel = "Material Value";

		public readonly static string InputVarStr = "input";
		public readonly static string OutputVarStr = "output";

		public readonly static string VertexShaderOutputStr = "o";
		public readonly static string VertexShaderInputStr = "vertexData";
		public readonly static string VertexDataFunc = "vertexDataFunc";


		public readonly static string VertexVecNameStr = "vertexVec";
		public readonly static string VertexVecDecStr = "float3 " + VertexVecNameStr;
		public readonly static string VertexVecVertStr = VertexShaderOutputStr + "." + VertexVecNameStr;

		public readonly static string NormalVecNameStr = "normalVec";
		public readonly static string NormalVecDecStr = "float3 " + NormalVecNameStr;
		public readonly static string NormalVecFragStr = InputVarStr + "." + NormalVecNameStr;
		public readonly static string NormalVecVertStr = VertexShaderOutputStr + "." + NormalVecNameStr;


		public readonly static string IncidentVecNameStr = "incidentVec";
		public readonly static string IncidentVecDecStr = "float3 " + IncidentVecNameStr;
		public readonly static string IncidentVecDefStr = VertexShaderOutputStr + "." + IncidentVecNameStr + " = normalize( " + VertexVecNameStr + " - _WorldSpaceCameraPos.xyz)";
		public readonly static string IncidentVecFragStr = InputVarStr + "." + IncidentVecNameStr;
		public readonly static string IncidentVecVertStr = VertexShaderOutputStr + "." + IncidentVecNameStr;


		public readonly static string NoStringValue = "None";
		public readonly static string EmptyPortValue = "   ";

		public readonly static string[] OverallInvalidChars = { "\r", "\n", "\\", " ", ".", ">", ",", "<", "\'", "\"", ";", ":", "[", "{", "]", "}", "=", "+", "`", "~", "/", "?", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-" };
		public readonly static string[] ShaderInvalidChars = { "\r", "\n", "\\", "\'", "\"", };

		public readonly static string InternalData = "INTERNAL_DATA";



		public readonly static string NoMaterialStr = "None";

		public readonly static string OptionalParametersSep = " ";

		public readonly static string NodeUndoId = "NODE_UNDO_ID";
		public readonly static string NodeCreateUndoId = "NODE_CREATE_UNDO_ID";
		public readonly static string NodeDestroyUndoId = "NODE_DESTROY_UNDO_ID";

		// Custom node tags
		//[InPortBegin:Id:Type:Name:InPortEnd]
		public readonly static string CNIP = "#IP";

		public readonly static float FLOAT_DRAW_HEIGHT_FIELD_SIZE = 17f;
		public readonly static float FLOAT_DRAW_WIDTH_FIELD_SIZE = 45f;
		public readonly static float FLOAT_WIDTH_SPACING = 3f;

		public readonly static Color LockedPortColor = new Color( 0.3f, 0.3f, 0.3f, 0.5f );

		public readonly static int[] AvailableUVChannels = { 0, 1, 2, 3 };
		public readonly static string[] AvailableUVChannelsStr = { "0", "1", "2", "3" };
		public readonly static string AvailableUVChannelLabel = "UV Channel";

		public readonly static int[] AvailableUVSets = { 0, 1 };
		public readonly static string[] AvailableUVSetsStr = { "1", "2" };
		public readonly static string AvailableUVSetsLabel = "UV Set";


		public readonly static string LineSeparator = "________________________________";

		public readonly static Vector2 CopyPasteDeltaPos = new Vector2( 40, 40 );

		//public readonly static string[,] UVChannelsDeclaration = { {    "float2 uv_Texture0 : TEXCOORD0",
		//																"float2 uv_Texture1 : TEXCOORD1",
		//																"float2 uv_Texture2 : TEXCOORD2",
		//																"float2 uv_Texture3 : TEXCOORD3"},
		//															{   "float2 uv2_Texture0 : TEXCOORD0",
		//																"float2 uv2_Texture1 : TEXCOORD1",
		//																"float2 uv2_Texture2 : TEXCOORD2",
		//																"float2 uv2_Texture3 : TEXCOORD3"}};

		//public readonly static string[,] UVChannelsName = { {   "uv_Texture0",
		//														"uv_Texture1",
		//														"uv_Texture2",
		//														"uv_Texture3" },
		//													{   "uv2_Texture0",
		//														"uv2_Texture1",
		//														"uv2_Texture2",
		//														"uv2_Texture3"}};


		//public readonly static string[] AvailableUVNames = {    "_Texture0",
		//														"_Texture1",
		//														"_Texture2",
		//														"_Texture3" };

	}
}
