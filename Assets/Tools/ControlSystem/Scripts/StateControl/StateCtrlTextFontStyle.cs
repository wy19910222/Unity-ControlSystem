/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 17:20:36 868
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 17:20:36 881
 */

using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace Control {
	public class StateCtrlTextFontStyle : BaseStateCtrl<FontStyles> {
		protected override FontStyles TargetValue {
			get {
				Text text = GetComponent<Text>();
				if (text) {
					return (FontStyles) text.fontStyle;
				}
				TMP_Text tmp_text = GetComponent<TMP_Text>();
				// ReSharper disable once ConvertIfStatementToReturnStatement
				if (tmp_text) {
					return tmp_text.fontStyle;
				}
				return FontStyles.Normal;
			}
			set {
				Text text = GetComponent<Text>();
				if (text) {
					text.fontStyle = (FontStyle) (value & (FontStyles) FontStyle.BoldAndItalic);
				}
				TMP_Text tmp_text = GetComponent<TMP_Text>();
				if (tmp_text) {
					tmp_text.fontStyle = value;
				}
			}
		}
	}
}