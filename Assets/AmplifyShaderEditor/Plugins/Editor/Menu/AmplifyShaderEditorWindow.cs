// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using AmplifyShaderEditor;
using System;
using System.Collections.Generic;

public class AmplifyShaderEditorWindow : EditorWindow, ISerializationCallbackReceiver
{
	public const string CopyCommand = "Copy";
	public const string PasteCommand = "Paste";
	public const string SelectAll = "SelectAll";
	public const string Duplicate = "Duplicate";
	public const string LiveShaderError = "Live Shader only works with an assigned Master Node on the graph";

	public Texture2D MasterNodeOnTexture = null;
	public Texture2D MasterNodeOffTexture = null;

	public Texture2D GPUInstancedOnTexture = null;
	public Texture2D GPUInstancedOffTexture = null;

	private bool m_initialized = false;

	// UI 
	private Rect m_graphArea;
	private Texture2D m_graphBgTexture;
	private Texture2D m_graphFgTexture;
	private GUIStyle m_graphFontStyle;
	//private GUIStyle _borderStyle;
	private Texture2D m_wireTexture;

	[SerializeField]
	private ASESelectionMode m_selectionMode = ASESelectionMode.Shader;

	[SerializeField]
	private DuplicatePreventionBuffer m_duplicatePreventionBuffer;

	// Prevent save ops every tick when on live mode
	[SerializeField]
	private double m_lastTimeSaved = 0;

	[SerializeField]
	private bool m_cacheSaveOp = false;
	private const double SaveTime = 1;

	// Graph logic
	[SerializeField]
	private ParentGraph m_mainGraphInstance;

	// Camera control
	[SerializeField]
	private Vector2 m_cameraOffset;

	private Rect m_cameraInfo;

	[SerializeField]
	private float m_cameraZoom;

	[SerializeField]
	private Vector2 m_minNodePos;

	[SerializeField]
	private Vector2 m_maxNodePos;

	[SerializeField]
	private bool m_isDirty;

	[SerializeField]
	private bool m_saveIsDirty;

	[SerializeField]
	private bool m_repaintIsDirty;

	[SerializeField]
	private bool m_liveShaderEditing = false;

	[SerializeField]
	private bool m_shaderIsModified = true;

	[SerializeField]
	private string m_lastOpenedLocation = string.Empty;

	[SerializeField]
	private bool m_zoomChanged = true;

	[SerializeField]
	private float m_lastWindowWidth = 0;


	private bool m_ctrlSCallback = false;

	// Events
	private Vector3 m_currentMousePos;
	private Vector2 m_keyEvtMousePos2D;
	private Vector2 m_currentMousePos2D;
	private Event m_currentEvent;
	private bool m_insideEditorWindow;

	private bool m_lostFocus = false;
	// Selection box for multiple node selection 
	private bool m_multipleSelectionActive = false;
	private bool m_lmbPressed = false;
	private Vector2 m_multipleSelectionStart;
	private Rect m_multipleSelectionArea;
	private bool m_autoPanDirActive = false;
	private bool m_forceAutoPanDir = false;
	private bool m_loadShaderOnSelection = false;

	//Context Menu
	private Vector2 m_rmbStartPos;
	private GraphContextMenu m_contextMenu;

	//Clipboard
	private Clipboard m_clipboard;

	//Node Parameters Window
	[SerializeField]
	private bool m_nodeParametersWindowMaximized = true;
	private NodeParametersWindow m_nodeParametersWindow;

	// Tools Window
	private ToolsWindow m_toolsWindow;

	// Mode Window
	private ShaderEditorModeWindow m_modeWindow;

	//Palette Window
	[SerializeField]
	private bool m_paletteWindowMaximized = true;
	private PaletteWindow m_paletteWindow;

	private ContextPalette m_contextPalette;
	private PalettePopUp m_palettePopup;
	private Type m_paletteChosenType;

	// In-Editor Message System
	GenericMessageUI m_genericMessageUI;
	private GUIContent m_genericMessageContent;

	// Drag&Drop Tool 
	private DragAndDropTool m_dragAndDropTool;

	//Custom Styles
	private CustomStylesContainer m_customStyles;

	//private ConfirmationWindow _confirmationWindow;

	private List<MenuParent> m_registeredMenus;

	private PreMadeShaders m_preMadeShaders;

	private AutoPanData[] m_autoPanArea;

	private DrawInfo m_drawInfo;
	private KeyCode m_lastKeyPressed = KeyCode.None;
	private Type m_commentaryTypeNode;

	private int m_onLoadDone = 0;

	private float m_copyPasteDeltaMul = 0;
	private Vector2 m_copyPasteInitialPos = Vector2.zero;
	private Vector2 m_copyPasteDeltaPos = Vector2.zero;

	private int m_repaintCount = 0;
	private bool m_forceUpdateFromMaterialFlag = false;

	private VersionInfo m_versionInfo;

	private UnityEngine.Object m_delayedLoadObject = null;
	private double m_focusOnSelectionTimestamp;
	private double m_focusOnMasterNodeTimestamp;
	private const double m_autoZoomTime = 0.25;

	private Material m_delayedMaterialSet = null;

	private bool m_mouseDownOnValidArea = false;

	// Unity Menu item
	[MenuItem( "Window/Amplify Shader Editor/Open Canvas" )]
	static void OpenMainShaderGraph()
	{
		AmplifyShaderEditorWindow currentWindow = OpenWindow();
		currentWindow.CreateNewGraph( "Empty" );
	}

	//[MenuItem( "CONTEXT/Shader/ConvertToASE" )]
	//public static void ConvertShaderToASEContext( MenuCommand menuCommand )
	//{
	//	ConvertShaderToASE( menuCommand.context as Shader );
	//}

	public static void ConvertShaderToASE( Shader shader )
	{
		if ( IOUtils.IsASEShader( shader ) )
		{
			if ( UIUtils.CurrentWindow == null )
			{
				OpenWindow();
			}
			UIUtils.CurrentWindow.LoadProjectSelected();

		}
		else
		{
			if ( UIUtils.CurrentWindow == null )
			{
				OpenWindow();
			}
			UIUtils.CreateEmptyFromInvalid( shader );
			UIUtils.ShowMessage( "Convertion complete. Old data will be lost when saving it" );
		}
	}


	public static void LoadMaterialToASE( Material material )
	{
		if ( IOUtils.IsASEShader( material.shader ) )
		{
			if ( UIUtils.CurrentWindow == null )
			{
				OpenWindow();
			}
			UIUtils.CurrentWindow.LoadProjectSelected( material );
		}
		else
		{
			if ( UIUtils.CurrentWindow == null )
			{
				OpenWindow();
			}
			UIUtils.CreateEmptyFromInvalid( material.shader );
			UIUtils.SetDelayedMaterialMode( material );

		}
	}


	public static AmplifyShaderEditorWindow OpenWindow()
	{
		AmplifyShaderEditorWindow currentWindow = ( AmplifyShaderEditorWindow ) AmplifyShaderEditorWindow.GetWindow( typeof( AmplifyShaderEditorWindow ) );
		currentWindow.minSize = new Vector2( ( Constants.MINIMIZE_WINDOW_LOCK_SIZE - 150 ), 350 );
		currentWindow.wantsMouseMove = true;
		return currentWindow;
	}


	// Shader Graph window
	public void OnEnable()
	{
		IOUtils.Init();
		m_contextMenu = new GraphContextMenu( m_mainGraphInstance );

		m_paletteWindow = new PaletteWindow( m_contextMenu.MenuItems );
		m_paletteWindow.OnPaletteNodeCreateEvt += OnPaletteNodeCreate;
		m_registeredMenus.Add( m_paletteWindow );

		m_contextPalette = new ContextPalette( m_contextMenu.MenuItems );
		m_contextPalette.OnPaletteNodeCreateEvt += OnContextPaletteNodeCreate;
		m_registeredMenus.Add( m_contextPalette );

		m_genericMessageUI = new GenericMessageUI();
		m_genericMessageUI.OnMessageDisplayEvent += ShowMessageImmediately;

		Selection.selectionChanged += OnProjectSelectionChanged;
		EditorApplication.projectWindowChanged += OnProjectWindowChanged;

		m_focusOnSelectionTimestamp = EditorApplication.timeSinceStartup;
		m_focusOnMasterNodeTimestamp = EditorApplication.timeSinceStartup;

		m_nodeParametersWindow.IsMaximized = m_nodeParametersWindowMaximized;
		m_paletteWindow.IsMaximized = m_paletteWindowMaximized;

		UpdateLiveUI();
	}

	public AmplifyShaderEditorWindow()
	{
		m_versionInfo = new VersionInfo();
		m_minNodePos = new Vector2( float.MaxValue, float.MaxValue );
		m_maxNodePos = new Vector2( float.MinValue, float.MinValue );

		m_duplicatePreventionBuffer = new DuplicatePreventionBuffer();
		m_commentaryTypeNode = typeof( CommentaryNode );
		titleContent = new GUIContent( "Shader Editor" );
		autoRepaintOnSceneChange = true;
		m_mainGraphInstance = new ParentGraph();
		m_mainGraphInstance.OnNodeEvent += OnNodeStoppedMovingEvent;
		m_mainGraphInstance.OnMaterialUpdatedEvent += OnMaterialUpdated;
		m_mainGraphInstance.OnShaderUpdatedEvent += OnShaderUpdated;
		m_mainGraphInstance.OnEmptyGraphDetectedEvt += OnEmptyGraphDetected;

		m_currentMousePos = new Vector3( 0, 0, 0 );
		m_keyEvtMousePos2D = new Vector2( 0, 0 );
		m_multipleSelectionStart = new Vector2( 0, 0 );
		m_initialized = false;
		m_graphBgTexture = null;
		m_graphFgTexture = null;

		m_cameraOffset = new Vector2( 0, 0 );
		CameraZoom = 1;

		m_registeredMenus = new List<MenuParent>();

		m_nodeParametersWindow = new NodeParametersWindow();
		m_registeredMenus.Add( m_nodeParametersWindow );

		m_modeWindow = new ShaderEditorModeWindow();
		//_registeredMenus.Add( _modeWindow );

		m_toolsWindow = new ToolsWindow();
		m_toolsWindow.ToolButtonPressedEvt += OnToolButtonPressed;
		m_registeredMenus.Add( m_toolsWindow );

		m_palettePopup = new PalettePopUp();

		m_clipboard = new Clipboard();

		m_genericMessageContent = new GUIContent();
		m_dragAndDropTool = new DragAndDropTool();
		m_dragAndDropTool.OnValidDropObjectEvt += OnValidObjectsDropped;

		//_confirmationWindow = new ConfirmationWindow( 100, 100, 300, 100 );

		m_customStyles = new CustomStylesContainer();
		m_saveIsDirty = false;

		m_preMadeShaders = new PreMadeShaders();

		Undo.undoRedoPerformed += UndoRedoPerformed;

		float autoPanSpeed = 2;
		m_autoPanArea = new AutoPanData[ 4 ];
		m_autoPanArea[ 0 ] = new AutoPanData( AutoPanLocation.TOP, 25, autoPanSpeed * Vector2.up );
		m_autoPanArea[ 1 ] = new AutoPanData( AutoPanLocation.BOTTOM, 25, autoPanSpeed * Vector2.down );
		m_autoPanArea[ 2 ] = new AutoPanData( AutoPanLocation.LEFT, 25, autoPanSpeed * Vector2.right );
		m_autoPanArea[ 3 ] = new AutoPanData( AutoPanLocation.RIGHT, 25, autoPanSpeed * Vector2.left );

		m_drawInfo = new DrawInfo();
		UIUtils.CurrentWindow = this;

		m_repaintIsDirty = false;
		m_initialized = false;
	}

	void UndoRedoPerformed()
	{
		m_repaintIsDirty = true;
		m_saveIsDirty = true;
	}

