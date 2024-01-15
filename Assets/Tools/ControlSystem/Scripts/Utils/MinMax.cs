/*
 * @Author: wangyun
 * @CreateTime: 2023-03-16 01:45:28 333
 * @LastEditor: wangyun
 * @EditTime: 2023-03-16 01:45:28 341
 */

using System;
using UnityEngine;

namespace Control {
	[Serializable]
	public struct MinMax : IEquatable<MinMax> {
		public float min;
		public float max;
		
		public MinMax(float min, float max) {
			this.min = min;
			this.max = max;
		}
		
		public void Set(float newMin, float newMax) {
			min = newMin;
			max = newMax;
		}

		public static MinMax Lerp(MinMax a, MinMax b, float t) => new MinMax(
				Mathf.Lerp(a.min, b.min, t),
				Mathf.Lerp(a.max, b.max, t)
		);
		public static MinMax LerpUnclamped(MinMax a, MinMax b, float t) => new MinMax(
				Mathf.LerpUnclamped(a.min, b.min, t),
				Mathf.LerpUnclamped(a.max, b.max, t)
		);

		public override int GetHashCode() => min.GetHashCode() ^ max.GetHashCode() << 2;
		public override bool Equals(object other) => other is MinMax other1 && Equals(other1);
		public bool Equals(MinMax other) => min.Equals(other.min) && max.Equals(other.max);

		public static bool operator ==(MinMax lhs, MinMax rhs) {
			float num1 = lhs.min - rhs.min;
			float num2 = lhs.max - rhs.max;
			return (double) num1 * num1 + (double) num2 * num2 < 9.99999943962493E-11;
		}
		public static bool operator !=(MinMax lhs, MinMax rhs) => !(lhs == rhs);

		public static implicit operator MinMax(Vector2 v) => new MinMax(v.x, v.y);
		public static implicit operator Vector2(MinMax m) => new Vector2(m.min, m.max);

		public override string ToString() => $"({min:F1}, {max:F1})";
	}
	[Serializable]
	public struct MinMax2 : IEquatable<MinMax2> {
		public MinMax x;
		public MinMax y;
		
		public Vector2 min {
			get => new Vector2(x.min, y.min);
			set {
				x.min = value.x;
				y.min = value.y;
			}
		}
		public Vector2 max {
			get => new Vector2(x.max, y.max);
			set {
				x.max = value.x;
				y.max = value.y;
			}
		}
		
		public MinMax this[int index] {
			get {
				switch (index) {
					case 0: return x;
					case 1: return y;
					default: throw new IndexOutOfRangeException("Invalid MinMaxVector2 index!");
				}
			}
			set {
				switch (index) {
					case 0: x = value; break;
					case 1: y = value; break;
					default: throw new IndexOutOfRangeException("Invalid MinMaxVector2 index!");
				}
			}
		}
		
		public MinMax2(MinMax x, MinMax y) {
			this.x = x;
			this.y = y;
		}

		public static MinMax2 Lerp(MinMax2 a, MinMax2 b, float t) => new MinMax2(
				MinMax.Lerp(a.x, b.x, t),
				MinMax.Lerp(a.y, b.y, t)
		);
		public static MinMax2 LerpUnclamped(MinMax2 a, MinMax2 b, float t) => new MinMax2(
				MinMax.LerpUnclamped(a.x, b.x, t),
				MinMax.LerpUnclamped(a.y, b.y, t)
		);

		public override int GetHashCode() {
			float num1 = x.min;
			int hashCode = num1.GetHashCode();
			num1 = x.max;
			int num2 = num1.GetHashCode() << 2;
			int num3 = hashCode ^ num2;
			num1 = y.min;
			int num4 = num1.GetHashCode() >> 2;
			int num5 = num3 ^ num4;
			num1 = y.max;
			int num6 = num1.GetHashCode() >> 1;
			return num5 ^ num6;
		}

		public override bool Equals(object other) => other is MinMax2 other1 && Equals(other1);
		public bool Equals(MinMax2 other) => x.Equals(other.x) && y.Equals(other.y);

		public static bool operator ==(MinMax2 lhs, MinMax2 rhs) {
			return lhs.x == rhs.x && lhs.y == rhs.y;
		}
		public static bool operator !=(MinMax2 lhs, MinMax2 rhs) => !(lhs == rhs);

		public override string ToString() => $"({x}, {y})";
	}
	[Serializable]
	public struct MinMax3 : IEquatable<MinMax3> {
		public MinMax x;
		public MinMax y;
		public MinMax z;
		
