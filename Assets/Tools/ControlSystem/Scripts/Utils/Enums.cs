/*
 * @Author: wangyun
 * @CreateTime: 2023-03-16 01:45:38 202
 * @LastEditor: wangyun
 * @EditTime: 2023-03-16 01:45:38 209
 */

using System;

namespace Control {
	[Flags]
	public enum Vector2Part {
		X = 1 << 0,
		Y = 1 << 1,
		XY = X | Y
	}
	
	public enum Vector2Type {
		X = 1 << 0,
		Y = 1 << 1
	}
	
	[Flags]
	public enum Vector3Part {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2,
		XYZ = X | Y | Z
	}
	
	public enum Vector3Type {
		X = 1 << 0,
		Y = 1 << 1,
		Z = 1 << 2
	}
	
	[Flags]
	public enum ColorPart {
		R = 1 << 0,
		G = 1 << 1,
		B = 1 << 2,
		RGB = R | G | B,
		A = 1 << 3,
	}
	
	[Flags]
	public enum PaddingPart {
		LEFT = 1 << 0,
		RIGHT = 1 << 1,
		TOP = 1 << 2,
		BOTTOM = 1 << 3
	}
	
	[Flags]
	public enum SizePart {
		WIDTH = 1 << 0,
		HEIGHT = 1 << 1,
		SIZE = WIDTH | HEIGHT
	}
}
