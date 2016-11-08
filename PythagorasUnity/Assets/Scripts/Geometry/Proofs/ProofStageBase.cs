using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	abstract public class ProofStageBase 
	{
		public enum EDirection
		{
			Forward,
			Reverse
		}

		#region operational data 

		protected GeometryFactory geometryFactory_ = null;
		protected Field field_ = null;

		private float durationSeconds_ = 1f;
		public float durationSeconds
		{
			get { return durationSeconds_; }
		}

		private ElementListDefinition startRequiredElementListDefinition_ = null;
		private ElementListDefinition endRequiredElementListDefinition_ = null;

		public ElementListDefinition startRequiredElementListDefinition
		{
			set { startRequiredElementListDefinition_ = value; }
		}

		public ElementListDefinition endRequiredElementListDefinition
		{
			set { endRequiredElementListDefinition_ = value; }
		}

		private ProofStageBase previousStage_ = null;
		private ProofStageBase nextStage_ = null;
		public void SetPreviousStage(ProofStageBase b)
		{
			previousStage_ = b;
		}

		public void SetNextStage( ProofStageBase b )
		{
			nextStage_ = b;
		}

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

		public void SetBorderingStages(ProofStageBase b0, ProofStageBase b1)
		{
			SetPreviousStage( b0 );
			SetNextStage( b1 );
		}

		private ElementList elements_ = null;
		protected void AddElement(string n, ElementBase e)
		{
			elements_.AddElement(n, e );
		}

		private float currentTimeSeconds_ = 0f;
		public float currentTimeFractional
		{
			get { return currentTimeSeconds_ / durationSeconds_;  }
		}

		private EDirection direction_ = EDirection.Forward;
		public EDirection direction
		{
			get { return direction_; }
		}

		protected string name_ = "UNNAMED PROOF STAGE";
		protected string description_ = string.Empty;
		public string name
		{
			get { return name_; }
		}
		public string description
		{
			get { return description_;  }
		}

		#endregion operational data 

		#region callbacks

		protected System.Action<ProofStageBase> onFinishedAction;
		
		#endregion callbacks

		#region Setup

		protected ProofStageBase( 
			string n, 
			string d, 
			GeometryFactory gf, 
			Field f,
			float dn,
			System.Action<ProofStageBase> a)
		{
			name_ = n;
			description_ = d;
			geometryFactory_ = gf;
			field_ = f;
			durationSeconds_ = dn;
			if (a != null)
			{
				onFinishedAction += a;
			}
		} 

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

		abstract protected void DoUpdateView( );
		abstract protected void HandleInit( );
		abstract protected void HandleFinished( );

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

