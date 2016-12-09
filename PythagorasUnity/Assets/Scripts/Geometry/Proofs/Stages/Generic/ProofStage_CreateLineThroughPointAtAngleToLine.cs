using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	/*
		Create a straight line element through a point, at an angle to a line

		??? DEPRECATED ??? No longer being used since triangles have edges via polygon base class. If we do want to create a side from it we can just clone that.
	*/
	class ProofStage_CreateLineThroughPointAtAngleToLine : ProofStageBase
	{
		static private readonly bool DEBUG_LOCAL = true;

		#region private data

		// the line
		Element_StraightLine line_ = null;

		// line settings for creation
		private string lineName_ = "[UNNAMED LINE]";
		private Color lineColour_;
		private float lineWidth_ = 0.01f;
		private float depth_ = 0f;

		// source details
		private IPointProvider pointProvider_ = null;
		private IStraightLineProvider lineProvider_ = null;
		private float angle_ = 90f;

		private List<ILineExtender> lineExtenders = new List< ILineExtender >( );

		#endregion private data

		#region setup

		public ProofStage_CreateLineThroughPointAtAngleToLine(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			IPointProvider pp,
			IStraightLineProvider slp,
			float a,
			float d,
			float lw,
			Color c,
			string ln
		): base( n, gf, f, durn, ac )
		{
			Init( pp, slp, a, d, lw, c, ln );
		}

		public ProofStage_CreateLineThroughPointAtAngleToLine(
			string n,  GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			IPointProvider pp,
			IStraightLineProvider slp,
			float a,
			float d,
			float lw,
			Color c,
			string ln,
            ILineExtender le
            ) : base (n, gf, f, durn, ac )
		{
			Init( pp, slp, a, d, lw, c, ln );

			lineExtenders.Add( le );
		}

		public ProofStage_CreateLineThroughPointAtAngleToLine(
			string n, GeometryFactory gf, Field f, float durn, System.Action<ProofStageBase> ac,
			IPointProvider pp,
			IStraightLineProvider slp,
			float a,
			float d,
			float lw,
			Color c,
			string ln,
			List < ILineExtender > le
		) : base( n, gf, f, durn, ac )
		{
			Init( pp, slp, a, d, lw, c, ln );

			lineExtenders.AddRange( le );
		}

		private void Init(
			IPointProvider pp,
			IStraightLineProvider slp,
			float a,
			float d,
			float lw,
			Color c,
			string ln
		) 
		{
			lineColour_ = c;
			lineName_ = ln;
			lineWidth_ = lw;
			depth_ = d;
			pointProvider_ = pp;
			lineProvider_ = slp;
			angle_ = a;
		}



		private void CreateLineIfNeeded( )
		{
			line_ = elements.GetElementOfType< Element_StraightLine >( lineName_ );
			if (line_ == null)
			{
				Vector2 point = pointProvider_.GetPoint( elements );
				Element_StraightLine line = lineProvider_.GetLine( elements );
				float lineAngleDegrees = line.GetAngleDegrees( );
				float lineAngle = lineAngleDegrees + angle_;

				StraightLineFormula newLineFormula = new StraightLineFormula( point, lineAngle );
				StraightLineFormula referenceLineFormula = line.GetFormula( );

				if (DEBUG_LOCAL)
				{
					Debug.LogWarning( "Through point "+point+", Line formula is " + newLineFormula.DebugDescribe( )+", reference is "+referenceLineFormula.DebugDescribe() );
				}

				Vector2 point1 = Vector2.zero;
				if (StraightLineFormula.GetIntersection( newLineFormula, referenceLineFormula, ref point1 ))
				{
					if (DEBUG_LOCAL)
					{
						Debug.LogWarning( "Found point on line = " + point1 );
					}
					/*
					Vector2 newPoint0 = point;
					Vector2 newPoint1 = point1;

                    if (lineExtension_.x > 0f)
					{
						newPoint0 = newPoint0 + (point - point1) * lineExtension_.x;
					}
					if (lineExtension_.y > 0f)
					{
						newPoint1 = newPoint1 - (point - point1) * lineExtension_.y;
					}
					*/
					Vector2[] ends = new Vector2[] { point, point1};

					line_ = geometryFactory.AddStraightLineToField(
						line.field,
						lineName_,
						depth_,
						ends,
						lineWidth_,
						lineColour_
						);
					for (int i = 0; i < lineExtenders.Count; i++)
					{
						lineExtenders[i].ExtendLine( elements, line_ );
					}

					elements.AddElement( lineName_, line_ );

					switch (direction)
					{
						case ProofEngine.EDirection.Forward:
							{
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
					// TODO deal with this properly
					throw new System.Exception( "Can't get intersection" );
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
