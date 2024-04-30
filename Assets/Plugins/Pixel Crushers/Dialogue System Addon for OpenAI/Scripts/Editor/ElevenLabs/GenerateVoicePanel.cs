// Copyright (c) Pixel Crushers. All rights reserved.

#if USE_OPENAI

using UnityEngine;
using UnityEditor;
using PixelCrushers.DialogueSystem.DialogueEditor;
#if USE_ADDRESSABLES
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
#endif

namespace PixelCrushers.DialogueSystem.OpenAIAddon.ElevenLabs
{

    public enum AudioSequencerCommands { None, AudioWait, SALSA, Other }

    /// <summary>
    /// Panel to generate voiceover for a dialogue entry using an ElevenLabs voice actor.
    /// </summary>
    public class GenerateVoicePanel : ElevenLabsPanel
    {

        private string dialogueText;
        private Actor actor;
        private string voiceName;
        private string voiceID;

        private static string lastFilename;
        private static AudioSequencerCommands sequencerCommand = AudioSequencerCommands.None;
        private static string otherSequencerCommand = "";

        private static GUIContent HeadingLabel = new GUIContent("Generate Voiceover");
        public static GUIContent SequencerCommandLabel = new GUIContent("Sequencer Command", "Add sequencer command to dialogue entry's Sequence field.");

        protected override string Operation => "Generate Voiceover";

        public GenerateVoicePanel(string apiKey, DialogueDatabase database,
            Asset asset, DialogueEntry entry, Field field)
            : base(apiKey, database, asset, entry, field)
        {
            dialogueText = (entry != null) ? entry.DialogueText : string.Empty;
            actor = (entry != null) ? database.GetActor(entry.ActorID) : null;
            voiceName = (actor != null) ? actor.LookupValue(DialogueSystemFields.Voice) : null;
            voiceID = (actor != null) ? actor.LookupValue(DialogueSystemFields.VoiceID) : null;
            lastFilename = EditorPrefs.GetString(DialogueSystemOpenAIWindow.ElevenLabsLastFilename);
        }

        ~GenerateVoicePanel()
        {
            DestroyAudioClip();
        }

