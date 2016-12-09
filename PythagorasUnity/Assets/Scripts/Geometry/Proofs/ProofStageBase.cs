using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	/*
		Base class for a proof stage
	*/
	abstract public class ProofStageBase 
	{
		public static bool DEBUG_PROOFSTAGE = true;

		#region private data 

		private GeometryFactory geometryFactory_ = null;
		private Field field_ = null;

		private ProofStageBase previousStage_ = null;
		private ProofStageBase nextStage_ = null;

		private float durationSeconds_ = 1f;

		private ElementListDefinition startRequiredElementListDefinition_ = null;
		private ElementListDefinition endRequiredElementListDefinition_ = null;

		private ElementListDefinition startReversedDestroyElementListDefinition_ = null;

		private ElementList elements_ = null;

		protected float currentTimeSeconds_ = 0f;
		private ProofEngine.EDirection direction_ = ProofEngine.EDirection.Forward;
		private string name_ = "[UNNAMED PROOF STAGE]";

		private Dictionary< ProofEngine.EDirection, bool > dontPauseOnFinish_ = new Dictionary< ProofEngine.EDirection, bool >( )
		{
			{ ProofEngine.EDirection.Forward, false },
			{ ProofEngine.EDirection.Reverse, false }
		};

		private bool isTimeRunning_ = false;

		protected ProofEngine proofEngine_ = null;
		
		#endregion private data 

		#region properties

		public bool IsTimeRunning
		{
			get { return isTimeRunning_; }
		}

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

		public ProofEngine.EDirection direction
		{
			get { return direction_; }
		}

		public string name
		{
			get { return name_; }
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

		public ElementListDefinition startReversedDestroyElementListDefinition // TODO ?deprecate? is this still needed?
		{
			set { startReversedDestroyElementListDefinition_ = value; }
		}

		protected ElementList elements
		{
			get { return elements_; }
		}

		public bool dontPauseOnFinish
		{
			get { return dontPauseOnFinish_[direction_];  }
		}

		/* Direction-dependent following stage (next if forward, previous if reverse)
		*/
		public ProofStageBase GetFollowingStage( )
		{
			ProofStageBase result = null;
			switch (direction_)
			{
				case ProofEngine.EDirection.Forward:
					{
						result = nextStage_;
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						result = previousStage_;
						break;
					}
			}
			return result;
		}

		public ProofStageBase NextStage
		{
			get { return nextStage_; }
		}

		public ProofStageBase PreviousStage
		{
			get { return previousStage_; }
		}

		#endregion properties

		#region setters

		public void SetProofEngine( ProofEngine pe )
		{
			proofEngine_ = pe;
		}

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

		public void SetDontPauseOnFinish( ProofEngine.EDirection d, bool b)
		{
			dontPauseOnFinish_[d] = b;
		}

		public void SetDontPauseOnFinish( ProofEngine.EDirection d )
		{
			SetDontPauseOnFinish( d, true );
		}

		public bool ChangeDirection()
		{
			if (DEBUG_PROOFSTAGE)
			{
				Debug.Log( "'" + name + "': ChangeDirection" );
			}

			bool changed = false;
			switch (direction_)
			{
				case ProofEngine.EDirection.Forward:
					{
						if (currentTimeFractional > 0f) 
						{
							direction_ = ProofEngine.EDirection.Reverse;
							changed = true;
							if (DEBUG_PROOFSTAGE)
							{
								Debug.Log( "ProofStage '" + name + "' changed direction to Reverse because not at start" );
							}
						}
						else if (previousStage_ != null)
						{
							direction_ = ProofEngine.EDirection.Reverse;
							changed = true;
							if (DEBUG_PROOFSTAGE)
							{
								Debug.Log( "ProofStage '" + name + "' changed direction to Reverse because at start but has a previous stage to switch to" );
							}
							Finish( );
						}
						else
						{
							if (DEBUG_PROOFSTAGE)
							{
								Debug.LogWarning( "ProofStage '" + name + "' failed to change direction to Reverse because at start but has no previous stage to switch to" );
							}
						}
						break;
					}
				case ProofEngine.EDirection.Reverse:
					{
						if (currentTimeFractional < 1f)
						{
							direction_ = ProofEngine.EDirection.Forward;
							changed = true;
							if (DEBUG_PROOFSTAGE)
							{
								Debug.Log( "ProofStage '" + name + "' changed direction to Forward because not at end" );
							}
						}
						else if (nextStage_ != null)
						{
							direction_ = ProofEngine.EDirection.Forward;
							changed = true;
							if (DEBUG_PROOFSTAGE)
							{
								Debug.Log( "ProofStage '" + name + "' changed direction to Forward because at end but has a next stage to switch to" );
							}
							Finish( );
						}
						else
						{
							if (DEBUG_PROOFSTAGE)
							{
								Debug.LogWarning( "ProofStage '" + name + "' failed to change direction to Forward because at end but has no next stage to switch to" );
							}
						}
						break;
					}
			}
			if (changed)
			{
				// TODO not necessary when changing direction in middlle. It's here to make sure we do initialisation stuff when changing direction at end. 
				// Perhaps makes more sense to re-init in that case?
				initNotUpdated_ = true;
			}
			return changed;
		}

		#endregion setters

		#region callbacks

		protected System.Action<ProofStageBase> onFinishedAction;
		
		#endregion callbacks

		#region Setup

		protected ProofStageBase( 
			string n, 
			GeometryFactory gf, 
			Field f,
			float durn,
			System.Action<ProofStageBase> a)
		{
			name_ = n;
			geometryFactory_ = gf;
			field_ = f;
			durationSeconds_ = durn;
			// zero duration causes div by zero error when calculating fractional time, so we fix it to a tiny value instead
			if (durationSeconds_ == 0f) 
			{
				durationSeconds_ = 0.000001f;
			}
			if (a != null)
			{
				onFinishedAction += a;
			}
		} 

		/* Start the stage 

			initialises the current time and validates the element list, depending on direction
		*/
		public void Init( ProofEngine.EDirection d, ElementList e)
		{
			if (DEBUG_PROOFSTAGE)
			{
				Debug.Log( "'" + name + "': Init (BASE), direction = "+d+", duration = "+durationSeconds_ );
			}

			if (e == null)
			{
				elements_ = null;
			}
			else
			{
				elements_ = e;
			}
			direction_ = d;
			switch (direction_)
			{
				case ProofEngine.EDirection.Forward:
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
				case ProofEngine.EDirection.Reverse:
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

			SetTimeRunning(true);

			initNotUpdated_ = true;

			// derived class
			HandleInit( ); 
		}

		private bool initNotUpdated_ = false;

		#endregion Setup

		#region Process

		public void SetTimeRunning(bool b)
		{
			if (b != isTimeRunning_)
			{
				isTimeRunning_ = b;
				if (DEBUG_PROOFSTAGE)
				{
					if (initNotUpdated_)
					{
						Debug.Log( "Time now " + ((isTimeRunning_) ? ("STARTED") : ("PAUSED WHEN NOT STARTED")) + " in proof stage '" + name + "'" );
					}
					else
					{
						Debug.Log( "Time now " + ((isTimeRunning_) ? ("RESUMED") : ("PAUSED")) + " in proof stage '" + name + "'" );
					}
				}
			}
			else
			{
				if (DEBUG_PROOFSTAGE)
				{
					Debug.LogWarning( "Time is already " + ((isTimeRunning_) ? ("RESUMED") : ("PAUSED")) + " in proof stage '" + name + "'" );
				}
			}
		}

		private void UpdateView()
		{
			// Anything to do here? If not then we don't need this function
			DoUpdateView( );
		}

		abstract protected void DoUpdateView( ); // derived class defines to set up view according to current time
		abstract protected void HandleInit( );  // derived class overrides to handle any initialisation work
		abstract protected void HandleFinished( ); // derived class overrides to handle anything needs doing on finish (in either direction) 
		abstract protected void HandleFirstUpdateAfterInit( ); 

		/* This is the main direction-dependent updating function. 

			Time elapsed since previosu update is passed in by engine (to enable speed change)
		*/
		public void HandleSecondsElapsed(float s)
		{
			bool shouldFinish = false;
			if (initNotUpdated_)
			{
				if (DEBUG_PROOFSTAGE)
				{
					Debug.Log( name + " initNotUpdated @ "+Time.time );
				}

				initNotUpdated_ = false;
				if (direction_ == ProofEngine.EDirection.Reverse)
				{
					if (startReversedDestroyElementListDefinition_ != null)
					{
						elements_.DestroyAll( startReversedDestroyElementListDefinition_ );
					}
				}
				HandleFirstUpdateAfterInit( );
				
				s = 0f;
			}
			if (isTimeRunning_)
			{
				switch (direction_)
				{
					case ProofEngine.EDirection.Forward:
						{
							currentTimeSeconds_ += s;
							break;
						}
					case ProofEngine.EDirection.Reverse:
						{
							currentTimeSeconds_ -= s;
							break;
						}
				}
				UpdateView( );
				switch (direction_)
				{
					case ProofEngine.EDirection.Forward:
						{
							if (currentTimeSeconds_ >= durationSeconds_)
							{
								currentTimeSeconds_ = durationSeconds_;
								shouldFinish = true;
							}
							break;
						}
					case ProofEngine.EDirection.Reverse:
						{
							if (currentTimeSeconds_ <= 0f)
							{
								currentTimeSeconds_ = 0f;
								shouldFinish = true;
							}
							break;
						}
				}
			}
			else
			{
				Debug.LogWarning( "Time not running in '" + name + "'" );
			}
			if (shouldFinish)
			{
				Finish( );
			}
		}

		/* Called when stage reaches target (direction-depoendent)

			validates element list against own requirements
			initialises next stage with this ones direction and element list
		*/
		protected void Finish()
		{
			if (DEBUG_PROOFSTAGE)
			{
				Debug.Log( "'" + name + "': FInish" );
			}
			HandleFinished( );
			switch (direction_)
			{
				case ProofEngine.EDirection.Reverse:
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
				case ProofEngine.EDirection.Forward:
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

			SetTimeRunning(false);

			if (onFinishedAction != null)
			{
				onFinishedAction( this );
			}

		}
		#endregion Process
	}
}

