using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	/*
		Create a straight line element corresponding to the side of a triangle.

		??? DEPRECATED ??? No longer being used since triangles have edges via polygon base class. If we do want to create a side from it we can just clone that.
	*/
	class ProofStage_CreatePolygonSide : ProofStageBase
	{
		#region private data

		// the line
		private Element_StraightLine line_ = null;
		private string lineName_ = "[UNNAMED LINE]";
        private float lineWidth_;
		private Color lineColour_;
		private float lineDepth_;

		// line settings for creation
		IStraightLineProvider lineProvider_ = null;

		List< ILineExtender > lineExtenders_ = new List< ILineExtender >( );

		#endregion private data

		#region setup

		public ProofStage_CreatePolygonSide(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			IStraightLineProvider lp,
			float depth,
			float lw,
			Color c,
			string ln) 
			: base (n, gf, f, durn, ac )
		{
			Init( lp, depth, lw, c, ln);
		}

		public ProofStage_CreatePolygonSide(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			IStraightLineProvider lp,
			float depth,
			float lw,
			Color c,
			string ln,
			List< ILineExtender> le)
			: base( n, gf, f, durn, ac )
		{
			Init( lp, depth, lw, c, ln );
			lineExtenders_.AddRange( le );
		}

		public ProofStage_CreatePolygonSide(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			IStraightLineProvider lp,
			float depth,
			float lw,
			Color c,
			string ln,
			ILineExtender le )
		: base( n, gf, f, durn, ac )
		{
			Init( lp, depth, lw, c, ln );
			lineExtenders_.Add( le );
		}


		private void Init(
			IStraightLineProvider lp,
			float depth,
			float lw,
			Color c,
			string ln )
		{
			lineColour_ = c;
			lineName_ = ln;
			lineWidth_ = lw;
			lineDepth_ = depth;

			lineProvider_ = lp;
		}


		private void CreateLineIfNeeded( )
		{
			line_ = elements.GetElementOfType< Element_StraightLine >( lineName_ );
			if (line_ == null)
			{
				Element_StraightLine srcLine = lineProvider_.GetLine( elements );
				if (srcLine == null)
				{
					throw new System.Exception( "No source line" );
				}
				else
				{
					line_ = geometryFactory.CreateClone( lineName_, srcLine, lineDepth_ - srcLine.depth, lineColour_ ) as Element_StraightLine;
					if (line_ == null)
					{
						throw new System.Exception( "Failed to create clone line" );
					}
					line_.SetWidth( lineWidth_ );
					for (int i = 0; i < lineExtenders_.Count; i++)
					{
						lineExtenders_[0].ExtendLine( elements, line_ );
					}
					elements.AddElement( lineName_, line_ );
				}

				switch (direction)
				{
					case ProofEngine.EDirection.Forward:
						{
							line_.gameObject.SetActive( true );
							line_.SetAlpha( 0f );
							break;
						}
					case ProofEngine.EDirection.Reverse:
						{
							line_.SetAlpha( 1f );
							break;
						}
				}
			}
			else
			{
				line_.gameObject.SetActive( true );
			}
		}

		#endregion setup

		#region ProofStageBase 

		override protected void HandleFirstUpdateAfterInit( )
		{
			CreateLineIfNeeded( );
		}

		protected override void HandleInit( )
		{ 
		}

		protected override void DoUpdateView( )
		{
			line_.SetAlpha( Mathf.Lerp( 0f, 1f, currentTimeFractional ) );
		}

		protected override void HandleFinished( )
		{
			switch (direction)
			{
				case ProofEngine.EDirection.Forward:
					{
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						if (line_ != null)
						{
							elements.DestroyElement( ref line_ );
						}
						break;
					}
			}
		}

		#endregion ProofStageBase 

	}
}
