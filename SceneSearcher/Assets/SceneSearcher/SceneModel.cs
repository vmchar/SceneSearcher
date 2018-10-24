using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SceneSearcher
{
	[Serializable]
	public class SceneModel
	{
		/// <summary>
		/// Name of the file itself without extension
		/// </summary>
		public string SceneName = "";
		
		/// <summary>
		/// Alias, which user will see in the menu title
		/// </summary>
		public string SceneVisibleName = "";
		
		/// <summary>
		/// Path to file in Unity project
		/// </summary>
		public string ScenePathInProject = "";

		/// <summary>
		/// Path 
		/// </summary>
		public string SceneVisblePathInProject
		{
			get
			{
				if (string.IsNullOrEmpty(ScenePathInProject)) return "";
				var projectPath = ScenePathInProject.Split(new string[] {"/Assets"}, StringSplitOptions.None).LastOrDefault();
				return "Assets" + projectPath;
			}
		}
		
		/// <summary>
		/// Order number iside ScenesData.
		/// Affects only the order in which 
		/// menu items will be drawn
		/// </summary>
		public int OrderNumber;
		
		public SceneModel() { }

		public SceneModel(string path)
		{
			ScenePathInProject = path;
			SceneName = Path.GetFileNameWithoutExtension(path);
		}
		
		/// <summary>
		/// Check if all field filled
		/// </summary>
		/// <returns></returns>
		public bool ValidateSelf()
		{
			if (string.IsNullOrEmpty(SceneName))
				return false;
			if (string.IsNullOrEmpty(ScenePathInProject))
				return false;
			if (string.IsNullOrEmpty(SceneVisibleName))
				SceneVisibleName = SceneName;
			return true;
		}
	}

	[System.Serializable]
	public class ScenesData
	{
		[UnityEngine.SerializeField]
		public List<SceneModel> Scenes = new List<SceneModel>();

		public int Count
		{
			get { return Scenes.Count; }
		}

		public bool ValidateSelf()
		{
			if (Scenes.Any(x => string.IsNullOrEmpty(x.ScenePathInProject)))
				return false;

			if (Scenes.Any(x => string.IsNullOrEmpty(x.SceneName)))
				return false;

			//Fix empty aliased
			var scenesWithEmptyAlias = Scenes.Where(x => string.IsNullOrEmpty(x.SceneVisibleName)).ToList();
			if (scenesWithEmptyAlias.Any())
			{
				foreach (var scene in scenesWithEmptyAlias)
					scene.SceneVisibleName = scene.SceneName;
			}
			
			var numberOfUniquePaths = Scenes.GroupBy(x => x.ScenePathInProject).Select(x => x.FirstOrDefault()).Count();
			return numberOfUniquePaths == Scenes.Count;
		}
	}
}
