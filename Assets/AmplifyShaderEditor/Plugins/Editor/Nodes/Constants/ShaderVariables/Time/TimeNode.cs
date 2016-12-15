// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Time Parameters", "Time", "Time since level load" )]
	public sealed class TimeNode : ConstVecShaderVariable
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputName( 1, "t/20" );
			ChangeOutputName( 2, "t" );
			ChangeOutputName( 3, "t*2" );
			ChangeOutputName( 4, "t*3" );
			m_value = "_Time";
		}
	}
}
