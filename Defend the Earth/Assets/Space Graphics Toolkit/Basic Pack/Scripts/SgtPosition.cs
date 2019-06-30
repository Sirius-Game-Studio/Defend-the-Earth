using UnityEngine;

namespace SpaceGraphicsToolkit
{
	/// <summary>This class stores a coordinate in the floating origin system, and provides methods to manipulate them.</summary>
	[System.Serializable]
	public struct SgtPosition
	{
		/// <summary>When the LocalX/Y/Z values exceed this value, the Global/X/Y/Z values will be modified to account for it, creating a grid system
		/// The universe is 4.4e+26~ meters in radius
		/// A long stores up to +- 9.2e+18~
		/// Divide them, and you get 47696652.0723
		/// Thus the below value allows you to simulate a universe that is larger than the current observable universe</summary>
		public static readonly double CellSize = 50000000.0;

		// The local high precision coordinate in meters
		public double LocalX;
		public double LocalY;
		public double LocalZ;

		// The global cell position
		public long GlobalX;
		public long GlobalY;
		public long GlobalZ;

		public SgtPosition(Vector3 localXYZ, double scale = 1)
		{
			LocalX = localXYZ.x * scale;
			LocalY = localXYZ.y * scale;
			LocalZ = localXYZ.z * scale;

			GlobalX = GlobalY = GlobalZ = 0;

			SnapLocal();
		}

		public static double Distance(SgtPosition a, SgtPosition b)
		{
			var x = b.LocalX - a.LocalX + (b.GlobalX - a.GlobalX) * CellSize;
			var y = b.LocalY - a.LocalY + (b.GlobalY - a.GlobalY) * CellSize;
			var z = b.LocalZ - a.LocalZ + (b.GlobalZ - a.GlobalZ) * CellSize;

			return System.Math.Sqrt(x * x + y * y + z * z);
		}

		public static double Distance(ref SgtPosition a, ref SgtPosition b)
		{
			var x = b.LocalX - a.LocalX + (b.GlobalX - a.GlobalX) * CellSize;
			var y = b.LocalY - a.LocalY + (b.GlobalY - a.GlobalY) * CellSize;
			var z = b.LocalZ - a.LocalZ + (b.GlobalZ - a.GlobalZ) * CellSize;

			return System.Math.Sqrt(x * x + y * y + z * z);
		}

		public static double SqrDistance(SgtPosition a, SgtPosition b)
		{
			var x = b.LocalX - a.LocalX + (b.GlobalX - a.GlobalX) * CellSize;
			var y = b.LocalY - a.LocalY + (b.GlobalY - a.GlobalY) * CellSize;
			var z = b.LocalZ - a.LocalZ + (b.GlobalZ - a.GlobalZ) * CellSize;

			return x * x + y * y + z * z;
		}

		public static double SqrDistance(ref SgtPosition a, ref SgtPosition b)
		{
			var x = b.LocalX - a.LocalX + (b.GlobalX - a.GlobalX) * CellSize;
			var y = b.LocalY - a.LocalY + (b.GlobalY - a.GlobalY) * CellSize;
			var z = b.LocalZ - a.LocalZ + (b.GlobalZ - a.GlobalZ) * CellSize;

			return x * x + y * y + z * z;
		}

		public static SgtPosition Delta(ref SgtPosition a, ref SgtPosition b)
		{
			var o = default(SgtPosition);

			o.LocalX = a.LocalX - b.LocalX;
			o.LocalY = a.LocalY - b.LocalY;
			o.LocalZ = a.LocalZ - b.LocalZ;
			o.GlobalX = a.GlobalX - b.GlobalX;
			o.GlobalY = a.GlobalY - b.GlobalY;
			o.GlobalZ = a.GlobalZ - b.GlobalZ;

			return o;
		}

		public static bool Equal(ref SgtPosition a, ref SgtPosition b)
		{
			if (a.GlobalX == b.GlobalX && a.GlobalY == b.GlobalY && a.GlobalZ == b.GlobalZ)
			{
				if (a.LocalX == b.LocalX && a.LocalY == b.LocalY && a.LocalZ == b.LocalZ)
				{
					return true;
				}
			}

			return false;
		}

		public static Vector3 Direction(ref SgtPosition a, ref SgtPosition b)
		{
			var x = b.LocalX - a.LocalX + (b.GlobalX - a.GlobalX) * CellSize;
			var y = b.LocalY - a.LocalY + (b.GlobalY - a.GlobalY) * CellSize;
			var z = b.LocalZ - a.LocalZ + (b.GlobalZ - a.GlobalZ) * CellSize;
			var m = System.Math.Sqrt(x * x + y * y + z * z);

			if (m > 0.0)
			{
				x /= m;
				y /= m;
				z /= m;
			}

			return new Vector3((float)x, (float)y, (float)z);
		}

		public static SgtPosition Lerp(SgtPosition a, SgtPosition b, double t)
		{
			var o = a;

			o.LocalX += ((b.GlobalX - a.GlobalX) * CellSize + b.LocalX - a.LocalX) * t;
			o.LocalY += ((b.GlobalY - a.GlobalY) * CellSize + b.LocalY - a.LocalY) * t;
			o.LocalZ += ((b.GlobalZ - a.GlobalZ) * CellSize + b.LocalZ - a.LocalZ) * t;

			o.SnapLocal();

			return o;
		}

		// Get the world space vector between two positions
		public static Vector3 Vector(SgtPosition a, SgtPosition b)
		{
			var ax = a.LocalX + a.GlobalX * CellSize;
			var ay = a.LocalY + a.GlobalY * CellSize;
			var az = a.LocalZ + a.GlobalZ * CellSize;
			var bx = b.LocalX + b.GlobalX * CellSize;
			var by = b.LocalY + b.GlobalY * CellSize;
			var bz = b.LocalZ + b.GlobalZ * CellSize;

			var x = bx - ax;
			var y = by - ay;
			var z = bz - az;

			return new Vector3((float)x, (float)y, (float)z);
		}

		// Did the local position stray too far from the origin?
		public bool SnapLocal()
		{
			var updatePosition = false;
			var shiftX         = CalculateShift(LocalX, CellSize);
			var shiftY         = CalculateShift(LocalY, CellSize);
			var shiftZ         = CalculateShift(LocalZ, CellSize);

			if (shiftX != 0)
			{
				GlobalX += shiftX;
				LocalX  -= shiftX * CellSize;

				updatePosition = true;
			}

			if (shiftY != 0)
			{
				GlobalY += shiftY;
				LocalY  -= shiftY * CellSize;

				updatePosition = true;
			}

			if (shiftZ != 0)
			{
				GlobalZ += shiftZ;
				LocalZ  -= shiftZ * CellSize;

				updatePosition = true;
			}

			return updatePosition;
		}

		public static SgtPosition operator + (SgtPosition a, SgtPosition b)
		{
			a.GlobalX += b.GlobalX;
			a.GlobalY += b.GlobalY;
			a.GlobalZ += b.GlobalZ;

			a.LocalX += b.LocalX;
			a.LocalY += b.LocalY;
			a.LocalZ += b.LocalZ;

			a.SnapLocal();

			return a;
		}

		public override string ToString()
		{
			return "(" + LocalX + ", " + LocalY + ", " + LocalZ + " - " + GlobalX + ", " + GlobalY + ", " + GlobalZ + ")";
		}

		private long CalculateShift(double coordinate, double cellSize)
		{
			var shift = coordinate / cellSize;

			return (long)shift;
		}
	}
}