﻿/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2024
 *	
 *	"SpeechPlayableClip.cs"
 * 
 *	A PlayableAsset used by SpeechPlayableBehaviour
 * 
 */

#if UNITY_EDITOR
using UnityEditor;
#endif

#if TimelineIsPresent
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AC
{

	/** A PlayableAsset used by SpeechPlayableBehaviour */
	[System.Serializable]
	public class SpeechPlayableClip : PlayableAsset, ITimelineClipAsset
	{

		#region Variables

		/** If True, the line is spoken by the Player */
		public bool isPlayerLine;
		/** The ID of the Player, if not the active one */
		public int playerID = -1;
		/** The speaking character */
		public Char speaker;
		/** Data for the speech line itself */
		public SpeechPlayableData speechPlayableData;
		/** The playback mode */
		public SpeechTrackPlaybackMode speechTrackPlaybackMode;

		private SpeechPlayableBehaviour lastSetBehaviour;
		public int trackInstanceID;


		#endregion


		#region PublicFunctions

		public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
		{
			SpeechPlayableBehaviour template = new SpeechPlayableBehaviour ();
			var playable = ScriptPlayable<SpeechPlayableBehaviour>.Create (graph, template);
			SpeechPlayableBehaviour clone = playable.GetBehaviour ();

			clone.Init (speechPlayableData, speaker, isPlayerLine, playerID, speechTrackPlaybackMode, trackInstanceID);
			lastSetBehaviour = clone;

			return playable;
		}


		public string GetDisplayName ()
		{
			if (!string.IsNullOrEmpty (speechPlayableData.messageText))
			{
				return speechPlayableData.messageText;
			}
			return "Speech text";
		}

		#endregion


		#if UNITY_EDITOR

		public void ShowGUI ()
		{
			if (speechPlayableData.lineID > -1)
			{
				EditorGUILayout.LabelField ("Speech Manager ID:", speechPlayableData.lineID.ToString ());
			}

			speechPlayableData.messageText = CustomGUILayout.TextArea ("Line text:", speechPlayableData.messageText);
			speechPlayableData.isBackground = CustomGUILayout.Toggle ("Play in background?", speechPlayableData.isBackground);
		}

		#endif


		#region GetSet

		public ClipCaps clipCaps
		{
			get
			{
				return ClipCaps.None;
			}
		}


		/** The last-set SpeechPlayableBehaviour generated by the clip */
		public SpeechPlayableBehaviour LastSetBehaviour
		{
			get
			{
				return lastSetBehaviour;
			}
		}

		#endregion

	}

}

#endif