using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	abstract public class ElementDecorator1DBase : ElementDecoratorBase
	{
		#region Setup

		protected ElementDecorator1DBase( 
			Color c, float a, System.Action< Color > cca, System.Action< float > aca,
			float w,
			System.Action<float> wca )
			: base(c,a,cca,aca)
		{
			widthChangedAction_ = wca;
			width = w;
		}

		#endregion Setup

		#region width

		private float width_; 
		public float width
		{
			get { return width_; }
			set
			{
				if (width_ != value)
				{
					width_ = value;
					if (widthChangedAction_ != null)
					{
						widthChangedAction_( width_ );
					}
				}
			}
		}

		private System.Action<float> widthChangedAction_;

		#endregion alpha

		#region setters

		public void SetWidth(float w )
		{
			width = w;
		}

		#endregion setters
	}
}
