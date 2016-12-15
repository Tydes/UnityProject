// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public enum BuiltInShaderCameraTypes
	{
		unity_CameraProjection = 0,
		unity_CameraInvProjection
	}

	[Serializable]
	[NodeAttributes( "Projection Matrices", "Camera and Screen", "Camera's Projection/Inverse Projection matrix" )]
	public sealed class CameraProjectionNode : ShaderVariablesNode
	{
		[SerializeField]
		private BuiltInShaderCameraTypes m_selectedType = BuiltInShaderCameraTypes.unity_CameraProjection;
		[SerializeField]
		private BuiltInShaderCameraTypes m_oldVarType = BuiltInShaderCameraTypes.unity_CameraProjection;

		private const string _projMatrixLabelStr = "Projection Matrix";
		private readonly string[] _projMatrixValuesStr = {  "Camera Projection",
															"Inverse Camera Projection"};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, _projMatrixValuesStr[ ( int ) m_selectedType ], WirePortDataType.FLOAT4x4 );
			m_textLabelWidth = 100;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_selectedType = ( BuiltInShaderCameraTypes ) EditorGUILayout.Popup( _projMatrixLabelStr, ( int ) m_selectedType, _projMatrixValuesStr );

			if ( m_selectedType != m_oldVarType )
			{
				m_oldVarType = m_selectedType;
				ChangeOutputName( 0, _projMatrixValuesStr[ ( int ) m_selectedType ] );
				SetSaveIsDirty();
			}
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			return m_selectedType.ToString();
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedType = ( BuiltInShaderCameraTypes ) Enum.Parse( typeof( BuiltInShaderCameraTypes ), GetCurrentParam( ref nodeParams ) );
			ChangeOutputName( 0, _projMatrixValuesStr[ ( int ) m_selectedType ] );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedType );
		}
	}
}
