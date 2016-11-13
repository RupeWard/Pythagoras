using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	abstract public class Element1DBase : ElementBase
	{
		#region properties

		public ElementDecorator1DBase decorator1D
		{
			get { return Decorator< ElementDecorator1DBase >( ); }
		}

		#endregion properties


	}
}
