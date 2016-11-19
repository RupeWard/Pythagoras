using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class StraightLineFormula : RJWard.Core.IDebugDescribable
	{
		#region parameters

		private float gradient_ = 0f;
		private float intersept_ = 0f;

		#endregion parameters

		#region properties

		public float gradient
		{
			get { return gradient_;  }
		}

		public float intersept
		{
			get { return intersept_; }
		}

		#endregion properties

		#region ctors

		public StraightLineFormula( Vector2 p0, Vector2 p1 )
		{
			Init( p0, p1 );
		}

		public StraightLineFormula( Vector2 p, float angleDegrees )
		{
			Vector2 p1 = p + new Vector2( Mathf.Cos( Mathf.Deg2Rad * angleDegrees ), Mathf.Sin( Mathf.Deg2Rad * angleDegrees ) );
			Init( p, p1 );
		}

		private void Init( Vector2 p0, Vector2 p1 )
		{
			gradient_ = (p1.y - p0.y) / (p1.x - p0.x);
			intersept_ = p1.y - gradient_ * p1.x;
		}

		#endregion ctors
	
		#region computation

		public float GetY(float x)
		{
			return gradient_ * x + intersept_;
		}

		public float GetX(float y)
		{
			return (y - intersept_) / gradient;
		}

		static public bool GetIntersection( StraightLineFormula l0, StraightLineFormula l1, ref Vector2 intersection)
		{
			if (Mathf.Approximately(l0.gradient, l1.gradient))
			{
				return false;
			}
			intersection.x = (l0.intersept - l1.intersept) / (l1.gradient - l0.gradient);
			intersection.y = l0.GetY( intersection.x );
			return true;
		}

		#endregion computation

		#region IDebugDescribable

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "[ gradient = " ).Append( gradient_ ).Append( ", intersept = " ).Append( intersept_ ).Append( " ]" );
		}
		#endregion IDebugDescribable

	}
}
