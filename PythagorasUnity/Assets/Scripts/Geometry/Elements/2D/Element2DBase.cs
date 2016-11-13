using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	abstract public class Element2DBase : ElementBase
	{
		#region private data 

		private int numVertices_ = 0;

		
		#endregion private data 

		#region properties

		public int numVertices
		{
			get { return numVertices_;  }
		}

		public ElementDecorator2DBase decorator2D
		{
			get { return Decorator< ElementDecorator2DBase >( ); }
		}

		#endregion properties

		#region Setup

		// Call this from derived classes' Init functions
		protected void Init( GeometryFactory gf, Field f, float d, int n )
		{
			base.Init( gf, f, d );
			numVertices_ = n;
		}

		#endregion Setup

	}
}
