// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Vertex Normal World", "Surface Standard Inputs", "Vertex Normal World" )]
	public sealed class WorldNormalInputsNode : SurfaceShaderINParentNode
	{
		private const string PerPixelLabelStr = "Per Pixel";

		[SerializeField]
		private bool m_perPixel = true;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.WORLD_NORMAL;
			InitialSetup();
			UIUtils.AddNormalDependentCount();
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.RemoveNormalDependentCount();
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_perPixel = EditorGUILayout.ToggleLeft( PerPixelLabelStr, m_perPixel );
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			dataCollector.AddToInput( m_uniqueId, UIUtils.GetInputDeclarationFromType( AvailableSurfaceInputs.WORLD_NORMAL ), true );
			dataCollector.AddToInput( m_uniqueId, Constants.InternalData, false );
			if ( m_perPixel && dataCollector.DirtyNormal )
			{
				string result = "WorldNormalVector( " + Constants.InputVarStr + " , float3( 0,0,1 ))";
				int count = 0;
				for ( int i = 0; i < m_outputPorts.Count; i++ )
				{
					if ( m_outputPorts[ i ].IsConnected )
					{
						if ( m_outputPorts[ i ].ConnectionCount > 2 )
						{
							count = 2;
							break;
						}
						count += 1;
						if ( count > 1 )
							break;

					}
				}
				if ( count > 1 )
				{
					string localVarName = "WorldNormal" + m_uniqueId;
					//dataCollector.AddToLocalVariables( _uniqueId, "float3 " + localVarName + " = " + result +";");
					dataCollector.AddToLocalVariables( m_uniqueId, m_outputPorts[ 0 ].DataType, localVarName, result );
					return GetOutputVectorItem( 0, outputId, localVarName );
				}
				else
				{
					return GetOutputVectorItem( 0, outputId, result );
				}
			}
			else
			{
				return base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalVar );
			}
		}
	}
}
