// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ConstantShaderVariable : ShaderVariablesNode
	{
		[SerializeField]
		protected string m_value;

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
			return m_value;
		}
	}
}