	void Destroy()
	{
		m_initialized = false;
		IOUtils.Destroy();

		m_preMadeShaders.Destroy();
		m_preMadeShaders = null;

		m_customStyles = null;

		m_registeredMenus.Clear();
		m_registeredMenus = null;

		m_mainGraphInstance.Destroy();
		m_mainGraphInstance = null;

		Resources.UnloadAsset( MasterNodeOnTexture );
		MasterNodeOnTexture = null;

		Resources.UnloadAsset( MasterNodeOffTexture );
		MasterNodeOffTexture = null;

		Resources.UnloadAsset( GPUInstancedOnTexture );
		GPUInstancedOnTexture = null;

		Resources.UnloadAsset( GPUInstancedOffTexture );
		GPUInstancedOffTexture = null;

		Resources.UnloadAsset( m_graphBgTexture );
		m_graphBgTexture = null;

		Resources.UnloadAsset( m_graphFgTexture );
		m_graphFgTexture = null;

		Resources.UnloadAsset( m_wireTexture );
		m_wireTexture = null;

		m_contextMenu.Destroy();
		m_contextMenu = null;

		m_nodeParametersWindow.Destroy();
		m_nodeParametersWindow = null;

		m_modeWindow.Destroy();
		m_modeWindow = null;

		m_toolsWindow.Destroy();
		m_toolsWindow = null;

		m_paletteWindow.Destroy();
		m_paletteWindow = null;

		m_palettePopup.Destroy();
		m_palettePopup = null;

		m_contextPalette.Destroy();
		m_contextPalette = null;

		m_clipboard.ClearClipboard();
		m_clipboard = null;

		m_genericMessageUI.Destroy();
		m_genericMessageUI = null;
		m_genericMessageContent = null;

		m_dragAndDropTool = null;

		//_confirmationWindow.Destroy();
		//_confirmationWindow = null;
		UIUtils.CurrentWindow = null;
		m_duplicatePreventionBuffer.ReleaseAllData();
		m_duplicatePreventionBuffer = null;

		EditorApplication.projectWindowChanged -= OnProjectWindowChanged;
		Selection.selectionChanged -= OnProjectSelectionChanged;
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	void Init()
	{
		m_graphBgTexture = Resources.Load<Texture2D>( "UI/Canvas/Grid128" );
		if ( m_graphBgTexture != null )
		{
			m_graphFgTexture = Resources.Load<Texture2D>( "UI/Canvas/TransparentOverlay" );

			//Setup usable area
			m_cameraInfo = position;
			m_graphArea = new Rect( 0, 0, m_cameraInfo.width, m_cameraInfo.height );

			// Creating style state to show current selected object
			m_graphFontStyle = new GUIStyle();
			m_graphFontStyle.fontSize = 32;
			m_graphFontStyle.normal.textColor = Color.white;
			m_graphFontStyle.alignment = TextAnchor.MiddleCenter;
			m_graphFontStyle.fixedWidth = m_cameraInfo.width;
			m_graphFontStyle.fixedHeight = 50;
			m_graphFontStyle.stretchWidth = true;
			m_graphFontStyle.stretchHeight = true;

			m_wireTexture = Resources.Load<Texture2D>( "Nodes/Bezier1X2AA" );

			MasterNodeOnTexture = Resources.Load<Texture2D>( "Nodes/MasterNodeIconON" );
			MasterNodeOffTexture = Resources.Load<Texture2D>( "Nodes/MasterNodeIconOFF" );
			if ( MasterNodeOffTexture == null )
			{
				MasterNodeOffTexture = Resources.Load<Texture2D>( "Nodes/MasterNodeIcon" );
			}

			GPUInstancedOnTexture = Resources.Load<Texture2D>( "Nodes/GPUInstancingIconON" );
			GPUInstancedOffTexture = Resources.Load<Texture2D>( "Nodes/GPUInstancingIconOFF" );

			m_initialized = m_graphBgTexture != null &&
							m_graphFgTexture != null &&
							m_wireTexture != null &&
							MasterNodeOnTexture != null &&
							MasterNodeOffTexture != null &&
							GPUInstancedOnTexture != null &&
							GPUInstancedOffTexture != null;
		}
	}

	//[MenuItem( "Assets/Amplify Shader/Open Shader" )]
	//public static void UpdateShaderToASE()
	//{

	//	Shader selectedObj = Selection.activeObject as Shader;
	//	if ( selectedObj != null )
	//	{
	//		AmplifyShaderEditorWindow window = OpenMainShaderGraph();
	//		eShaderLoadResult result = window.LoadDroppedObject( true, selectedObj, null );
	//		if ( result == eShaderLoadResult.FILE_NOT_FOUND || result == eShaderLoadResult.UNITY_NATIVE_PATHS )
	//		{
	//			window.CreateNewGraph( selectedObj.name );
	//		}
	//	}
	//	else
	//	{
	//		Material mat = Selection.activeObject as Material;
	//		if ( mat )
	//		{
	//			AmplifyShaderEditorWindow window = OpenMainShaderGraph();
	//			eShaderLoadResult result = window.LoadDroppedObject( true, mat.shader, mat );
	//			if ( result == eShaderLoadResult.FILE_NOT_FOUND || result == eShaderLoadResult.UNITY_NATIVE_PATHS )
	//			{
	//				window.CreateNewGraph( mat.name );
	//				window.CurrentGraph.UpdateMaterialOnMasterNode( mat );
	//			}
	//		}
	//	}
	//}

	//[MenuItem( "Assets/DebugSelection" )]
	//public static void DebugSelections()
	//{
	//	string path = Selection.activeObject == null ? Application.dataPath : AssetDatabase.GetAssetPath( Selection.activeObject );
	//	if ( path.IndexOf( '.' ) > -1 )
	//	{
	//		path = path.Substring( 0, path.LastIndexOf( '/' ) );
	//		Debug.Log( "Selected object at " + path );
	//	}
	//	else
	//	{
	//		Debug.Log( "Selected folder at " + path );
	//	}
	//}

	[OnOpenAssetAttribute()]
	static bool OnOpenAsset( int instanceID, int line )
	{
		if ( line > -1 )
		{
			return false;
		}

		Shader selectedShader = Selection.activeObject as Shader;
		if ( selectedShader != null )
		{
			if ( IOUtils.IsASEShader( selectedShader ) )
			{
				if ( UIUtils.CurrentWindow == null )
				{
					OpenWindow();
					UIUtils.CurrentWindow.DelayedObjToLoad = Selection.activeObject;
				}
				else
				{
					UIUtils.CurrentWindow.LoadProjectSelected();
				}
				return true;
			}
		}
		else
		{
			Material mat = Selection.activeObject as Material;
			if ( mat != null )
			{
				if ( IOUtils.IsASEShader( mat.shader ) )
				{
					if ( UIUtils.CurrentWindow == null )
					{
						OpenWindow();
						UIUtils.CurrentWindow.DelayedObjToLoad = Selection.activeObject;
					}
					else
					{
						UIUtils.CurrentWindow.LoadProjectSelected();
					}
					return true;
				}
			}
		}
		return false;
	}


	[MenuItem( "Assets/Create/Shader/Amplify Surface Shader" )]
	public static void CreateNewShader()
	{
		string path = Selection.activeObject == null ? Application.dataPath : ( IOUtils.dataPath + AssetDatabase.GetAssetPath( Selection.activeObject ) );
		if ( path.IndexOf( '.' ) > -1 )
		{
			path = path.Substring( 0, path.LastIndexOf( '/' ) );
		}
		path += "/";
		OpenMainShaderGraph();
		Shader shader = UIUtils.CreateNewEmpty( path );
		Selection.activeObject = shader;
		Selection.objects = new UnityEngine.Object[] { shader };
	}

	[MenuItem( "Assets/Create/Shader/Amplify Post-Process Shader" )]
	public static void CreateNewPostProcess() { }

	[MenuItem( "Assets/Create/Shader/Amplify Post-Process Shader", true )]
	public static bool ValidateCreateNewPostProcess() { return false; }

	public void OnProjectWindowChanged()
	{
		Shader selectedShader = Selection.activeObject as Shader;
		if ( selectedShader != null )
		{
			if ( m_mainGraphInstance != null && selectedShader == m_mainGraphInstance.CurrentMasterNode.CurrentShader )
			{
				m_lastOpenedLocation = AssetDatabase.GetAssetPath( selectedShader );
			}
		}
	}

	public void LoadProjectSelected( UnityEngine.Object selectedObject = null )
	{
		if ( m_mainGraphInstance != null && m_mainGraphInstance.CurrentMasterNode != null )
		{
			LoadObject( selectedObject == null ? Selection.activeObject : selectedObject );
		}
		else
		{
			m_delayedLoadObject = selectedObject == null ? Selection.activeObject : selectedObject;
		}
	}

	public void LoadObject( UnityEngine.Object objToLoad )
	{
		Focus();
		Shader selectedShader = objToLoad as Shader;
		Shader currentShader = m_mainGraphInstance.CurrentMasterNode.CurrentShader;
		if ( selectedShader != null )
		{
			if ( m_mainGraphInstance.CurrentMasterNode != null &&
				!ShaderIsModified &&
				selectedShader == currentShader &&
				m_selectionMode == ASESelectionMode.Shader )
				return;

			if ( ShaderIsModified && ( selectedShader != currentShader ) )
			{
				//_confirmationWindow.ActivateConfirmation( selectedShader, null, "Save changes on previous shader?", OnSaveShader, true );
				bool savePrevious = UIUtils.DisplayDialog( AssetDatabase.GetAssetPath( currentShader ) );
				OnSaveShader( savePrevious, selectedShader, null );
			}
			else
			{
				LoadDroppedObject( true, selectedShader, null );
			}
		}
		else
		{
			Material selectedMaterial = objToLoad as Material;
			if ( selectedMaterial )
			{
				if ( m_selectionMode == ASESelectionMode.Material )
				{
					if ( !ShaderIsModified && selectedMaterial == m_mainGraphInstance.CurrentMasterNode.CurrentMaterial
						&& selectedMaterial.shader == m_mainGraphInstance.CurrentMasterNode.CurrentShader )
						return;

					if ( !ShaderIsModified && selectedMaterial.shader == m_mainGraphInstance.CurrentMasterNode.CurrentShader )
					{
						m_mainGraphInstance.UpdateMaterialOnMasterNode( selectedMaterial );
						return;
					}
				}

				if ( IOUtils.IsASEShader( selectedMaterial.shader ) )
				{
					if ( ShaderIsModified && ( selectedMaterial.shader != currentShader ) )
					{
						//_confirmationWindow.ActivateConfirmation( selectedMaterial.shader, selectedMaterial, "Save changes on previous shader?", OnSaveShader, true );
						bool savePrevious = UIUtils.DisplayDialog( AssetDatabase.GetAssetPath( currentShader ) );
						OnSaveShader( savePrevious, selectedMaterial.shader, selectedMaterial );
					}
					else
					{
						LoadDroppedObject( true, selectedMaterial.shader, selectedMaterial );
					}
				}
			}
		}

		ShaderIsModified = false;
		Repaint();
	}

	public void OnProjectSelectionChanged()
	{
		if ( m_loadShaderOnSelection )
		{
			LoadProjectSelected();
		}
	}

	ShaderLoadResult OnSaveShader( bool value, Shader shader, Material material )
	{
		if ( value )
		{
			SaveToDisk( false );
		}

		if ( shader != null || material != null )
		{
			LoadDroppedObject( true, shader, material );
		}

		return value ? ShaderLoadResult.LOADED : ShaderLoadResult.FILE_NOT_FOUND;
	}

	public void ResetCameraSettings()
	{
		m_cameraInfo = position;
		m_cameraOffset = new Vector2( m_cameraInfo.width * 0.5f, m_cameraInfo.height * 0.5f );
		CameraZoom = 1;
	}


	public void Reset()
	{
		m_toolsWindow.BorderStyle = null;
		m_selectionMode = ASESelectionMode.Shader;
		ResetCameraSettings();
		UIUtils.ResetMainSkin();
		m_duplicatePreventionBuffer.ReleaseAllData();
		if ( m_genericMessageUI != null )
			m_genericMessageUI.CleanUpMessageStack();
	}

	public Shader CreateNewGraph( string name )
	{
		Reset();
		UIUtils.DirtyMask = false;
		m_mainGraphInstance.CreateNewEmpty( name );
		m_lastOpenedLocation = string.Empty;
		UIUtils.DirtyMask = true;
		return m_mainGraphInstance.CurrentMasterNode.CurrentShader;
	}


	public Shader CreateNewGraph( Shader shader )
	{
		Reset();
		UIUtils.DirtyMask = false;
		m_mainGraphInstance.CreateNewEmpty( shader.name );
		m_mainGraphInstance.CurrentMasterNode.CurrentShader = shader;

		m_lastOpenedLocation = string.Empty;
		UIUtils.DirtyMask = true;
		return m_mainGraphInstance.CurrentMasterNode.CurrentShader;
	}


	public bool SaveToDisk( bool checkTimestamp )
	{
		if ( checkTimestamp )
		{
			if ( !m_cacheSaveOp )
			{
				m_lastTimeSaved = EditorApplication.timeSinceStartup;
				m_cacheSaveOp = true;
			}
			return false;
		}

		m_cacheSaveOp = false;
		ShaderIsModified = false;
		m_mainGraphInstance.LoadedShaderVersion = m_versionInfo.FullNumber;
		m_lastTimeSaved = EditorApplication.timeSinceStartup;

		if ( m_mainGraphInstance.CurrentMasterNodeId == Constants.INVALID_NODE_ID )
		{
			Shader currentShader = m_mainGraphInstance.CurrentMasterNode != null ? m_mainGraphInstance.CurrentMasterNode.CurrentShader : null;
			string newShader;
			if ( !String.IsNullOrEmpty( m_lastOpenedLocation ) )
			{
				newShader = m_lastOpenedLocation;
			}
			else if ( currentShader != null )
			{
				newShader = AssetDatabase.GetAssetPath( currentShader );
			}
			else
			{
				newShader = EditorUtility.SaveFilePanel( "Select Shader to save", Application.dataPath, "MyShader", "shader" );
			}

			if ( !String.IsNullOrEmpty( newShader ) )
			{
				ShowMessage( "No Master node assigned.\nShader file will only have node info" );
				IOUtils.StartSaveThread( GenerateGraphInfo(), newShader );
				//IOUtils.SaveTextfileToDisk( GenerateGraphInfo(), newShader );
				AssetDatabase.Refresh();
				LoadFromDisk( newShader );
				return true;
			}
		}
		else
		{
			Shader currShader = m_mainGraphInstance.CurrentMasterNode.CurrentShader;
			if ( currShader != null )
			{
				m_mainGraphInstance.FireMasterNode( currShader );
				Material material = m_mainGraphInstance.CurrentMaterial;
				EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, ( material != null ) ? AssetDatabase.GetAssetPath( material ) : AssetDatabase.GetAssetPath( currShader ) );
				return true;
			}
			else
			{
				string shaderName;
				string pathName;
				IOUtils.GetShaderName( out shaderName, out pathName, "MyNewShader", UIUtils.LatestOpenedFolder );
				if ( !String.IsNullOrEmpty( pathName ) )
				{
					UIUtils.CurrentWindow.CurrentGraph.CurrentMasterNode.SetName( shaderName );
					m_mainGraphInstance.FireMasterNode( pathName, true );
					EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, pathName );
					return true;
				}
			}
		}
		return false;
	}

	public void OnToolButtonPressed( ToolButtonType type )
	{
		switch ( type )
		{
			case ToolButtonType.New:
			{
				UIUtils.CreateNewEmpty();
			}
			break;
			case ToolButtonType.Open:
			{
				UIUtils.OpenFile();
			}
			break;
			case ToolButtonType.Save:
			{
				SaveToDisk( false );
			}
			break;
			case ToolButtonType.Library:
			{
				ShowShaderLibrary();
			}
			break;
			case ToolButtonType.Options:
			{
			}
			break;
			case ToolButtonType.Update:
			{

				SaveToDisk( false );
				//if ( _mainGraphInstance.CurrentMasterNode.CurrentShader == null )
				//{
				//	//ShowMessage( "UPDATE OFF: Open or create a new shader first." );
				//	SaveToDisk( false );
				//}
				//else
				//{
				//	//if ( _shaderIsModified )
				//	//{
				//	//	ShowMessage( _mainGraphInstance.CurrentMasterNode.CurrentMaterial != null ? "Shader and Material successfully updated." : "Shader successfully updated." );
				//	//}
				//	//else
				//	//{
				//	//	ShowMessage( _mainGraphInstance.CurrentMasterNode.CurrentMaterial != null ? "Shader and Material up-to-date." : "Shader up-to-date." );
				//	//}
				//	SaveToDisk( false );
				//}
			}
			break;
			case ToolButtonType.Live:
			{
				m_liveShaderEditing = !m_liveShaderEditing;
				// 0 off
				// 1 on
				// 2 pending
				if ( m_liveShaderEditing && m_mainGraphInstance.CurrentMasterNode.CurrentShader == null )
				{
					//ShowMessage( "LIVE PREVIEW OFF: Open or create a new shader first." );
					m_liveShaderEditing = false;
				}
				//else
				//{
				//	if ( _mainGraphInstance.CurrentMasterNode.CurrentMaterial != null )
				//	{
				//		if ( _liveShaderEditing )
				//			ShowMessage( "LIVE PREVIEW ON: Shader and Material." );
				//	}
				//	else
				//	{
				//		if ( _liveShaderEditing )
				//			ShowMessage( "LIVE PREVIEW ON:  Shader only, no material selected." );
				//	}

				//	if ( !_liveShaderEditing )
				//	{
				//		ShowMessage( "LIVE PREVIEW OFF" );
				//	}
				//}

				UpdateLiveUI();

				if ( m_liveShaderEditing )
				{
					SaveToDisk( false );
				}
			}
			break;
			case ToolButtonType.OpenSourceCode:
			{
				AssetDatabase.OpenAsset( m_mainGraphInstance.CurrentMasterNode.CurrentShader, 1 );
			}
			break;
			case ToolButtonType.MasterNode:
			{
				m_mainGraphInstance.AssignMasterNode();
			}
			break;

			case ToolButtonType.FocusOnMasterNode:
			{
				double currTime = EditorApplication.timeSinceStartup;
				bool autoZoom = ( currTime - m_focusOnMasterNodeTimestamp ) < m_autoZoomTime;
				m_focusOnMasterNodeTimestamp = currTime;

				//m_mainGraphInstance.SelectMasterNode();
				FocusOnNode( m_mainGraphInstance.CurrentMasterNode, autoZoom ? 1 : m_cameraZoom, true );
			}
			break;

			case ToolButtonType.FocusOnSelection:
			{

				List<ParentNode> selectedNodes = ( m_mainGraphInstance.SelectedNodes.Count > 0 ) ? m_mainGraphInstance.SelectedNodes : m_mainGraphInstance.AllNodes;

				Vector2 minPos = new Vector2( float.MaxValue, float.MaxValue );
				Vector2 maxPos = new Vector2( float.MinValue, float.MinValue );
				Vector2 centroid = Vector2.zero;

				//float nodeWidth = 0;
				//float nodeHeight = 0;

				for ( int i = 0; i < selectedNodes.Count; i++ )
				{
					Rect currPos = selectedNodes[ i ].Position;

					minPos.x = ( currPos.x < minPos.x ) ? currPos.x : minPos.x;
					minPos.y = ( currPos.y < minPos.y ) ? currPos.y : minPos.y;

					maxPos.x = ( ( currPos.x + currPos.width ) > maxPos.x ) ? ( currPos.x + currPos.width ) : maxPos.x;
					maxPos.y = ( ( currPos.y + currPos.height ) > maxPos.y ) ? ( currPos.y + currPos.height ) : maxPos.y;

				}
				centroid = ( maxPos - minPos );


				double currTime = EditorApplication.timeSinceStartup;
				bool autoZoom = ( currTime - m_focusOnSelectionTimestamp ) < m_autoZoomTime;
				m_focusOnSelectionTimestamp = currTime;

				float zoom = m_cameraZoom;
				if ( autoZoom )
				{
					zoom = 1f;
					float canvasWidth = AvailableCanvasWidth;
					float canvasHeight = AvailableCanvasHeight;
					if ( centroid.x > canvasWidth ||
						centroid.y > canvasHeight )
					{
						float hZoom = float.MinValue;
						float vZoom = float.MinValue;
						if ( centroid.x > canvasWidth )
						{
							hZoom = ( centroid.x ) / canvasWidth;
						}

						if ( centroid.y > canvasHeight )
						{
							vZoom = ( centroid.y ) / canvasHeight;
						}
						zoom = ( hZoom > vZoom ) ? hZoom : vZoom;
					}
				}

				FocusOnPoint( minPos + centroid * 0.5f, zoom );
			}
			break;
			case ToolButtonType.CleanUnusedNodes:
			{
				m_mainGraphInstance.CleanUnusedNodes();
			}
			break;
			//case eToolButtonType.SelectShader:
			//{
			//	Shader shader = _mainGraphInstance.CurrentMasterNode.CurrentShader;
			//	if ( shader != null )
			//	{
			//		Selection.activeObject = shader;
			//	}
			//}
			//break;
			case ToolButtonType.Help:
			{
				Application.OpenURL( Constants.HelpURL );
			}
			break;
		}
	}

	void UpdateLiveUI()
	{
		if ( m_toolsWindow != null )
		{
			m_toolsWindow.SetStateOnButton( ToolButtonType.Live, ( m_liveShaderEditing ) ? 1 : 0 );
		}
	}

	public void FocusOnNode( ParentNode node, float zoom, bool selectNode )
	{
		if ( selectNode )
		{
			m_mainGraphInstance.SelectNode( node, false, false );
		}
		FocusOnPoint( node.CenterPosition, zoom );
	}

	public void FocusOnPoint( Vector2 point, float zoom )
	{
		CameraZoom = zoom;
		m_cameraOffset = -point + new Vector2( ( m_cameraInfo.width + m_nodeParametersWindow.RealWidth - m_paletteWindow.RealWidth ) * 0.5f, m_cameraInfo.height * 0.5f ) * zoom;
	}

	void PreTestLeftMouseDown()
	{
		if ( m_currentEvent.type == EventType.mouseDown && m_currentEvent.button == ButtonClickId.LeftMouseButton )
		{
			ParentNode node = m_mainGraphInstance.CheckNodeAt( m_currentMousePos );
			if ( node != null )
			{
				m_mainGraphInstance.NodeClicked = node.UniqueId;
				return;
			}
		}

		m_mainGraphInstance.NodeClicked = -1;
	}
	void OnLeftMouseDown()
	{
		Focus();
		m_mouseDownOnValidArea = true;
		m_lmbPressed = true;
		ParentNode node = ( m_mainGraphInstance.NodeClicked < 0 ) ? m_mainGraphInstance.CheckNodeAt( m_currentMousePos ) : m_mainGraphInstance.GetClickedNode();
		if ( node != null )
		{
			m_mainGraphInstance.NodeClicked = node.UniqueId;

			if ( m_contextMenu.CheckShortcutKey() )
			{
				if ( node.ConnStatus == NodeConnectionStatus.Island )
				{
					if ( !m_multipleSelectionActive )
					{
						ParentNode newNode = m_contextMenu.CreateNodeFromShortcutKey();
						if ( newNode != null )
						{
							newNode.Vec2Position = TranformedMousePos;
							m_mainGraphInstance.AddNode( newNode, true );
							ForceRepaint();
						}
						( node as CommentaryNode ).AddNodeToCommentary( newNode );
					}
				}
			}
			else
			{
				if ( !node.Selected )
				{
					m_mainGraphInstance.SelectNode( node, m_currentEvent.modifiers == EventModifiers.Shift, true );
				}
				else if ( m_currentEvent.modifiers == EventModifiers.Shift )
				{
					m_mainGraphInstance.DeselectNode( node );
				}
				return;
			}
		}
		else if ( !m_multipleSelectionActive )
		{
			ParentNode newNode = m_contextMenu.CreateNodeFromShortcutKey();
			if ( newNode != null )
			{
				newNode.Vec2Position = TranformedMousePos;
				m_mainGraphInstance.AddNode( newNode, true );
				ForceRepaint();
			}
			else
			{
				//Reset focus from any textfield which may be selected at this time
				GUIUtility.keyboardControl = 0;
			}
		}

		if ( m_currentEvent.modifiers != EventModifiers.Shift )
			m_mainGraphInstance.DeSelectAll();

		if ( UIUtils.ValidReferences() )
		{
			UIUtils.InvalidateReferences();
			return;
		}

		if ( !m_contextMenu.CheckShortcutKey() )
		{
			// Only activate multiple selection if no node is selected
			m_multipleSelectionActive = true;

			m_multipleSelectionStart = TranformedMousePos;
			m_multipleSelectionArea.position = m_multipleSelectionStart;
			m_multipleSelectionArea.size = Vector2.zero;
		}
		UseCurrentEvent();
	}

	void OnLeftMouseDrag()
	{
		if ( m_lostFocus )
		{
			m_lostFocus = false;
			return;
		}

		if ( !UIUtils.ValidReferences() )
		{
			if ( m_mouseDownOnValidArea && m_insideEditorWindow )
			{
				m_mainGraphInstance.MoveSelectedNodes( m_cameraZoom * m_currentEvent.delta );
				m_autoPanDirActive = true;
			}
		}
		else
		{
			List<ParentNode> nodes = m_mainGraphInstance.GetNodesInGrid( m_drawInfo.TransformedMousePos );
			if ( nodes != null && nodes.Count > 0 )
			{
				Vector2 currentPortPos = new Vector2();
				Vector2 mousePos = TranformedMousePos;

				if ( UIUtils.InputPortReference.IsValid )
				{
					OutputPort currentPort = null;
					float smallestDistance = float.MaxValue;
					Vector2 smallestPosition = Vector2.zero;
					for ( int nodeIdx = 0; nodeIdx < nodes.Count; nodeIdx++ )
					{
						List<OutputPort> outputPorts = nodes[ nodeIdx ].OutputPorts;
						if ( outputPorts != null )
						{
							for ( int o = 0; o < outputPorts.Count; o++ )
							{
								currentPortPos.x = outputPorts[ o ].Position.x;
								currentPortPos.y = outputPorts[ o ].Position.y;

								currentPortPos = currentPortPos * m_cameraZoom - m_cameraOffset;
								float dist = ( mousePos - currentPortPos ).sqrMagnitude;
								if ( dist < smallestDistance )
								{
									smallestDistance = dist;
									smallestPosition = currentPortPos;
									currentPort = outputPorts[ o ];
								}
							}
						}
					}

					if ( currentPort != null && ( smallestDistance < Constants.SNAP_SQR_DIST || currentPort.InsideActiveArea( ( mousePos + m_cameraOffset ) / m_cameraZoom ) ) )
					{
						UIUtils.ActivateSnap( smallestPosition, currentPort );
					}
					else
					{
						UIUtils.DeactivateSnap();
					}
				}

				if ( UIUtils.OutputPortReference.IsValid )
				{
					InputPort currentPort = null;
					float smallestDistance = float.MaxValue;
					Vector2 smallestPosition = Vector2.zero;
					for ( int nodeIdx = 0; nodeIdx < nodes.Count; nodeIdx++ )
					{
						List<InputPort> inputPorts = nodes[ nodeIdx ].InputPorts;
						if ( inputPorts != null )
						{
							for ( int i = 0; i < inputPorts.Count; i++ )
							{
								currentPortPos.x = inputPorts[ i ].Position.x;
								currentPortPos.y = inputPorts[ i ].Position.y;

								currentPortPos = currentPortPos * m_cameraZoom - m_cameraOffset;
								float dist = ( mousePos - currentPortPos ).sqrMagnitude;
								if ( dist < smallestDistance )
								{
									smallestDistance = dist;
									smallestPosition = currentPortPos;
									currentPort = inputPorts[ i ];
								}
							}
						}
					}
					if ( currentPort != null && ( smallestDistance < Constants.SNAP_SQR_DIST || currentPort.InsideActiveArea( ( mousePos + m_cameraOffset ) / m_cameraZoom ) ) )
					{
						UIUtils.ActivateSnap( smallestPosition, currentPort );
					}
					else
					{
						UIUtils.DeactivateSnap();
					}
				}
			}
			else if ( UIUtils.SnapEnabled )
			{
				UIUtils.DeactivateSnap();
			}
		}
		UseCurrentEvent();
	}

	void OnLeftMouseUp()
	{
		m_lmbPressed = false;
		if ( m_multipleSelectionActive )
		{
			m_multipleSelectionActive = false;
			UpdateSelectionArea();
			m_mainGraphInstance.MultipleSelection( m_multipleSelectionArea, m_currentEvent.modifiers == EventModifiers.Shift, true );
		}

		if ( UIUtils.ValidReferences() )
		{
			//Check if there is some kind of port beneath the mouse ... if so connect to it
			ParentNode targetNode = UIUtils.SnapEnabled ? m_mainGraphInstance.GetNode( UIUtils.SnapPort.NodeId ) : m_mainGraphInstance.CheckNodeAt( m_currentMousePos );
			if ( targetNode != null && targetNode.ConnStatus != NodeConnectionStatus.Island )
			{
				if ( UIUtils.InputPortReference.IsValid && UIUtils.InputPortReference.NodeId != targetNode.UniqueId )
				{
					OutputPort outputPort = UIUtils.SnapEnabled ? targetNode.GetOutputPortById( UIUtils.SnapPort.PortId ) : targetNode.CheckOutputPortAt( m_currentMousePos );
					if ( outputPort != null && ( !UIUtils.InputPortReference.TypeLocked ||
												UIUtils.InputPortReference.DataType == WirePortDataType.OBJECT ||
												( UIUtils.InputPortReference.TypeLocked && outputPort.DataType == UIUtils.InputPortReference.DataType ) ) )
					{
						ParentNode originNode = m_mainGraphInstance.GetNode( UIUtils.InputPortReference.NodeId );
						InputPort inputPort = originNode.GetInputPortById( UIUtils.InputPortReference.PortId );

						inputPort.DummyAdd( outputPort.NodeId, outputPort.PortId );
						outputPort.DummyAdd( UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId );

						if ( UIUtils.DetectNodeLoopsFrom( originNode, new Dictionary<int, int>() ) )
						{
							inputPort.DummyRemove();
							outputPort.DummyRemove();
							UIUtils.InvalidateReferences();
							ShowMessage( "Infinite Loop detected" );
							UseCurrentEvent();
							return;
						}

						inputPort.DummyRemove();
						outputPort.DummyRemove();

						if ( inputPort.IsConnected )
						{
							DeleteConnection( true, UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId, true );
						}

						//link output to input
						if ( outputPort.ConnectTo( UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId, UIUtils.InputPortReference.DataType, UIUtils.InputPortReference.TypeLocked ) )
							targetNode.OnOutputPortConnected( outputPort.PortId, UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId );

						//link input to output
						if ( inputPort.ConnectTo( outputPort.NodeId, outputPort.PortId, outputPort.DataType, UIUtils.InputPortReference.TypeLocked ) )
							originNode.OnInputPortConnected( UIUtils.InputPortReference.PortId, targetNode.UniqueId, outputPort.PortId );
						m_mainGraphInstance.MarkWireHighlights();
					}
					else if ( outputPort != null && UIUtils.InputPortReference.TypeLocked && UIUtils.InputPortReference.DataType != outputPort.DataType )
					{
						ShowMessage( "Attempting to connect a port locked to type " + UIUtils.InputPortReference.DataType + " into a port of type " + outputPort.DataType );
					}
					ShaderIsModified = true;
				}

				if ( UIUtils.OutputPortReference.IsValid && UIUtils.OutputPortReference.NodeId != targetNode.UniqueId )
				{
					InputPort inputPort = UIUtils.SnapEnabled ? targetNode.GetInputPortById( UIUtils.SnapPort.PortId ) : targetNode.CheckInputPortAt( m_currentMousePos );
					if ( inputPort != null && ( !inputPort.TypeLocked ||
												 inputPort.DataType == WirePortDataType.OBJECT ||
												 ( inputPort.TypeLocked && inputPort.DataType == UIUtils.OutputPortReference.DataType ) ) )
					{
						ParentNode originNode = m_mainGraphInstance.GetNode( UIUtils.OutputPortReference.NodeId );
						OutputPort outputPort = originNode.GetOutputPortById( UIUtils.OutputPortReference.PortId );
						inputPort.DummyAdd( UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId );
						outputPort.DummyAdd( inputPort.NodeId, inputPort.PortId );
						if ( UIUtils.DetectNodeLoopsFrom( targetNode, new Dictionary<int, int>() ) )
						{
							inputPort.DummyRemove();
							outputPort.DummyRemove();
							UIUtils.InvalidateReferences();
							ShowMessage( "Infinite Loop detected" );
							UseCurrentEvent();
							return;
						}

						inputPort.DummyRemove();
						outputPort.DummyRemove();

						if ( inputPort.IsConnected )
						{
							DeleteConnection( true, inputPort.NodeId, inputPort.PortId, true );
						}
						inputPort.InvalidateAllConnections();


						//link input to output
						if ( inputPort.ConnectTo( UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId, UIUtils.OutputPortReference.DataType, inputPort.TypeLocked ) )
							targetNode.OnInputPortConnected( inputPort.PortId, UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId );
						//link output to input

						if ( outputPort.ConnectTo( inputPort.NodeId, inputPort.PortId, inputPort.DataType, inputPort.TypeLocked ) )
							originNode.OnOutputPortConnected( UIUtils.OutputPortReference.PortId, targetNode.UniqueId, inputPort.PortId );
						m_mainGraphInstance.MarkWireHighlights();
					}
					else if ( inputPort != null && inputPort.TypeLocked && inputPort.DataType != UIUtils.OutputPortReference.DataType )
					{
						ShowMessage( "Attempting to connect a " + UIUtils.OutputPortReference.DataType + "to a port locked to type " + inputPort.DataType );
					}
					ShaderIsModified = true;
				}
				UIUtils.InvalidateReferences();
			}
			else
			{
				//_contextMenu.Show(_currentMousePos2D, _cameraOffset, _cameraZoom);
				m_contextPalette.Show( m_currentMousePos2D, m_cameraInfo );
			}
		}
		UseCurrentEvent();
	}

	public void ConnectInputToOutput( int inNodeId, int inPortId, int outNodeId, int outPortId )
	{
		ParentNode inNode = m_mainGraphInstance.GetNode( inNodeId );
		ParentNode outNode = m_mainGraphInstance.GetNode( outNodeId );
		if ( inNode != null && outNode != null )
		{
			InputPort inPort = inNode.GetInputPortById( inPortId );
			OutputPort outPort = outNode.GetOutputPortById( outPortId );
			if ( inPort != null && outPort != null )
			{
				if ( inPort.ConnectTo( outNodeId, outPortId, inPort.DataType, inPort.TypeLocked ) )
				{
					inNode.OnInputPortConnected( inPortId, outNodeId, outPortId );
				}

				if ( outPort.ConnectTo( inNodeId, inPortId, inPort.DataType, inPort.TypeLocked ) )
				{
					outNode.OnOutputPortConnected( outPortId, inNodeId, inPortId );
				}
			}
			m_mainGraphInstance.MarkWireHighlights();
			ShaderIsModified = true;
		}
	}

	void OnRightMouseDown()
	{
		Focus();
		m_rmbStartPos = m_currentMousePos2D;
		UseCurrentEvent();
	}

	void OnRightMouseDrag()
	{
		m_cameraOffset += m_cameraZoom * m_currentEvent.delta;
		UseCurrentEvent();
	}

	void OnRightMouseUp()
	{
		if ( ( m_rmbStartPos - m_currentMousePos2D ).sqrMagnitude < Constants.RMB_SCREEN_DIST )
		{
			ParentNode node = m_mainGraphInstance.CheckNodeAt( m_currentMousePos, true );
			if ( node == null )
			{
				//_contextMenu.Show(_currentMousePos2D, _cameraOffset, _cameraZoom);
				m_contextPalette.Show( m_currentMousePos2D, m_cameraInfo );
			}
		}
		UseCurrentEvent();
	}

	void UpdateSelectionArea()
	{
		m_multipleSelectionArea.size = TranformedMousePos - m_multipleSelectionStart;
	}

	public void OnValidObjectsDropped( UnityEngine.Object[] droppedObjs )
	{
		bool propagateDraggedObjsToNode = true;
		// Only supporting single drag&drop object selection
		if ( droppedObjs.Length == 1 )
		{
			ShaderIsModified = true;
			// Check if its a shader, material or game object  and if so load the shader graph code from it
			Shader newShader = droppedObjs[ 0 ] as Shader;
			Material newMaterial = null;
			if ( newShader == null )
			{
				newMaterial = droppedObjs[ 0 ] as Material;
				if ( newMaterial != null )
				{
					newShader = newMaterial.shader;
					m_mainGraphInstance.UpdateMaterialOnMasterNode( newMaterial );
				}
				else
				{
					GameObject go = droppedObjs[ 0 ] as GameObject;
					if ( go != null )
					{
						Renderer renderer = go.GetComponent<Renderer>();
						if ( renderer )
						{
							newMaterial = renderer.sharedMaterial;
							newShader = newMaterial.shader;
						}
					}
				}
			}

			if ( newShader != null )
			{
				bool savePrevious = false;
				if ( ShaderIsModified )
				{
					Shader currentShader = m_mainGraphInstance.CurrentShader;
					savePrevious = UIUtils.DisplayDialog( AssetDatabase.GetAssetPath( currentShader ) );
				}
				OnSaveShader( savePrevious, newShader, newMaterial );
				//OnValidShaderFound( currentShader, currentMaterial );
				propagateDraggedObjsToNode = false;
			}

			// if not shader loading then propagate the seletion to whats bellow the mouse
			if ( propagateDraggedObjsToNode )
			{
				ParentNode node = m_mainGraphInstance.CheckNodeAt( m_currentMousePos );
				if ( node != null )
				{
					// if there's a node then pass the object into it to see if there's a setup with it
					node.OnObjectDropped( droppedObjs[ 0 ] );
				}
				else
				{
					// If not then check if there's a node that can be created through the dropped object
					ParentNode newNode = m_contextMenu.CreateNodeFromCastType( droppedObjs[ 0 ].GetType() );
					if ( newNode )
					{
						newNode.Vec2Position = TranformedMousePos;
						m_mainGraphInstance.AddNode( newNode, true );
						newNode.SetupFromCastObject( droppedObjs[ 0 ] );
						ForceRepaint();
					}
				}
			}
		}
	}


	public void SetDelayedMaterialMode( Material material )
	{
		if ( material == null )
			return;
		m_delayedMaterialSet = material;
	}

	//public void OnValidShaderFound( Shader shader, Material material )
	//{
	//	_confirmationWindow.ActivateConfirmation( shader, material, "Loading new shader. Are you sure?", LoadDroppedObject, false );
	//}

	public ShaderLoadResult LoadDroppedObject( bool value, Shader shader, Material material )
	{
		ShaderLoadResult result;
		if ( value && shader != null )
		{
			string assetDatapath = AssetDatabase.GetAssetPath( shader );
			string latestOpenedFolder = Application.dataPath + assetDatapath.Substring( 6 );
			UIUtils.LatestOpenedFolder = latestOpenedFolder.Substring( 0, latestOpenedFolder.LastIndexOf( '/' ) + 1 );
			result = LoadFromDisk( assetDatapath );
			switch ( result )
			{
				case ShaderLoadResult.LOADED:
				{
					m_mainGraphInstance.UpdateShaderOnMasterNode( shader );
					//_confirmationWindow.Deactivate();
				}
				break;
				case ShaderLoadResult.ASE_INFO_NOT_FOUND:
				{
					//_confirmationWindow.ActivateConfirmation( shader, material, "Loaded shader wasn't created with ASE. Using it will remove existing content.Are you sure?\n", OnLoadingNonASEShader, false );
					ShowMessage( "Loaded shader wasn't created with ASE. Saving it will remove previous data." );
					UIUtils.CreateEmptyFromInvalid( shader );
				}
				break;
				case ShaderLoadResult.FILE_NOT_FOUND:
				case ShaderLoadResult.UNITY_NATIVE_PATHS:
				{
					//_confirmationWindow.Deactivate();
					UIUtils.CreateEmptyFromInvalid( shader );
				}
				break;
			}

			m_mainGraphInstance.UpdateMaterialOnMasterNode( material );
			m_mainGraphInstance.SetMaterialModeOnGraph( material );

			if ( material != null )
			{
				CurrentSelection = ASESelectionMode.Material;
				if ( material.HasProperty( IOUtils.DefaultASEDirtyCheckId ) )
				{
					material.SetInt( IOUtils.DefaultASEDirtyCheckId, 1 );
				}
				EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, AssetDatabase.GetAssetPath( material ) );
			}
			else
			{
				CurrentSelection = ASESelectionMode.Shader;
				EditorPrefs.SetString( IOUtils.LAST_OPENED_OBJ_ID, AssetDatabase.GetAssetPath( shader ) );
			}
		}
		else
		{
			//_confirmationWindow.Deactivate();
			result = ShaderLoadResult.FILE_NOT_FOUND;
		}
		return result;
	}

	//eShaderLoadResult OnLoadingNonASEShader( bool value, Shader shader, Material material )
	//{
	//	if ( value )
	//		InjectNonASEShaderIntoGraph( shader );

	//	_confirmationWindow.Deactivate();
	//	return value ? eShaderLoadResult.LOADED : eShaderLoadResult.FILE_NOT_FOUND;
	//}

	//void InjectNonASEShaderIntoGraph( Shader shader )
	//{
	//	_mainGraphInstance.UpdateShaderOnMasterNode( shader );
	//}

	bool InsideMenus( Vector2 position )
	{
		for ( int i = 0; i < m_registeredMenus.Count; i++ )
		{
			if ( m_registeredMenus[ i ].IsInside( position ) )
			{
				return true;
			}
		}
		return false;
	}

	void HandleGUIEvents()
	{
		if ( m_currentEvent.type == EventType.KeyDown )
		{
			m_contextMenu.UpdateKeyPress( m_currentEvent.keyCode );
		}
		else if ( m_currentEvent.type == EventType.keyUp )
		{
			m_contextMenu.UpdateKeyReleased( m_currentEvent.keyCode );
		}

		//if ( _confirmationWindow.IsActive )
		//{
		//	_confirmationWindow.OnGUI();
		//	return;
		//}
		if ( InsideMenus( m_currentMousePos2D ) )
		{
			if ( m_currentEvent.type == EventType.mouseDown )
			{
				m_mouseDownOnValidArea = false;
				UseCurrentEvent();
			}
			return;
		}

		int controlID = GUIUtility.GetControlID( FocusType.Passive );
		switch ( m_currentEvent.GetTypeForControl( controlID ) )
		{
			case EventType.MouseDown:
			{
				GUIUtility.hotControl = controlID;
				switch ( m_currentEvent.button )
				{
					case ButtonClickId.LeftMouseButton:
					{
						OnLeftMouseDown();
					}
					break;
					case ButtonClickId.RightMouseButton:
					case ButtonClickId.MiddleMouseButton:
					{
						OnRightMouseDown();
					}
					break;
				}
			}
			break;

			case EventType.MouseUp:
			{
				GUIUtility.hotControl = 0;
				switch ( m_currentEvent.button )
				{
					case ButtonClickId.LeftMouseButton:
					{
						OnLeftMouseUp();
					}
					break;
					case ButtonClickId.MiddleMouseButton: break;
					case ButtonClickId.RightMouseButton:
					{
						OnRightMouseUp();
					}
					break;
				}
			}
			break;
			case EventType.MouseDrag:
			{
				switch ( m_currentEvent.button )
				{
					case ButtonClickId.LeftMouseButton:
					{
						OnLeftMouseDrag();
					}
					break;
					case ButtonClickId.MiddleMouseButton:
					case ButtonClickId.RightMouseButton:
					{
						OnRightMouseDrag();
					}
					break;
				}
			}
			break;
			case EventType.ScrollWheel:
			{
				OnScrollWheel();
			}
			break;
			case EventType.keyDown:
			{
				OnKeyboardDown();
			}
			break;
			case EventType.keyUp:
			{
				OnKeyboardUp();
			}
			break;
			case EventType.ExecuteCommand:
			case EventType.ValidateCommand:
			{
				switch ( m_currentEvent.commandName )
				{
					case CopyCommand: CopyToClipboard(); break;
					case PasteCommand: PasteFromClipboard( true ); break;
					case SelectAll:
					{
						m_mainGraphInstance.SelectAll();
						ForceRepaint();
					}
					break;
					case Duplicate:
					{
						CopyToClipboard();
						PasteFromClipboard( true );
					}
					break;
				}
			}
			break;
			case EventType.Repaint:
			{
			}
			break;
		}

		m_dragAndDropTool.TestDragAndDrop( m_graphArea );

		//if ( _currentEvent.modifiers == EventModifiers.Shift )
		//{
		//	GUI.Label( new Rect( _currentMousePos2D, new Vector2( 200, 200 ) ), _currentMousePos2D.ToString() );
		//}
	}

	public void DeleteConnection( bool isInput, int nodeId, int portId, bool registerOnLog )
	{
		m_mainGraphInstance.DeleteConnection( isInput, nodeId, portId, registerOnLog );
	}

	void DeleteSelectedNodes()
	{
		m_mainGraphInstance.DeleteSelectedNodes();
		ForceRepaint();
	}

	void OnKeyboardUp()
	{
		m_lastKeyPressed = KeyCode.None;
	}

	bool OnKeyboardPress( KeyCode code )
	{
		return ( m_currentEvent.keyCode == code && m_lastKeyPressed == KeyCode.None );
	}

	void OnKeyboardDown()
	{
		if ( m_currentEvent.keyCode == KeyCode.F1 )
		{
			m_duplicatePreventionBuffer.DumpUniformNames();
		}

		if ( OnKeyboardPress( KeyCode.C ) )
		{
			// Create commentary
			CommentaryNode node = m_mainGraphInstance.CreateNode( m_commentaryTypeNode, true, -1, false ) as CommentaryNode;
			node.CreateFromSelectedNodes( TranformedMousePos, m_mainGraphInstance.SelectedNodes );
			m_mainGraphInstance.DeSelectAll();
			m_mainGraphInstance.SelectNode( node, false, false );
			ForceRepaint();
		}

		if ( OnKeyboardPress( KeyCode.F ) )
		{
			OnToolButtonPressed( ToolButtonType.FocusOnSelection );
			ForceRepaint();
		}

		if ( OnKeyboardPress( KeyCode.Space ) )
		{
			m_contextPalette.Show( m_currentMousePos2D, m_cameraInfo );
		}

		if ( m_currentEvent.control && m_currentEvent.shift && m_currentEvent.keyCode == KeyCode.V )
		{
			PasteFromClipboard( false );
		}

		//if ( _currentEvent.keyCode == KeyCode.W && _currentEvent.modifiers == EventModifiers.Control )
		//{
		//	CopyToClipboard();
		//	PasteFromClipboard( true );
		//}

		if ( m_currentEvent.keyCode == KeyCode.Delete || m_currentEvent.keyCode == KeyCode.Backspace )
		{
			DeleteSelectedNodes();
			//_repaintIsDirty = true;
			ForceRepaint();
		}

		if ( m_currentEvent.keyCode != KeyCode.None )
			m_lastKeyPressed = m_currentEvent.keyCode;
	}

	void OnScrollWheel()
	{
		float minCam = Mathf.Min( ( m_cameraInfo.width - ( m_nodeParametersWindow.RealWidth + m_paletteWindow.RealWidth ) ), ( m_cameraInfo.height - ( m_toolsWindow.Height ) ) );
		if ( minCam < 1 )
			minCam = 1;

		float dynamicMaxZoom = m_mainGraphInstance.MaxNodeDist / minCam;

		Vector2 canvasPos = TranformedMousePos;
		CameraZoom = Mathf.Clamp( m_cameraZoom + m_currentEvent.delta.y * Constants.CAMERA_ZOOM_SPEED, Constants.CAMERA_MIN_ZOOM, Mathf.Max( Constants.CAMERA_MAX_ZOOM, dynamicMaxZoom ) );
		m_cameraOffset.x = m_currentMousePos2D.x * m_cameraZoom - canvasPos.x;
		m_cameraOffset.y = m_currentMousePos2D.y * m_cameraZoom - canvasPos.y;

		UseCurrentEvent();
	}

	void OnSelectionChange()
	{
		//_repaintIsDirty = true;
		ForceRepaint();
	}

	void OnLostFocus()
	{

		m_lostFocus = true;
		m_multipleSelectionActive = false;
		UIUtils.InvalidateReferences();
		m_genericMessageUI.CleanUpMessageStack();
	}

	void CopyToClipboard()
	{
		m_copyPasteDeltaMul = 0;
		m_copyPasteDeltaPos = new Vector2( float.MaxValue, float.MaxValue );
		m_clipboard.ClearClipboard();
		m_clipboard.AddToClipboard( m_mainGraphInstance.SelectedNodes );
		m_copyPasteInitialPos = m_mainGraphInstance.SelectedNodesCentroid;
	}

	ParentNode CreateNodeFromClipboardData( int clipId )
	{
		string[] parameters = m_clipboard.CurrentClipboardStrData[ clipId ].Data.Split( IOUtils.FIELD_SEPARATOR );
		ParentNode newNode = ( ParentNode ) ScriptableObject.CreateInstance( Type.GetType( parameters[ IOUtils.NodeTypeId ] ) );
		if ( newNode != null )
		{
			try
			{
				newNode.ReadFromString( ref parameters );
				newNode.ReadInputDataFromString( ref parameters );
			}
			catch ( Exception e )
			{
				Debug.LogError( e );
			}

			newNode.ReleaseUniqueIdData();
			m_mainGraphInstance.AddNode( newNode, true );
			m_clipboard.CurrentClipboardStrData[ clipId ].NewNodeId = newNode.UniqueId;
			return newNode;
		}
		return null;
	}

	void CreateConnectionsFromClipboardData( int clipId )
	{
		if ( String.IsNullOrEmpty( m_clipboard.CurrentClipboardStrData[ clipId ].Connections ) )
			return;
		string[] lines = m_clipboard.CurrentClipboardStrData[ clipId ].Connections.Split( IOUtils.LINE_TERMINATOR );

		// last line is always an empty one
		for ( int lineIdx = 0; lineIdx < lines.Length - 1; lineIdx++ )
		{
			string[] parameters = lines[ lineIdx ].Split( IOUtils.FIELD_SEPARATOR );

			int InNodeId = 0;
			int InPortId = 0;
			int OutNodeId = 0;
			int OutPortId = 0;

			try
			{
				InNodeId = Convert.ToInt32( parameters[ IOUtils.InNodeId ] );
				InPortId = Convert.ToInt32( parameters[ IOUtils.InPortId ] );

				OutNodeId = Convert.ToInt32( parameters[ IOUtils.OutNodeId ] );
				OutPortId = Convert.ToInt32( parameters[ IOUtils.OutPortId ] );
			}
			catch ( Exception e )
			{
				Debug.LogError( e );
			}


			int newInNodeId = m_clipboard.GeNewNodeId( InNodeId );
			int newOutNodeId = m_clipboard.GeNewNodeId( OutNodeId );

			if ( newInNodeId > -1 && newOutNodeId > -1 )
			{
				ParentNode inNode = m_mainGraphInstance.GetNode( newInNodeId );
				ParentNode outNode = m_mainGraphInstance.GetNode( newOutNodeId );

				InputPort inputPort = null;
				OutputPort outputPort = null;

				if ( inNode != null && outNode != null )
				{
					inputPort = inNode.GetInputPortById( InPortId );
					outputPort = outNode.GetOutputPortById( OutPortId );
					if ( inputPort != null && outputPort != null )
					{
						inputPort.ConnectTo( newOutNodeId, OutPortId, outputPort.DataType, false );
						outputPort.ConnectTo( newInNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked );

						inNode.OnInputPortConnected( InPortId, newOutNodeId, OutPortId );
						outNode.OnOutputPortConnected( OutPortId, newInNodeId, InPortId );
					}
				}
			}
		}
	}

	void PasteFromClipboard( bool copyConnections )
	{
		if ( m_clipboard.CurrentClipboardStrData.Count == 0 )
		{
			return;
		}

		Vector2 deltaPos = TranformedKeyEvtMousePos - m_copyPasteInitialPos;
		if ( ( m_copyPasteDeltaPos - deltaPos ).magnitude > 5.0f )
		{
			m_copyPasteDeltaMul = 0;
		}
		else
		{
			m_copyPasteDeltaMul += 1;
		}
		m_copyPasteDeltaPos = deltaPos;

		m_mainGraphInstance.DeSelectAll();
		UIUtils.InhibitMessages = true;
		for ( int i = 0; i < m_clipboard.CurrentClipboardStrData.Count; i++ )
		{
			ParentNode node = CreateNodeFromClipboardData( i );
			m_clipboard.CurrentClipboardStrData[ i ].NewNodeId = node.UniqueId;
			Vector2 pos = node.Vec2Position;
			node.Vec2Position = pos + deltaPos + m_copyPasteDeltaMul * Constants.CopyPasteDeltaPos;
			m_mainGraphInstance.SelectNode( node, true, false );
		}

		if ( copyConnections )
		{
			for ( int i = 0; i < m_clipboard.CurrentClipboardStrData.Count; i++ )
			{
				CreateConnectionsFromClipboardData( i );
			}
		}

		UIUtils.InhibitMessages = false;
		ShaderIsModified = true;
		ForceRepaint();
	}

	public string GenerateGraphInfo()
	{
		string graphInfo = IOUtils.ShaderBodyBegin + '\n';
		string nodesInfo = "";
		string connectionsInfo = "";
		graphInfo += m_versionInfo.FullLabel + '\n';
		graphInfo += (
						m_cameraInfo.x.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraInfo.y.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraInfo.width.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraInfo.height.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraOffset.x.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraOffset.y.ToString() + IOUtils.FIELD_SEPARATOR +
						m_cameraZoom.ToString() + IOUtils.FIELD_SEPARATOR +
						m_nodeParametersWindow.IsMaximized + IOUtils.FIELD_SEPARATOR +
						m_paletteWindow.IsMaximized + '\n'
						);
		m_mainGraphInstance.WriteToString( ref nodesInfo, ref connectionsInfo );
		graphInfo += nodesInfo;
		graphInfo += connectionsInfo;
		graphInfo += IOUtils.ShaderBodyEnd + '\n';

		return graphInfo;
	}


	public ShaderLoadResult LoadFromDisk( string pathname )
	{
		UIUtils.DirtyMask = false;
		if ( UIUtils.IsUnityNativeShader( pathname ) )
		{
			ShowMessage( "Cannot edit native unity shaders.\nReplacing by a new one." );
			return ShaderLoadResult.UNITY_NATIVE_PATHS;
		}

		m_lastOpenedLocation = pathname;
		string buffer = IOUtils.LoadTextFileFromDisk( pathname );
		if ( String.IsNullOrEmpty( buffer ) )
		{
			ShowMessage( "Could not open file " + pathname );
			return ShaderLoadResult.FILE_NOT_FOUND;
		}

		if ( !IOUtils.HasValidShaderBody( ref buffer ) )
		{
			return ShaderLoadResult.ASE_INFO_NOT_FOUND;
		}

		m_mainGraphInstance.CleanNodes();
		Reset();

		ShaderLoadResult loadResult = ShaderLoadResult.LOADED;
		// Find checksum value on body
		int checksumId = buffer.IndexOf( IOUtils.CHECKSUM );
		if ( checksumId > -1 )
		{
			string checkSumStoredValue = buffer.Substring( checksumId );
			string trimmedBuffer = buffer.Remove( checksumId );

			string[] typeValuePair = checkSumStoredValue.Split( IOUtils.VALUE_SEPARATOR );
			if ( typeValuePair != null && typeValuePair.Length == 2 )
			{
				// Check read checksum and compare with the actual shader body to detect external changes
				string currentChecksumValue = IOUtils.CreateChecksum( trimmedBuffer );
				if ( !currentChecksumValue.Equals( typeValuePair[ 1 ] ) )
				//{
				//	ShowMessage( "Correct checksum" );
				//}
				//else
				{
					ShowMessage( "Wrong checksum" );
				}

				// find node info body
				int shaderBodyId = trimmedBuffer.IndexOf( IOUtils.ShaderBodyBegin );
				if ( shaderBodyId > -1 )
				{
					trimmedBuffer = trimmedBuffer.Substring( shaderBodyId );

					//Find set of instructions
					string[] instructions = trimmedBuffer.Split( IOUtils.LINE_TERMINATOR );
					// First line is to be ignored and second line contains version
					string[] versionParams = instructions[ 1 ].Split( IOUtils.VALUE_SEPARATOR );
					if ( versionParams.Length == 2 )
					{
						int version = 0;
						try
						{
							version = Convert.ToInt32( versionParams[ 1 ] );
						}
						catch ( Exception e )
						{
							Debug.LogError( e );
						}

						/*if ( version == IOUtils.CurrentVersionFlt )
						{
							ShowMessage( "Version match" );
						}
						else*/
						if ( version > m_versionInfo.FullNumber )
						{
							ShowMessage( "This shader was created on a new ASE version\nPlease install v." + version );
						}
						else if ( version < m_versionInfo.FullNumber )
						{
							ShowMessage( "This shader was created on a older ASE version\nSaving will update it to the new one." );
						}

						m_mainGraphInstance.LoadedShaderVersion = version;
					}
					else
					{
						ShowMessage( "Corrupted version" );
					}

					// Dummy values,camera values can only be applied after node loading is complete
					Rect dummyCameraInfo = new Rect();
					Vector2 dummyCameraOffset = new Vector2();
					float dummyCameraZoom = 0;
					bool applyDummy = false;
					bool dummyNodeParametersWindowMaximized = false;
					bool dummyPaletteWindowMaximized = false;

					//Second line contains camera information ( position, size, offset and zoom )
					string[] cameraParams = instructions[ 2 ].Split( IOUtils.FIELD_SEPARATOR );
					if ( cameraParams.Length == 9 )
					{
						applyDummy = true;
						try
						{
							dummyCameraInfo.x = Convert.ToSingle( cameraParams[ 0 ] );
							dummyCameraInfo.y = Convert.ToSingle( cameraParams[ 1 ] );
							dummyCameraInfo.width = Convert.ToSingle( cameraParams[ 2 ] );
							dummyCameraInfo.height = Convert.ToSingle( cameraParams[ 3 ] );
							dummyCameraOffset.x = Convert.ToSingle( cameraParams[ 4 ] );
							dummyCameraOffset.y = Convert.ToSingle( cameraParams[ 5 ] );
							dummyCameraZoom = Convert.ToSingle( cameraParams[ 6 ] );
							dummyNodeParametersWindowMaximized = Convert.ToBoolean( cameraParams[ 7 ] );
							dummyPaletteWindowMaximized = Convert.ToBoolean( cameraParams[ 8 ] );
						}
						catch ( Exception e )
						{
							Debug.LogError( e );
						}
					}
					else
					{
						ShowMessage( "Camera parameters are corrupted" );
					}

					// valid instructions are only between the line after version and the line before the last one ( which contains ShaderBodyEnd ) 
					for ( int instructionIdx = 3; instructionIdx < instructions.Length - 1; instructionIdx++ )
					{
						//TODO: After all is working, convert string parameters to ints in order to speed up reading
						string[] parameters = instructions[ instructionIdx ].Split( IOUtils.FIELD_SEPARATOR );

						// All nodes must be created before wiring the connections ... 
						// Since all nodes on the save op are written before the wires, we can safely create them
						// If that order is not maintained the it's because of external editing and its the users responsability
						switch ( parameters[ 0 ] )
						{
							case IOUtils.NodeParam:
							{
								ParentNode newNode = ( ParentNode ) ScriptableObject.CreateInstance( Type.GetType( parameters[ IOUtils.NodeTypeId ] ) );
								if ( newNode != null )
								{
									try
									{
										newNode.ReadFromString( ref parameters );
										newNode.ReadInputDataFromString( ref parameters );
									}
									catch ( Exception e )
									{
										Debug.LogError( e );
									}
									m_mainGraphInstance.AddNode( newNode, false );
								}
							}
							break;
							case IOUtils.WireConnectionParam:
							{
								int InNodeId = 0;
								int InPortId = 0;
								int OutNodeId = 0;
								int OutPortId = 0;

								try
								{
									InNodeId = Convert.ToInt32( parameters[ IOUtils.InNodeId ] );
									InPortId = Convert.ToInt32( parameters[ IOUtils.InPortId ] );
									OutNodeId = Convert.ToInt32( parameters[ IOUtils.OutNodeId ] );
									OutPortId = Convert.ToInt32( parameters[ IOUtils.OutPortId ] );
								}
								catch ( Exception e )
								{
									Debug.LogError( e );
								}
								ParentNode inNode = m_mainGraphInstance.GetNode( InNodeId );
								ParentNode outNode = m_mainGraphInstance.GetNode( OutNodeId );
								InputPort inputPort = null;
								OutputPort outputPort = null;
								if ( inNode != null && outNode != null )
								{

									inputPort = inNode.GetInputPortById( InPortId );
									outputPort = outNode.GetOutputPortById( OutPortId );
									if ( inputPort != null && outputPort != null )
									{
										inputPort.ConnectTo( OutNodeId, OutPortId, outputPort.DataType, false );
										outputPort.ConnectTo( InNodeId, InPortId, inputPort.DataType, inputPort.TypeLocked );

										inNode.OnInputPortConnected( InPortId, OutNodeId, OutPortId, false );
										outNode.OnOutputPortConnected( OutPortId, InNodeId, InPortId );
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
							break;
						}
					}

					//string relPath = pathname.Substring(Application.dataPath.Length - 6 );// the -6 part is because the path needs to include the Assets string
					Shader shader = AssetDatabase.LoadAssetAtPath<Shader>( pathname );
					if ( shader )
					{
						m_mainGraphInstance.ForceSignalPropagationOnMasterNode();
						m_mainGraphInstance.UpdateShaderOnMasterNode( shader );
						m_onLoadDone = 2;
						if ( applyDummy )
						{
							m_cameraInfo = dummyCameraInfo;
							m_cameraOffset = dummyCameraOffset;
							CameraZoom = dummyCameraZoom;
							m_nodeParametersWindowMaximized = m_nodeParametersWindow.IsMaximized = dummyNodeParametersWindowMaximized;
							m_paletteWindowMaximized = m_paletteWindow.IsMaximized = dummyPaletteWindowMaximized;
						}
						//ShowMessage( "Shader loaded correctly" );
					}
					else
					{
						ShowMessage( "Could not load shader asset" );
					}
				}
				else
				{
					ShowMessage( "Graph info not found" );
				}
			}
			else
			{
				ShowMessage( "Corrupted checksum" );
			}
		}
		else
		{
			ShowMessage( "Checksum not found" );
		}
		UIUtils.DirtyMask = true;
		return loadResult;
	}

	public void ShowPortInfo()
	{
		GetWindow<PortLegendInfo>();
	}

	public void ShowShaderLibrary()
	{
		GetWindow<ShaderLibrary>();
	}

	public void ShowMessage( string message, MessageSeverity severity = MessageSeverity.Normal )
	{
		if ( UIUtils.InhibitMessages )
			return;

		if ( m_genericMessageUI.DisplayingMessage )
		{
			m_genericMessageUI.AddToQueue( message, severity );
		}
		else
		{
			m_genericMessageUI.StartMessageCounter();
			ShowMessageImmediately( message, severity );
		}
	}

	public void ShowMessageImmediately( string message, MessageSeverity severity = MessageSeverity.Normal )
	{
		if ( UIUtils.InhibitMessages )
			return;

		switch ( severity )
		{
			case MessageSeverity.Normal: { m_genericMessageContent.text = string.Empty; } break;
			case MessageSeverity.Warning: { m_genericMessageContent.text = "Warning!\n"; } break;
			case MessageSeverity.Error: { m_genericMessageContent.text = "Error!!!\n"; } break;
		}
		m_genericMessageContent.text += message;
		Debug.Log( message );
		ShowNotification( m_genericMessageContent );
	}

	void OnGUI()
	{
		if ( !m_initialized )
		{
			UIUtils.InitMainSkin();
			Init();
		}
		
		Vector2 pos = Event.current.mousePosition;
		pos.x += position.x;
		pos.y += position.y;
		m_insideEditorWindow = position.Contains( pos );

		if ( m_delayedLoadObject != null && m_mainGraphInstance.CurrentMasterNode != null )
		{
			LoadObject( m_delayedLoadObject );
			m_delayedLoadObject = null;
		}

		if ( m_delayedMaterialSet != null && m_mainGraphInstance.CurrentMasterNode != null )
		{
			m_mainGraphInstance.UpdateMaterialOnMasterNode( m_delayedMaterialSet );
			m_mainGraphInstance.SetMaterialModeOnGraph( m_delayedMaterialSet );
			CurrentSelection = ASESelectionMode.Material;
			m_delayedMaterialSet = null;
		}

		Material currentMaterial = m_mainGraphInstance.CurrentMaterial;
		if ( m_forceUpdateFromMaterialFlag )
		{
			m_forceUpdateFromMaterialFlag = false;
			if ( currentMaterial != null )
			{
				m_mainGraphInstance.CopyValuesFromMaterial( currentMaterial );
				m_repaintIsDirty = true;
			}
		}

		m_repaintCount = 0;
		m_cameraInfo = position;
		m_currentEvent = Event.current;

		if ( m_currentEvent.type == EventType.keyDown )
			m_keyEvtMousePos2D = m_currentEvent.mousePosition;

		m_currentMousePos2D = m_currentEvent.mousePosition;
		m_currentMousePos.x = m_currentMousePos2D.x;
		m_currentMousePos.y = m_currentMousePos2D.y;

		m_graphArea.width = m_cameraInfo.width;
		m_graphArea.height = m_cameraInfo.height;

		m_autoPanDirActive = m_lmbPressed || m_forceAutoPanDir || m_multipleSelectionActive || UIUtils.ValidReferences();


		// Need to use it in order to prevent Mismatched LayoutGroup on ValidateCommand when rendering nodes
		if ( Event.current.type == EventType.ValidateCommand )
		{
			Event.current.Use();
		}

		// Nodes Graph background area
		GUILayout.BeginArea( m_graphArea, "Nodes" );
		{
			// Camera movement is simulated by grabing the current camera offset, transforming it into texture space and manipulating the tiled texture uv coords
			GUI.DrawTextureWithTexCoords( m_graphArea, m_graphBgTexture,
				new Rect( -m_cameraZoom * m_cameraOffset.x / ( m_cameraZoom * m_graphBgTexture.width ),
							m_cameraZoom * m_cameraOffset.y / ( m_cameraZoom * m_graphBgTexture.height ),
							m_cameraZoom * m_cameraInfo.width / m_graphBgTexture.width,
							m_cameraZoom * m_cameraInfo.height / m_graphBgTexture.height ) );
			//Rect fgArea = _graphArea;
			//float maxSize = _graphArea.width > _graphArea.height ? _graphArea.width : _graphArea.height;
			//fgArea.width = fgArea.height = maxSize;
			//GUI.DrawTexture( fgArea, _graphFgTexture, ScaleMode.ScaleAndCrop, true );
			Color col = GUI.color;
			GUI.color = new Color( 1, 1, 1, 0.7f );
			GUI.DrawTexture( m_graphArea, m_graphFgTexture, ScaleMode.StretchToFill, true );
			GUI.color = col;
		}
		GUILayout.EndArea();

		bool restoreMouse = false;
		if ( InsideMenus( m_currentMousePos2D ) /*|| _confirmationWindow.IsActive*/ )
		{
			if ( Event.current.type == EventType.mouseDown )
			{
				restoreMouse = true;
				Event.current.type = EventType.ignore;
			}

			// Must guarantee that mouse up ops on menus will reset auto pan if it is set
			if ( Event.current.type == EventType.MouseUp && m_currentEvent.button == ButtonClickId.LeftMouseButton )
			{
				m_lmbPressed = false;
			}

		}
		// Nodes
		GUILayout.BeginArea( m_graphArea );
		{
			m_drawInfo.CameraArea = m_cameraInfo;
			m_drawInfo.TransformedCameraArea = m_graphArea;

			m_drawInfo.MousePosition = m_currentMousePos2D;
			m_drawInfo.CameraOffset = m_cameraOffset;
			m_drawInfo.InvertedZoom = 1 / m_cameraZoom;
			m_drawInfo.LeftMouseButtonPressed = m_currentEvent.button == ButtonClickId.LeftMouseButton;
			m_drawInfo.CurrentEventType = m_currentEvent.type;
			m_drawInfo.ZoomChanged = m_zoomChanged;

			m_drawInfo.TransformedMousePos = m_currentMousePos2D * m_cameraZoom - m_cameraOffset;
			UIUtils.UpdateMainSkin( m_drawInfo );

			// Draw mode indicator

			m_modeWindow.Draw( m_graphArea, m_currentMousePos2D, m_mainGraphInstance.CurrentShader, currentMaterial,
								0.5f * ( m_graphArea.width - m_paletteWindow.RealWidth - m_nodeParametersWindow.RealWidth ),
								( m_nodeParametersWindow.IsMaximized ? m_nodeParametersWindow.RealWidth : 0 ),
								( m_paletteWindow.IsMaximized ? m_paletteWindow.RealWidth : 0 ) );

			PreTestLeftMouseDown();
			m_mainGraphInstance.DrawWires( m_wireTexture, m_drawInfo, m_contextPalette.IsActive, m_contextPalette.CurrentPosition );
			m_repaintIsDirty = m_repaintIsDirty || m_mainGraphInstance.Draw( m_drawInfo );
			m_mainGraphInstance.DrawGrid( m_drawInfo );
			bool hasUnusedConnNodes = m_mainGraphInstance.HasUnConnectedNodes;
			m_toolsWindow.SetStateOnButton( ToolButtonType.CleanUnusedNodes, hasUnusedConnNodes ? 1 : 0 );
			if ( m_repaintIsDirty )
			{
				m_repaintIsDirty = false;
				ForceRepaint();
			}
			m_zoomChanged = false;

			MasterNode masterNode = m_mainGraphInstance.CurrentMasterNode;
			if ( masterNode )
			{
				m_toolsWindow.DrawShaderTitle( m_nodeParametersWindow, m_paletteWindow, AvailableCanvasWidth, m_graphArea.height, masterNode.ShaderName );
				//float leftAdjust = _nodeParametersWindow.IsMaximized ? _nodeParametersWindow.RealWidth : 0;
				//float rightAdjust = _paletteWindow.IsMaximized ? 0 : _paletteWindow.RealWidth;

				//Rect boxRect = new Rect( leftAdjust + rightAdjust, 0, AvailableCanvasWidth, 35 );
				//boxRect.x += _paletteWindow.IsMaximized ? 0 : -_paletteWindow.RealWidth;
				//boxRect.width += _nodeParametersWindow.IsMaximized ? 0 : _nodeParametersWindow.RealWidth;
				//boxRect.width += _paletteWindow.IsMaximized ? 0 : _paletteWindow.RealWidth;

				//Rect borderRect = new Rect( boxRect );
				//borderRect.height = _graphArea.height;


				//if ( _borderStyle == null )
				//	_borderStyle = UIUtils.CustomStyle( eCustomStyle.ShaderBorder );

				//GUI.Box( borderRect, masterNode.ShaderName, _borderStyle );
				//GUI.Box( boxRect, masterNode.ShaderName, UIUtils.CustomStyle( eCustomStyle.MainCanvasTitle ) );
			}
		}

		GUILayout.EndArea();

		if ( restoreMouse )
		{
			Event.current.type = EventType.mouseDown;
		}

		m_toolsWindow.InitialX = m_nodeParametersWindow.RealWidth;
		m_toolsWindow.Width = m_cameraInfo.width - ( m_nodeParametersWindow.RealWidth + m_paletteWindow.RealWidth );
		//_toolsWindow.CleanUpDelta = _paletteWindow.IsMaximized ? 0f : _paletteWindow.RealWidth;
		m_toolsWindow.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button );

		//_nodeParametersWindow.Height = _cameraInfo.height - ( _toolsWindow.Height + _toolsWindow.Height );

		bool autoMinimize = false;
		if ( position.width < m_lastWindowWidth && position.width < Constants.MINIMIZE_WINDOW_LOCK_SIZE )
		{
			autoMinimize = true;
		}

		if ( autoMinimize )
			m_nodeParametersWindow.IsMaximized = false;

		ParentNode selectedNode = ( m_mainGraphInstance.SelectedNodes.Count == 1 ) ? m_mainGraphInstance.SelectedNodes[ 0 ] : m_mainGraphInstance.CurrentMasterNode;
		m_repaintIsDirty = m_repaintIsDirty || m_nodeParametersWindow.Draw( m_cameraInfo, selectedNode, m_currentMousePos2D, m_currentEvent.button ); //TODO: If multiple nodes from the same type are selected also show a parameters window which modifies all of them 
		if ( m_repaintIsDirty )
		{
			m_repaintIsDirty = false;
			ForceRepaint();
		}

		// Test to ignore mouse on main palette when inside context palette ... IsInside also takes active state into account 
		bool ignoreMouseForPalette = m_contextPalette.IsInside( m_currentMousePos2D );
		if ( ignoreMouseForPalette && Event.current.type == EventType.mouseDown )
		{
			Event.current.type = EventType.ignore;
		}
		if ( autoMinimize )
			m_paletteWindow.IsMaximized = false;

		m_paletteWindow.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button );

		if ( ignoreMouseForPalette )
		{
			if ( restoreMouse )
			{
				Event.current.type = EventType.mouseDown;
			}
		}

		if ( m_contextPalette.IsActive )
		{
			m_contextPalette.Draw( m_cameraInfo, m_currentMousePos2D, m_currentEvent.button );
		}

		if ( m_palettePopup.IsActive )
		{
			m_palettePopup.Draw( m_currentMousePos2D );
			m_repaintIsDirty = true;
			int controlID = GUIUtility.GetControlID( FocusType.Passive );
			if ( m_currentEvent.GetTypeForControl( controlID ) == EventType.MouseUp )
			{
				if ( m_currentEvent.button == ButtonClickId.LeftMouseButton )
				{
					m_palettePopup.Deactivate();
					if ( !InsideMenus( m_currentMousePos2D ) )
					{
						CreateNode( m_paletteChosenType, TranformedMousePos );
					}
				}
			}
		}

		// Handle all events ( mouse interaction + others )
		HandleGUIEvents();

		// UI Overlay
		// Selection Box
		if ( m_multipleSelectionActive )
		{
			UpdateSelectionArea();
			Rect transformedArea = m_multipleSelectionArea;
			transformedArea.position = ( transformedArea.position + m_cameraOffset ) / m_cameraZoom;
			transformedArea.size /= m_cameraZoom;

			if ( transformedArea.width < 0 )
			{
				transformedArea.width = -transformedArea.width;
				transformedArea.x -= transformedArea.width;
			}

			if ( transformedArea.height < 0 )
			{
				transformedArea.height = -transformedArea.height;
				transformedArea.y -= transformedArea.height;
			}
			Color original = GUI.color;
			GUI.color = Constants.BoxSelectionColor;
			GUI.Box( transformedArea, "", m_customStyles.Box );
			GUI.backgroundColor = original;
		}

		//Test boundaries for auto-pan
		if ( m_autoPanDirActive )
		{
			m_autoPanArea[ ( int ) AutoPanLocation.LEFT ].AdjustInitialX = m_nodeParametersWindow.IsMaximized ? m_nodeParametersWindow.RealWidth : 0;
			m_autoPanArea[ ( int ) AutoPanLocation.RIGHT ].AdjustInitialX = m_paletteWindow.IsMaximized ? -m_paletteWindow.RealWidth : 0;
			Vector2 autoPanDir = Vector2.zero;
			for ( int i = 0; i < m_autoPanArea.Length; i++ )
			{
				if ( m_autoPanArea[ i ].CheckArea( m_currentMousePos2D, m_cameraInfo, false ) )
				{
					autoPanDir += m_autoPanArea[ i ].Velocity;
				}
			}
			m_cameraOffset += autoPanDir;
			if ( !UIUtils.ValidReferences() && m_insideEditorWindow )
			{
				m_mainGraphInstance.MoveSelectedNodes( -autoPanDir );
			}

			m_repaintIsDirty = true;
		}


		m_isDirty = m_isDirty || m_mainGraphInstance.IsDirty;
		if ( m_isDirty )
		{
			m_isDirty = false;
			ShaderIsModified = true;
			EditorUtility.SetDirty( this );
		}

		m_saveIsDirty = m_saveIsDirty || m_mainGraphInstance.SaveIsDirty;
		if ( m_saveIsDirty )
		{
			ShaderIsModified = true;
			m_saveIsDirty = false;
			if ( m_liveShaderEditing && focusedWindow )
			{
				if ( m_mainGraphInstance.CurrentMasterNodeId != Constants.INVALID_NODE_ID )
				{
					SaveToDisk( true );
				}
				else
				{
					ShowMessage( LiveShaderError );
				}
			}
		}

		if ( m_onLoadDone > 0 )
		{
			m_onLoadDone--;
			if ( m_onLoadDone == 0 )
			{
				ShaderIsModified = false;
			}
		}

		if ( m_repaintIsDirty )
		{
			m_repaintIsDirty = false;
			ForceRepaint();
		}

		if ( m_cacheSaveOp )
		{
			if ( ( EditorApplication.timeSinceStartup - m_lastTimeSaved ) > SaveTime )
			{
				SaveToDisk( false );
			}
		}
		m_genericMessageUI.CheckForMessages();

		if ( m_ctrlSCallback )
		{
			m_ctrlSCallback = false;
			OnToolButtonPressed( ToolButtonType.Update );
		}

		//Rect rect = new Rect( _minNodePos + _cameraOffset, _maxNodePos - _minNodePos );
		//Debug.Log( rect );
		//GUI.Box( rect, string.Empty ,_customStyles.Window);
		m_lastWindowWidth = position.width;
	}

	public void SetCtrlSCallback( bool imediate )
	{
		//MasterNode node = _mainGraphInstance.CurrentMasterNode;
		if ( /*node != null && node.CurrentShader != null && */m_shaderIsModified )
		{
			if ( imediate )
			{
				OnToolButtonPressed( ToolButtonType.Update );
			}
			else
			{
				m_ctrlSCallback = true;
			}
		}
	}

	public void SetSaveIsDirty()
	{
		m_saveIsDirty = true && UIUtils.DirtyMask;
	}

	public void OnPaletteNodeCreate( Type type, string name )
	{
		m_mainGraphInstance.DeSelectAll();
		m_paletteChosenType = type;
		m_palettePopup.Activate( name );
	}

	public void OnContextPaletteNodeCreate( Type type, string name )
	{
		m_mainGraphInstance.DeSelectAll();
		CreateNode( type, UIUtils.ValidReferences() ? ( m_contextPalette.CurrentPosition2D * m_cameraZoom - m_cameraOffset ) : TranformedMousePos );
	}

	void OnNodeStoppedMovingEvent( ParentNode node )
	{
		CheckZoomBoundaries( node.Vec2Position );
		ShaderIsModified = true;
	}

	void OnMaterialUpdated( MasterNode masterNode )
	{
		if ( masterNode != null )
		{
			if ( masterNode.CurrentMaterial )
			{
				m_toolsWindow.SetStateOnButton( ToolButtonType.Update, ShaderIsModified ? 0 : 2, ShaderIsModified ? "Click to update Shader preview." : "Preview up-to-date." );
			}
			else
			{
				m_toolsWindow.SetStateOnButton( ToolButtonType.Update, 1, "Set an active Material in the Master Node." );
			}
			UpdateLiveUI();
		}
		else
		{
			m_toolsWindow.SetStateOnButton( ToolButtonType.Update, 1, "Set an active Material in the Master Node." );
		}
	}

	void OnShaderUpdated( MasterNode masterNode )
	{
		m_toolsWindow.SetStateOnButton( ToolButtonType.OpenSourceCode, masterNode.CurrentShader != null ? 1 : 0 );
	}

	public void CheckZoomBoundaries( Vector2 newPosition )
	{
		if ( newPosition.x < m_minNodePos.x )
		{
			m_minNodePos.x = newPosition.x;
		}
		else if ( newPosition.x > m_maxNodePos.x )
		{
			m_maxNodePos.x = newPosition.x;
		}

		if ( newPosition.y < m_minNodePos.y )
		{
			m_minNodePos.y = newPosition.y;
		}
		else if ( newPosition.y > m_maxNodePos.y )
		{
			m_maxNodePos.y = newPosition.y;
		}
	}

	public void CreateNode( Type type, Vector2 position )
	{
		ParentNode node = m_mainGraphInstance.CreateNode( type, true );
		Vector2 newPosition = position;
		node.Vec2Position = newPosition;
		CheckZoomBoundaries( newPosition );

		// Connect node if a wire is active 
		if ( UIUtils.ValidReferences() )
		{
			if ( UIUtils.InputPortReference.IsValid )
			{
				ParentNode originNode = m_mainGraphInstance.GetNode( UIUtils.InputPortReference.NodeId );
				OutputPort outputPort = node.GetFirstOutputPortOfType( UIUtils.InputPortReference.DataType, true );
				if ( outputPort != null && ( !UIUtils.InputPortReference.TypeLocked ||
											UIUtils.InputPortReference.DataType == WirePortDataType.OBJECT ||
											( UIUtils.InputPortReference.TypeLocked && outputPort.DataType == UIUtils.InputPortReference.DataType ) ) )
				{

					//link output to input
					if ( outputPort.ConnectTo( UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId, UIUtils.InputPortReference.DataType, UIUtils.InputPortReference.TypeLocked ) )
						node.OnOutputPortConnected( outputPort.PortId, UIUtils.InputPortReference.NodeId, UIUtils.InputPortReference.PortId );

					//link input to output
					if ( originNode.GetInputPortById( UIUtils.InputPortReference.PortId ).ConnectTo( outputPort.NodeId, outputPort.PortId, UIUtils.InputPortReference.DataType, UIUtils.InputPortReference.TypeLocked ) )
						originNode.OnInputPortConnected( UIUtils.InputPortReference.PortId, node.UniqueId, outputPort.PortId );
				}
			}

			if ( UIUtils.OutputPortReference.IsValid )
			{
				ParentNode originNode = m_mainGraphInstance.GetNode( UIUtils.OutputPortReference.NodeId );
				InputPort inputPort = node.GetFirstInputPortOfType( UIUtils.OutputPortReference.DataType, true );

				if ( inputPort != null && ( !inputPort.TypeLocked ||
												inputPort.DataType == WirePortDataType.OBJECT ||
												( inputPort.TypeLocked && inputPort.DataType == UIUtils.OutputPortReference.DataType ) ) )
				{

					inputPort.InvalidateAllConnections();
					//link input to output
					if ( inputPort.ConnectTo( UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId, UIUtils.OutputPortReference.DataType, inputPort.TypeLocked ) )
						node.OnInputPortConnected( inputPort.PortId, UIUtils.OutputPortReference.NodeId, UIUtils.OutputPortReference.PortId );
					//link output to input

					if ( originNode.GetOutputPortById( UIUtils.OutputPortReference.PortId ).ConnectTo( inputPort.NodeId, inputPort.PortId, UIUtils.OutputPortReference.DataType, inputPort.TypeLocked ) )
						originNode.OnOutputPortConnected( UIUtils.OutputPortReference.PortId, node.UniqueId, inputPort.PortId );
				}
			}
			UIUtils.InvalidateReferences();

			for ( int i = 0; i < m_mainGraphInstance.VisibleNodes.Count; i++ )
			{
				m_mainGraphInstance.VisibleNodes[ i ].OnNodeInteraction( node );
			}
		}

		m_mainGraphInstance.SelectNode( node, false, false );
		//_repaintIsDirty = true;
		ForceRepaint();
	}

	public void ForceRepaint()
	{
		m_repaintCount += 1;
		Repaint();
	}

	public void ForceUpdateFromMaterial() { m_forceUpdateFromMaterialFlag = true; }
	void UseCurrentEvent()
	{
		m_currentEvent.Use();
	}



	public void OnBeforeSerialize()
	{
		m_mainGraphInstance.DeSelectAll();
		if ( m_nodeParametersWindow != null )
			m_nodeParametersWindowMaximized = m_nodeParametersWindow.IsMaximized;

		if ( m_paletteWindow != null )
			m_paletteWindowMaximized = m_paletteWindow.IsMaximized;
	}

	public void OnAfterDeserialize()
	{
		if ( m_nodeParametersWindow != null )
			m_nodeParametersWindow.IsMaximized = m_nodeParametersWindowMaximized;

		if ( m_paletteWindow != null )
			m_paletteWindow.IsMaximized = m_paletteWindowMaximized;
	}

	void OnDestroy()
	{
		m_ctrlSCallback = false;
		Destroy();
	}

	void OnDisable()
	{
		m_ctrlSCallback = false;
	}

	void OnEmptyGraphDetected( ParentGraph graph )
	{
		if ( m_delayedLoadObject != null )
		{
			LoadObject( m_delayedLoadObject );
			m_delayedLoadObject = null;
			Repaint();
		}
		else
		{
			string lastOpenedObj = EditorPrefs.GetString( IOUtils.LAST_OPENED_OBJ_ID );
			if ( !string.IsNullOrEmpty( lastOpenedObj ) )
			{
				Shader shader = AssetDatabase.LoadAssetAtPath<Shader>( lastOpenedObj );
				if ( shader == null )
				{
					Material material = AssetDatabase.LoadAssetAtPath<Material>( lastOpenedObj );
					if ( material != null )
					{
						LoadDroppedObject( true, material.shader, material );
					}
				}
				else
				{
					LoadDroppedObject( true, shader, null );
				}
				Repaint();
			}
		}
	}

	public ParentGraph CurrentGraph
	{
		get { return m_mainGraphInstance; }
	}

	public bool ShaderIsModified
	{
		get { return m_shaderIsModified; }
		set
		{
			m_shaderIsModified = value && UIUtils.DirtyMask;
			//if ( _shaderIsModified && !Application.isPlaying )
			//{
			//	EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene() );
			//	EditorUtility.SetDirty( Shader.Find( "SimpleColor" ));
			//}

			m_toolsWindow.SetStateOnButton( ToolButtonType.Save, m_shaderIsModified ? 1 : 0 );
			MasterNode masterNode = m_mainGraphInstance.CurrentMasterNode;
			if ( masterNode != null )
			{
				if ( masterNode.CurrentMaterial )
				{
					m_toolsWindow.SetStateOnButton( ToolButtonType.Update, m_shaderIsModified ? 0 : 2 );
				}
				else
				{
					m_toolsWindow.SetStateOnButton( ToolButtonType.Update, m_shaderIsModified ? 1 : 2 );
				}
			}
			else
			{
				m_toolsWindow.SetStateOnButton( ToolButtonType.Update, 1 );
			}
		}
	}

	public CustomStylesContainer CustomStylesInstance
	{
		get { return m_customStyles; }
	}

	public PreMadeShaders PreMadeShadersInstance
	{
		get { return m_preMadeShaders; }
	}

	public Rect CameraInfo
	{
		get { return m_cameraInfo; }
	}

	public Vector2 TranformedMousePos
	{
		get { return m_currentMousePos2D * m_cameraZoom - m_cameraOffset; }
	}


	public Vector2 TranformedKeyEvtMousePos
	{
		get { return m_keyEvtMousePos2D * m_cameraZoom - m_cameraOffset; }
	}

	public PalettePopUp PalettePopUpInstance
	{
		get { return m_palettePopup; }
	}

	public float AvailableCanvasWidth
	{
		get { return ( m_cameraInfo.width - m_paletteWindow.RealWidth - m_nodeParametersWindow.RealWidth ); }
	}

	public float AvailableCanvasHeight
	{
		get { return ( m_cameraInfo.height ); }
	}

	public DuplicatePreventionBuffer DuplicatePrevBufferInstance
	{
		get { return m_duplicatePreventionBuffer; }
	}

	public string LastOpenedLocation
	{
		get { return m_lastOpenedLocation; }
	}

	public float CameraZoom
	{
		get { return m_cameraZoom; }
		set
		{
			m_cameraZoom = value;
			m_zoomChanged = true;
		}
	}

	public bool ForceAutoPanDir
	{
		get { return m_forceAutoPanDir; }
		set { m_forceAutoPanDir = value; }
	}

	public ASESelectionMode CurrentSelection
	{
		get { return m_selectionMode; }
		set
		{
			m_selectionMode = value;
			m_toolsWindow.BorderStyle = ( m_selectionMode == ASESelectionMode.Material ) ? UIUtils.CustomStyle( CustomStyle.MaterialBorder ) : UIUtils.CustomStyle( CustomStyle.ShaderBorder );
		}
	}

	public void MarkToRepaint() { m_repaintIsDirty = true; }
	public UnityEngine.Object DelayedObjToLoad
	{
		set { m_delayedLoadObject = value; }
	}
	public int CurrentVersion
	{
		get { return m_versionInfo.FullNumber; }
	}
}
