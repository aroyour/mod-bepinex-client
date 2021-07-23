﻿using HarmonyLib;
using UnityEngine.SceneManagement;

namespace Polus.Patches.Permanent
{
    public class ChatBubbleRichTextPatch
    {
        [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetText))]
        public class DisableHudManagerOOGPatch {
            [HarmonyPostfix]
            public static void Update(ChatBubble __instance)
            {
                __instance.TextArea.richText = true;
            }
        }
    }
}