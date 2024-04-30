// Copyright (c) Pixel Crushers. All rights reserved.

#if USE_OPENAI

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if USE_ADDRESSABLES
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
#endif

namespace PixelCrushers.DialogueSystem.OpenAIAddon.ElevenLabs
{

    /// <summary>
    /// Panel to select an ElevenLabs voice actor.
    /// </summary>
    public class SelectVoicePanel : ElevenLabsPanel
    {

        private VoiceList voiceList;
        private string[] voiceNames = null;
        private int voiceIndex = -1;
        private string previewText = "Hello, world.";
        private bool isGeneratingAllLines = false;

#if USE_OVERTONE
        private static string[] overtoneVoiceNames = null;
        private int overtoneVoiceIndex = -1;
#endif

        private static GUIContent HeadingLabel = new GUIContent("Select Voice Actor");
        private static GUIContent GenerateAllLinesLabel = new GUIContent("Generate All Lines", "Generate voice audio for this actor for all dialogue entries that don't already have audio.");

        protected override string Operation => "Select Voice";
        private Actor Actor => (asset is Actor) ? asset as Actor : null;
        private bool IsVoiceSelected => voiceNames != null && (0 <= voiceIndex && voiceIndex < voiceNames.Length);

        private static AudioSequencerCommands sequencerCommand = AudioSequencerCommands.None;
        private static string otherSequencerCommand = "";
        private static string path = "Assets";

        private class VoiceGenerationRequest
        {
            public Conversation conversation;
            public DialogueEntry entry;
            public string text;
            public string description;
            public int entryQueueNum;

            public VoiceGenerationRequest(Conversation conversation, DialogueEntry entry, string text, string description, int entryQueueNum)
            {
                this.conversation = conversation;
                this.entry = entry;
                this.text = text;
                this.description = description;
                this.entryQueueNum = entryQueueNum;
            }
        }

        private Queue<VoiceGenerationRequest> voiceGenerationQueue = new Queue<VoiceGenerationRequest>();
        private int totalEntryQueueSize;
        private VoiceGenerationRequest currentJob = null;
        private EntrytagFormat entrytagFormat;

        public SelectVoicePanel(string apiKey, DialogueDatabase database,
            Asset asset, DialogueEntry entry, Field field)
            : base(apiKey, database, asset, entry, field)
        {
            RefreshVoiceList();
#if USE_OVERTONE
            overtoneVoiceIndex = GetOvertoneVoiceIndex();
#endif
        }

        ~SelectVoicePanel()
        {
            DestroyAudioClip();
        }

        public override void Draw()
        {
            base.Draw();
            DrawHeading(HeadingLabel, "Select an ElevenLabs voice actor.");
            DrawVoiceActorDropdown();
            DrawPreviewButton();
            DrawAcceptButton();
            DrawGenerateAllLinesButton();
#if USE_OVERTONE
            DrawOvertoneSection();
#endif
        }

        private void DrawVoiceActorDropdown()
        {
            if (IsAwaitingReply && !isGeneratingAllLines)
            {
                DrawStatus();
            }
            else if (voiceNames == null)
            {
                EditorGUILayout.HelpBox("Error retrieving voice actor list.", MessageType.Warning);
            }
            else
            {
                voiceIndex = EditorGUILayout.Popup("Voice Actor", voiceIndex, voiceNames);
            }
        }

