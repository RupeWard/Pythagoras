using UnityEngine;
using System.Collections;
using RJWard.Core.UI.Extensions;

public class ProofTextPanel : MonoBehaviour
{
	public class ProofMessageDefinition
	{
		private string text_;
		public string text
		{
			get { return text_;  }
		}

		private float duration_;
		public float duration
		{
			get { return duration_; }
		}

		public ProofMessageDefinition(string t, float d, System.Action<ProofMessageDefinition> a)
		{
			text_ = t;
			duration_ = d;
			onCloseAction_ = a;
		}

		public ProofMessageDefinition( string t, System.Action<ProofMessageDefinition> a )
		{
			text_ = t;
			duration_ = -1f;
			onCloseAction_ = a;
		}

		private System.Action< ProofMessageDefinition > onCloseAction_ = null;
		public void SendOnCloseAction()
		{
			if (onCloseAction_ != null)
			{
				onCloseAction_( this );
			}
		}
	}

	static public bool DEBUG_PROOFTEXTPANEL = true;

	#region inspector hooks

	public UnityEngine.UI.Text proofText;
	public RectTransform timerImageRT;
	public UnityEngine.UI.Image timerImage;

	#endregion inspector hooks

	#region inspector data

	public Color stoppedColour = Color.red;
	public Color runningColour = Color.green;

	#endregion inspector data

	#region private data 

	private const float defaultDuration = 10f;

	private float timeTillClose_ = -1f;

	private ProofMessageDefinition messageDefinition_ = null;

	private Vector2 sizeDeltaRange_ = Vector2.zero;

	private bool timerRunning_ = true;

	#endregion private data

	#region Flow

	private void Awake()
	{
		proofText.text = string.Empty;

		sizeDeltaRange_.x = timerImageRT.sizeDelta.y;
		sizeDeltaRange_.y = sizeDeltaRange_.x - timerImageRT.GetHeight( );

		MessageBus.Instance.showMessageAction += SetMessageDefinition;

		if (DEBUG_PROOFTEXTPANEL)
		{
			Debug.Log( "sdr = " + sizeDeltaRange_ );
		}
	}

	private void Start( )
	{
		Close( );
#if UNITY_EDITOR
		if (DEBUG_PROOFTEXTPANEL)
		{
			SetMessageDefinition( new ProofMessageDefinition("TEST TEXT", 10f, null ));
		}
#endif
	}

	private bool isQuitting_ = false;

	private void OnApplicationQuit()
	{
		isQuitting_ = true;
	}

	private void OnDestroy()
	{
		if (isQuitting_ == false && MessageBus.IsInitialised())
		{
			MessageBus.Instance.showMessageAction += SetMessageDefinition;
		}
	}

	#endregion Flow

	#region interface

	public void Close()
	{
		if (messageDefinition_ != null)
		{
			messageDefinition_.SendOnCloseAction( );
		}
		messageDefinition_ = null;
		gameObject.SetActive( false );
	}

	public void SetMessageDefinition(ProofMessageDefinition d)
	{
		messageDefinition_ = d;
		proofText.text = messageDefinition_.text;
		ResetTimer( messageDefinition_.duration );
		StartTimer( );
		gameObject.SetActive( true );
	}

	#endregion interface

	#region Process

	private void ResetTimer(float t)
	{
		timeTillClose_ = t;
		SetTimerImage( );
	}

	private void StopTimer()
	{
		timerRunning_ = false;
		timerImage.color = stoppedColour;
		SetTimerImage( );
	}

	private void StartTimer( )
	{
		if (messageDefinition_ == null)
		{
			Debug.LogError( "Message definition is null" );
		}
		else
		{
			if (messageDefinition_.duration > 0f)
			{
				timerImage.color = runningColour;
				timerRunning_ = true;
				SetTimerImage( );
			}
		}
	}


	private void Update()
	{
		if (messageDefinition_ != null)
		{
			if (messageDefinition_.duration > 0f)
			{
				if (timeTillClose_ >= 0f)
				{
					if (timerRunning_)
					{
						timeTillClose_ -= Time.deltaTime;
						if (timeTillClose_ < 0f)
						{
							Close( );
						}
						else
						{
							SetTimerImage( );
						}
					}
				}
			}
		}
	}

	private void SetTimerImage()
	{
		if (messageDefinition_ == null)
		{
			Debug.LogError( "MessageDefinition is null" );
			timerImageRT.sizeDelta = new Vector2( timerImageRT.sizeDelta.x, sizeDeltaRange_.x );
		}
		else
		{
			if (messageDefinition_.duration > 0 && timeTillClose_ > 0f)
			{
				timerImageRT.sizeDelta = new Vector2( timerImageRT.sizeDelta.x, Mathf.Lerp( sizeDeltaRange_.y, sizeDeltaRange_.x, timeTillClose_ / messageDefinition_.duration ) );
			}
			else
			{
				timerImageRT.sizeDelta = new Vector2( timerImageRT.sizeDelta.x, sizeDeltaRange_.x );
			}
		}
	}

	#endregion Process


	#region Handlers

	public void OnOkButtonClicked()
	{
		Close( );
	}

	public void OnTimerButtonClicked()
	{
		if (messageDefinition_ == null)
		{
			Debug.LogError( "MessageDefinition is null" );
		}
		else
		{
			if (timerRunning_)
			{
				ResetTimer( messageDefinition_.duration );
				StopTimer( );
			}
			else
			{
				if (messageDefinition_.duration > 0f)
				{
					StartTimer( );
				}
			}
		}
	}

	#endregion Handlers

}
