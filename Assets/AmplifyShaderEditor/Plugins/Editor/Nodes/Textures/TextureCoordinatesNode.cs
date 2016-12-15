// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Texture Coordinates", "Surface Standard Inputs", "Texture UV coordinates set", null, KeyCode.U )]
	public sealed class TextureCoordinatesNode : ParentNode
	{
		private const string TilingStr = "Tiling";
		private const string OffsetStr = "Offset";
		private const string TexCoordStr = "texcoord_";


		private readonly int[] AvailableUVChannels = { 0, 1 };
		private readonly string[] AvailableUVChannelsStr = { "1", "2" };


		[SerializeField]
		private int m_textureCoordChannel = 0;

		[SerializeField]
		private int m_texcoordId = -1;

		[SerializeField]
		private string m_surfaceTexcoordName = string.Empty;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "Tiling" );
			m_inputPorts[ 0 ].Vector2InternalData = new Vector2( 1, 1 );
			AddInputPort( WirePortDataType.FLOAT2, false, "Offset" );
			AddOutputVectorPorts( WirePortDataType.FLOAT2, Constants.EmptyPortValue );
			m_textLabelWidth = 75;
			m_useInternalPortData = true;
		}

		public override void Reset()
		{
			m_texcoordId = -1;
			m_surfaceTexcoordName = string.Empty;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_textureCoordChannel = EditorGUILayout.IntPopup( Constants.AvailableUVSetsLabel, m_textureCoordChannel, AvailableUVChannelsStr, AvailableUVChannels );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_textureCoordChannel = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_textureCoordChannel );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_texcoordId < 0 )
			{
				m_texcoordId = dataCollector.AvailableUvIndex;
				string texcoordName = TexCoordStr + m_texcoordId;
				string uvChannel = m_textureCoordChannel == 0 ? ".xy" : "1.xy";
				string tiling = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false );
				string offset = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT2, false );

				string vertexUV = Constants.VertexShaderInputStr + ".texcoord" + uvChannel;
				dataCollector.AddToInput( m_uniqueId, "float2 " + texcoordName, true );
				dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + "." + texcoordName + ".xy = " + vertexUV + " * " + tiling + " + " + offset, m_uniqueId );
				m_surfaceTexcoordName = Constants.InputVarStr + "." + texcoordName;
			}

			return GetOutputVectorItem( 0, outputId, m_surfaceTexcoordName );
		}
	}
}
