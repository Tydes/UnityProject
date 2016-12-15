// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Int", "Constants", "Int property", null, KeyCode.Alpha0 )]
	public sealed class IntNode : PropertyNode
	{
		[SerializeField]
		private int m_defaultValue;

		[SerializeField]
		private int m_materialValue;


		public IntNode() : base() { }
		public IntNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.INT, "Out" );
			m_fixedSize.x += 10;
		}

		public override void CopyDefaultsToMaterial()
		{
			m_materialValue = m_defaultValue;
		}

		public override void DrawSubProperties()
		{
			m_defaultValue = EditorGUILayout.IntField( Constants.DefaultValueLabel, m_defaultValue );
		}

		public override void DrawMaterialProperties()
		{
			if ( m_materialMode )
				EditorGUI.BeginChangeCheck();

			m_materialValue = EditorGUILayout.IntField( Constants.MaterialValueLabel, m_materialValue );

			if ( m_materialMode && EditorGUI.EndChangeCheck() )
			{
				m_requireMaterialUpdate = true;
				//if ( _currentParameterType != ePropertyType.Constant )
				//{
				//	_propertyNameIsDirty = true;
				//}
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( m_isVisible )
			{
				m_propertyDrawPos.x = m_globalPosition.x + Constants.FLOAT_WIDTH_SPACING;
				m_propertyDrawPos.y = m_outputPorts[ 0 ].Position.y;
				m_propertyDrawPos.width = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_WIDTH_FIELD_SIZE;
				m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;

				float labelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 5;
				if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
				{
					EditorGUI.BeginChangeCheck();
					m_materialValue = EditorGUI.IntField( m_propertyDrawPos, "  ", m_materialValue, UIUtils.MainSkin.textField );
					if ( EditorGUI.EndChangeCheck() )
					{
						m_requireMaterialUpdate = true;
						if ( m_currentParameterType != PropertyType.Constant )
							BeginDelayedDirtyProperty();
					}
				}
				else
				{
					EditorGUI.BeginChangeCheck();

					m_defaultValue = EditorGUI.IntField( m_propertyDrawPos, "  ", m_defaultValue, UIUtils.MainSkin.textField );

					if ( EditorGUI.EndChangeCheck() )
						BeginDelayedDirtyProperty();
				}
				EditorGUIUtility.labelWidth = labelWidth;
			}
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, inputType, ref dataCollector, ignoreLocalvar );

			if ( m_currentParameterType != PropertyType.Constant )
				return PropertyData;

			return m_defaultValue.ToString();
		}

		public override string GetPropertyValue()
		{
			return m_propertyName + "(\"" + m_propertyInspectorName + "\", Int) = " + m_defaultValue;
		}

		public override string GetUniformValue()
		{
			return "uniform int " + m_propertyName + ";";
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) )
			{
				mat.SetInt( m_propertyName, m_materialValue );
			}
		}

		public override void SetMaterialMode( Material mat )
		{
			base.SetMaterialMode( mat );
			if ( m_materialMode && UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
			{
				m_materialValue = mat.GetInt( m_propertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
				m_materialValue = material.GetInt( m_propertyName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_defaultValue = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue );
		}

		public override string GetPropertyValStr()
		{
			return ( m_materialMode && m_currentParameterType != PropertyType.Constant ) ?
				m_materialValue.ToString( Mathf.Abs( m_materialValue ) > 1000 ? Constants.PropertyBigIntFormatLabel : Constants.PropertyIntFormatLabel ) :
				m_defaultValue.ToString( Mathf.Abs( m_defaultValue ) > 1000 ? Constants.PropertyBigIntFormatLabel : Constants.PropertyIntFormatLabel );
		}
	}
}