        private void DrawPreviewButton()
        {
            if (IsVoiceSelected)
            {
                EditorGUILayout.BeginHorizontal();
                previewText = EditorGUILayout.TextField("Preview Text", previewText);
                if (GUILayout.Button("Preview", GUILayout.Width(60)))
                {
                    PreviewSelectedVoice();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (IsAwaitingReply) return;
        }

        private void RefreshVoiceList()
        {
            IsAwaitingReply = true;
            ProgressText = "Retrieving ElevenLabs voice list.";
            ElevenLabs.GetVoiceList(apiKey, OnReceivedVoiceList);
        }

        private void OnReceivedVoiceList(VoiceList voiceList)
        {
            IsAwaitingReply = false;
            this.voiceList = voiceList;
            if (voiceList == null)
            {
                voiceNames = null;
            }
            else
            {
                var currentActorVoiceID = Actor.LookupValue(DialogueSystemFields.VoiceID);
                voiceNames = new string[voiceList.voices.Count];
                for (int i = 0; i < voiceNames.Length; i++)
                {
                    voiceNames[i] = voiceList.voices[i].name;
                    if (voiceList.voices[i].voice_id == currentActorVoiceID) voiceIndex = i;
                }
            }
            Repaint();
        }

        private void PreviewSelectedVoice()
        {
            DestroyAudioClip();
            if (!(0 <= voiceIndex && voiceIndex < voiceList.voices.Count)) return;
            IsAwaitingReply = true;
            ProgressText = $"Retrieving preview sample for {voiceNames[voiceIndex]}.";
            var voiceData = voiceList.voices[voiceIndex];
            ElevenLabs.GetTextToSpeech(apiKey, modelId, voiceData.name, voiceData.voice_id, 0, 0, previewText, OnReceivedTextToSpeech);
        }

        private void OnReceivedTextToSpeech(AudioClip audioClip)
        {
            IsAwaitingReply = false;
            if (audioClip == null) return;
            this.audioClip = audioClip;
            Debug.Log($"Playing voice sample for {voiceNames[voiceIndex]}.");
            EditorAudioUtility.PlayAudioClip(audioClip);
        }

        private void DrawAcceptButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!(Actor != null && IsVoiceSelected));
            if (GUILayout.Button("Accept"))
            {
                Field.SetValue(Actor.fields, DialogueSystemFields.Voice, voiceList.voices[voiceIndex].name);
                Field.SetValue(Actor.fields, DialogueSystemFields.VoiceID, voiceList.voices[voiceIndex].voice_id);
                RefreshEditor();
                CloseWindow();
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Cancel"))
            {
                CloseWindow();
            }
            EditorGUILayout.EndHorizontal();
        }

        #region Generate All Lines

        private void DrawGenerateAllLinesButton()
        {
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            GUILayout.Label("Generate All Lines", EditorStyles.boldLabel);
            sequencerCommand = (AudioSequencerCommands)EditorGUILayout.EnumPopup(GenerateVoicePanel.SequencerCommandLabel, sequencerCommand);
            if (sequencerCommand == AudioSequencerCommands.Other)
            {
                otherSequencerCommand = EditorGUILayout.TextField("Command", otherSequencerCommand);
            }
            var needToInputSequencerCommand = sequencerCommand == AudioSequencerCommands.Other && string.IsNullOrEmpty(otherSequencerCommand);
            EditorGUI.BeginDisabledGroup(IsAwaitingReply || needToInputSequencerCommand || Actor == null || voiceIndex == -1);
            if (GUILayout.Button(GenerateAllLinesLabel))
            {
                if (EditorUtility.DisplayDialog("Generate All Lines",
                    "This may take a long time to complete. Proceed?", "OK", "Cancel"))
                {
                    path = EditorUtility.OpenFolderPanel("Generate All Lines", path, "");
                    if (!string.IsNullOrEmpty(path))
                    {
                        GenerateAllLines();
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
            if (isGeneratingAllLines)
            {
                DrawStatus();
                if (GUILayout.Button("Cancel"))
                {
                    CancelGenerateAllLines();
                }
            }
        }

        private void GenerateAllLines()
        {
            IsAwaitingReply = true;
            isGeneratingAllLines = true;
            entrytagFormat = GenerateVoicePanel.GetEntrytagFormat();
            voiceGenerationQueue.Clear();
            int actorID = Actor.id;
            int entryQueueNum = 0;
            for (int i = 0; i < database.conversations.Count; i++)
            {
                var conversation = database.conversations[i];
                var conversationTitle = conversation.Title;
                for (int j = 0; j < conversation.dialogueEntries.Count; j++)
                {
                    var dialogueEntry = conversation.dialogueEntries[j];
                    if (dialogueEntry.ActorID != actorID) continue;
                    var fullPath = GetAudioClipFullPath(conversation, dialogueEntry);
                    if (System.IO.File.Exists(fullPath)) continue;
                    var text = dialogueEntry.DialogueText;
                    if (string.IsNullOrEmpty(text)) text = dialogueEntry.MenuText;
                    if (string.IsNullOrEmpty(text)) continue;
                    var description = $"{conversationTitle} entry {dialogueEntry.id}";
                    voiceGenerationQueue.Enqueue(new VoiceGenerationRequest(conversation, dialogueEntry, text, description, entryQueueNum++));
                }
            }
            totalEntryQueueSize = voiceGenerationQueue.Count;
            GenerateNextLine();
        }

        private void GenerateNextLine()
        {
            if (voiceGenerationQueue.Count > 0)
            {
                var job = voiceGenerationQueue.Dequeue();
                currentJob = job;
                float progress = (float)job.entryQueueNum / (float)totalEntryQueueSize;
                ProgressText = $"[{progress}%] {job.description}: {job.text}";
                Debug.Log(ProgressText);
                var voiceData = voiceList.voices[voiceIndex];
                ElevenLabs.GetTextToSpeech(apiKey, modelId, voiceData.name, voiceData.voice_id, 0, 0, job.text, OnReceivedLine);
            }
            else
            {
                Debug.Log("Finished generating lines for actor.");
                FinishGenerateAllLines();
            }
        }

        private void OnReceivedLine(AudioClip audioClip)
        {
            if (audioClip != null)
            {
                try
                {
                    var filename = SaveAudioClip(audioClip);
                    GenerateVoicePanel.AddSelectedSequencerCommand(sequencerCommand, System.IO.Path.GetFileNameWithoutExtension(filename), currentJob.entry);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            GenerateNextLine();
        }

        private string GetAudioClipFullPath(Conversation conversation, DialogueEntry dialogueEntry)
        {
            var entrytag = database.GetEntrytag(conversation, dialogueEntry, entrytagFormat);
            return $"{path}/{entrytag}.wav";
        }

        private string SaveAudioClip(AudioClip audioClip)
        {
            var fullPath = GetAudioClipFullPath(currentJob.conversation, currentJob.entry);
            var filename = "Assets/" + fullPath.Substring(Application.dataPath.Length);
            Debug.Log($"Saving audio clip to {fullPath}");
            SavWav.Save(fullPath, audioClip);
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

        private void CancelGenerateAllLines()
        {
            Debug.Log("Cancelled generating all lines of voice acting for actor.");
            FinishGenerateAllLines();
        }

        private void FinishGenerateAllLines()
        {
            isGeneratingAllLines = false;
            IsAwaitingReply = false;
            ProgressText = string.Empty;
            voiceGenerationQueue.Clear();
            AssetDatabase.Refresh();
        }


        #endregion

        #region Overtone

#if USE_OVERTONE

        private static GUIContent OvertoneHeading = new GUIContent("Overtone");
        private static GUIContent OvertoneRefreshLabel = new GUIContent("Refresh", "Refresh list of Overtone voices in project.");

        private void DrawOvertoneSection()
        {
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            DrawHeading(OvertoneHeading, "Select an Overtone voice actor.");
            if (overtoneVoiceNames == null)
            {
                RefreshOvertoneVoices();
            }
            overtoneVoiceIndex = EditorGUILayout.Popup("Overtone Voice", overtoneVoiceIndex, overtoneVoiceNames);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(OvertoneRefreshLabel))
            {
                RefreshOvertoneVoices();
            }
            EditorGUI.BeginDisabledGroup(overtoneVoiceIndex == -1);
            if (GUILayout.Button("Accept"))
            {
                Field.SetValue(Actor.fields, DialogueSystemFields.Voice, overtoneVoiceNames[overtoneVoiceIndex]);
                Field.SetValue(Actor.fields, DialogueSystemFields.VoiceID, string.Empty);
                RefreshEditor();
                CloseWindow();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private string[] LoadOvertoneVoices()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Overtone Integration", "Identifying Overtone voices in project. Please wait...", 0);
                var list = new List<string>();
                string[] dirs = System.IO.Directory.GetDirectories(Application.dataPath, "Resources", System.IO.SearchOption.AllDirectories);
                foreach (string dir in dirs)
                {
                    string[] files = System.IO.Directory.GetFiles(dir, "*.config.json", System.IO.SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        var text = System.IO.File.ReadAllText(file);
                        if (text.Contains("\"espeak\""))
                        {
                            var voiceName = System.IO.Path.GetFileNameWithoutExtension(file);
                            voiceName = voiceName.Substring(0, voiceName.Length - ".config".Length);
                            list.Add(voiceName);
                        }
                    }
                }
                return list.ToArray();
            }
            catch (System.Exception)
            {
                return new string[0];
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }            
        }

        private void RefreshOvertoneVoices()
        {
            overtoneVoiceNames = LoadOvertoneVoices();
            overtoneVoiceIndex = GetOvertoneVoiceIndex();
        }

        private int GetOvertoneVoiceIndex()
        {
            if (overtoneVoiceNames == null)
            {
                RefreshOvertoneVoices();
                return overtoneVoiceIndex;
            }
            else
            { 
                string actorVoice = Field.LookupValue(Actor.fields, DialogueSystemFields.Voice);
                for (int i = 0; i < overtoneVoiceNames.Length; i++)
                {
                    if (overtoneVoiceNames[i] == actorVoice)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

#endif

        #endregion

    }
}

#endif
