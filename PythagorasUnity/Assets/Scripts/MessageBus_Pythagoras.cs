﻿using UnityEngine;
using System.Collections;

public partial class MessageBus : RJWard.Core.Singleton.SingletonApplicationLifetimeLazy<MessageBus>
{
	public System.Action<ProofTextPanel.MessageDefinition> showMessageAction;
	public void sendShowMessageAction( ProofTextPanel.MessageDefinition d )
	{
		if (showMessageAction != null)
		{
			showMessageAction( d );
		}
		else
		{
			Debug.LogWarning( "No showMessageAction" );
		}
	}
	public void clear( )
	{
		showMessageAction = null;
	}
}
