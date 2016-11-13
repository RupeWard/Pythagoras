using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	abstract public class ElementDecoratorBase : RJWard.Core.IDebugDescribable
	{
		public static bool DEBUG_DECORATORS = true;

		#region Setup

		protected ElementDecoratorBase( 
			Color c, 
			float a, 
			System.Action< Color > cca, 
			System.Action< float > aca )
		{
			colourChangedAction_ += cca;
			alphaChangedAction_ += aca;
			colour_ = c;
			alpha_ = a;
		}

		// Use this in element's Init when using a shared decorator
		public void AddActions( System.Action<Color> cca, System.Action<float> aca )
		{
			if (cca != null)
			{
				colourChangedAction_ += cca;
				cca( colour_ );
			}
			if (aca != null)
			{
				alphaChangedAction_ += aca;
				aca( alpha_ );
			}
		}

		// Use this in element's OnDestroy when decoratoir might be shared
		public void RemoveActions( System.Action<Color> cca, System.Action<float> aca )
		{
			if (cca != null)
			{
				colourChangedAction_ -= cca;
			}
			if (aca != null)
			{
				alphaChangedAction_ -= aca;
			}
		}

		#endregion Setup

		#region applier

		public virtual void Apply()
		{
			DoColourChangedaction( );
			DoAlphaChangedAction( );
		}

		#endregion applier

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
					DoColourChangedaction( );
				}
			}
		}

		private System.Action<Color> colourChangedAction_;

		private void DoColourChangedaction()
		{
			if (colourChangedAction_ != null)
			{
				colourChangedAction_( colour_ );
			}
			else
			{
				if (DEBUG_DECORATORS)
				{
					Debug.LogWarning( "No colourChangedAction" );
				}
			}
		}

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
					DoAlphaChangedAction( );
				}
			}
		}

		private System.Action<float> alphaChangedAction_;

		private void DoAlphaChangedAction()
		{
			if (alphaChangedAction_ != null)
			{
				alphaChangedAction_( alpha_ );
			}
			else
			{
				if (DEBUG_DECORATORS)
				{
					Debug.LogWarning( "No alphaChangedAction" );
				}
			}
		}

		#endregion alpha

		#region setters

		public void SetColour( Color c, float a )
		{
			colour = c;
			alpha = a;
		}

		#endregion setters

		#region IDebugDescribable

		public void DebugDescribe( System.Text.StringBuilder sb)
		{
			sb.Append( GetType( ).ToString( ) + ": " );
			sb.Append( " C=" ).Append( colour_ );
			sb.Append( " A=" ).Append( alpha_ );
			DebugDescribeDetails( sb );
		}

		virtual protected void DebugDescribeDetails( System.Text.StringBuilder sb ) { } // override as required

		#endregion IDebugDescribable
	}
}
