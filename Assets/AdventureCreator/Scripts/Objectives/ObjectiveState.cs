﻿/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2024
 *	
 *	"ObjectiveState.cs"
 * 
 *	Stores data for a state that an Objective can take.
 * 
 */

using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif

namespace AC
{

	/** Stores data for a state that an Objective can take. */
	[System.Serializable]
	public class ObjectiveState : ITranslatable
	{

		#region Variables

		/** A unique identifier */
		public int ID;
		[SerializeField] protected string label;
		/** The translation ID for the label text, generated by the Speech Manager */
		public int labelLineID = -1;
		/** An description for the state */
		public string description;
		/** The translation ID for the description text, generated by the Speech Manager */
		public int descriptionLineID = -1;
		/** The type of state it is (Active, Complete, Fail) */
		public ObjectiveStateType stateType;
		/** An ActionList to run when the state is entered */
		public ActionListAsset actionListOnEnter = null;
		/** If True, all Objectives in the sub-category will begin automatically when this state is entered */
		public bool autoStartSubObjectives = false;

		[SerializeField] private int autoStateIDOnAllSubObsCompletePlusOne = 0;
		[SerializeField] private int autoStateIDOnAnySubObsCompletePlusOne = 0;
		[SerializeField] private int autoStateIDOnAllSubObsFailPlusOne = 0;
		[SerializeField] private int autoStateIDOnAnySubObsFailPlusOne = 0;
		[SerializeField] private int linkedCategoryIDPlusOne = 0;

		#endregion


		#region Constructors

		public ObjectiveState (int _ID, string _label, ObjectiveStateType _stateType)
		{
			ID = _ID;
			stateType = _stateType;
			label = _label;
			labelLineID = -1;
			description = string.Empty;
			descriptionLineID = -1;
			actionListOnEnter = null;
			linkedCategoryIDPlusOne = 0;
			autoStartSubObjectives = false;
		}


		public ObjectiveState (int[] idArray)
		{
			stateType = ObjectiveStateType.Active;
			label = string.Empty;
			labelLineID = -1;
			description = string.Empty;
			descriptionLineID = -1;
			actionListOnEnter = null;
			linkedCategoryIDPlusOne = 0;
			autoStartSubObjectives = false;

			ID = 0;
			// Update id based on array
			foreach (int _id in idArray)
			{
				if (ID == _id)
					ID ++;
			}
		}

		#endregion


		#region PublicFunctions

		/**
		 * <summary>Gets the states's label text in a given language</summary>
		 * <param name = "languageNumber">The language index, where 0 = the game's default language</param>
		 * <returns>The label text</returns>
		 */
		public string GetLabel (int languageNumber = 0)
		{
			return KickStarter.runtimeLanguages.GetTranslation (label, labelLineID, languageNumber, GetTranslationType (0));
		}


		/**
		 * <summary>Gets the states's description text in a given language</summary>
		 * <param name = "languageNumber">The language index, where 0 = the game's default language</param>
		 * <returns>The description text</returns>
		 */
		public string GetDescription (int languageNumber = 0)
		{
			return KickStarter.runtimeLanguages.GetTranslation (description, descriptionLineID, languageNumber, GetTranslationType (0));
		}


		/**
		 * <summary>Checks if the state matches a given display type, as used by InventoryBox elements</summary>
		 * <param name = "displayType">The display type</param>
		 * <returns>True if the state matches</returns>
		 */
		public bool DisplayTypeMatches (ObjectiveDisplayType displayType)
		{
			switch (displayType)
			{
				case ObjectiveDisplayType.All:
					return true;

				case ObjectiveDisplayType.ActiveOnly:
					return (stateType == ObjectiveStateType.Active);

				case ObjectiveDisplayType.CompleteOnly:
					return (stateType == ObjectiveStateType.Complete);

				case ObjectiveDisplayType.FailedOnly:
					return (stateType == ObjectiveStateType.Fail);

				default:
					return false;
			}
		}


		public string GetStateTypeText (int languageNumber = 0)
		{
			switch (stateType)
			{
				case ObjectiveStateType.Active:
					return KickStarter.runtimeLanguages.GetTranslation (KickStarter.inventoryManager.objectiveStateActiveLabel.label, KickStarter.inventoryManager.objectiveStateActiveLabel.lineID, languageNumber, KickStarter.inventoryManager.objectiveStateActiveLabel.GetTranslationType (0));

				case ObjectiveStateType.Complete:
					return KickStarter.runtimeLanguages.GetTranslation (KickStarter.inventoryManager.objectiveStateCompleteLabel.label, KickStarter.inventoryManager.objectiveStateCompleteLabel.lineID, languageNumber, KickStarter.inventoryManager.objectiveStateCompleteLabel.GetTranslationType (0));

				case ObjectiveStateType.Fail:
					return KickStarter.runtimeLanguages.GetTranslation (KickStarter.inventoryManager.objectiveStateFailLabel.label, KickStarter.inventoryManager.objectiveStateFailLabel.lineID, languageNumber, KickStarter.inventoryManager.objectiveStateFailLabel.GetTranslationType (0));

				default:
					return string.Empty;
			}

		}

