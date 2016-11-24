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
			widthChangedAction_ += wca;
			width_ = w;
		}

		protected ElementDecorator1DBase(
			Color c, float a, 
			float w)
			: base( c, a)
		{
			width_ = w;
		}


		public void AddActions( System.Action<Color> cca, System.Action<float> aca, System.Action<float> wca )
		{
			base.AddActions(cca, aca);
			if (wca != null)
			{
				widthChangedAction_ += wca;
				wca( width_ );
			}
		}

		public void RemoveActions( System.Action<Color> cca, System.Action<float> aca, System.Action<float> wca )
		{
			base.RemoveActions( cca, aca );
			if (wca != null)
			{
				widthChangedAction_ -= wca;
			}
		}

		#endregion Setup

		#region applier

		public override void Apply( )
		{
			base.Apply( );
			DoWidthChangedAction( );
		}

		#endregion applier

		#region width

		private float width_; 
		public float width
		{
			get { return width_; }
			set
			{
				if (!Mathf.Approximately(width_, value))
				{
					width_ = value;
					DoWidthChangedAction( );
				}
			}
		}

		private System.Action<float> widthChangedAction_;

		private void DoWidthChangedAction()
		{
			if (widthChangedAction_ != null)
			{
				widthChangedAction_( width_ );
			}
			else
			{
				Debug.LogWarning( "No widthChangedAction" );
			}
		}

		#endregion alpha

		#region setters

		public void SetWidth(float w )
		{
			width = w;
		}

		#endregion setters

		#region IDebugDescribable

		protected override void DebugDescribeDetails( System.Text.StringBuilder sb )
		{
			sb.Append( " W=" ).Append( width_ );
		}

		#endregion IDebugDescribable

	}
}