		public Vector3 min {
			get => new Vector3(x.min, y.min, z.min);
			set {
				x.min = value.x;
				y.min = value.y;
				z.min = value.z;
			}
		}
		public Vector3 max {
			get => new Vector3(x.max, y.max, z.max);
			set {
				x.max = value.x;
				y.max = value.y;
				z.max = value.z;
			}
		}
		
		public MinMax this[int index] {
			get {
				switch (index) {
					case 0: return x;
					case 1: return y;
					case 2: return z;
					default: throw new IndexOutOfRangeException("Invalid MinMaxVector2 index!");
				}
			}
			set {
				switch (index) {
					case 0: x = value; break;
					case 1: y = value; break;
					case 2: z = value; break;
					default: throw new IndexOutOfRangeException("Invalid MinMaxVector2 index!");
				}
			}
		}
		
		public MinMax3(MinMax x, MinMax y, MinMax z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public static MinMax3 Lerp(MinMax3 a, MinMax3 b, float t) => new MinMax3(
				MinMax.Lerp(a.x, b.x, t),
				MinMax.Lerp(a.y, b.y, t),
				MinMax.Lerp(a.z, b.z, t)
		);
		public static MinMax3 LerpUnclamped(MinMax3 a, MinMax3 b, float t) => new MinMax3(
				MinMax.LerpUnclamped(a.x, b.x, t),
				MinMax.LerpUnclamped(a.y, b.y, t),
				MinMax.LerpUnclamped(a.z, b.z, t)
		);

		public override int GetHashCode() {
			int hashCode = x.min.GetHashCode() ^ y.min.GetHashCode() << 2 ^ z.min.GetHashCode() >> 2;
			int num = (x.max.GetHashCode() ^ y.max.GetHashCode() << 2 ^ z.max.GetHashCode() >> 2) << 2;
			return hashCode ^ num;
		}

		public override bool Equals(object other) => other is MinMax3 other1 && Equals(other1);
		public bool Equals(MinMax3 other) => x.Equals(other.x) && y.Equals(other.y);

		public static bool operator ==(MinMax3 lhs, MinMax3 rhs) {
			return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
		}
		public static bool operator !=(MinMax3 lhs, MinMax3 rhs) => !(lhs == rhs);
		
		public static implicit operator MinMax3(MinMax2 m) => new MinMax3(m.x, m.y, new MinMax());
		public static implicit operator MinMax2(MinMax3 m) => new MinMax2(m.x, m.y);

		public override string ToString() => $"({x}, {y}, {z})";
	}
	
	[Serializable]
	public struct MinMaxVector2 : IEquatable<MinMaxVector2> {
		public Vector2 min;
		public Vector2 max;

		public MinMax x {
			get => new MinMax(min.x, max.x);
			set {
				min.x = value.min;
				max.x = value.max;
			}
		}
		public MinMax y {
			get => new MinMax(min.y, max.y);
			set {
				min.y = value.min;
				max.y = value.max;
			}
		}
		
		public MinMax this[int index] {
			get {
				switch (index) {
					case 0: return x;
					case 1: return y;
					default: throw new IndexOutOfRangeException("Invalid MinMaxVector2 index!");
				}
			}
			set {
				switch (index) {
					case 0: x = value; break;
					case 1: y = value; break;
					default: throw new IndexOutOfRangeException("Invalid MinMaxVector2 index!");
				}
			}
		}
		
		public MinMaxVector2(Vector2 min, Vector2 max) {
			this.min = min;
			this.max = max;
		}
		public MinMaxVector2(MinMax x, MinMax y) {
			min = new Vector2(x.min, y.min);
			max = new Vector2(x.max, y.max);
		}

		public static MinMaxVector2 Lerp(MinMaxVector2 a, MinMaxVector2 b, float t) => new MinMaxVector2(
				Vector2.Lerp(a.min, b.min, t),
				Vector2.Lerp(a.max, b.max, t)
		);
		public static MinMaxVector2 LerpUnclamped(MinMaxVector2 a, MinMaxVector2 b, float t) => new MinMaxVector2(
				Vector2.LerpUnclamped(a.min, b.min, t),
				Vector2.LerpUnclamped(a.max, b.max, t)
		);

		public override int GetHashCode() {
			float num1 = min.x;
			int hashCode = num1.GetHashCode();
			num1 = max.x;
			int num2 = num1.GetHashCode() << 2;
			int num3 = hashCode ^ num2;
			num1 = min.y;
			int num4 = num1.GetHashCode() >> 2;
			int num5 = num3 ^ num4;
			num1 = max.y;
			int num6 = num1.GetHashCode() >> 1;
			return num5 ^ num6;
		}

