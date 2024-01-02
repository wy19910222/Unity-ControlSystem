/*
 * @Author: wangyun
 * @CreateTime: 2023-01-20 04:11:57 802
 * @LastEditor: wangyun
 * @EditTime: 2023-01-20 04:11:57 814
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public static class CurveLerpUtils {
		public static ParticleSystem.MinMaxCurve MinMaxCurveLerpUnclamped(ParticleSystem.MinMaxCurve from, ParticleSystem.MinMaxCurve to, float time) {
			if (Mathf.Approximately(time, 0)) {
				return from;
			}
			if (Mathf.Approximately(time, 1)) {
				return to;
			}
			switch (from.mode) {
				case ParticleSystemCurveMode.Constant:
					switch (to.mode) {
						case ParticleSystemCurveMode.Constant:
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.Constant,
								constant = Mathf.LerpUnclamped(from.constant, to.constant, time)
							};
						case ParticleSystemCurveMode.Curve: {
							Keyframe[] keys = ConstToCurveLerpUnclamped(from.constant, to.curve, to.curveMultiplier, time);
							float multiplier = Normalize(keys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.Curve,
								curve = new AnimationCurve(keys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.TwoCurves: {
							Keyframe[] minKeys = ConstToCurveLerpUnclamped(from.constant, to.curveMin, to.curveMultiplier, time);
							Keyframe[] maxKeys = ConstToCurveLerpUnclamped(from.constant, to.curveMax, to.curveMultiplier, time);
							float multiplier = Normalize(minKeys, maxKeys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoCurves,
								curveMin = new AnimationCurve(minKeys),
								curveMax = new AnimationCurve(maxKeys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.TwoConstants:
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoConstants,
								constantMin = Mathf.LerpUnclamped(from.constant, to.constantMin, time),
								constantMax = Mathf.LerpUnclamped(from.constant, to.constantMax, time)
							};
						default:
							throw new ArgumentOutOfRangeException();
					}
				case ParticleSystemCurveMode.Curve:
					switch (to.mode) {
						case ParticleSystemCurveMode.Constant: {
							Keyframe[] keys = ConstToCurveLerpUnclamped(to.constant, from.curve, from.curveMultiplier, 1 - time);
							float multiplier = Normalize(keys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.Curve,
								curve = new AnimationCurve(keys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.Curve: {
							Keyframe[] keys = CurveToCurveLerpUnclamped(from.curve, from.curveMultiplier, to.curve, to.curveMultiplier, time);
							float multiplier = Normalize(keys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.Curve,
								curve = new AnimationCurve(keys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.TwoCurves: {
							Keyframe[] minKeys = CurveToCurveLerpUnclamped(from.curve, from.curveMultiplier, to.curveMin, to.curveMultiplier, time);
							Keyframe[] maxKeys = CurveToCurveLerpUnclamped(from.curve, from.curveMultiplier, to.curveMax, to.curveMultiplier, time);
							float multiplier = Normalize(minKeys, maxKeys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoCurves,
								curveMin = new AnimationCurve(minKeys),
								curveMax = new AnimationCurve(maxKeys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.TwoConstants: {
							Keyframe[] minKeys = ConstToCurveLerpUnclamped(to.constantMin, from.curve, from.curveMultiplier, 1 - time);
							Keyframe[] maxKeys = ConstToCurveLerpUnclamped(to.constantMax, from.curve, from.curveMultiplier, 1 - time);
							float multiplier = Normalize(minKeys, maxKeys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoCurves,
								curveMin = new AnimationCurve(minKeys),
								curveMax = new AnimationCurve(maxKeys),
								curveMultiplier = multiplier
							};
						}
						default:
							throw new ArgumentOutOfRangeException();
					}
				case ParticleSystemCurveMode.TwoCurves:
					switch (to.mode) {
						case ParticleSystemCurveMode.Constant: {
							Keyframe[] minKeys = ConstToCurveLerpUnclamped(to.constant, from.curveMin, from.curveMultiplier, 1 - time);
							Keyframe[] maxKeys = ConstToCurveLerpUnclamped(to.constant, from.curveMax, from.curveMultiplier, 1 - time);
							float multiplier = Normalize(minKeys, maxKeys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoCurves,
								curveMin = new AnimationCurve(minKeys),
								curveMax = new AnimationCurve(maxKeys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.Curve: {
							Keyframe[] minKeys = CurveToCurveLerpUnclamped(from.curveMin, from.curveMultiplier, to.curve, to.curveMultiplier, time);
							Keyframe[] maxKeys = CurveToCurveLerpUnclamped(from.curveMax, from.curveMultiplier, to.curve, to.curveMultiplier, time);
							float multiplier = Normalize(minKeys, maxKeys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoCurves,
								curveMin = new AnimationCurve(minKeys),
								curveMax = new AnimationCurve(maxKeys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.TwoCurves: {
							Keyframe[] minKeys = CurveToCurveLerpUnclamped(from.curveMin, from.curveMultiplier, to.curveMin, to.curveMultiplier, time);
							Keyframe[] maxKeys = CurveToCurveLerpUnclamped(from.curveMax, from.curveMultiplier, to.curveMax, to.curveMultiplier, time);
							float multiplier = Normalize(minKeys, maxKeys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoCurves,
								curveMin = new AnimationCurve(minKeys),
								curveMax = new AnimationCurve(maxKeys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.TwoConstants: {
							Keyframe[] minKeys = ConstToCurveLerpUnclamped(to.constantMin, from.curveMin, from.curveMultiplier, 1 - time);
							Keyframe[] maxKeys = ConstToCurveLerpUnclamped(to.constantMax, from.curveMax, from.curveMultiplier, 1 - time);
							float multiplier = Normalize(minKeys, maxKeys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoCurves,
								curveMin = new AnimationCurve(minKeys),
								curveMax = new AnimationCurve(maxKeys),
								curveMultiplier = multiplier
							};
						}
						default:
							throw new ArgumentOutOfRangeException();
					}
				case ParticleSystemCurveMode.TwoConstants:
					switch (to.mode) {
						case ParticleSystemCurveMode.Constant:
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoConstants,
								constantMin = Mathf.LerpUnclamped(from.constantMin, to.constant, time),
								constantMax = Mathf.LerpUnclamped(from.constantMax, to.constant, time)
							};
						case ParticleSystemCurveMode.Curve: {
							Keyframe[] minKeys = ConstToCurveLerpUnclamped(from.constantMin, to.curve, to.curveMultiplier, time);
							Keyframe[] maxKeys = ConstToCurveLerpUnclamped(from.constantMax, to.curve, to.curveMultiplier, time);
							float multiplier = Normalize(minKeys, maxKeys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoCurves,
								curveMin = new AnimationCurve(minKeys),
								curveMax = new AnimationCurve(maxKeys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.TwoCurves: {
							Keyframe[] minKeys = ConstToCurveLerpUnclamped(from.constantMin, to.curveMin, to.curveMultiplier, time);
							Keyframe[] maxKeys = ConstToCurveLerpUnclamped(from.constantMax, to.curveMax, to.curveMultiplier, time);
							float multiplier = Normalize(minKeys, maxKeys);
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoCurves,
								curveMin = new AnimationCurve(minKeys),
								curveMax = new AnimationCurve(maxKeys),
								curveMultiplier = multiplier
							};
						}
						case ParticleSystemCurveMode.TwoConstants:
							return new ParticleSystem.MinMaxCurve {
								mode = ParticleSystemCurveMode.TwoConstants,
								constantMin = Mathf.LerpUnclamped(from.constantMin, to.constantMin, time),
								constantMax = Mathf.LerpUnclamped(from.constantMax, to.constantMax, time)
							};
						default:
							throw new ArgumentOutOfRangeException();
					}
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static AnimationCurve CurveLerpUnclamped(AnimationCurve fromCurve, AnimationCurve toCurve, float time) {
			Keyframe[] keys = CurveToCurveLerpUnclamped(fromCurve, 1, toCurve, 1, time);
			return new AnimationCurve(keys);
		}

		private static Keyframe[] ConstToCurveLerpUnclamped(float fromValue, AnimationCurve toCurve, float toCurveMultiplier, float time) {
			Keyframe[] keys = toCurve.keys;
			for (int i = 0, length = keys.Length; i < length; i++) {
				Keyframe key = keys[i];
				key.value = Mathf.LerpUnclamped(fromValue, key.value * toCurveMultiplier, time);
				key.inTangent *= toCurveMultiplier * time;
				key.outTangent *= toCurveMultiplier * time;
				keys[i] = key;
			}
			return keys;
		}

		private const float CURVE_DELTA_TIME = 0.001F;
		private static Keyframe[] CurveToCurveLerpUnclamped(AnimationCurve fromCurve, float fromCurveMultiplier, AnimationCurve toCurve, float toCurveMultiplier, float time) {
			Keyframe[] fromKeys = fromCurve.keys;
			Keyframe[] toKeys = toCurve.keys;
			List<Keyframe> keyList = new List<Keyframe>();
			foreach (var fromKey in fromKeys) {
				Keyframe? toKey = null;
				foreach (var key in toKeys) {
					if (Mathf.Approximately(fromKey.time, key.time)) {
						toKey = key;
						break;
					}
				}
				if (toKey == null) {
					float toValue = toCurve.Evaluate(fromKey.time);
					float toInTangent = (toValue - toCurve.Evaluate(fromKey.time - CURVE_DELTA_TIME)) / CURVE_DELTA_TIME;
					float toOutTangent = (toCurve.Evaluate(fromKey.time + CURVE_DELTA_TIME) - toValue) / CURVE_DELTA_TIME;
					if (fromKey.time <= 0) {
						toInTangent = toOutTangent;
					} else if (fromKey.time >= 1) {
						toOutTangent = toInTangent;
					}
					toKey = new Keyframe(fromKey.time, toValue, toInTangent, toOutTangent);
				}
				keyList.Add(new Keyframe {
					time = fromKey.time,
					value = Mathf.LerpUnclamped(fromKey.value * fromCurveMultiplier, toKey.Value.value * toCurveMultiplier, time),
					inTangent = Mathf.LerpUnclamped(fromKey.inTangent * fromCurveMultiplier, toKey.Value.inTangent * toCurveMultiplier, time),
					outTangent = Mathf.LerpUnclamped(fromKey.outTangent * fromCurveMultiplier, toKey.Value.outTangent * toCurveMultiplier, time)
				});
			}
			foreach (var toKey in toKeys) {
				// 去除两边Curve都有的Keyframe
				bool exist = false;
				foreach (var key in keyList) {
					if (Mathf.Approximately(toKey.time, key.time)) {
						exist = true;
						break;
					}
				}
				if (!exist) {
					// 去重后fromCurve里必定没有对应的Keyframe
					float fromValue = fromCurve.Evaluate(toKey.time);
					float fromInTangent = (fromValue - fromCurve.Evaluate(toKey.time - CURVE_DELTA_TIME)) / CURVE_DELTA_TIME;
					float fromOutTangent = (fromCurve.Evaluate(toKey.time + CURVE_DELTA_TIME) - fromValue) / CURVE_DELTA_TIME;
					if (toKey.time <= 0) {
						fromInTangent = fromOutTangent;
					} else if (toKey.time >= 1) {
						fromOutTangent = fromInTangent;
					}
					keyList.Add(new Keyframe {
						value = Mathf.LerpUnclamped(fromValue * fromCurveMultiplier, toKey.value * toCurveMultiplier, time),
						inTangent = Mathf.LerpUnclamped(fromInTangent * fromCurveMultiplier, toKey.inTangent * toCurveMultiplier, time),
						outTangent = Mathf.LerpUnclamped(fromOutTangent * fromCurveMultiplier, toKey.outTangent * toCurveMultiplier, time)
					});
				}
			}
			return keyList.ToArray();
		}

		private static float Normalize(params Keyframe[][] keyArrays) {
			float multiplier = 0;
			foreach (var keyArray in keyArrays) {
				foreach (var keyframe in keyArray) {
					if (Mathf.Abs(keyframe.value) > multiplier) {
						multiplier = keyframe.value;
					}
				}
			}
			if (!Mathf.Approximately(multiplier, 0)) {
				foreach (var keyArray in keyArrays) {
					for (int i = 0, length = keyArray.Length; i < length; i++) {
						Keyframe toKey = keyArray[i];
						toKey.value /= multiplier;
						toKey.inTangent /= multiplier;
						toKey.outTangent /= multiplier;
						keyArray[i] = toKey;
					}
				}
				return multiplier;
			}
			return 1;
		}
	}
}