		#endregion


		#region GetSet

		/** The states's label.  This will set the title to '(Untitled)' if empty. */
		public string Label
		{
			get
			{
				if (string.IsNullOrEmpty (label))
				{
					label = "(Untitled)";
				}
				return label;
			}
			set
			{
				label = value;
			}
		}

		#endregion


		#if UNITY_EDITOR

		public void ShowGUI (Objective objective, string apiPrefix)
		{
			label = CustomGUILayout.TextField ("Label:", label, apiPrefix + "label");
			if (labelLineID > -1)
			{
				EditorGUILayout.LabelField ("Speech Manager ID:", labelLineID.ToString ());
			}

			if (ID >= 2)
			{
				stateType = (ObjectiveStateType) CustomGUILayout.EnumPopup ("State type:", stateType, apiPrefix + "stateType");
			}
			else
			{
				EditorGUILayout.LabelField ("State type: " + stateType.ToString ());
			}

			EditorGUILayout.BeginHorizontal ();
			CustomGUILayout.LabelField ("Description:", GUILayout.Width (140f), apiPrefix + "description");
			EditorStyles.textField.wordWrap = true;
			description = CustomGUILayout.TextArea (description, GUILayout.MaxWidth (800f), apiPrefix + "description");
			EditorGUILayout.EndHorizontal ();
			if (descriptionLineID > -1)
			{
				EditorGUILayout.LabelField ("Speech Manager ID:", descriptionLineID.ToString ());
			}

			actionListOnEnter = ActionListAssetMenu.AssetGUI ("ActionList on enter:", actionListOnEnter, objective.Title + "_" + Label + "_OnEnter");

			LinkedCategoryID = KickStarter.inventoryManager.ChooseCategoryGUI ("Sub-Objectives category:", LinkedCategoryID, false, false, true, apiPrefix + "LinkedCategoryID", "A category for Objectives considered to be this state's 'sub-objectives'", true);

			if (LinkedCategoryID >= 0 && LinkedCategoryID == objective.binID)
			{
				LinkedCategoryID = -1;
				ACDebug.LogWarning ("A state cannot have the same sub-Objectives category as its parent Objective");
			}

			if (LinkedCategoryID >= 0)
			{
				autoStartSubObjectives = CustomGUILayout.ToggleLeft ("Auto-start sub-Objectives when enter?", autoStartSubObjectives, apiPrefix + "autoStartSubObjectives", "If True, all Objectives in the sub-category will begin automatically when this state is entered");

				EditorGUILayout.Space ();
				CustomGUILayout.LabelField ("Automatic state-switching:");
				AutoStateIDOnAllSubObsComplete = ChooseStateGUI (objective, "All sub-obs complete:", AutoStateIDOnAllSubObsComplete, apiPrefix + "AutoStateIDOnAllSubObsComplete", "The state to automatically enter when all sub-objectives are complete");
				AutoStateIDOnAnySubObsComplete = ChooseStateGUI (objective, "Any sub-ob completes:", AutoStateIDOnAnySubObsComplete, apiPrefix + "AutoStateIDOnAnySubObsComplete", "The state to automatically enter when any sub-objective is complete");
				AutoStateIDOnAllSubObsFail = ChooseStateGUI (objective, "All sub-obs fail:", AutoStateIDOnAllSubObsFail, apiPrefix + "AutoStateIDOnAllSubObsFail", "The state to automatically enter when all sub-objectives are failed");
				AutoStateIDOnAnySubObsFail = ChooseStateGUI (objective, "Any sub-ob fails:", AutoStateIDOnAnySubObsFail, apiPrefix + "AutoStateIDOnAnySubObsFail", "The state to automatically enter when any sub-objective is failed");
			}
		}


		private int ChooseStateGUI (Objective objective, string label, int stateID, string apiPrefix = "", string tooltip = "")
		{
			if (objective.NumStates <= 1)
			{
				return -1;
			}

			int chosenNumber = 0;
			List<PopupSelectData> popupSelectDataList = new List<PopupSelectData> ();

			popupSelectDataList.Add (new PopupSelectData (-1, "None", -1));

			for (int i = 0; i < objective.NumStates; i++)
			{
				if (objective.states[i] == this) continue;

				PopupSelectData popupSelectData = new PopupSelectData (objective.states[i].ID, objective.states[i].Label, i);
				popupSelectDataList.Add (popupSelectData);

				if (popupSelectData.ID == stateID)
				{
					chosenNumber = popupSelectDataList.Count - 1;
				}
			}

			List<string> labelList = new List<string> ();

			foreach (PopupSelectData popupSelectData in popupSelectDataList)
			{
				labelList.Add (popupSelectData.label);
			}

			chosenNumber = CustomGUILayout.Popup (label, chosenNumber, labelList.ToArray (),  apiPrefix, tooltip);

			if (chosenNumber <= 0)
			{
				return -1;
			}

			int rootIndex = popupSelectDataList[chosenNumber].rootIndex;
			return objective.states[rootIndex].ID;
		}

