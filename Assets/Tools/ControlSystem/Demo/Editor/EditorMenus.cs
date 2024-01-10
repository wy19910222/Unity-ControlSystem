using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public static class GameObjectMenu {
	[MenuItem("GameObject/UI/New Text #&t", false, -1)]
	public static void AddText(MenuCommand menuCommand) {
		GameObject go = new GameObject("Text");
		Undo.RegisterCreatedObjectUndo(go, "NewText");
		
		RectTransform trans = go.AddComponent<RectTransform>();
		if (Selection.activeTransform) {
			Undo.SetTransformParent(trans, Selection.activeTransform, "NewText");
			trans.localPosition = Vector3.zero;
			trans.localRotation = Quaternion.identity;
			trans.localScale = Vector3.one;
		}
		Selection.activeTransform = trans;
		
		trans.sizeDelta = new Vector2(160, 30);
		UnityEngine.UI.Text text = go.AddComponent<UnityEngine.UI.Text>();
		text.raycastTarget = false;
		text.maskable = false;
		text.text = "New Text";
		text.fontSize = 24;
		text.color = Color.white;
		text.supportRichText = false;
		text.alignment = TextAnchor.MiddleCenter;
	}

	[MenuItem("GameObject/UI/New Image #&s", false, -1)]
	public static void AddImage(MenuCommand menuCommand) {
		GameObject go = new GameObject("Image");
		Undo.RegisterCreatedObjectUndo(go, "NewImage");
		
		RectTransform trans = go.AddComponent<RectTransform>();
		if (Selection.activeTransform) {
			Undo.SetTransformParent(trans, Selection.activeTransform, "NewImage");
			trans.localPosition = Vector3.zero;
			trans.localRotation = Quaternion.identity;
			trans.localScale = Vector3.one;
		}
		Selection.activeTransform = trans;
		
		UnityEngine.UI.Image image = go.AddComponent<UnityEngine.UI.Image>();
		image.raycastTarget = false;
		image.maskable = false;
	}

	public class TestClass {
		private int iValue1;
		protected int iValue2;
		public int iValue3;
	}

	public class TestClass1 : TestClass {
		private float fValue1;
		protected float fValue2;
		public float fValue3;
	}

	[MenuItem("GameObject/Test", false, -1)]
	public static void Test() {
		if (Selection.activeObject is MonoScript script) {
			Debug.LogError(script.GetClass());
		}
	}
}