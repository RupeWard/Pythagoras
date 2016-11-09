using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class AngleProvider_Constant : IAngleProvider
	{
		#region private data

		private float angle_ = 0f;

		#endregion

		#region setup

		public AngleProvider_Constant( float a)
		{
			angle_ = a;
		}

		#endregion setup

		#region IAngleProvider

		public float GetAngle( ElementList elements )
		{
			return angle_;
		}

		#endregion
	}
}

