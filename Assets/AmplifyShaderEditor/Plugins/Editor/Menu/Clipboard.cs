// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System.Collections.Generic;

namespace AmplifyShaderEditor
{

	public class ClipboardData
	{
		public string Data = string.Empty;
		public string Connections = string.Empty;
		public int OldNodeId = -1;
		public int NewNodeId = -1;
		public ClipboardData( string data, string connections, int oldNodeId )
		{
			Data = data;
			Connections = connections;
			OldNodeId = oldNodeId;
		}

		public override string ToString()
		{
			return Data + IOUtils.CLIPBOARD_DATA_SEPARATOR + Connections + IOUtils.CLIPBOARD_DATA_SEPARATOR + OldNodeId + IOUtils.CLIPBOARD_DATA_SEPARATOR + NewNodeId;
		}
	}

	public class Clipboard
	{
		private List<ClipboardData> m_clipboardStrData;
		private Dictionary<int, ClipboardData> m_clipboardAuxData;

		public Clipboard()
		{
			m_clipboardStrData = new List<ClipboardData>();
			m_clipboardAuxData = new Dictionary<int, ClipboardData>();
		}

		public void AddToClipboard( List<ParentNode> selectedNodes )
		{
			m_clipboardStrData.Clear();
			m_clipboardAuxData.Clear();

			int masterNodeId = UIUtils.CurrentWindow.CurrentGraph.CurrentMasterNodeId;
			for ( int i = 0; i < selectedNodes.Count; i++ )
			{
				if ( selectedNodes[ i ].UniqueId != masterNodeId )
				{
					string nodeData = string.Empty;
					string connections = string.Empty;
					selectedNodes[ i ].WriteToString( ref nodeData, ref connections );
					selectedNodes[ i ].WriteInputDataToString( ref nodeData );
					ClipboardData data = new ClipboardData( nodeData, connections, selectedNodes[ i ].UniqueId );
					m_clipboardStrData.Add(data);
					m_clipboardAuxData.Add( selectedNodes[ i ].UniqueId, data );
				}
			}
		}

		public void GenerateFullString()
		{
			string data = string.Empty;
			for ( int i = 0; i < m_clipboardStrData.Count; i++ )
			{
				data += m_clipboardStrData[ i ].ToString();
				if ( i < m_clipboardStrData.Count - 1 )
				{
					data += IOUtils.LINE_TERMINATOR;
				}
			}
		}

		public void ClearClipboard()
		{
			m_clipboardStrData.Clear();
			m_clipboardAuxData.Clear();
		}

		public ClipboardData GetClipboardData( int oldNodeId )
		{
			if ( m_clipboardAuxData.ContainsKey( oldNodeId ) )
				return m_clipboardAuxData[ oldNodeId ];
			return null;
		}

		public int GeNewNodeId( int oldNodeId )
		{
			if ( m_clipboardAuxData.ContainsKey( oldNodeId ) )
				return m_clipboardAuxData[ oldNodeId ].NewNodeId;
			return -1;
		}
		
		public List<ClipboardData> CurrentClipboardStrData
		{
			get { return m_clipboardStrData; }
		}
	}
}
