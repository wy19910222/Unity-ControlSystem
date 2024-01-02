/*
 * @Author: wangyun
 * @CreateTime: 2022-08-03 13:51:54 271
 * @LastEditor: wangyun
 * @EditTime: 2022-08-03 13:51:54 276
 */

using UnityEngine;
using UnityEngine.UI;

namespace Control {
	public class StateCtrlSprite : BaseStateCtrl<Sprite> {
		protected override Sprite TargetValue {
			get {
				Image image = GetComponent<Image>();
				if (image) {
					return image.sprite;
				}
				SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
					return spriteRenderer.sprite;
				}
				return null;
			}
			set {
				Image image = GetComponent<Image>();
				if (image) {
					image.sprite = value;
				}
				SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer) {
					spriteRenderer.sprite = value;
				}
			}
		}
	}
}