using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	/*
		Base class for a proof stage
	*/
	abstract public class ProofStageBase 
	{
		public enum EDirection
		{
			Forward,
			Reverse
		}

		#region private data 

		private GeometryFactory geometryFactory_ = null;
		private Field field_ = null;

		private ProofStageBase previousStage_ = null;
		private ProofStageBase nextStage_ = null;

		private float durationSeconds_ = 1f;

		private ElementListDefinition startRequiredElementListDefinition_ = null;
		private ElementListDefinition endRequiredElementListDefinition_ = null;

		private ElementList elements_ = null;

		private float currentTimeSeconds_ = 0f;
		private EDirection direction_ = EDirection.Forward;
		private string name_ = "[UNNAMED PROOF STAGE]";
		private string description_ = string.Empty;

		#endregion private data 

		#region properties

		protected GeometryFactory geometryFactory
		{
			get { return geometryFactory_;  }
		}

		protected Field field
		{
			get { return field_; }
		}

		public float currentTimeFractional
		{
			get { return currentTimeSeconds_ / durationSeconds_; }
		}

		public EDirection direction
		{
			get { return direction_; }
		}

		public string name
		{
			get { return name_; }
		}

		public string description
		{
			get { return description_; }
		}

		public float durationSeconds
		{
			get { return durationSeconds_; }
		}

		protected ElementListDefinition startRequiredElementListDefinition
		{
			set { startRequiredElementListDefinition_ = value; }
		}

		protected ElementListDefinition endRequiredElementListDefinition
		{
			set { endRequiredElementListDefinition_ = value; }
		}

		protected ElementList elements
		{
			get { return elements_; }
		}

		#endregion properties

		#region setters

		public void SetPreviousStage( ProofStageBase b )
		{
			previousStage_ = b;
		}

		public void SetNextStage( ProofStageBase b )
		{
			nextStage_ = b;
		}

		public void SetBorderingStages( ProofStageBase b0, ProofStageBase b1 )
		{
			SetPreviousStage( b0 );
			SetNextStage( b1 );
		}

		static public void ConnectStages(ProofStageBase first, ProofStageBase second)
		{
			first.SetNextStage( second );
			second.SetPreviousStage( first );
		}

		#endregion setters

		/* Direction-dependent following stage (next if forward, previous if reverse)
		*/
		public ProofStageBase GetFollowingStage()
		{
			ProofStageBase result = null;
			switch (direction_)
			{
				case EDirection.Forward:
					{
						result = nextStage_;
						break;
					}
				case EDirection.Reverse:
					{
						result = previousStage_;
						break;
					}
			}
			return result;
		}

		/* Derived class should call this on creating new elements
		*/
		protected void AddElement(string n, ElementBase e)
		{
			elements_.AddElement(n, e );
		}


		#region callbacks

		protected System.Action<ProofStageBase> onFinishedAction;
		
		#endregion callbacks

		#region Setup

		protected ProofStageBase( 
			string n, 
			string descn, 
			GeometryFactory gf, 
			Field f,
			float durn,
			System.Action<ProofStageBase> a)
		{
			name_ = n;
			description_ = descn;
			geometryFactory_ = gf;
			field_ = f;
			durationSeconds_ = durn;
			if (a != null)
			{
				onFinishedAction += a;
			}
		} 

		/* Start the stage 

			initialises the current time and validates the element list, depending on direction
		*/
		public void Init(EDirection d, ElementList e)
		{
			if (e == null)
			{
				elements_ = null;
			}
			else
			{
				elements_ = new ElementList( e );
			}
			direction_ = d;
			switch (direction_)
			{
				case EDirection.Forward:
					{
						currentTimeSeconds_ = 0f;
						if (startRequiredElementListDefinition_ != null)
						{
							if (false == startRequiredElementListDefinition_.Validate(elements_, true))
							{
								throw new System.Exception( "ElementList does not satisfy start conditions for proof stage " + name_ + " on Init()" );
							}
						}
						break;
					}
				case EDirection.Reverse:
					{
						currentTimeSeconds_ = durationSeconds_;
						if (endRequiredElementListDefinition_ != null)
						{
							if (false == endRequiredElementListDefinition_.Validate( elements_, true ))
							{
								throw new System.Exception( "ElementList does not satisfy start conditions for proof stage " + name_ +" on Init()");
							}
						}
						break;
					}
			}

			// derived class
			HandleInit( ); 
			UpdateView( );
		}

		#endregion Setup

		#region Process

		public void UpdateView()
		{
			// Anything to do here? If not then we don't need this function
			DoUpdateView( );
		}

		abstract protected void DoUpdateView( ); // derived class defines to set up view according to current time
		abstract protected void HandleInit( );  // derived class overrides to handle any initialisation work
		abstract protected void HandleFinished( ); // derived class overrides to handle anything needs doing on finish (in either direction) 

		/* This is the main direction-dependent updating function. 

			Time elapsed since previosu update is passed in by engine (to enable speed change)
		*/
		public void HandleSecondsElapsed(float s)
		{
			bool shouldFinish = false;
			switch (direction_)
			{
				case EDirection.Forward:
					{
						currentTimeSeconds_ += s;
						if (currentTimeSeconds_ >= durationSeconds_)
						{
							currentTimeSeconds_ = durationSeconds_;
							shouldFinish = true;
						}
						break;
					}
				case EDirection.Reverse:
					{
						currentTimeSeconds_ -= s;
						if (currentTimeSeconds_ <= 0f)
						{
							currentTimeSeconds_ = 0f;
							shouldFinish = true;
						}
						break;
					}
			}
			DoUpdateView( );
			if (shouldFinish)
			{
				Finish( );
			}
		}

		/* Called when stage reaches target (direction-depoendent)

			validates element list against own requirements
			initialises next stage with this ones direction and element list
		*/
		private void Finish()
		{
			HandleFinished( );
			switch (direction_)
			{
				case EDirection.Reverse:
					{
						if (startRequiredElementListDefinition_ != null)
						{
							if (false == startRequiredElementListDefinition_.Validate( elements_, true ))
							{
								throw new System.Exception( "ElementList does not satisfy start conditions for proof stage " + name_ +" on Finish()");
							}
						}
						if (previousStage_ != null)
						{
							previousStage_.Init( direction_, elements_ );
						}
						break;
					}
				case EDirection.Forward:
					{
						if (endRequiredElementListDefinition_ != null)
						{
							if (false == endRequiredElementListDefinition_.Validate( elements_, true ))
							{
								throw new System.Exception( "ElementList does not satisfy end conditions for proof stage " + name_ +" on Finish()");
							}
						}
						if (nextStage_ != null)
						{
							nextStage_.Init( direction_, elements_ );
						}
						break;
					}
			}

			if (onFinishedAction != null)
			{
				onFinishedAction( this );
			}

		}
		#endregion Process


	}
}

