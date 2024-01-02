/*
 * @Author: wangyun
 * @CreateTime: 2022-04-28 00:41:42 075
 * @LastEditor: wangyun
 * @EditTime: 2022-04-28 00:41:42 079
 */

#if SPINE_EXIST

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Spine;
using Spine.Unity;

namespace Control {
	[CustomPropertyDrawer(typeof(SelfSkeletonAnimationNameSelectAttribute))]
	public class SelfSkeletonAnimationNameSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = property.serializedObject.targetObject as Component;
			if (!target) {
				EditorGUI.PropertyField(position, property, label);
				return;
			}
			
			SkeletonAnimation anim = target.GetComponent<SkeletonAnimation>();
			List<string> names = new List<Spine.Animation>(anim.skeleton.Data.Animations).ConvertAll(animation => animation.Name);
			List<string> options = new List<string>(names);
			names.Insert(0, string.Empty);
			options.Insert(0, "无");
			int index = names.IndexOf(property.stringValue);
			int newIndex = EditorGUI.Popup(position, label.text, index, options.ToArray());
			if (newIndex != index) {
				property.stringValue = names[newIndex];
			}
		}
	}
	
	[CustomPropertyDrawer(typeof(SkeletonAnimationNameSelectAttribute))]
	public class SkeletonAnimationNameSelectDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			Component target = property.serializedObject.targetObject as Component;
			if (!target) {
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			string[] fieldNames = ((SkeletonAnimationNameSelectAttribute) attribute).FieldNames;
			Type type = target.GetType();
			foreach (var fieldName in fieldNames) {
				FieldInfo fi = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				object value = fi?.GetValue(target);
				if (value != null) {
					SkeletonData data = null;
					switch (value) {
						case SkeletonAnimation sa:
							data = sa.skeleton?.Data;
							break;
						case SkeletonGraphic sg:
							data = sg.SkeletonData;
							break;
					}
					List<string> names = data == null ? new List<string>() : new List<Spine.Animation>(data.Animations).ConvertAll(animation => animation.Name);
					
					EditorGUI.BeginDisabledGroup(names.Count <= 0);
					
					List<string> options = new List<string>(names);
					names.Insert(0, string.Empty);
					options.Insert(0, "无");
					int index = names.IndexOf(property.stringValue);
					int newIndex = EditorGUI.Popup(position, label.text, index, options.ToArray());
					if (newIndex != index) {
						property.stringValue = names[newIndex];
					}
					
					EditorGUI.EndDisabledGroup();

					return;
				}
			}
			EditorGUI.PropertyField(position, property, label);
		}
	}
}

#endif