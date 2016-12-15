// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[CustomEditor( typeof( Material ) )]
	internal class CustomMaterialInspector : MaterialEditor
	{
		private const string CopyButtonStr = "Copy Values";
		private const string PasteButtonStr = "Paste Values";

		public override void OnInspectorGUI()
		{
			Material mat = target as Material;

			if ( mat == null )
				return;

			if ( Event.current.type == EventType.repaint &&
				mat.HasProperty( IOUtils.DefaultASEDirtyCheckId ) &&
				mat.GetInt( IOUtils.DefaultASEDirtyCheckId ) == 1 )
			{
				mat.SetInt( IOUtils.DefaultASEDirtyCheckId, 0 );
				UIUtils.ForceUpdateFromMaterial();
			}

			if ( isVisible )
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Space( 3 );
					if ( GUILayout.Button( "Open in Shader Editor" ) )
					{
						AmplifyShaderEditorWindow.LoadMaterialToASE( mat );
					}

					GUILayout.BeginHorizontal();
					{
						if ( GUILayout.Button( CopyButtonStr ) )
						{
							Shader shader = mat.shader;
							int propertyCount = ShaderUtil.GetPropertyCount( shader );
							string allProperties = string.Empty;
							for ( int i = 0; i < propertyCount; i++ )
							{
								ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType( shader, i );
								string name = ShaderUtil.GetPropertyName( shader, i );
								string valueStr = string.Empty;
								switch ( type )
								{
									case ShaderUtil.ShaderPropertyType.Color:
									{
										Color value = mat.GetColor( name );
										valueStr = value.r.ToString() + IOUtils.VECTOR_SEPARATOR +
													value.g.ToString() + IOUtils.VECTOR_SEPARATOR +
													value.b.ToString() + IOUtils.VECTOR_SEPARATOR +
													value.a.ToString();
									}
									break;
									case ShaderUtil.ShaderPropertyType.Vector:
									{
										Vector4 value = mat.GetVector( name );
										valueStr = value.x.ToString() + IOUtils.VECTOR_SEPARATOR +
													value.y.ToString() + IOUtils.VECTOR_SEPARATOR +
													value.z.ToString() + IOUtils.VECTOR_SEPARATOR +
													value.w.ToString();
									}
									break;
									case ShaderUtil.ShaderPropertyType.Float:
									{
										float value = mat.GetFloat( name );
										valueStr = value.ToString();
									}
									break;
									case ShaderUtil.ShaderPropertyType.Range:
									{
										float value = mat.GetFloat( name );
										valueStr = value.ToString();
									}
									break;
									case ShaderUtil.ShaderPropertyType.TexEnv:
									{
										Texture value = mat.GetTexture( name );
										valueStr = AssetDatabase.GetAssetPath( value );
									}
									break;
								}

								allProperties += name + IOUtils.FIELD_SEPARATOR + type + IOUtils.FIELD_SEPARATOR + valueStr;

								if ( i < ( propertyCount - 1 ) )
								{
									allProperties += IOUtils.LINE_TERMINATOR;
								}
							}
							EditorPrefs.SetString( IOUtils.MAT_CLIPBOARD_ID, allProperties );
						}

						if ( GUILayout.Button( PasteButtonStr ) )
						{
							string properties = EditorPrefs.GetString( IOUtils.MAT_CLIPBOARD_ID, string.Empty );
							if ( !string.IsNullOrEmpty( properties ) )
							{
								string[] propertyArr = properties.Split( IOUtils.LINE_TERMINATOR );
								bool validData = true;
								try
								{
									for ( int i = 0; i < propertyArr.Length; i++ )
									{
										string[] valuesArr = propertyArr[ i ].Split( IOUtils.FIELD_SEPARATOR );
										if ( valuesArr.Length != 3 )
										{
											Debug.LogWarning( "Material clipboard data is corrupted" );
											validData = false;
											break;
										}
										else if ( mat.HasProperty( valuesArr[ 0 ] ) )
										{
											ShaderUtil.ShaderPropertyType type = ( ShaderUtil.ShaderPropertyType ) Enum.Parse( typeof( ShaderUtil.ShaderPropertyType ), valuesArr[ 1 ] );
											switch ( type )
											{
												case ShaderUtil.ShaderPropertyType.Color:
												{
													string[] colorVals = valuesArr[ 2 ].Split( IOUtils.VECTOR_SEPARATOR );
													if ( colorVals.Length != 4 )
													{
														Debug.LogWarning( "Material clipboard data is corrupted" );
														validData = false;
														break;
													}
													else
													{
														mat.SetColor( valuesArr[ 0 ], new Color( Convert.ToSingle( colorVals[ 0 ] ),
																									Convert.ToSingle( colorVals[ 1 ] ),
																									Convert.ToSingle( colorVals[ 2 ] ),
																									Convert.ToSingle( colorVals[ 3 ] ) ) );
													}
												}
												break;
												case ShaderUtil.ShaderPropertyType.Vector:
												{
													string[] vectorVals = valuesArr[ 2 ].Split( IOUtils.VECTOR_SEPARATOR );
													if ( vectorVals.Length != 4 )
													{
														Debug.LogWarning( "Material clipboard data is corrupted" );
														validData = false;
														break;
													}
													else
													{
														mat.SetVector( valuesArr[ 0 ], new Vector4( Convert.ToSingle( vectorVals[ 0 ] ),
																									Convert.ToSingle( vectorVals[ 1 ] ),
																									Convert.ToSingle( vectorVals[ 2 ] ),
																									Convert.ToSingle( vectorVals[ 3 ] ) ) );
													}
												}
												break;
												case ShaderUtil.ShaderPropertyType.Float:
												{
													mat.SetFloat( valuesArr[ 0 ], Convert.ToSingle( valuesArr[ 2 ] ) );
												}
												break;
												case ShaderUtil.ShaderPropertyType.Range:
												{
													mat.SetFloat( valuesArr[ 0 ], Convert.ToSingle( valuesArr[ 2 ] ) );
												}
												break;
												case ShaderUtil.ShaderPropertyType.TexEnv:
												{
													mat.SetTexture( valuesArr[ 0 ], AssetDatabase.LoadAssetAtPath<Texture>( valuesArr[ 2 ] ) );
												}
												break;
											}
										}
									}
								}
								catch ( Exception e )
								{
									Debug.LogError( e );
									validData = false;
								}


								if ( validData )
								{
									PropertiesChanged();
								}
								else
								{
									EditorPrefs.SetString( IOUtils.MAT_CLIPBOARD_ID, string.Empty );
								}
							}
						}
					}
					GUILayout.EndHorizontal();
					GUILayout.Space( 5 );
				}
				GUILayout.EndVertical();
			}
			EditorGUI.BeginChangeCheck();
			base.OnInspectorGUI();
			if ( EditorGUI.EndChangeCheck() )
			{
				UIUtils.CopyValuesFromMaterial( mat );
			}
		}
	}
}

