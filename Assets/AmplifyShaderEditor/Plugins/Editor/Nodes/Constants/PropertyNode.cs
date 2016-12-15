// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum PropertyType
	{
		Constant,
		Property,
		InstancedProperty,
		Uniform
	}

	[Serializable]
	public class PropertyNode : ParentNode
	{
		private const string IsPropertyStr = "Is Property";
		private const string PropertyNameStr = "Property Name";
		private const string PropertyInspectorStr = "Name";
		private const string ParameterTypeStr = "Parameter Type";
		private const string PropertyTextfieldControlName = "PropertyName";
		private const string PropertyInspTextfieldControlName = "PropertyInspectorName";
		private const string OrderIndexStr = "Order Index";
		private const double MaxTimestamp = 2;
		private const double MaxPropertyTimestamp = 2;

		[SerializeField]
		protected PropertyType m_currentParameterType;

		[SerializeField]
		private PropertyType m_lastParameterType;

		[SerializeField]
		protected string m_propertyName;

		[SerializeField]
		protected string m_propertyInspectorName;

		[SerializeField]
		private int m_orderIndex = -1;

		protected bool m_freeType;
		protected bool m_propertyNameIsDirty;

		protected bool m_propertyFromInspector;
		protected double m_propertyFromInspectorTimestamp;

		protected bool m_delayedDirtyProperty;
		protected double m_delayedDirtyPropertyTimestamp;

		protected string m_defaultPropertyName;
		protected string m_oldName = string.Empty;

		private bool m_reRegisterName = false;

		protected bool m_useCustomPrefix = false;
		protected string m_customPrefix = null;

		public PropertyNode() : base() { }
		public PropertyNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_textLabelWidth = 95;

			m_currentParameterType = PropertyType.Constant;
			m_freeType = true;
			m_propertyNameIsDirty = true;
		}

		protected void BeginDelayedDirtyProperty()
		{
			m_delayedDirtyProperty = true;
			m_delayedDirtyPropertyTimestamp = EditorApplication.timeSinceStartup;
		}
		
		private void CheckDelayedDirtyProperty()
		{
			if ( m_delayedDirtyProperty )
			{
				if ( ( EditorApplication.timeSinceStartup - m_delayedDirtyPropertyTimestamp ) > MaxPropertyTimestamp )
				{
					m_delayedDirtyProperty = false;
					m_propertyNameIsDirty = true;
					m_sizeIsDirty = true;
				}
			}
		}

		private void BeginPropertyFromInspectorCheck()
		{
			m_propertyFromInspector = true;
			m_propertyFromInspectorTimestamp = EditorApplication.timeSinceStartup;
		}

		private void CheckPropertyFromInspector( bool forceUpdate = false )
		{
			if ( m_propertyFromInspector )
			{
				if ( forceUpdate || ( EditorApplication.timeSinceStartup - m_propertyFromInspectorTimestamp ) > MaxTimestamp )
				{
					m_propertyFromInspector = false;
					RegisterPropertyName( true, m_propertyInspectorName );
					m_propertyNameIsDirty = true;
				}
			}
		}

		public override void ReleaseUniqueIdData()
		{
			UIUtils.ReleaseUniformName( m_uniqueId, m_oldName );
			RegisterFirstAvailablePropertyName( false );
		}

		protected override void OnUniqueIDAssigned()
		{
			RegisterFirstAvailablePropertyName( false );
		}

		public bool CheckLocalVariable( ref MasterNodeDataCollector dataCollector )
		{
			bool addToLocalValue = false;
			int count = 0;
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
				{
					if ( m_outputPorts[ i ].ConnectionCount > 1 )
					{
						addToLocalValue = true;
						break;
					}
					count += 1;
					if ( count > 1 )
					{
						addToLocalValue = true;
						break;
					}
				}
			}

			if ( addToLocalValue )
			{
				ConfigureLocalVariable( ref dataCollector );
			}

			return addToLocalValue;
		}

		public virtual void ConfigureLocalVariable( ref MasterNodeDataCollector dataCollector ) { }
		public virtual void CopyDefaultsToMaterial() { }

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			RegisterPropertyName( true, obj.name );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUILayout.BeginVertical();
			{
				if ( m_freeType )
				{
					PropertyType parameterType = ( PropertyType ) EditorGUILayout.EnumPopup( ParameterTypeStr, m_currentParameterType );
					if ( parameterType != m_currentParameterType )
					{
						if ( m_currentParameterType == PropertyType.Constant )
						{
							CopyDefaultsToMaterial();
						}

						if ( parameterType == PropertyType.InstancedProperty )
						{
							UIUtils.AddInstancePropertyCount();
						}
						else if ( m_currentParameterType == PropertyType.InstancedProperty )
						{
							UIUtils.RemoveInstancePropertyCount();
						}
					}
					m_currentParameterType = parameterType;
				}

				switch ( m_currentParameterType )
				{
					case PropertyType.Property:
					case PropertyType.InstancedProperty:
					{
						ShowPropertyInspectorNameGUI();
						ShowPropertyNameGUI( true );
					}
					break;
					case PropertyType.Uniform:
					{
						ShowPropertyInspectorNameGUI();
						ShowPropertyNameGUI( false );
					}
					break;
					case PropertyType.Constant:
					{
						ShowPropertyInspectorNameGUI();
					}
					break;
				}

				EditorGUI.BeginChangeCheck();

				DrawSubProperties();
				if ( CanDrawMaterial )
				{
					DrawMaterialProperties();
				}

				if ( EditorGUI.EndChangeCheck() )
				{
					//_propertyNameIsDirty = true;
					BeginDelayedDirtyProperty();
				}
			}
			EditorGUILayout.EndVertical();
			CheckPropertyFromInspector();
		}

		void ShowPropertyInspectorNameGUI()
		{
			EditorGUI.BeginChangeCheck();
			m_propertyInspectorName = EditorGUILayout.TextField( PropertyInspectorStr, m_propertyInspectorName );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( m_propertyInspectorName.Length > 0 )
				{
					BeginPropertyFromInspectorCheck();
				}
			}
		}

		void ShowPropertyNameGUI( bool isProperty )
		{
			bool guiEnabledBuffer = GUI.enabled;
			GUI.enabled = false;
			m_propertyName = EditorGUILayout.TextField( PropertyNameStr, m_propertyName );
			GUI.enabled = guiEnabledBuffer;

			if ( isProperty )
				m_orderIndex = EditorGUILayout.IntField( OrderIndexStr, m_orderIndex );
		}

		public virtual string GetPropertyValStr() { return string.Empty; }

		public override void Draw( DrawInfo drawInfo )
		{
			if ( m_reRegisterName )
			{
				m_reRegisterName = false;
				UIUtils.RegisterUniformName( m_uniqueId, m_propertyName );
			}

			CheckDelayedDirtyProperty();


			if ( m_currentParameterType != m_lastParameterType || m_propertyNameIsDirty )
			{
				m_lastParameterType = m_currentParameterType;
				m_propertyNameIsDirty = false;
				if ( m_currentParameterType != PropertyType.Constant )
				{
					m_content.text = m_propertyInspectorName;
					m_additionalContent.text = string.Format( Constants.PropertyValueLabel, GetPropertyValStr() );
					m_titleLineAdjust = 5;
				}
				else
				{
					m_content.text = m_propertyInspectorName;
					m_titleLineAdjust = 5;
					m_additionalContent.text = string.Format( Constants.ConstantsValueLabel, GetPropertyValStr() );
				}
				m_sizeIsDirty = true;
			}

			CheckPropertyFromInspector();
			base.Draw( drawInfo );
		}

		protected void RegisterFirstAvailablePropertyName( bool releaseOldOne )
		{
			if ( releaseOldOne )
				UIUtils.ReleaseUniformName( m_uniqueId, m_oldName );

			UIUtils.GetFirstAvailableName( m_uniqueId, m_outputPorts[ 0 ].DataType, out m_propertyName, out m_propertyInspectorName, m_useCustomPrefix, m_customPrefix );
			m_oldName = m_propertyName;
			m_propertyNameIsDirty = true;
			m_reRegisterName = false;
			OnPropertyNameChanged();
		}

		protected void RegisterPropertyName( bool releaseOldOne, string newName )
		{
			string propertyName = UIUtils.GeneratePropertyName( newName );
			if ( UIUtils.IsUniformNameAvailable( propertyName ) )
			{
				if ( releaseOldOne )
					UIUtils.ReleaseUniformName( m_uniqueId, m_oldName );

				m_oldName = propertyName;
				m_propertyName = propertyName;
				m_propertyInspectorName = newName;
				m_propertyNameIsDirty = true;
				m_reRegisterName = false;
				UIUtils.RegisterUniformName( m_uniqueId, propertyName );
				OnPropertyNameChanged();
			}
			else
			{

				GUI.FocusControl( string.Empty );
				RegisterFirstAvailablePropertyName( releaseOldOne );
				UIUtils.ShowMessage( string.Format( "Duplicate name found on edited node.\nAssigning first valid one {0}", m_propertyInspectorName ) );
			}
		}

		public override string GenerateShaderForOutput( int outputId, WirePortDataType inputType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			CheckPropertyFromInspector( true );
			if ( m_propertyName.Length == 0 )
			{
				RegisterFirstAvailablePropertyName( false );
			}
			switch ( CurrentParameterType )
			{
				case PropertyType.Property:
				{
					dataCollector.AddToProperties( m_uniqueId, GetPropertyValue(), m_orderIndex );
					dataCollector.AddToUniforms( m_uniqueId, GetUniformValue() );
				}
				break;
				case PropertyType.InstancedProperty:
				{
					dataCollector.AddToProperties( m_uniqueId, GetPropertyValue(), m_orderIndex );
					dataCollector.AddToInstancedProperties( m_uniqueId, GetInstancedPropertyValue(), m_orderIndex );
				}
				break;
				case PropertyType.Uniform:
				{
					dataCollector.AddToUniforms( m_uniqueId, GetUniformValue() );
				}
				break;
				case PropertyType.Constant: break;
			}
			dataCollector.AddPropertyNode( this );
			return string.Empty;
		}

		public override void Destroy()
		{
			base.Destroy();
			UIUtils.ReleaseUniformName( m_uniqueId, m_propertyName );
			if ( m_currentParameterType == PropertyType.InstancedProperty )
				UIUtils.RemoveInstancePropertyCount();
		}

		public virtual void OnPropertyNameChanged() { }
		public virtual void DrawSubProperties() { }
		public virtual void DrawMaterialProperties() { }

		public virtual string GetPropertyValue()
		{
			return string.Empty;
		}

		public virtual string GetInstancedPropertyValue()
		{
			return string.Format( IOUtils.InstancedPropertiesElement, UIUtils.WirePortToCgType( m_outputPorts[ 0 ].DataType ), m_propertyName );
		}

		public virtual string GetUniformValue()
		{
			return string.Empty;
		}

		public PropertyType CurrentParameterType
		{
			get { return m_currentParameterType; }
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentParameterType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_propertyInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_orderIndex );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_currentParameterType = ( PropertyType ) Enum.Parse( typeof( PropertyType ), GetCurrentParam( ref nodeParams ) );
			if ( m_currentParameterType == PropertyType.InstancedProperty )
			{
				UIUtils.AddInstancePropertyCount();
			}
			
			m_propertyName = GetCurrentParam( ref nodeParams );
			m_propertyInspectorName = GetCurrentParam( ref nodeParams );

			if ( UIUtils.CurrentShaderVersion() > 13 )
			{
				m_orderIndex = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}


			m_propertyNameIsDirty = true;
			m_reRegisterName = false;

			UIUtils.ReleaseUniformName( m_uniqueId, m_oldName );
			UIUtils.RegisterUniformName( m_uniqueId, m_propertyName );
			m_oldName = m_propertyName;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			m_reRegisterName = true;
		}

		public bool CanDrawMaterial { get { return m_materialMode && m_currentParameterType != PropertyType.Constant; } }
		public int OrderIndex { get { return m_orderIndex; } }
		public string PropertyData { get { return ( m_currentParameterType == PropertyType.InstancedProperty ) ? string.Format( IOUtils.InstancedPropertiesData, m_propertyName ) : m_propertyName; } }
		public string PropertyName { get { return m_propertyName; } }
	}
}
