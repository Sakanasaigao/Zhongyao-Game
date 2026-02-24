using System.Collections;
using System.Collections.Generic;
using CHARACTERS;
using UnityEngine;

namespace DIALOGUE
{
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private Coroutine process = null;
        public bool isRunning => process != null;

        private TextArchitect architect = null;
        private bool userPrompt = false;

        public System.Action onConversationEnd;

        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;
        }

        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }

        public Coroutine StartConversation(List<string> conversation)
        {
            StopConversation();
            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
            return process;
        }

        public void StopConversation()
        {
            if (!isRunning) return;
            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation(List<string> conversation)
        {
            for (int i = 0; i < conversation.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(conversation[i])) continue;

                DIALOGUE_LINE line = DialogueParser.Parse(conversation[i]);

                // Open dialogueData
                if (line.hasDialogue)
                    yield return Line_RunDialogue(line);

                // Run any commandData
                if (line.hasCommand)
                    yield return Line_RunCommands(line);

                if (line.hasDialogue)
                {
                    yield return WaitForUserInput();
                    CommandManager.instance.StopAllProcesses();
                }
            }

            process = null;
            onConversationEnd?.Invoke();
        }

        IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
        {
            if (line.hasSpeaker)
                HandleSpeakerLogic(line.speakerData);

            PassedDialogueLineManager.instance.AddLineToPassedLines(line);

            if (ReviewManager.Instance != null)
            {
                string logName = line.hasSpeaker ? line.speakerData.displayname : "旁白";
                string logContent = "";
                if (line.dialogueData != null && line.dialogueData.segments != null)
                {
                    foreach (var segment in line.dialogueData.segments)
                        logContent += segment.dialogue;
                }

                if (!string.IsNullOrEmpty(logContent))
                {
                    ReviewManager.Instance.AddDialogue(logName, logContent);
                }
            }
            yield return BuildLineSegments(line.dialogueData);
        }

        private void HandleSpeakerLogic(DL_SPEAKER_DATA speakerData)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesNotExist: false);
            if (character == null)
                CharacterManager.instance.CreateCharacter(speakerData.name, revealAfterCreation: false);

            if (speakerData.makeCharacterEnter)
            {
                if (character == null)
                    CharacterManager.instance.CreateCharacter(speakerData.name, revealAfterCreation: true);
                else
                    character.Show();
            }

            dialogueSystem.ShowSpeakerName(speakerData.displayname);
            DialogueSystem.instance.ApplySpeakerDataToDialogueContainer(speakerData.name);

            if (speakerData.isCastingPosition)
                character.MoveToPosition(speakerData.castPosition, smooth: true);

            if (speakerData.isCastingExpressions)
            {
                foreach (var ce in speakerData.CastExpressions)
                    character.OnReceiveCastingExpression(ce.layer, ce.expression);
            }
        }

        IEnumerator Line_RunCommands(DIALOGUE_LINE line)
        {
            List<DL_COMMAND_DATA.Command> commands = line.commandData.commands;
            foreach (DL_COMMAND_DATA.Command command in commands)
            {
                if (command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while (!cw.IsDone)
                    {
                        if (userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }
            yield return null;
        }

        IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];
                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);
                yield return BuildDialogue(segment.dialogue, segment.appendText);
                yield return null;
            }
        }

        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
        {
            switch (segment.startSignal)
            {
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                    yield return new WaitForSeconds(segment.signalDelay);
                    break;
            }
        }

        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            architect.Stop();
            if (!append) architect.Build(dialogue);
            else architect.Append(dialogue);

            bool isFirstClickProcessed = false;
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!isFirstClickProcessed)
                    {
                        if (!architect.hurryUp) architect.hurryUp = true;
                        else architect.ForceComplete();
                        isFirstClickProcessed = true;
                    }
                    userPrompt = false;
                }
                yield return null;
                isFirstClickProcessed = false;
            }
        }

        IEnumerator WaitForUserInput()
        {
            userPrompt = false;
            while (!userPrompt) yield return null;
            userPrompt = false;
        }
    }
}