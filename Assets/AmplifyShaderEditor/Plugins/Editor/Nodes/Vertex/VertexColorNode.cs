// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex Color on Frag", "Vertex Data", "Vertex color interpolated on fragment" )]
	public sealed class VertexColorNode : ParentNode
	{
		private const string _inputColorStr = "float4 vertexColor : COLOR";
		private const string _colorValueStr = ".vertexColor";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_fixedSize.x += 50;
			AddOutputColorPorts( WirePortDataType.COLOR, Constants.EmptyPortValue );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{

			base.GenerateShaderForOutput( outputId, inputType, ref dataCollector, ignoreLocalVar );

			dataCollector.AddToInput( m_uniqueId, _inputColorStr, true );

			string result = Constants.InputVarStr + _colorValueStr;
			switch ( outputId )
			{

				case 1:
				{
					result += ".r";
				}
				break;
				case 2:
				{
					result += ".g";
				}
				break;
				case 3:
				{
					result += ".b";
				}
				break;
				case 4:
				{
					result += ".a";
				}
				break;
			}
			return result;
		}
	}
}
