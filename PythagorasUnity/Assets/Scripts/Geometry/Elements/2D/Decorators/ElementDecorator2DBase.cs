using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	abstract public class ElementDecorator2DBase : ElementDecoratorBase
	{
		static public float defaultEdgeWidth = 0.01f;

		#region subelement decorators

		private ElementDecorator1DBase defaultEdgeDecorator_ = null;
		public ElementDecorator1DBase defaultEdgeDecorator
		{
			get
			{
				if (defaultEdgeDecorator_ == null)
				{
					defaultEdgeDecorator_ = new ElementDecorator_StraightLine( colour, alpha,  defaultEdgeWidth );
				}
				return defaultEdgeDecorator_;
			}

			set { defaultEdgeDecorator_ = value; }
		}

		#endregion subelement decorators

		#region Setup

		protected ElementDecorator2DBase( 
			Color c, float a, System.Action< Color > cca, System.Action< float > aca)
			: base(c,a,cca,aca)
		{
		}

		protected ElementDecorator2DBase(
			Color c, float a )
			: base( c, a )
		{
		}

		#endregion Setup

		#region setters

		#endregion setters

	}
}