        public override void Draw()
        {
            base.Draw();
            DrawHeading(HeadingLabel, "Generate voiceover for this line using the actor's ElevenLabs voice.");
            if (actor == null)
            {
                EditorGUILayout.LabelField("Assign an actor to this dialogue entry first.");
            }
            else if (string.IsNullOrEmpty(voiceID))
            {
                EditorGUILayout.LabelField("Select a voice for this actor first.");
                if (GUILayout.Button("Select Voice"))
                {
                    DialogueSystemOpenAIWindow.Open(AIRequestType.SelectVoice, database, actor, null, null);
                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                DrawGenerateButton();
                DrawPreviewButton();
                DrawAcceptButton();
            }
        }

        private void DrawGenerateButton()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Dialogue Text");
            EditorGUILayout.TextArea(dialogueText);
            EditorGUILayout.TextField("Voice", voiceName);
            EditorGUI.EndDisabledGroup();
            sequencerCommand = (AudioSequencerCommands)EditorGUILayout.EnumPopup(SequencerCommandLabel, sequencerCommand);
            if (sequencerCommand == AudioSequencerCommands.Other)
            {
                otherSequencerCommand = EditorGUILayout.TextField("Command", otherSequencerCommand);
            }
            var needToInputSequencerCommand = sequencerCommand == AudioSequencerCommands.Other && string.IsNullOrEmpty(otherSequencerCommand);
            EditorGUI.BeginDisabledGroup(IsAwaitingReply || needToInputSequencerCommand);
            if (GUILayout.Button("Generate"))
            {
                GenerateAudio();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawPreviewButton()
        {
            EditorGUI.BeginDisabledGroup(audioClip == null || IsAwaitingReply);
            if (GUILayout.Button("Preview"))
            {
                EditorAudioUtility.PlayAudioClip(audioClip);
            }
            EditorGUI.EndDisabledGroup();
        }

        private void GenerateAudio()
        {
            DestroyAudioClip();
            IsAwaitingReply = true;
            ProgressText = $"Generating voiceover: {dialogueText}.";
            Debug.Log($"Generating voiceover for: {dialogueText}.");
            ElevenLabs.GetTextToSpeech(apiKey, modelId, voiceName, voiceID, 0, 0, dialogueText, OnReceivedTextToSpeech);
        }

        private void OnReceivedTextToSpeech(AudioClip audioClip)
        {
            IsAwaitingReply = false;
            if (audioClip == null) return;
            this.audioClip = audioClip;
            Debug.Log($"Playing: {dialogueText}.");
            EditorAudioUtility.PlayAudioClip(audioClip);
        }

        private void DrawAcceptButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(audioClip == null || IsAwaitingReply);
            if (GUILayout.Button("Accept"))
            {
                var filename = SaveAudioClip();
                if (!string.IsNullOrEmpty(filename))
                {
                    AddSelectedSequencerCommand(sequencerCommand, System.IO.Path.GetFileNameWithoutExtension(filename), entry);
                    SaveDatabaseChanges();
                    RefreshEditor();
                    CloseWindow();
                }
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Cancel"))
            {
                CloseWindow();
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void AddSelectedSequencerCommand(AudioSequencerCommands sequencerCommand, string entrytag, DialogueEntry entry)
        {
            switch (sequencerCommand)
            {
                case AudioSequencerCommands.AudioWait:
                case AudioSequencerCommands.SALSA:
                    AddSequencerCommand(sequencerCommand.ToString(), entrytag, entry);
                    break;
                case AudioSequencerCommands.Other:
                    AddSequencerCommand(otherSequencerCommand, entrytag, entry);
                    break;
            }
        }

        public static void AddSequencerCommand(string command, string entrytag, DialogueEntry entry)
        {
            var sequence = entry.Sequence;
            if (!(string.IsNullOrEmpty(sequence) || sequence.EndsWith(";")))
            {
                sequence += ";\n";
            }
            sequence += $"{command}({entrytag})";
            entry.Sequence = sequence;
        }

        private string SaveAudioClip()
        {
            if (audioClip == null) return string.Empty;
            var conversation = database.GetConversation(entry.conversationID);
            var defaultName = database.GetEntrytag(conversation, entry, GetEntrytagFormat());
            var path = "Assets";
            if (!string.IsNullOrEmpty(lastFilename))
            {
                path = Application.dataPath + "/" + System.IO.Path.GetDirectoryName(lastFilename).Replace("\\", "/").Substring("Assets/".Length);
            }
#if USE_ADDRESSABLES
            var title = "Save Audio Clip";
#else
            var title = "Save Audio Clip in Resources Folder";
#endif
            var filename = EditorUtility.SaveFilePanelInProject(title, defaultName, "wav", "", path);
            if (string.IsNullOrEmpty(filename)) return string.Empty;
            lastFilename = filename;
            EditorPrefs.SetString(DialogueSystemOpenAIWindow.ElevenLabsLastFilename, lastFilename);
            // Remove extra Assets/ & extension:
            var fullPath = Application.dataPath + "/" + filename.Substring("Assets/".Length); 
            Debug.Log($"Saving audio clip to {filename}");
            SavWav.Save(fullPath, audioClip);
            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(filename);

#if USE_ADDRESSABLES
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null) settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            if (settings != null)
            {
                var asset = AssetDatabase.LoadAssetAtPath<AudioClip>(filename);
                string assetPath = AssetDatabase.GetAssetPath(asset);
                string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                settings.CreateAssetReference(assetGUID);
                AddressableAssetEntry addressableEntry = settings.FindAssetEntry(assetGUID);
                if (addressableEntry != null)
                {
                    addressableEntry.address = System.IO.Path.GetFileNameWithoutExtension(filename);
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
                    AssetDatabase.SaveAssets();
                }
            }
#endif

            return filename;
        }

        public static EntrytagFormat GetEntrytagFormat()
        {
            var dialogueManager = GameObject.FindObjectOfType<DialogueSystemController>();
            if (dialogueManager != null)
            {
                return dialogueManager.displaySettings.cameraSettings.entrytagFormat;
            }
            else
            {
                return EntrytagFormat.ActorName_ConversationID_EntryID;
            }
        }

        private void SaveDatabaseChanges()
        {
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            DialogueEditorWindow.instance?.Repaint();
        }

        protected override void RefreshEditor()
        {
            Undo.RecordObject(database, Operation);
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            DialogueEditorWindow.instance?.Reset();
            DialogueEditorWindow.OpenDialogueEntry(database, entry.conversationID, entry.id);
            DialogueEditorWindow.instance?.Repaint();
            GUIUtility.ExitGUI();
        }

    }
}

#endif
