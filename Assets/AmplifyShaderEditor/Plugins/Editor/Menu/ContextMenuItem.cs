// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class ContextMenuItem
	{
		private const string PALETTE_NAME_MOD_STR = "   ";
		public delegate ParentNode CreateNode( Type type, Vector2 position );
		public CreateNode CreateNodeFuncPtr;

		private string m_paletteName;
		private string m_name;
		private string m_category;
		private string m_description;
		private Type m_type;
		private GUIContent m_guiContent;
		private string m_nameWithShortcut;

		public ContextMenuItem( Type type, string name, string category, string description, KeyCode shortcut, CreateNode funcPtr )
		{
			m_name = name;
			m_nameWithShortcut = shortcut != KeyCode.None ? ( name + " [ " + UIUtils.KeyCodeToString( shortcut ) + " ]" ) : name;
			m_paletteName = PALETTE_NAME_MOD_STR + m_name;
			m_type = type;
			m_category = category;
			m_description = description;
			m_guiContent = new GUIContent( m_nameWithShortcut, m_description );
			CreateNodeFuncPtr = funcPtr;
		}

		public string PaletteName { get { return m_paletteName; } }
		public string Name { get { return m_name; } }
		public string NameWithShortcut { get { return m_nameWithShortcut; } }
		public string Category { get { return m_category; } }
		public string Description { get { return m_description; } }
		public Type NodeType { get { return m_type; } }
		public GUIContent ItemUIContent { get { return m_guiContent; } }

		public override string ToString()
		{
			return m_name + ":" + m_category + ":" + m_description;
		}
	}
}
