using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	abstract public class ElementDecoratorBase 
	{
		#region Setup

		protected ElementDecoratorBase( 
			Color c, 
			float a, 
			System.Action< Color > cca, 
			System.Action< float > aca )
		{
			colourChangedAction_ = cca;
			alphaChangedAction_ = aca;
			colour = c;
			alpha = a;
		}

		#endregion Setup

		#region Colour

		private Color colour_; 
		public Color colour
		{
			get { return colour_; }
			set
			{
				if (colour_ != value)
				{
					colour_ = value;
					if (colourChangedAction_ != null)
					{
						colourChangedAction_( colour_ );
					}
				}
			}
		}

		private System.Action<Color> colourChangedAction_;

		#endregion Colour

		#region alpha

		private float alpha_; 
		public float alpha
		{
			get { return alpha_; }
			set
			{
				if (!Mathf.Approximately(alpha_, value))
				{
					alpha_ = value;
					if (alphaChangedAction_ != null)
					{
						alphaChangedAction_( alpha_ );
					}
				}
			}
		}

		private System.Action<float> alphaChangedAction_;

		#endregion alpha

		#region setters

		public void SetColour( Color c, float a )
		{
			colour = c;
			alpha = a;
		}

		#endregion setters
	}
}
