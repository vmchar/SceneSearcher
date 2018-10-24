using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SceneSearcher
{
	public class SceneSearcherGenerator
	{
		private int helper = 0;

		#region Public Methods

		public string GenerateSourceCode(ScenesData data, SceneSearcherPreferences preferences)
		{
			var menuItemTemplate = preferences.ProjectName + "/Scenes/";
			var scenesSorted = data.Scenes;
			scenesSorted = preferences.SceneOrder == SearcherSceneOrder.Alphabetic ?
				scenesSorted.OrderBy(x => x.SceneVisibleName.ToLower()).ToList() :
				scenesSorted.OrderBy(x => x.OrderNumber).ToList();
			
			var builder = new StringBuilder(GenerateHeader());
			foreach (var scene in scenesSorted)
			{
				builder.Append(MethodTemplate(menuItemTemplate + scene.SceneVisibleName, GenerateMethodName(), scene.SceneVisblePathInProject));
			}
			builder.Append(GenerateFooter());
			return builder.ToString();
		}
		
		#endregion
		
		#region Private Methods

		private string GenerateMethodName()
		{
			var timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			helper++;
			return string.Format("A" + timestamp.ToString() + helper.ToString());
		}

		private string GenerateHeader()
		{
			const string classHeader = "using UnityEditor; \n" +
			                           "using UnityEditor.SceneManagement; \n" +
			                           "using UnityEngine; \n \n " +
			                           "namespace SceneSearcher \n " +
			                           "{ \n " +
			                           "    public class SceneSearcherGeneratedClass : MonoBehaviour \n " +
			                           "    { \n ";

			return classHeader;
		}

		private string GenerateFooter()
		{
			var classFooter = "    } \n }";
			return classFooter;
		}


		private string MethodTemplate(string menuItemPath, string methodName, string scenePathInProject)
		{
			const string methodTemplate = "        [MenuItem(\"{0}\")] \n" +
			                              "        public static void {1}() \n" +
			                              "        { \n" +
			                              "             EditorSceneManager.OpenScene(\"{2}\"); \n" +
			                              "        } \n ";
			var builder = new StringBuilder(methodTemplate);
			builder.Replace("{0}", menuItemPath);
			builder.Replace("{1}", methodName);
			builder.Replace("{2}", scenePathInProject);

			return builder.ToString();

		}
		
		#endregion
	}
}