		#endif


		#region GetSet

		/** The cateogry ID for the state's sub-objectives, if >= 0 */
		public int LinkedCategoryID
		{
			get
			{
				return linkedCategoryIDPlusOne - 1;
			}
			set
			{
				linkedCategoryIDPlusOne = value + 1;
			}
		}


		/** The state ID to automatically switch to when all sub-objectives are complete */
		public int AutoStateIDOnAllSubObsComplete
		{
			get
			{
				return autoStateIDOnAllSubObsCompletePlusOne - 1;
			}
			set
			{
				autoStateIDOnAllSubObsCompletePlusOne = value + 1;
			}
		}


		/** The state ID to automatically switch to when any sub-objective is complete */
		public int AutoStateIDOnAnySubObsComplete
		{
			get
			{
				return autoStateIDOnAnySubObsCompletePlusOne - 1;
			}
			set
			{
				autoStateIDOnAnySubObsCompletePlusOne = value + 1;
			}
		}


		/** The state ID to automatically switch to when all sub-objectives are failed */
		public int AutoStateIDOnAllSubObsFail
		{
			get
			{
				return autoStateIDOnAllSubObsFailPlusOne - 1;
			}
			set
			{
				autoStateIDOnAllSubObsFailPlusOne = value + 1;
			}
		}


		/** The state ID to automatically switch to when any sub-objectives are failed */
		public int AutoStateIDOnAnySubObsFail
		{
			get
			{
				return autoStateIDOnAnySubObsFailPlusOne - 1;
			}
			set
			{
				autoStateIDOnAnySubObsFailPlusOne = value + 1;
			}
		}

		#endregion


		#region ITranslatable

		public string GetTranslatableString (int index)
		{
			if (index == 0)
			{
				return label;
			}
			else
			{
				return description;
			}
		}


		public int GetTranslationID (int index)
		{
			if (index == 0)
			{
				return labelLineID;
			}
			else
			{
				return descriptionLineID;
			}
		}


		public AC_TextType GetTranslationType (int index)
		{
			return AC_TextType.Objective;
		}


		#if UNITY_EDITOR

		public void UpdateTranslatableString (int index, string updatedText)
		{
			if (index == 0)
			{
				label = updatedText;
			}
			else
			{
				description = updatedText;
			}
		}


		public int GetNumTranslatables ()
		{
			return 2;
		}


		public bool HasExistingTranslation (int index)
		{
			if (index == 0)
			{
				return (labelLineID > -1);
			}
			else
			{
				return (descriptionLineID > -1);
			}
		}



		public void SetTranslationID (int index, int _lineID)
		{
			if (index == 0)
			{
				labelLineID = _lineID;
			}
			else
			{
				descriptionLineID = _lineID;
			}
		}


		public string GetOwner (int index)
		{
			return string.Empty;
		}


		public bool OwnerIsPlayer (int index)
		{
			return false;
		}


		public bool CanTranslate (int index)
		{
			if (index == 0)
			{
				return !string.IsNullOrEmpty (label);
			}
			return !string.IsNullOrEmpty (description);
		}

		#endif

		#endregion

	}


	[System.Serializable]
	public class ObjectiveStateLabel : ITranslatable
	{
		
		/** The display text */
		public string label;
		/** The translation ID, as used by SpeechManager */
		public int lineID;
		
		
		/** The default Constructor. */
		public ObjectiveStateLabel (string text)
		{
			label = text;
			lineID = -1;
		}


		#region ITranslatable

		public string GetTranslatableString (int index)
		{
			return label;
		}


		public int GetTranslationID (int index)
		{
			return lineID;
		}

		
		public AC_TextType GetTranslationType (int index)
		{
			return AC_TextType.Objective;
		}


		#if UNITY_EDITOR

		public void UpdateTranslatableString (int index, string updatedText)
		{
			label = updatedText;
		}

		
		public int GetNumTranslatables ()
		{
			return 1;
		}


		public bool HasExistingTranslation (int index)
		{
			return lineID > -1;
		}


		public void SetTranslationID (int index, int _lineID)
		{
			lineID = _lineID;
		}


		public string GetOwner (int index)
		{
			return string.Empty;
		}


		public bool OwnerIsPlayer (int index)
		{
			return false;
		}


		public bool CanTranslate (int index)
		{
			return (!string.IsNullOrEmpty (label));
		}

		#endif

		#endregion
		
	}

}