// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;

namespace AmplifyShaderEditor
{
	public enum ShaderLoadResult
	{
		LOADED,
		FILE_NOT_FOUND,
		ASE_INFO_NOT_FOUND,
		UNITY_NATIVE_PATHS
	}

	public class Worker
	{
		public static readonly object locker = new object();
		public void DoWork()
		{
			while ( IOUtils.ActiveThread )
			{
				if ( IOUtils.SaveInThreadFlag )
				{
					IOUtils.SaveInThreadFlag = false;
					lock ( locker )
					{
						IOUtils.SaveInThreadShaderBody = IOUtils.ShaderCopywriteMessage + IOUtils.SaveInThreadShaderBody;
						// Add checksum 
						string checksum = IOUtils.CreateChecksum( IOUtils.SaveInThreadShaderBody );
						IOUtils.SaveInThreadShaderBody += IOUtils.CHECKSUM + IOUtils.VALUE_SEPARATOR + checksum;

						// Write to disk
						StreamWriter fileWriter = new StreamWriter( IOUtils.SaveInThreadPathName );
						try
						{
							fileWriter.Write( IOUtils.SaveInThreadShaderBody );
							Debug.Log( "Saving complete" );
						}
						catch ( Exception e )
						{
							Debug.LogError( e );
						}
						finally
						{
							fileWriter.Close();
						}
					}
				}
			}
			Debug.Log( "Thread closed" );
		}
	}

	public static class IOUtils
	{
		public static readonly string ShaderCopywriteMessage = "// Made with Amplify Shader Editor\n// Available at the Unity Asset Store - http://u3d.as/y3X \n";
		public static readonly string GrabPassEmpty = "\t\tGrabPass{ }\n";
		public static readonly string GrabPassBegin = "\t\tGrabPass{ \"";
		public static readonly string GrabPassEnd = "\" }\n";
		public static readonly string PropertiesBegin = "\tProperties\n\t{\n";
		public static readonly string PropertiesEnd = "\t}\n";
		public static readonly string PropertiesElement = "\t\t{0}\n";

		public static readonly string PragmaTargetHeader = "\t\t#pragma target 3.0\n";
		public static readonly string InstancedPropertiesHeader = "\t\t#pragma multi_compile_instancing\n";

		public static readonly string InstancedPropertiesBegin = "\t\tUNITY_INSTANCING_CBUFFER_START({0})\n";
		public static readonly string InstancedPropertiesEnd = "\t\tUNITY_INSTANCING_CBUFFER_END\n";
		public static readonly string InstancedPropertiesElement = "\t\t\tUNITY_DEFINE_INSTANCED_PROP({0}, {1})\n";
		public static readonly string InstancedPropertiesData = "UNITY_ACCESS_INSTANCED_PROP({0})";

		public static readonly string MetaBegin = "defaultTextures:";
		public static readonly string MetaEnd = "userData:";
		public static readonly string ShaderBodyBegin = "/*ASEBEGIN";
		public static readonly string ShaderBodyEnd = "ASEEND*/";
		//public static readonly float CurrentVersionFlt = 0.4f;
		//public static readonly string CurrentVersionStr = "Version=" + CurrentVersionFlt;

		public static readonly string CHECKSUM = "//CHKSM";
		public static readonly string LAST_OPENED_OBJ_ID = "ASELASTOPENOBJID";

		public static readonly string MAT_CLIPBOARD_ID = "ASEMATCLIPBRDID";
		public static readonly char FIELD_SEPARATOR = ';';
		public static readonly char VALUE_SEPARATOR = '=';
		public static readonly char LINE_TERMINATOR = '\n';
		public static readonly char VECTOR_SEPARATOR = ',';
		public static readonly char CLIPBOARD_DATA_SEPARATOR = '|';
		public static readonly char MATRIX_DATA_SEPARATOR = '|';
		public readonly static string NO_TEXTURES = "<None>";
		public static readonly string SaveShaderStr = "Please enter shader name to save";
		public static readonly string FloatifyStr = ".0";

