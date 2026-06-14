using System;
using System.Collections;
using DIALOGUE;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_General : CMD_DatabaseExtension
    {
        private static string[] PARAM_SPEED => new string[] { "spd", "speed" };
        private static string[] PARAM_FILE => new string[] { "f", "file" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));
            database.AddCommand("startdialogue", new Action<string[]>(StartDialogue));
            database.AddCommand("openloader", new Action<string[]>(OpenDialogueLoader));
            database.AddCommand("closeloader", new Action<string[]>(CloseDialogueLoader));
            database.AddCommand("resetloader", new Action<string[]>(ResetLoader));
        }

        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }

        private static void StartDialogue(string[] data)
        {
            string fileName;

            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_FILE, out fileName, defaultValue: "testFile");

            DialogueManager.instance.SetFileToRead(fileName);

            if (int.TryParse(fileName, out int gameScriptIndex))
            {
                PlayerManager.instance.UpdateGameScriptIndex(gameScriptIndex);
            }

            DialogueManager.instance.StartDialogue();
        }

        private static void OpenDialogueLoader(string[] data)
        {
            if (DialogueLoaderManager.instance == null)
                return;

            float speed = 1f;
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            DialogueLoaderManager.instance.Open(speed);
        }

        private static void CloseDialogueLoader(string[] data)
        {
            if (DialogueLoaderManager.instance == null)
                return;

            float speed = 1f;
            var parameters = ConvertDataToParameters(data);
            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            DialogueLoaderManager.instance.Close(speed);
        }
        private static void ResetLoader(string[] data)
        {
            if (DialogueLoaderManager.instance == null)
                return;

            DialogueLoaderManager.instance.ResetDialogueLoader();
        }
    }
}