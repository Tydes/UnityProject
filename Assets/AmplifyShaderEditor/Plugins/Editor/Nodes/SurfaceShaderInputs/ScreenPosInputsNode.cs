// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Screen Position", "Surface Standard Inputs", "Screen space position" )]
	public sealed class ScreenPosInputsNode : SurfaceShaderINParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.SCREEN_POS;
			InitialSetup();
		}
	}
}
