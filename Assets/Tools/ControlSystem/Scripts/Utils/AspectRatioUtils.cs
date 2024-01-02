using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Control {
	public static class AspectRatioUtils {
		public static Tweener CreateTweenerBasedSize(AspectRatioFitter fitter, float aspectRatio, float tweenDuration) {
			switch (fitter.aspectMode) {
				case AspectRatioFitter.AspectMode.WidthControlsHeight: {
					// aspectRatio是宽/高，宽不变，高与aspectRatio的倒数呈线性关系
					return DOTween.To(
							() => 1 / fitter.aspectRatio,
							v => fitter.aspectRatio = 1 / v,
							1 / aspectRatio,
							tweenDuration
					);
				}
				case AspectRatioFitter.AspectMode.FitInParent:
				case AspectRatioFitter.AspectMode.EnvelopeParent: {
					bool isEnvelope = fitter.aspectMode == AspectRatioFitter.AspectMode.EnvelopeParent;
					RectTransform trans = fitter.transform as RectTransform;
					// ReSharper disable once PossibleNullReferenceException 因为有AspectRatioFitter，所以RectTransform必存在
					Rect fromRect = trans.rect;
					float fromWidth = fromRect.width;
					float fromHeight = fromRect.height;
					float fromAspectRatio = fromWidth / fromHeight;
					RectTransform parent = trans.parent as RectTransform;
					float parentWidth = fromWidth;
					float parentHeight = fromHeight;
					if (parent) {
						Rect parentRect = parent.rect;
						parentWidth = parentRect.width;
						parentHeight = parentRect.height;
					}
					float parentAspectRatio = parentWidth / parentHeight;
					switch (isEnvelope) {
						case false when fromAspectRatio <= parentAspectRatio && aspectRatio <= parentAspectRatio:
						case true when fromAspectRatio >= parentAspectRatio && aspectRatio >= parentAspectRatio:
							// fit模式下from和to都比parentAspectRatio小，或者envelope模式下from和to都比parentAspectRatio大，
							// 说明只变化宽，而宽与aspectRatio呈线性关系
							return DOTween.To(
									() => fitter.aspectRatio,
									v => fitter.aspectRatio = v,
									aspectRatio,
									tweenDuration
							);
						case false when fromAspectRatio >= parentAspectRatio && aspectRatio >= parentAspectRatio:
						case true when fromAspectRatio <= parentAspectRatio && aspectRatio <= parentAspectRatio:
							// fit模式下from和to都比parentAspectRatio大，或者envelope模式下from和to都比parentAspectRatio小，
							// 说明只变化高，而高与aspectRatio的倒数呈线性关系
							return DOTween.To(
									() => 1 / fitter.aspectRatio,
									v => fitter.aspectRatio = 1 / v,
									1 / aspectRatio,
									tweenDuration
							);
						case false when fromAspectRatio < parentAspectRatio:
						case true when fromAspectRatio > parentAspectRatio: {
							// 已排除from和to在parentAspectRatio同一边的情况，fit模式下from比to小，或者envelope模式下from比to大，
							// 说明第一段只变化宽，第二段只变化高
							float toWidth = parentWidth;
							float toHeight = parentWidth / aspectRatio;
							float deltaWidth = Mathf.Abs(toWidth - fromWidth);
							float deltaHeight = Mathf.Abs(toHeight - fromHeight);
							float demarcation = deltaWidth / (deltaWidth + deltaHeight);
							float time = 0;
							return DOTween.To(
									() => time,
									v => fitter.aspectRatio =
											Mathf.Lerp(fromWidth, toWidth, v / demarcation) /
											Mathf.Lerp(fromHeight, toHeight, (v - demarcation) / (1 - demarcation)),
									1,
									tweenDuration
							);
						}
						default: {
							// 已排除from和to在parentAspectRatio同一边的情况，fit模式下from比to大，或者envelope模式下from比to小，
							// 说明第一段只变化高，第二段只变化宽
							float toWidth = parentHeight * aspectRatio;
							float toHeight = parentHeight;
							float deltaWidth = Mathf.Abs(toWidth - fromWidth);
							float deltaHeight = Mathf.Abs(toHeight - fromHeight);
							float demarcation = deltaHeight / (deltaHeight + deltaWidth);
							float time = 0;
							return DOTween.To(
									() => time,
									v => fitter.aspectRatio =
											Mathf.Lerp(fromWidth, toWidth, (v - demarcation) / (1 - demarcation)) /
											Mathf.Lerp(fromHeight, toHeight, v / demarcation),
									1,
									tweenDuration
							);
						}
					}
				}
				case AspectRatioFitter.AspectMode.HeightControlsWidth:
				default: {
					// aspectRatio是宽/高，高不变，宽与aspectRatio呈线性关系
					return DOTween.To(
							() => fitter.aspectRatio,
							v => fitter.aspectRatio = v,
							aspectRatio,
							tweenDuration
					);
				}
			}
		}
		
		public static float LerpUnclampedBasedSize(float aspectRatioFrom, float aspectRatioTo, float time, AspectRatioFitter fitter) {
			if (!fitter) {
				return Mathf.LerpUnclamped(aspectRatioFrom, aspectRatioTo, time);
			}
			switch (fitter.aspectMode) {
				case AspectRatioFitter.AspectMode.WidthControlsHeight: {
					// aspectRatio是宽/高，宽不变，高与aspectRatio的倒数呈线性关系
					return 1 / Mathf.LerpUnclamped(1 / aspectRatioFrom, 1 / aspectRatioTo, time);
				}
				case AspectRatioFitter.AspectMode.FitInParent:
				case AspectRatioFitter.AspectMode.EnvelopeParent: {
					bool isEnvelope = fitter.aspectMode == AspectRatioFitter.AspectMode.EnvelopeParent;
					RectTransform trans = fitter.transform as RectTransform;
					// ReSharper disable once PossibleNullReferenceException 因为有AspectRatioFitter，所以RectTransform必存在
					Rect fromRect = trans.rect;
					float parentWidth = fromRect.width;		// 如果没有parent就用自己的
					float parentHeight = fromRect.height;	// 如果没有parent就用自己的
					RectTransform parent = trans.parent as RectTransform;
					if (parent) {
						Rect parentRect = parent.rect;
						parentWidth = parentRect.width;
						parentHeight = parentRect.height;
					}
					float parentAspectRatio = parentWidth / parentHeight;
					switch (isEnvelope) {
						case false when aspectRatioFrom <= parentAspectRatio && aspectRatioTo <= parentAspectRatio:
						case true when aspectRatioFrom >= parentAspectRatio && aspectRatioTo >= parentAspectRatio:
							// fit模式下from和to都比parentAspectRatio小，或者envelope模式下from和to都比parentAspectRatio大，
							// 说明只变化宽，而宽与aspectRatio呈线性关系
							return Mathf.LerpUnclamped(aspectRatioFrom, aspectRatioTo, time);
						case false when aspectRatioFrom >= parentAspectRatio && aspectRatioTo >= parentAspectRatio:
						case true when aspectRatioFrom <= parentAspectRatio && aspectRatioTo <= parentAspectRatio:
							// fit模式下from和to都比parentAspectRatio大，或者envelope模式下from和to都比parentAspectRatio小，
							// 说明只变化高，而高与aspectRatio的倒数呈线性关系
							return 1 / Mathf.LerpUnclamped(1 / aspectRatioFrom, 1 / aspectRatioTo, time);
						case false when aspectRatioFrom < aspectRatioTo:
						case true when aspectRatioFrom > aspectRatioTo: {
							// 已排除from和to在parentAspectRatio同一边的情况，
							// fit模式下from比parent小且to比parent大，或者envelope模式下from比parent大且to比parent小，
							// 说明第一段只变化宽，第二段只变化高
							float fromWidth = parentHeight * aspectRatioFrom;
							float fromHeight = parentHeight;
							float toWidth = parentWidth;
							float toHeight = parentWidth / aspectRatioTo;
							float deltaWidth = Mathf.Abs(toWidth - fromWidth);
							float deltaHeight = Mathf.Abs(toHeight - fromHeight);
							float demarcation = deltaWidth / (deltaWidth + deltaHeight);
							return Mathf.LerpUnclamped(fromWidth, toWidth, Mathf.Min(time, demarcation) / demarcation) /
									Mathf.LerpUnclamped(fromHeight, toHeight, Mathf.Max(time - demarcation, 0) / (1 - demarcation));
						}
						default: {
							// 已排除from和to在parentAspectRatio同一边的情况，
							// fit模式下from比parent大且to比parent小，或者envelope模式下from比parent小且to比parent大，
							// 说明第一段只变化高，第二段只变化宽
							float fromWidth = parentWidth;
							float fromHeight = parentWidth / aspectRatioFrom;
							float toWidth = parentHeight * aspectRatioTo;
							float toHeight = parentHeight;
							float deltaWidth = Mathf.Abs(toWidth - fromWidth);
							float deltaHeight = Mathf.Abs(toHeight - fromHeight);
							float demarcation = deltaHeight / (deltaHeight + deltaWidth);
							return Mathf.LerpUnclamped(fromWidth, toWidth, Mathf.Max(time - demarcation, 0) / (1 - demarcation)) /
									Mathf.LerpUnclamped(fromHeight, toHeight, Mathf.Min(time, demarcation) / demarcation);
						}
					}
				}
				case AspectRatioFitter.AspectMode.HeightControlsWidth:
				default: {
					// aspectRatio是宽/高，高不变，宽与aspectRatio呈线性关系
					return Mathf.LerpUnclamped(aspectRatioFrom, aspectRatioTo, time);
				}
			}
		}
		
		public static float GetTimeBasedSize(float aspectRatioFrom, float aspectRatioTo, float value, AspectRatioFitter fitter) {
			if (!fitter) {
				float aspectRatioDelta = aspectRatioTo - aspectRatioFrom;
				return aspectRatioDelta == 0 ? 0 : (value - aspectRatioFrom) / aspectRatioDelta;
			}
			switch (fitter.aspectMode) {
				case AspectRatioFitter.AspectMode.WidthControlsHeight: {
					// aspectRatio是宽/高，宽不变，高与aspectRatio的倒数呈线性关系
					float aspectRatioFromReverted = 1 / aspectRatioFrom;
					float aspectRatioToReverted = 1 / aspectRatioTo;
					float valueReverted = 1 / value;
					float aspectRatioRevertedDelta = aspectRatioToReverted - aspectRatioFromReverted;
					return aspectRatioRevertedDelta == 0 ? 0 : (valueReverted - aspectRatioFromReverted) / aspectRatioRevertedDelta;
				}
				case AspectRatioFitter.AspectMode.FitInParent:
				case AspectRatioFitter.AspectMode.EnvelopeParent: {
					bool isEnvelope = fitter.aspectMode == AspectRatioFitter.AspectMode.EnvelopeParent;
					RectTransform trans = fitter.transform as RectTransform;
					// ReSharper disable once PossibleNullReferenceException 因为有AspectRatioFitter，所以RectTransform必存在
					Rect fromRect = trans.rect;
					float parentWidth = fromRect.width;		// 如果没有parent就用自己的
					float parentHeight = fromRect.height;	// 如果没有parent就用自己的
					RectTransform parent = trans.parent as RectTransform;
					if (parent) {
						Rect parentRect = parent.rect;
						parentWidth = parentRect.width;
						parentHeight = parentRect.height;
					}
					float parentAspectRatio = parentWidth / parentHeight;
					switch (isEnvelope) {
						case false when aspectRatioFrom <= parentAspectRatio && aspectRatioTo <= parentAspectRatio:
						case true when aspectRatioFrom >= parentAspectRatio && aspectRatioTo >= parentAspectRatio:
							// fit模式下from和to都比parentAspectRatio小，或者envelope模式下from和to都比parentAspectRatio大，
							// 说明只变化宽，而宽与aspectRatio呈线性关系
							float aspectRatioDelta = aspectRatioTo - aspectRatioFrom;
							return aspectRatioDelta == 0 ? 0 : (value - aspectRatioFrom) / aspectRatioDelta;
						case false when aspectRatioFrom >= parentAspectRatio && aspectRatioTo >= parentAspectRatio:
						case true when aspectRatioFrom <= parentAspectRatio && aspectRatioTo <= parentAspectRatio:
							// fit模式下from和to都比parentAspectRatio大，或者envelope模式下from和to都比parentAspectRatio小，
							// 说明只变化高，而高与aspectRatio的倒数呈线性关系
							float aspectRatioFromReverted = 1 / aspectRatioFrom;
							float aspectRatioToReverted = 1 / aspectRatioTo;
							float valueReverted = 1 / value;
							float aspectRatioRevertedDelta = aspectRatioToReverted - aspectRatioFromReverted;
							return aspectRatioRevertedDelta == 0 ? 0 : (valueReverted - aspectRatioFromReverted) / aspectRatioRevertedDelta;
						case false when aspectRatioFrom < aspectRatioTo:
						case true when aspectRatioFrom > aspectRatioTo: {
							// 已排除from和to在parentAspectRatio同一边的情况，
							// fit模式下from比parent小且to比parent大，或者envelope模式下from比parent大且to比parent小，
							// 说明第一段只变化宽，第二段只变化高
							float fromWidth = parentHeight * aspectRatioFrom;
							float fromHeight = parentHeight;
							float toWidth = parentWidth;
							float toHeight = parentWidth / aspectRatioTo;
							float deltaWidth = Mathf.Abs(toWidth - fromWidth);
							float deltaHeight = Mathf.Abs(toHeight - fromHeight);
							float demarcation = deltaWidth / (deltaWidth + deltaHeight);
							if (Mathf.Approximately(value, parentAspectRatio)) {
								return demarcation;
							} else if (!isEnvelope && value < parentAspectRatio || isEnvelope && value > parentAspectRatio) {
								float valueWidth = fromHeight * value;
								return (valueWidth - fromWidth) / (toWidth - fromWidth) * demarcation;
							} else {
								float valueHeight = toWidth / value;
								return 1 - (valueHeight - toHeight) / (fromHeight - toHeight) * (1 - demarcation);
							}
						}
						default: {
							// 已排除from和to在parentAspectRatio同一边的情况，
							// fit模式下from比parent大且to比parent小，或者envelope模式下from比parent小且to比parent大，
							// 说明第一段只变化高，第二段只变化宽
							float fromWidth = parentWidth;
							float fromHeight = parentWidth / aspectRatioFrom;
							float toWidth = parentHeight * aspectRatioTo;
							float toHeight = parentHeight;
							float deltaWidth = Mathf.Abs(toWidth - fromWidth);
							float deltaHeight = Mathf.Abs(toHeight - fromHeight);
							float demarcation = deltaHeight / (deltaHeight + deltaWidth);
							if (Mathf.Approximately(value, parentAspectRatio)) {
								return demarcation;
							} else if (!isEnvelope && value > parentAspectRatio || isEnvelope && value < parentAspectRatio) {
								float valueHeight = fromWidth / value;
								return (valueHeight - fromHeight) / (toHeight - fromHeight) * demarcation;
							} else {
								float valueWidth = toHeight * value;
								return 1 - (valueWidth - toWidth) / (fromWidth - toWidth) * (1 - demarcation);
							}
						}
					}
				}
				case AspectRatioFitter.AspectMode.HeightControlsWidth:
				default: {
					// aspectRatio是宽/高，高不变，宽与aspectRatio呈线性关系
					float aspectRatioDelta = aspectRatioTo - aspectRatioFrom;
					return aspectRatioDelta == 0 ? 0 : (value - aspectRatioFrom) / aspectRatioDelta;
				}
			}
		}
	}
}
