// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Matrix3X3", "Constants", "Matrix3X3 property" )]
	public sealed class Matrix3X3Node : PropertyNode
	{
		[SerializeField]
		private Matrix4x4 m_defaultValue = Matrix4x4.identity;

		[SerializeField]
		private Matrix4x4 m_materialValue = Matrix4x4.identity;


		public Matrix3X3Node() : base() { }
		public Matrix3X3Node( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT3x3, Constants.EmptyPortValue );
			m_fixedSize.x = 180;
			m_fixedSize.y = 45;

			m_defaultValue = new Matrix4x4();
			m_materialValue = new Matrix4x4();
		}

		public override void CopyDefaultsToMaterial()
		{
			m_materialValue = m_defaultValue;
		}

		public override void DrawSubProperties()
		{
			EditorGUILayout.LabelField( Constants.DefaultValueLabel );
			for ( int row = 0; row < 3; row++ )
			{
				EditorGUILayout.BeginHorizontal();
				for ( int column = 0; column < 3; column++ )
				{
					m_defaultValue[ row, column ] = EditorGUILayout.FloatField( string.Empty, m_defaultValue[ row, column ], GUILayout.MaxWidth( 76 ) );
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		public override void DrawMaterialProperties()
		{
			if ( m_materialMode )
				EditorGUI.BeginChangeCheck();

			EditorGUILayout.LabelField( Constants.MaterialValueLabel );
			for ( int row = 0; row < 3; row++ )
			{
				EditorGUILayout.BeginHorizontal();
				for ( int column = 0; column < 3; column++ )
				{
					m_materialValue[ row, column ] = EditorGUILayout.FloatField( string.Empty, m_materialValue[ row, column ], GUILayout.MaxWidth( 76 ) );
				}
				EditorGUILayout.EndHorizontal();
			}

			if ( m_materialMode && EditorGUI.EndChangeCheck() )
				m_requireMaterialUpdate = true;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( m_isVisible )
			{
				m_propertyDrawPos = m_globalPosition;

				m_propertyDrawPos.x += UIUtils.PortsSize.y * drawInfo.InvertedZoom;
				m_propertyDrawPos.y = m_outputPorts[ 0 ].Position.y;
				m_propertyDrawPos.width *= 0.8f;
				m_propertyDrawPos.height *= 0.75f;
				float maxWidth = 45 * drawInfo.InvertedZoom;
				float maxHeight = 15 * drawInfo.InvertedZoom;
				float spacingAdjust = 1.0f - 1.0f / drawInfo.InvertedZoom;

				bool currMode = m_materialMode && m_currentParameterType != PropertyType.Constant;
				Matrix4x4 value = currMode ? m_materialValue : m_defaultValue;

				EditorGUI.BeginChangeCheck();

				GUILayout.BeginArea( m_propertyDrawPos );
				{
					EditorGUILayout.BeginVertical();
					for ( int row = 0; row < 3; row++ )
					{
						EditorGUILayout.BeginHorizontal();
						for ( int column = 0; column < 3; column++ )
						{
							value[ row, column ] = EditorGUILayout.FloatField( string.Empty, value[ row, column ], UIUtils.MainSkin.textField, GUILayout.Width( maxWidth ), GUILayout.Height( maxHeight ) );
							GUILayout.Space( spacingAdjust );
						}
						EditorGUILayout.EndHorizontal();
						GUILayout.Space( spacingAdjust );
					}
					EditorGUILayout.EndVertical();
				}
				GUILayout.EndArea();

				if ( currMode )
				{
					m_materialValue = value;
				}
				else
				{
					m_defaultValue = value;
				}

				if ( EditorGUI.EndChangeCheck() )
				{
					m_requireMaterialUpdate = m_materialMode;
					BeginDelayedDirtyProperty();
				}
			}
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, inputType, ref dataCollector, ignoreLocalvar );

			if ( m_currentParameterType != PropertyType.Constant )
				return PropertyData;

			Matrix4x4 value = m_defaultValue;

			return "float3x3(" + value[ 0, 0 ] + "," + value[ 0, 1 ] + "," + value[ 0, 2 ] + "," +
								+value[ 1, 0 ] + "," + value[ 1, 1 ] + "," + value[ 1, 2 ] + "," +
								+value[ 2, 0 ] + "," + value[ 2, 1 ] + "," + value[ 2, 2 ] + ")";

		}


		public override string GetUniformValue()
		{
			return "uniform float3x3 " + m_propertyName + ";";
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) )
			{
				mat.SetMatrix( m_propertyName, m_materialValue );
			}
		}

		public override void SetMaterialMode( Material mat )
		{
			base.SetMaterialMode( mat );
			if ( m_materialMode && UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
			{
				m_materialValue = mat.GetMatrix( m_propertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
				m_materialValue = material.GetMatrix( m_propertyName );
		}


		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			string[] matrixVals = GetCurrentParam( ref nodeParams ).Split( IOUtils.VECTOR_SEPARATOR );
			if ( matrixVals.Length == 9 )
			{
				m_defaultValue[ 0, 0 ] = Convert.ToSingle( matrixVals[ 0 ] );
				m_defaultValue[ 0, 1 ] = Convert.ToSingle( matrixVals[ 1 ] );
				m_defaultValue[ 0, 2 ] = Convert.ToSingle( matrixVals[ 2 ] );

				m_defaultValue[ 1, 0 ] = Convert.ToSingle( matrixVals[ 3 ] );
				m_defaultValue[ 1, 1 ] = Convert.ToSingle( matrixVals[ 4 ] );
				m_defaultValue[ 1, 2 ] = Convert.ToSingle( matrixVals[ 5 ] );

				m_defaultValue[ 2, 0 ] = Convert.ToSingle( matrixVals[ 6 ] );
				m_defaultValue[ 2, 1 ] = Convert.ToSingle( matrixVals[ 7 ] );
				m_defaultValue[ 2, 2 ] = Convert.ToSingle( matrixVals[ 8 ] );
			}
			else
			{
				UIUtils.ShowMessage( "Incorrect number of matrix4x4 values", MessageSeverity.Error );
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );

			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue[ 0, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 0, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 0, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR +
															m_defaultValue[ 1, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 1, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 1, 2 ].ToString() + IOUtils.VECTOR_SEPARATOR +
															m_defaultValue[ 2, 0 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 2, 1 ].ToString() + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 2, 2 ].ToString() );
		}

		public override string GetPropertyValStr()
		{
			return ( m_materialMode && m_currentParameterType != PropertyType.Constant ) ? m_materialValue[ 0, 0 ].ToString( Mathf.Abs( m_materialValue[ 0, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 0, 1 ].ToString( Mathf.Abs( m_materialValue[ 0, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 0, 2 ].ToString( Mathf.Abs( m_materialValue[ 0, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_materialValue[ 1, 0 ].ToString( Mathf.Abs( m_materialValue[ 1, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 1, 1 ].ToString( Mathf.Abs( m_materialValue[ 1, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 1, 2 ].ToString( Mathf.Abs( m_materialValue[ 1, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_materialValue[ 2, 0 ].ToString( Mathf.Abs( m_materialValue[ 2, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 2, 1 ].ToString( Mathf.Abs( m_materialValue[ 2, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_materialValue[ 2, 2 ].ToString( Mathf.Abs( m_materialValue[ 2, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) :

																							m_defaultValue[ 0, 0 ].ToString( Mathf.Abs( m_defaultValue[ 0, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 0, 1 ].ToString( Mathf.Abs( m_defaultValue[ 0, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 0, 2 ].ToString( Mathf.Abs( m_defaultValue[ 0, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_defaultValue[ 1, 0 ].ToString( Mathf.Abs( m_defaultValue[ 1, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 1, 1 ].ToString( Mathf.Abs( m_defaultValue[ 1, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 1, 2 ].ToString( Mathf.Abs( m_defaultValue[ 1, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.MATRIX_DATA_SEPARATOR +
																							m_defaultValue[ 2, 0 ].ToString( Mathf.Abs( m_defaultValue[ 2, 0 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 2, 1 ].ToString( Mathf.Abs( m_defaultValue[ 2, 1 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel ) + IOUtils.VECTOR_SEPARATOR + m_defaultValue[ 2, 2 ].ToString( Mathf.Abs( m_defaultValue[ 2, 2 ] ) > 1000 ? Constants.PropertyBigMatrixFormatLabel : Constants.PropertyMatrixFormatLabel );
		}

	}
}
