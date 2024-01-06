/*
 * @Author: wangyun
 * @CreateTime: 2022-05-28 22:36:48 844
 * @LastEditor: wangyun
 * @EditTime: 2022-05-28 22:36:48 849
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Control {
	public enum PhysicsEventType {
		ON_TRIGGER_ENTER = 0,
		ON_TRIGGER_STAY = 1,
		ON_TRIGGER_EXIT = 2,
		ON_COLLISION_ENTER = 3,
		ON_COLLISION_STAY = 4,
		ON_COLLISION_EXIT = 5
	}
	
	public class PhysicsListener : BaseListener {
		public PhysicsEventType type = PhysicsEventType.ON_TRIGGER_ENTER;
		public List<Collider> colliders = new List<Collider>();
		[Tooltip("勾选后，双方任一勾选isTrigger都能响应Trigger相关回调")]
		public bool triggerSelf = true;
		public bool checkColliderEnabled;
		
		protected override bool ExecutorEnabled => true;
		
		private void OnTriggerEnter(Collider other) {
			if (enabled && type == PhysicsEventType.ON_TRIGGER_ENTER && (triggerSelf || other.isTrigger)) {
				if (colliders.Count == 0 || colliders.Contains(other)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnTriggerStay(Collider other) {
			if (enabled && type == PhysicsEventType.ON_TRIGGER_STAY && (triggerSelf || other.isTrigger)) {
				if (colliders.Count == 0 || colliders.Contains(other)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnTriggerExit(Collider other) {
			if (enabled && type == PhysicsEventType.ON_TRIGGER_EXIT && (triggerSelf || other.isTrigger)) {
				if (colliders.Count == 0 || colliders.Contains(other)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnCollisionEnter(Collision collision) {
			if (enabled && type == PhysicsEventType.ON_COLLISION_ENTER) {
				if (colliders.Count == 0 || colliders.Contains(collision.collider)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnCollisionStay(Collision collision) {
			if (enabled && type == PhysicsEventType.ON_COLLISION_STAY) {
				if (colliders.Count == 0 || colliders.Contains(collision.collider)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}

		private void OnCollisionExit(Collision collision) {
			if (enabled && type == PhysicsEventType.ON_COLLISION_EXIT) {
				if (colliders.Count == 0 || colliders.Contains(collision.collider)) {
					if (!checkColliderEnabled || Array.Exists(GetComponents<Collider>(), c => c.enabled)) {
						Execute();
					}
				}
			}
		}
	}
}