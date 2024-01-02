/*
 * @Author: wangyun
 * @CreateTime: 2023-01-28 17:20:36 868
 * @LastEditor: wangyun
 * @EditTime: 2023-01-28 17:20:36 881
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Control {
	public class StateCtrlTextFontStyleBool : BaseStateCtrl<bool> {
		public FontStyles type = FontStyles.Bold;
		protected override bool TargetValue {
			get {
				Text text = GetComponent<Text>();
				if (text) {
					return ((FontStyles) text.fontStyle & type) == type;
				}
				TMP_Text tmp_text = GetComponent<TMP_Text>();
				if (tmp_text) {
					return (tmp_text.fontStyle & type) == type;
				}
				return false;
			}
			set {
				Text text = GetComponent<Text>();
				if (text) {
					FontStyles style = (FontStyles) text.fontStyle;
					if (value) {
						style |= type;
					} else {
						style &= ~type;
					}
					text.fontStyle = (FontStyle) (style & (FontStyles) FontStyle.BoldAndItalic);
				}
				TMP_Text tmp_text = GetComponent<TMP_Text>();
				if (tmp_text) {
					if (value) {
						tmp_text.fontStyle |= type;
					} else {
						tmp_text.fontStyle &= ~type;
					}
				}
			}
		}
	}
}