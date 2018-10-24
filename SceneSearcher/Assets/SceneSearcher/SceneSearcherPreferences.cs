namespace SceneSearcher
{
	public enum SceneDataLocation
	{
		None,
		FromData,
		FromSelectedFolder
	}

	[System.Serializable]
	public enum SearcherSceneOrder
	{
		Numeric,
		Alphabetic
	}
	
	[System.Serializable]
	public class SceneSearcherPreferences
	{
		/// <summary>
		/// Menu item top name
		/// </summary>
		public string ProjectName;

		/// <summary>
		/// How to order
		/// </summary>
		public SearcherSceneOrder SceneOrder;
		
		public string HelperJsonFilePath;
	}
}