		// Node parameter names
		public const string NodeParam = "Node";
		public const string NodePosition = "Position";
		public const string NodeId = "Id";
		public const string NodeType = "Type";
		public const string WireConnectionParam = "WireConnection";

		public static readonly uint NodeTypeId = 1;

		public static readonly int InNodeId = 1;
		public static readonly int InPortId = 2;
		public static readonly int OutNodeId = 3;
		public static readonly int OutPortId = 4;

		public readonly static string DefaultASEDirtyCheckName = "__dirty";
		public readonly static string DefaultASEDirtyCheckProperty = "[HideInInspector] " + DefaultASEDirtyCheckName + "( \"\", Int ) = 1";
		public readonly static string DefaultASEDirtyCheckUniform = "uniform int " + DefaultASEDirtyCheckName + " = 1;";

		public readonly static string MaskClipValueName = "_MaskClipValue";
		public readonly static string MaskClipValueProperty = MaskClipValueName + "( \"{0}\", Float ) = {1}";
		public readonly static string MaskClipValueUniform = "uniform float " + MaskClipValueName + " = {0};";


		public static int DefaultASEDirtyCheckId;

		// this is to be used in combination with AssetDatabase.GetAssetPath, both of these include the Assets/ path so we need to remove from one of them 
		public static string dataPath;


		////////////////////////////////////////////////////////////////////////////
		// THREAD IO UTILS
		public static bool SaveInThreadFlag = false;
		public static string SaveInThreadShaderBody;
		public static string SaveInThreadPathName;
		public static Thread SaveInThreadMainThread;
		public static bool ActiveThread = true;
		private static bool UseSaveThread = false;

		public static void StartSaveThread( string shaderBody, string pathName )
		{
			if ( UseSaveThread )
			{
				if ( !SaveInThreadFlag )
				{
					if ( SaveInThreadMainThread == null )
					{
						Worker worker = new Worker();
						SaveInThreadMainThread = new Thread( worker.DoWork );
						SaveInThreadMainThread.Start();
						Debug.Log( "Thread created" );
					}

					SaveInThreadShaderBody = shaderBody;
					SaveInThreadPathName = pathName;
					SaveInThreadFlag = true;
				}
			}
			else
			{
				SaveTextfileToDisk( shaderBody, pathName );
			}
		}

		////////////////////////////////////////////////////////////////////////////

		public static void Init()
		{
			DefaultASEDirtyCheckId = Shader.PropertyToID( DefaultASEDirtyCheckName );
			dataPath = Application.dataPath.Remove( Application.dataPath.Length - 6 );
		}

		public static void Destroy()
		{
			ActiveThread = false;
			if ( SaveInThreadMainThread != null )
			{
				SaveInThreadMainThread.Abort();
				SaveInThreadMainThread = null;
			}
		}

		public static void GetShaderName( out string shaderName, out string fullPathname, string defaultName, string customDatapath )
		{
			string currDatapath = String.IsNullOrEmpty( customDatapath ) ? Application.dataPath : customDatapath;
			fullPathname = EditorUtility.SaveFilePanelInProject( "Select Shader to save", defaultName, "shader", SaveShaderStr, currDatapath );
			if ( !String.IsNullOrEmpty( fullPathname ) )
			{
				shaderName = fullPathname.Remove( fullPathname.Length - 7 ); // -7 remove .shader extension
				string[] subStr = shaderName.Split( '/' );
				if ( subStr.Length > 0 )
				{
					shaderName = subStr[ subStr.Length - 1 ]; // Remove pathname 
				}
			}
			else
			{
				shaderName = string.Empty;
			}
		}

		public static void AddTypeToString( ref string myString, string typeName )
		{
			myString += typeName;
		}

		public static void AddFieldToString( ref string myString, string fieldName, object fieldValue )
		{
			myString += FIELD_SEPARATOR + fieldName + VALUE_SEPARATOR + fieldValue;
		}

