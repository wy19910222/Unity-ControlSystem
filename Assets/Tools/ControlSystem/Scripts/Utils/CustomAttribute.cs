/*
 * @Author: wangyun
 * @CreateTime: 2022-03-30 16:21:41 627
 * @LastEditor: wangyun
 * @EditTime: 2022-07-03 02:51:35 982
 */

using System;
using System.Diagnostics;
using UnityEngine;

namespace Control {
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class BlankAttribute : PropertyAttribute {
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class StateControllerSelectAttribute : PropertyAttribute {
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class ProgressControllerSelectAttribute : PropertyAttribute {
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class AnimatorParamSelectAttribute : PropertyAttribute {
		public AnimatorControllerParameterType Type { get; }
		public string AnimatorVarName { get; }
		public AnimatorParamSelectAttribute(string animatorVarName) {
			AnimatorVarName = animatorVarName;
		}
		public AnimatorParamSelectAttribute(string animatorVarName, AnimatorControllerParameterType type) {
			AnimatorVarName = animatorVarName;
			Type = type;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class SelfAnimatorParamSelectAttribute : PropertyAttribute {
		public AnimatorControllerParameterType Type { get; }
		public SelfAnimatorParamSelectAttribute() {
		}
		public SelfAnimatorParamSelectAttribute(AnimatorControllerParameterType type) {
			Type = type;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class ComponentSelectAttribute : PropertyAttribute {
		public bool ExceptSelf { get; }
		public Type[] ConstraintTypes { get; }
		public ComponentSelectAttribute(bool exceptSelf = false, params Type[] constraintTypes) {
			ExceptSelf = exceptSelf;
			ConstraintTypes = constraintTypes;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class ObjectSelectAttribute : PropertyAttribute {
		public bool ExceptSelf { get; }
		public Type[] ConstraintTypes { get; }
		public ObjectSelectAttribute(bool exceptSelf = false, params Type[] constraintTypes) {
			ExceptSelf = exceptSelf;
			ConstraintTypes = constraintTypes;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class CanResetCurveAttribute : PropertyAttribute {
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class TailSpaceAttribute : PropertyAttribute {
		public float Space { get; }
		public TailSpaceAttribute(float space) {
			Space = space;
		}
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class LimitedEditableAttribute : PropertyAttribute {
	}
	
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	[Conditional("UNITY_EDITOR")]
	public class AssetPathAttribute : PropertyAttribute {
		public Type mainType { get; }
		public Type[] constraintTypes { get; }
		public AssetPathAttribute(params Type[] types) {
			if (types.Length == 1) {
				mainType = types[0];
			} else if (types.Length > 1) {
				mainType = types[0];
				constraintTypes = types;
				while (mainType != null && Array.Exists(constraintTypes, type => !mainType.IsAssignableFrom(type))) {
					mainType = mainType.BaseType;
				}
			}
		}
	}
}