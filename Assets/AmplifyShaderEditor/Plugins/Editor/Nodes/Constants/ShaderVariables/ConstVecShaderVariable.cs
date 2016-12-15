// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	public class ConstVecShaderVariable : ShaderVariablesNode
	{
		[SerializeField]
		protected string m_value;
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "out", WirePortDataType.FLOAT4 );
			AddOutputPort( WirePortDataType.FLOAT, "0" );
			AddOutputPort( WirePortDataType.FLOAT, "1" );
			AddOutputPort( WirePortDataType.FLOAT, "2" );
			AddOutputPort( WirePortDataType.FLOAT, "3" );
		}
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			switch ( outputId )
			{
				case 0: return m_value;
				case 1: return ( m_value + "[0]" );
				case 2: return ( m_value + "[1]" );
				case 3: return ( m_value + "[2]" );
				case 4: return ( m_value + "[3]" );
			}

			UIUtils.ShowMessage( "ConstVecShaderVariable generating empty code", MessageSeverity.Warning );
			return string.Empty;
		}

	}
}
