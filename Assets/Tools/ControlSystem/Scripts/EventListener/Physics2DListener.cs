/*
 * @Author: wangyun
 * @CreateTime: 2022-05-28 22:42:21 923
 * @LastEditor: wangyun
 * @EditTime: 2022-05-28 22:42:21 927
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public enum Physics2DType {
		ON_TRIGGER_ENTER_2D = 0,
		ON_TRIGGER_STAY_2D = 1,
		ON_TRIGGER_EXIT_2D = 2,
		ON_COLLISION_ENTER_2D = 3,
		ON_COLLISION_STAY_2D = 4,
		ON_COLLISION_EXIT_2D = 5
	}
	
	public class Physics2DListener : BaseListener {
		public Physics2DType type = Physics2DType.ON_TRIGGER_ENTER_2D;
		public List<Collider2D> colliders = new List<Collider2D>();
		[Tooltip("勾选后，双方任一勾选isTrigger都能响应Trigger相关回调")]
		public bool triggerSelf = true;
		public bool checkColliderEnabled;
		
		protected override bool ExecutorEnabled => true;

		private void OnTriggerEnter2D(Collider2D other) {
			if (enabled && type == Physics2DType.ON_TRIGGER_ENTER_2D && (triggerSelf || other.isTrigger)) {
				if (colliders.Count == 0 || colliders.Contains(other)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider2D>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnTriggerStay2D(Collider2D other) {
			if (enabled && type == Physics2DType.ON_TRIGGER_STAY_2D && (triggerSelf || other.isTrigger)) {
				if (colliders.Count == 0 || colliders.Contains(other)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider2D>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnTriggerExit2D(Collider2D other) {
			if (enabled && type == Physics2DType.ON_TRIGGER_EXIT_2D && (triggerSelf || other.isTrigger)) {
				if (colliders.Count == 0 || colliders.Contains(other)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider2D>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnCollisionEnter2D(Collision2D collision) {
			if (enabled && type == Physics2DType.ON_COLLISION_ENTER_2D) {
				if (colliders.Count == 0 || colliders.Contains(collision.collider)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider2D>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnCollisionStay2D(Collision2D collision) {
			if (enabled && type == Physics2DType.ON_COLLISION_STAY_2D) {
				if (colliders.Count == 0 || colliders.Contains(collision.collider)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider2D>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnCollisionExit2D(Collision2D collision) {
			if (enabled && type == Physics2DType.ON_COLLISION_EXIT_2D) {
				if (colliders.Count == 0 || colliders.Contains(collision.collider)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider2D>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}
	}
}