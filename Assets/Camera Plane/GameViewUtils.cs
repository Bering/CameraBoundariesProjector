using System.Reflection;
using UnityEditor;


/**
 * Inspired by http://answers.unity3d.com/questions/956123/add-and-select-game-view-resolution.html
 * from vexe http://answers.unity3d.com/users/146979/vexe.html
 * */


public static class GameViewUtils
{
	static object gameViewSizesInstance;
	static MethodInfo getGroup;

	public enum GameViewSizeType
	{
		AspectRatio,
		FixedResolution
	}

	public class GameViewSize
	{
		public GameViewSizeType sizeType;
		public string baseText;
		public string displayText;
		public int width;
		public int height;
	}


	static GameViewUtils ()
	{
		var sizesType = typeof(Editor).Assembly.GetType ("UnityEditor.GameViewSizes");
		var singleType = typeof(ScriptableSingleton<>).MakeGenericType (sizesType);
		getGroup = sizesType.GetMethod ("GetGroup");

		// gameViewSizesInstance  = ScriptableSingleton<GameViewSizes>.instance;
		var instanceProp = singleType.GetProperty ("instance");
		gameViewSizesInstance = instanceProp.GetValue (null, null);
	}


	public static GameViewSizeGroupType GetCurrentGroupType ()
	{
		var getCurrentGroupTypeProp = gameViewSizesInstance.GetType ().GetProperty ("currentGroupType");
		return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue (gameViewSizesInstance, null);
	}


	static object GetGroup (GameViewSizeGroupType type)
	{
		return getGroup.Invoke (gameViewSizesInstance, new object[] { (int)type });
	}


	static public GameViewSize[] GetGroupSizes (GameViewSizeGroupType sizeGroupType)
	{
		var group = GetGroup (sizeGroupType);

		var groupType = group.GetType ();

		var getTotalCount = groupType.GetMethod ("GetTotalCount");
		int count = (int)getTotalCount.Invoke (group, null);

		var getGameViewSize = groupType.GetMethod ("GetGameViewSize");
		var gvsType = getGameViewSize.ReturnType;

		var gameViewSizeTypeProp = gvsType.GetProperty ("sizeType");
		var widthProp = gvsType.GetProperty ("width");
		var heightProp = gvsType.GetProperty ("height");
		var baseTextProp = gvsType.GetProperty ("baseText");
		var displayTextProp = gvsType.GetProperty ("displayText");

		var indexValue = new object[1];

		GameViewSize gvs = null;
		GameViewSize[] list = new GameViewSize[count];

		for (int i = 0; i < count; i++) {
			indexValue [0] = i;
			var gvsTypeInstance = getGameViewSize.Invoke (group, indexValue);

			gvs = new GameViewSize ();

			gvs.sizeType = (GameViewSizeType)gameViewSizeTypeProp.GetValue (gvsTypeInstance, null);
			gvs.width = (int)widthProp.GetValue (gvsTypeInstance, null);
			gvs.height = (int)heightProp.GetValue (gvsTypeInstance, null);
			gvs.baseText = (string)baseTextProp.GetValue (gvsTypeInstance, null);
			gvs.displayText = (string)displayTextProp.GetValue (gvsTypeInstance, null);

			list [i] = gvs;
		}
		return list;
	}

}
