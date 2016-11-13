using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	/*
		Base class for all 2D elements (including polygons)
	*/
	abstract public class Element2DBase : ElementBase
	{
		static public bool DEBUG_ELEMENT2DBASE = true;

		static public float defaultEdgeWidth = 0.01f;


		#region properties

		public ElementDecorator2DBase decorator2D
		{
			get { return Decorator< ElementDecorator2DBase >( ); }
		}

		#endregion properties

	}
}
