// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	//[NodeAttributes( "Ranged Float", "Constants", "Ranged float property" )]
	[NodeAttributes( "Float", "Constants", "Float property", null, KeyCode.Alpha1 )]
	public sealed class RangedFloatNode : PropertyNode
	{

		private const int OriginalFontSize = 11;

		private const string MinValueStr = "Min";
		private const string MaxValueStr = "Max";

		[SerializeField]
		private float m_defaultValue = 0;

		[SerializeField]
		private float m_materialValue = 0;

		[SerializeField]
		private float m_min = 0;

		[SerializeField]
		private float m_max = 0;

		[SerializeField]
		private bool m_floatMode = true;

		public RangedFloatNode() : base() { }
		public RangedFloatNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_fixedSize.x = 60;
		}

		public void SetFloatMode( bool value )
		{
			m_floatMode = value;
			if ( value )
			{
				m_fixedSize.x = 60;
			}
			else
			{
				m_fixedSize.x = 200;
			}
			m_sizeIsDirty = true;
		}

		public override void CopyDefaultsToMaterial()
		{
			m_materialValue = m_defaultValue;
		}

		public override void DrawSubProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_min = EditorGUILayout.FloatField( MinValueStr, m_min );
			m_max = EditorGUILayout.FloatField( MaxValueStr, m_max );
			if ( m_min > m_max )
				m_min = m_max;

			if ( m_max < m_min )
				m_max = m_min;

			if ( EditorGUI.EndChangeCheck() )
			{
				SetFloatMode( m_min == m_max );
			}

			if ( m_floatMode )
			{
				m_defaultValue = EditorGUILayout.FloatField( Constants.DefaultValueLabel, m_defaultValue );
			}
			else
			{
				m_defaultValue = EditorGUILayout.Slider( Constants.DefaultValueLabel, m_defaultValue, m_min, m_max );
			}
		}

		public override void DrawMaterialProperties()
		{
			if ( m_materialMode )
				EditorGUI.BeginChangeCheck();

			if ( m_floatMode )
			{
				m_materialValue = EditorGUILayout.FloatField( Constants.MaterialValueLabel, m_materialValue );
			}
			else
			{
				m_materialValue = EditorGUILayout.Slider( Constants.MaterialValueLabel, m_materialValue, m_min, m_max );
			}
			if ( m_materialMode && EditorGUI.EndChangeCheck() )
				m_requireMaterialUpdate = true;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( m_isVisible )
			{
				if ( m_floatMode )
				{
					m_propertyDrawPos.x = m_globalPosition.x + drawInfo.InvertedZoom * 2.5f * Constants.FLOAT_WIDTH_SPACING;
					m_propertyDrawPos.y = m_outputPorts[ 0 ].Position.y;
					m_propertyDrawPos.width = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_WIDTH_FIELD_SIZE;
					m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;
				}
				else
				{
					m_propertyDrawPos.x = m_globalPosition.x + 0.05f * m_globalPosition.width;
					m_propertyDrawPos.y = m_globalPosition.y + 0.5f * m_globalPosition.height;
					m_propertyDrawPos.width = 0.7f * m_globalPosition.width;
					m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;
				}

				if ( m_materialMode && m_currentParameterType != PropertyType.Constant )
				{
					EditorGUI.BeginChangeCheck();
					if ( m_floatMode )
					{
						UIUtils.DrawFloat( ref m_propertyDrawPos, ref m_materialValue, 1 );
					}
					else
					{
						int originalFontSize = EditorStyles.numberField.fontSize;
						EditorStyles.numberField.fontSize = ( int ) ( OriginalFontSize * drawInfo.InvertedZoom );
						m_materialValue = EditorGUI.Slider( m_propertyDrawPos, m_materialValue, m_min, m_max );
						EditorStyles.numberField.fontSize = originalFontSize;
					}
					if ( EditorGUI.EndChangeCheck() )
					{
						m_requireMaterialUpdate = true;
						if ( m_currentParameterType != PropertyType.Constant )
						{
							BeginDelayedDirtyProperty();
						}
					}

				}
				else
				{
					EditorGUI.BeginChangeCheck();

					if ( m_floatMode )
					{
						UIUtils.DrawFloat( ref m_propertyDrawPos, ref m_defaultValue, 1 );
					}
					else
					{
						int originalFontSize = EditorStyles.numberField.fontSize;
						EditorStyles.numberField.fontSize = ( int ) ( OriginalFontSize * drawInfo.InvertedZoom );
						m_defaultValue = EditorGUI.Slider( m_propertyDrawPos, m_defaultValue, m_min, m_max );
						EditorStyles.numberField.fontSize = originalFontSize;
					}
					if ( EditorGUI.EndChangeCheck() )
						BeginDelayedDirtyProperty();
				}
			}

		}
		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, inputType, ref dataCollector, ignoreLocalvar );

			if ( m_currentParameterType != PropertyType.Constant )
				return PropertyData;

			return IOUtils.Floatify( m_defaultValue );
		}

		public override string GetPropertyValue()
		{
			if ( m_floatMode )
			{
				return m_propertyName + "(\"" + m_propertyInspectorName + "\", Float) = " + m_defaultValue;
			}
			else
			{
				return m_propertyName + "(\"" + m_propertyInspectorName + "\", Range( " + m_min + " , " + m_max + ")) = " + m_defaultValue;
			}
		}

		public override string GetUniformValue()
		{
			return "uniform float " + m_propertyName + ";";
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) )
			{
				mat.SetFloat( m_propertyName, m_materialValue );
			}
		}

		public override void SetMaterialMode( Material mat )
		{
			base.SetMaterialMode( mat );
			if ( m_materialMode && UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
			{
				m_materialValue = mat.GetFloat( m_propertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
				m_materialValue = material.GetFloat( m_propertyName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_defaultValue = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			m_min = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			m_max = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			SetFloatMode( m_min == m_max );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_defaultValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_min );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_max );
		}

		public override string GetPropertyValStr()
		{
			return ( m_materialMode && m_currentParameterType != PropertyType.Constant ) ?
				m_materialValue.ToString( Mathf.Abs( m_materialValue ) > 1000 ? Constants.PropertyBigFloatFormatLabel : Constants.PropertyFloatFormatLabel ) :
				m_defaultValue.ToString( Mathf.Abs( m_defaultValue ) > 1000 ? Constants.PropertyBigFloatFormatLabel : Constants.PropertyFloatFormatLabel );
		}

		public float Value
		{
			get { return m_defaultValue; }
			set { m_defaultValue = value; }
		}
	}
}
