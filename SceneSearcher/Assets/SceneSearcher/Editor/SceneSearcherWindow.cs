using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SceneSearcher
{
	public class SceneSearcherWindow : EditorWindow
	{
		#region Variables
		
		//File paths helpers
		private string _pathToPreferences;
		private const string PrefsFileName = "prefs.json";
		private const string DataFileName = "sceneData.json";
		private string _dataFilePath;
		private string _prefsFilePath;

		//Data models
		private SceneSearcherPreferences _preferences;
		private ScenesData _scenesData;
		private ScenesData _scenesInSelectedFolder = new ScenesData();
		private SceneModel _newSceneToAdd = new SceneModel();
		
		//Drawing Variables
		private bool _showPreferences;
		private bool _showData;
		private bool _showDataInSelectedFolder;
		private bool _showSelectFolder;
		private bool _showAddNewScene;
		private const string NoSceneSelected = "No scene selected";
		private Vector2 _globalScrollPosition;
		private Vector2 _folderScenesPosition;
		private Vector2 _dataScenesPosition;
		private string _processedSceneLabel = "";
		private string _scenesInFolderLabel = "";
		private string _selectedFolder = "";

		
		#endregion

		[MenuItem("Window/SceneSearcher")]
		static void Init()
		{
			var window = (SceneSearcherWindow)GetWindow(typeof(SceneSearcherWindow));			
			window.PrepareData();
			window.Show();
			var generator = new SceneSearcherGenerator();
		}

		public void PrepareData()
		{
			GetPluginData();
		}

		private void GetPluginData()
		{
			var seaercherFolder = AssetDatabase.FindAssets("SceneSearcher", null);
			string pluginPath = "";
			foreach (var s in seaercherFolder)
			{
				var file = AssetDatabase.GUIDToAssetPath(s);
				if(file.Contains('.')) continue;
				pluginPath = file;
			}
			pluginPath += "/";
			
			if (!Directory.Exists(pluginPath))
			{
				Debug.LogError("Directory " + pluginPath + " does not exist");
				return;
			}
			//Here all the services json files live.
			pluginPath = pluginPath + "Data/";
			var dataDirectory = !Directory.Exists(pluginPath) ?
				Directory.CreateDirectory(pluginPath) :
				new DirectoryInfo(pluginPath);

//			var files = dataDirectory.GetFiles("*.json");

			//Cache files paths
			_dataFilePath = dataDirectory.FullName + DataFileName;
			_prefsFilePath = dataDirectory.FullName + PrefsFileName;

			//Parse data about plugin preferences
			if (File.Exists(dataDirectory.FullName + PrefsFileName))
			{
				var fileContents = File.ReadAllText(_prefsFilePath);
				if(!string.IsNullOrEmpty(fileContents))
					_preferences = JsonUtility.FromJson<SceneSearcherPreferences>(fileContents);
			}
			if(_preferences == null) _preferences = new SceneSearcherPreferences();

			//Parse data about already cached scenes
			if (File.Exists(dataDirectory.FullName + DataFileName))
			{
				var fileContents = File.ReadAllText(_dataFilePath);
				if (!string.IsNullOrEmpty(fileContents))
					_scenesData = JsonUtility.FromJson<ScenesData>(fileContents);
			}
			if(_scenesData == null) _scenesData = new ScenesData();

		}

		private void OnGUI()
		{
//			_globalScrollPosition = GUILayout.BeginScrollView(_globalScrollPosition);
			DrawPreferences();
			DrawData();
			AddNewScene();
			
			DrawSelectFolderToSearchScenes();
			
//			GUILayout.EndScrollView();
			GUILayout.FlexibleSpace();
			DrawFunctionButtons();
		}

		private void GuiLine( int i_height = 1 )
		{
			var rect = EditorGUILayout.GetControlRect(false, i_height );
			rect.height = i_height;
			EditorGUI.DrawRect(rect, new Color ( 0f,0f,0f, 1 ) );
		}

		private void DrawSelectFolderToSearchScenes()
		{
			_scenesInFolderLabel = "Add scenes in folder (" + _scenesInSelectedFolder.Count + ")";
			_showSelectFolder = EditorGUILayout.Foldout(_showSelectFolder, _scenesInFolderLabel, true, EditorStyles.boldLabel);
			if (!_showSelectFolder) return;
			
			GuiLine();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			
			EditorGUILayout.LabelField("Select folder to search scenes automatically. All found scenes in this folder will be processed.");
			
			EditorGUILayout.BeginHorizontal();
			
			//Select folder and recusrively scan contents to find scenes
			if (GUILayout.Button("Select Folder"))
			{
				var path = EditorUtility.OpenFolderPanel("Select folder to scan", Application.dataPath, "");
				var sceneFilesInDirectory = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories);
				_selectedFolder = path;
				foreach (var file in sceneFilesInDirectory)
				{
					var sceneModel = new SceneModel(file);
					_scenesInSelectedFolder.Scenes.Add(sceneModel);
				}
				
			}
			
			EditorGUILayout.LabelField("Selected folder : " + _selectedFolder);
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.LabelField("Scenes in selected folder : " + _scenesInSelectedFolder.Count);

			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Add Selected Scenes"))
			{
				var processedScenesDictionary = _scenesData.Scenes.ToDictionary(x => x.ScenePathInProject, x => x);
				foreach (var scene in _scenesInSelectedFolder.Scenes)
				{
					if(!processedScenesDictionary.ContainsKey(scene.ScenePathInProject))
						processedScenesDictionary.Add(scene.ScenePathInProject, scene);
				}
				_scenesData.Scenes = processedScenesDictionary.Values.ToList();
				_scenesInSelectedFolder.Scenes.Clear();
			}
			
			if (GUILayout.Button("Reset Selection"))
			{
				if (EditorUtility.DisplayDialog("Reset selected folder?", "Are you sure you want to reset selected folder?", "YES",
					"NO"))
				{
					_selectedFolder = "";
					_scenesInSelectedFolder = new ScenesData();
				}
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			_showDataInSelectedFolder = EditorGUILayout.Foldout(_showDataInSelectedFolder, "Scenes in selected folder", true, EditorStyles.boldLabel);
			if (_showDataInSelectedFolder && _scenesInSelectedFolder.Count != 0)
			{
				_folderScenesPosition = GUILayout.BeginScrollView(_folderScenesPosition);

				foreach (var scene in _scenesInSelectedFolder.Scenes)
				{
					DrawSceneModel(scene, SceneDataLocation.FromSelectedFolder);
				}
				GUILayout.EndScrollView();
			}
		}

		private void WriteToFile(string pathToFile, string contents)
		{
			var fCreate = File.Open(pathToFile, FileMode.Create);
			var stream = new StreamWriter(fCreate);
			
			try { stream.Write(contents); }
			catch (Exception exception) { Debug.LogError(exception); }
			finally { stream.Close(); }
			
			AssetDatabase.Refresh();
		}

		private void SaveDataAndPrefs()
		{
			if (!_scenesData.ValidateSelf())
			{
				EditorUtility.DisplayDialog("Data error",
					"Some non optional fields are empty. Please fill them and try to save changes again.", "OK");
			}
			else
			{
				var prefsJson = JsonUtility.ToJson(_preferences);
				var dataJson = JsonUtility.ToJson(_scenesData);
				if (!(string.IsNullOrEmpty(prefsJson) || string.IsNullOrEmpty(dataJson)))
				{
					WriteToFile(pathToFile:_prefsFilePath, contents:prefsJson);
					WriteToFile(pathToFile:_dataFilePath, contents:dataJson);
				}
			}
		}

		private void DrawFunctionButtons()
		{
			GuiLine();
			EditorGUILayout.BeginHorizontal();
			
			//Validate data, then write it to file
			if (GUILayout.Button("Save Changes"))
			{
				SaveDataAndPrefs();
			}

			//Reset all plugin data
			if (GUILayout.Button("Remove All Scenes"))
			{
				if (EditorUtility.DisplayDialog("Remove all scenes?", "Are you sure you want to remove all scenes from the menu?", "YES",
					"NO"))
				{
					_scenesData.Scenes.Clear();
					SaveDataAndPrefs();
					//Remove generated source file
					var files = AssetDatabase.FindAssets("SceneSearcherWindow", null);
					if (files.Length != 0)
					{
						var thisScriptPath = AssetDatabase.GUIDToAssetPath(files[0]);
						var pathToFile = Path.GetDirectoryName(thisScriptPath);
						var fileName = "SceneSearcherGeneratedClass.cs";
						File.Delete(pathToFile + "/" + fileName);
						AssetDatabase.Refresh();
					}
				}
			}

			if (GUILayout.Button("Generate Menu"))
			{
				var generator = new SceneSearcherGenerator();
				if (_scenesData != null && !(_scenesData.Scenes.Count == 0 | _preferences == null))
				{
					var sourceText = generator.GenerateSourceCode(_scenesData, _preferences);

					var files = AssetDatabase.FindAssets("SceneSearcherWindow", null);
					if (files.Length != 0)
					{
						var thisScriptPath = AssetDatabase.GUIDToAssetPath(files[0]);
						var pathToSave = Path.GetDirectoryName(thisScriptPath);
						var fileName = "SceneSearcherGeneratedClass.cs";
						WriteToFile(pathToSave + "/" + fileName, sourceText);
						AssetDatabase.Refresh();
					}
				}
			}
			
			EditorGUILayout.EndHorizontal();
			
			GuiLine();
		}

		private void DrawPreferences()
		{
			_showPreferences = EditorGUILayout.Foldout(_showPreferences, "Preferences", true, EditorStyles.boldLabel);
			if(!_showPreferences) return;

			EditorGUILayout.LabelField("Name of the top panel where menu items will be groupped");
			_preferences.ProjectName = EditorGUILayout.TextField("Menu Item Top Name", _preferences.ProjectName);
			_preferences.SceneOrder = (SearcherSceneOrder)EditorGUILayout.EnumPopup("How to order scenes:", _preferences.SceneOrder);

		}

		/// <summary>
		/// Draw the list of scenes, already processed by the plugin
		/// </summary>
		private void DrawData()
		{
			_processedSceneLabel = "Processd Scenes (" + _scenesData.Count + ")";
			_showData = EditorGUILayout.Foldout(_showData, _processedSceneLabel, true, EditorStyles.boldLabel);
			if(!_showData || _scenesData.Count == 0) return;
			
			_dataScenesPosition = GUILayout.BeginScrollView(_dataScenesPosition);
			foreach (var scene in _scenesData.Scenes)
			{
				DrawSceneModel(scene, SceneDataLocation.FromData);
			}
			GuiLine();
			GUILayout.EndScrollView();
		}

		/// <summary>
		/// Base Method to draw a scene data model.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="dataLocation"></param>
		private void DrawSceneModel(SceneModel model, SceneDataLocation dataLocation)
		{
			GuiLine();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
			
			if(string.IsNullOrEmpty(model.SceneName))
				EditorGUILayout.LabelField("Name of the new scene : " + NoSceneSelected);
			else
				EditorGUILayout.LabelField("Name of the new scene : " + model.SceneName);

			if (dataLocation != SceneDataLocation.None)
			{
				if (GUILayout.Button("Remove scene"))
				{
					if (dataLocation == SceneDataLocation.FromData)
					{
						var scenes = new List<SceneModel>(_scenesData.Scenes);
						scenes.Remove(model);
						_scenesData.Scenes = scenes;
					}

					if (dataLocation == SceneDataLocation.FromSelectedFolder)
					{
						var scenes = new List<SceneModel>(_scenesInSelectedFolder.Scenes);
						scenes.Remove(model);
						_scenesInSelectedFolder.Scenes = scenes;
					}
				}
			}
			
			EditorGUILayout.EndHorizontal();	

			EditorGUILayout.BeginHorizontal();
			
			if(string.IsNullOrEmpty(model.ScenePathInProject))
				EditorGUILayout.LabelField("Path to scene : " + NoSceneSelected);
			else 
				EditorGUILayout.LabelField("Path to scene : " + model.SceneVisblePathInProject);

			if (GUILayout.Button("Select Scene"))
			{
				var pathToNewScene = EditorUtility.OpenFilePanel("Select Scene To Add", Application.dataPath, "unity");
				if (!string.IsNullOrEmpty(pathToNewScene))
				{

					//Check if scene with the same path alredy presents in list
					if (_scenesData.Scenes.Any(x => x.ScenePathInProject == pathToNewScene))
					{
						EditorUtility.DisplayDialog("Duplicate scene", "Scene with path: " + pathToNewScene + " already presenst in data",
							"OK");
						return;
					}
					else
					{
						var testSceneName = Path.GetFileNameWithoutExtension(pathToNewScene);
						Debug.Log(testSceneName);

						model.SceneName = testSceneName;
						model.ScenePathInProject = pathToNewScene;
					}
				}
			}
			
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			model.SceneVisibleName =
				EditorGUILayout.TextField("Your alias for the scene : ", model.SceneVisibleName);

			model.OrderNumber = EditorGUILayout.IntField("Order number among menu items : ", model.OrderNumber);
			
			EditorGUILayout.EndHorizontal();

		}

		private void AddNewScene()
		{
			_showAddNewScene = EditorGUILayout.Foldout(_showAddNewScene, "Add New Scene", true, EditorStyles.boldLabel);
			if (!_showAddNewScene) return;
				
			DrawSceneModel(_newSceneToAdd, SceneDataLocation.None);

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Add a scene"))
			{
				//Validate new scene model, add and reset it
				if (_newSceneToAdd.ValidateSelf())
				{
					_scenesData.Scenes.Add(_newSceneToAdd);
					_newSceneToAdd = new SceneModel();
				}
				else
				{
					EditorUtility.DisplayDialog("Error adding scene", "Some fields are emty or incorrect. Please fix it.", "OK");
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			if (GUILayout.Button("Reset All Fields"))
			{
				if (EditorUtility.DisplayDialog("Reset fields?", "Are you sure you want to reset all fields of the new scene?",
						"YES",
						"NO"))
				{
					_newSceneToAdd = new SceneModel();
				}
				
			}

			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			GuiLine();
			
		}

	}
}