		public override bool Equals(object other) => other is MinMaxVector2 other1 && Equals(other1);
		public bool Equals(MinMaxVector2 other) => min.Equals(other.min) && max.Equals(other.max);

		public static bool operator ==(MinMaxVector2 lhs, MinMaxVector2 rhs) {
			return lhs.min == rhs.min && lhs.max == rhs.max;
		}
		public static bool operator !=(MinMaxVector2 lhs, MinMaxVector2 rhs) => !(lhs == rhs);

		public static implicit operator MinMaxVector2(Rect r) => new MinMaxVector2(r.min, r.max);
		public static implicit operator Rect(MinMaxVector2 m) => new Rect {min = m.min, max = m.max};
		public static implicit operator MinMaxVector2(MinMax2 m) => new MinMaxVector2 {x = m.x, y = m.y};
		public static implicit operator MinMax2(MinMaxVector2 m) => new MinMax2(m.x, m.y);

		public override string ToString() => $"({min}, {max})";
	}
	[Serializable]
	public struct MinMaxVector3 : IEquatable<MinMaxVector3> {
		public Vector3 min;
		public Vector3 max;

		public MinMax x {
			get => new MinMax(min.x, max.x);
			set {
				min.x = value.min;
				max.x = value.max;
			}
		}
		public MinMax y {
			get => new MinMax(min.y, max.y);
			set {
				min.y = value.min;
				max.y = value.max;
			}
		}
		public MinMax z {
			get => new MinMax(min.z, max.z);
			set {
				min.z = value.min;
				max.z = value.max;
			}
		}
		
		public MinMax this[int index] {
			get {
				switch (index) {
					case 0: return x;
					case 1: return y;
					case 2: return z;
					default: throw new IndexOutOfRangeException("Invalid MinMaxVector3 index!");
				}
			}
			set {
				switch (index) {
					case 0: x = value; break;
					case 1: y = value; break;
					case 2: z = value; break;
					default: throw new IndexOutOfRangeException("Invalid MinMaxVector3 index!");
				}
			}
		}
		
		public MinMaxVector3(Vector3 min, Vector3 max) {
			this.min = min;
			this.max = max;
		}
		public MinMaxVector3(MinMax _x, MinMax _y, MinMax _z) {
			min = new Vector3(_x.min, _y.min, _z.min);
			max = new Vector3(_x.max, _y.max, _z.max);
		}

		public static MinMaxVector3 Lerp(MinMaxVector3 a, MinMaxVector3 b, float t) => new MinMaxVector3(
				Vector3.Lerp(a.min, b.min, t),
				Vector3.Lerp(a.max, b.max, t)
		);
		public static MinMaxVector3 LerpUnclamped(MinMaxVector3 a, MinMaxVector3 b, float t) => new MinMaxVector3(
				Vector3.LerpUnclamped(a.min, b.min, t),
				Vector3.LerpUnclamped(a.max, b.max, t)
		);

		public override int GetHashCode() {
			Vector3 vector3 = min;
			int hashCode = vector3.GetHashCode();
			vector3 = max;
			int num = vector3.GetHashCode() << 2;
			return hashCode ^ num;
		}

		public override bool Equals(object other) => other is MinMaxVector3 other1 && Equals(other1);
		public bool Equals(MinMaxVector3 other) => min.Equals(other.min) && max.Equals(other.max);

		public static bool operator ==(MinMaxVector3 lhs, MinMaxVector3 rhs) {
			return lhs.min == rhs.min && lhs.max == rhs.max;
		}
		public static bool operator !=(MinMaxVector3 lhs, MinMaxVector3 rhs) => !(lhs == rhs);

		public static implicit operator MinMaxVector3(Bounds b) => new MinMaxVector3(b.min, b.max);
		public static implicit operator Bounds(MinMaxVector3 m) => new Bounds {min = m.min, max = m.max};
		public static implicit operator MinMaxVector3(MinMax3 m) => new MinMaxVector3(m.min, m.max);
		public static implicit operator MinMax3(MinMaxVector3 m) => new MinMax3(m.x, m.y, m.z);
		public static implicit operator MinMaxVector3(MinMaxVector2 m) => new MinMaxVector3(m.min, m.max);
		public static implicit operator MinMaxVector2(MinMaxVector3 m) => new MinMaxVector2(m.min, m.max);

		public override string ToString() => $"({min}, {max})";
	}
}