		public static void AddFieldValueToString( ref string myString, object fieldValue )
		{
			myString += FIELD_SEPARATOR + fieldValue.ToString();
		}

		public static void AddLineTerminator( ref string myString )
		{
			myString += LINE_TERMINATOR;
		}

		public static string CreateChecksum( string buffer )
		{
			SHA1 sha1 = SHA1.Create();
			byte[] buf = System.Text.Encoding.UTF8.GetBytes( buffer );
			byte[] hash = sha1.ComputeHash( buf, 0, buf.Length );
			string hashstr = BitConverter.ToString( hash ).Replace( "-", "" );
			return hashstr;
		}

		public static void SaveTextfileToDisk( string shaderBody, string pathName )
		{
			shaderBody = ShaderCopywriteMessage + shaderBody;
			// Add checksum 
			string checksum = CreateChecksum( shaderBody );
			shaderBody += CHECKSUM + VALUE_SEPARATOR + checksum;

			// Write to disk
			StreamWriter fileWriter = new StreamWriter( pathName );
			try
			{
				fileWriter.Write( shaderBody );
			}
			catch ( Exception e )
			{
				Debug.LogError( e );
			}
			finally
			{
				fileWriter.Close();
			}
		}

		public static string LoadTextFileFromDisk( string pathName )
		{
			string result = string.Empty;
			StreamReader fileReader = new StreamReader( pathName );
			try
			{
				result = fileReader.ReadToEnd();
			}
			catch ( Exception e )
			{
				Debug.LogError( e );
			}
			finally
			{
				fileReader.Close();
			}
			return result;
		}
		
		public static bool IsASEShader( Shader shader )
		{
			string datapath = AssetDatabase.GetAssetPath( shader );
			if ( UIUtils.IsUnityNativeShader( datapath ) )
			{
				return false;
			}

			string buffer = LoadTextFileFromDisk( datapath );
			if ( String.IsNullOrEmpty( buffer ) || !IOUtils.HasValidShaderBody( ref buffer ) )
			{
				return false;
			}
			return true;
		}

		public static bool HasValidShaderBody( ref string shaderBody )
		{
			int shaderBodyBeginId = shaderBody.IndexOf( ShaderBodyBegin );
			if ( shaderBodyBeginId > -1 )
			{
				int shaderBodyEndId = shaderBody.IndexOf( ShaderBodyEnd );
				return ( shaderBodyEndId > -1 && shaderBodyEndId > shaderBodyBeginId );
			}
			return false;
		}

		public static int[] AllIndexesOf( this string str, string substr, bool ignoreCase = false )
		{
			if ( string.IsNullOrEmpty( str ) || string.IsNullOrEmpty( substr ) )
			{
				throw new ArgumentException( "String or substring is not specified." );
			}

			List<int> indexes = new List<int>();
			int index = 0;

			while ( ( index = str.IndexOf( substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal ) ) != -1 )
			{
				indexes.Add( index++ );
			}

			return indexes.ToArray();
		}

		public static void AddFunctionHeader( ref string function, string header )
		{
			function += "\t\t" + header + "\n\t\t{\n";
		}

		public static void AddFunctionLine( ref string function, string line )
		{
			function += "\t\t\t" + line + "\n";
		}

		public static void CloseFunctionBody( ref string function )
		{
			function += "\t\t}\n";
		}

		public static string GetUVChannelDeclaration( string uvName, int channelId, int set )
		{
			string uvSetStr = ( set == 0 ) ? "uv" : "uv2";
			return "float2 " + uvSetStr + uvName /*+ " : TEXCOORD" + channelId*/;
		}

		public static string GetUVChannelName( string uvName, int set )
		{
			string uvSetStr = ( set == 0 ) ? "uv" : "uv2";
			return uvSetStr + uvName;
		}

		public static string Floatify( float value )
		{
			return ( value % 1 ) != 0 ? value.ToString() : ( value.ToString() + FloatifyStr );
		}
	}
}
