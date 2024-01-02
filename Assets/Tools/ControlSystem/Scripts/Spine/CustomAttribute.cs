/*
 * @Author: wangyun
 * @CreateTime: 2022-04-28 00:41:51 749
 * @LastEditor: wangyun
 * @EditTime: 2022-04-28 00:41:51 753
 */

#if SPINE_EXIST

using System;
using UnityEngine;

namespace Control {
	public class SelfSkeletonAnimationNameSelectAttribute : PropertyAttribute {
	}
	public class SkeletonAnimationNameSelectAttribute : PropertyAttribute {
		public string[] FieldNames { get; }
		public SkeletonAnimationNameSelectAttribute(params string[] fieldNames) {
			FieldNames = fieldNames;
		}
	}
}

#endif