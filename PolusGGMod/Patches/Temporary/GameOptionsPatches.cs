﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;
using Hazel;
using PolusGG.Enums;
using PolusGG.Extensions;
using PolusGG.Utils;
using TMPro;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PolusGG.Patches.Temporary {
    public static class GameOptionsPatches {
        public static Dictionary<string, List<GameOption>> Categories = new();
        public static List<GameOption> NoCategory = new();
        public static Dictionary<string, GameOption> OptionMap = new();
        internal static object Lockable = new();

        private static TextMeshPro _groupTitle;
        private static KeyValueOption _enumOption;
        private static NumberOption _numbOption;
        private static ToggleOption _boolOption;

        public static void UpdateHudString() {
            CatchHelper.TryCatch(
                () => HudManager.Instance.GameSettings.text = PlayerControl.GameOptions.ToHudString(69));
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
        public class OnEnablePatch {
            [HarmonyPrefix]
            public static bool OnEnable(GameSettingMenu __instance) {
                lock (Lockable) {
                    if (!CustomPlayerMenu.Instance || CustomPlayerMenu.Instance.selectedTab != 4) return true;
                    if (_groupTitle == null) {
                        _groupTitle = Object.Instantiate(__instance.AllItems[3].GetComponentInChildren<TextMeshPro>())
                            .DontDestroy();
                        _groupTitle.gameObject.active = false;
                        _groupTitle.name = "CategoryTitlePrefab";
                        _groupTitle.transform.localPosition = new Vector3(-1.254f, 0);
                    }

                    if (_enumOption == null) {
                        _enumOption = Object.Instantiate(__instance.AllItems[1].gameObject)
                            .GetComponent<KeyValueOption>().DontDestroy();
                        _enumOption.gameObject.active = false;
                        _enumOption.name = "EnumOptionPrefab";
                        _enumOption.transform.localPosition = new Vector3(0, 0);
                    }

                    if (_numbOption == null) {
                        _numbOption = Object.Instantiate(__instance.AllItems[2].gameObject)
                            .GetComponent<NumberOption>().DontDestroy();
                        _numbOption.gameObject.active = false;
                        _numbOption.name =
                            "NumberOptionPrefab"; //todo fix this semicolon and also x pos on game options :(
                        _numbOption.transform.localPosition = new Vector3(0, 0);
                    }

                    if (_boolOption == null) {
                        _boolOption = Object.Instantiate(__instance.AllItems[3].gameObject)
                            .GetComponent<ToggleOption>().DontDestroy();
                        _boolOption.gameObject.active = false;
                        _boolOption.name = "BooleanOptionPrefab";
                        _boolOption.transform.localPosition = new Vector3(0, 0);
                    }

                    List<Transform> options = new();

                    void GenerateGameOption(GameOption gameOption) {
                        switch (gameOption.Type) {
                            case OptionType.Number: {
                                FloatValue value = (FloatValue) gameOption.Value;
                                NumberOption option =
                                    Object.Instantiate(_numbOption);
                                option.name = gameOption.Title;
                                option.Increment = value.Step;
                                option.ValidRange = new FloatRange(value.Lower, value.Upper);
                                option.Value = value.Value;
                                option.TitleText.text = gameOption.Title;
                                option.FormatString = value.FormatString;
                                option.ZeroIsInfinity = value.IsInfinity;
                                if (!AmongUsClient.Instance.AmHost) option.SetAsPlayer();
                                options.Add(option.transform);
                                break;
                            }
                            case OptionType.Boolean: {
                                BooleanValue value = (BooleanValue) gameOption.Value;
                                ToggleOption option =
                                    Object.Instantiate(_boolOption);
                                option.name = gameOption.Title;
                                option.CheckMark.enabled = value.Value;
                                option.TitleText.text = gameOption.Title;
                                if (!AmongUsClient.Instance.AmHost)
                                    option.GetComponent<PassiveButton>().enabled = false;
                                options.Add(option.transform);
                                break;
                            }
                            case OptionType.Enum: {
                                EnumValue value = (EnumValue) gameOption.Value;
                                KeyValueOption option =
                                    Object.Instantiate(_enumOption);
                                option.name = gameOption.Title;
                                option.Values =
                                    new Il2CppSystem.Collections.Generic.List<
                                        Il2CppSystem.Collections.Generic.KeyValuePair<string, int>>();
                                for (int i = 0; i < value.Values.Length; i++)
                                    option.Values.Add(new Il2CppSystem.Collections.Generic.KeyValuePair<string, int> {
                                        key = value.Values[i],
                                        value = i
                                    });

                                option.Selected = (int) value.OptionIndex;
                                option.ValueText.text = value.Values[value.OptionIndex];
                                option.TitleText.text = gameOption.Title;
                                if (!AmongUsClient.Instance.AmHost) option.SetAsPlayer();
                                options.Add(option.transform);
                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    foreach (GameOption gameOption in NoCategory) {
                        GenerateGameOption(gameOption);
                    }

                    foreach ((string name, List<GameOption> opts) in Categories.OrderBy(pair => pair.Value.Min(x => x.Priority)).ThenBy(x => x.Key)) {
                        TextMeshPro newTitle =
                            Object.Instantiate(_groupTitle);
                        newTitle.text = name;
                        options.Add(newTitle.transform);

                        foreach (GameOption gameOption in opts.OrderBy(x => x.Priority).ThenBy(x => x.Title)) {
                            GenerateGameOption(gameOption);
                        }
                    }


                    __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
                    foreach (Transform transforms in __instance.AllItems) Object.Destroy(transforms.gameObject);

                    __instance.AllItems = options.ToArray();

                    Scroller scroller = __instance.GetComponent<Scroller>();
                    int index = 0;
                    foreach (Transform transforms in __instance.AllItems) {
                        float lx = transforms.localPosition.x;
                        transforms.SetParent(scroller.Inner);
                        transforms.localPosition =
                            new Vector3(lx, __instance.YStart - index++ * __instance.YOffset, -1);
                        transforms.gameObject.SetActive(true);
                    }

                    scroller.YBounds.max =
                        index * __instance.YOffset - 2f * __instance.YStart - 0.5f;
                }

                return true;
            }

            public static void HandleToggleChanged(OptionBehaviour toggleBehaviour) {
                UpdateHudString();
                ToggleOption toggle = toggleBehaviour.Cast<ToggleOption>();
                GameOption gameOption = OptionMap[toggle.TitleText.text];
                BooleanValue value = (BooleanValue) gameOption.Value;
                value.Value = toggle.CheckMark.enabled;
                MessageWriter writer = MessageWriter.Get(SendOption.Reliable);
                writer.StartMessage((byte) PolusRootPackets.SetGameOption);
                // TODO do this when serverside sequence id handling
                writer.Write((ushort) 0);
                writer.Write(gameOption.CategoryName);
                writer.Write(gameOption.Priority);
                writer.Write(toggle.TitleText.text);
                writer.Write((byte) 1);
                writer.Write(toggle.CheckMark.enabled);
                PolusMod.EndSend(writer);
            }

            public static void HandleNumberChanged(OptionBehaviour toggleBehaviour) {
                UpdateHudString();
                NumberOption toggle = toggleBehaviour.Cast<NumberOption>();
                GameOption gameOption = OptionMap[toggle.TitleText.text];
                FloatValue value = (FloatValue) gameOption.Value;
                value.Value = (uint) toggle.Value;
                MessageWriter writer = MessageWriter.Get(SendOption.Reliable);
                writer.StartMessage((byte) PolusRootPackets.SetGameOption);
                writer.Write((ushort) 0);
                writer.Write(gameOption.CategoryName);
                writer.Write(gameOption.Priority);
                writer.Write(toggle.TitleText.text);
                writer.Write((byte) 0);
                writer.Write(toggle.Value);
                // just for the fans (not used on server, just to avoid server crashes)
                writer.Write(toggle.Increment);
                writer.Write(toggle.ValidRange.min);
                writer.Write(toggle.ValidRange.max);
                writer.Write(toggle.ZeroIsInfinity);
                writer.Write(toggle.FormatString);
                PolusMod.EndSend(writer);
            }

            public static void HandleStringChanged(OptionBehaviour toggleBehaviour) {
                UpdateHudString();
                KeyValueOption toggle = toggleBehaviour.Cast<KeyValueOption>();
                GameOption gameOption = OptionMap[toggle.TitleText.text];
                EnumValue value = (EnumValue) gameOption.Value;
                value.OptionIndex = (uint) toggle.Selected;
                MessageWriter writer = MessageWriter.Get(SendOption.Reliable);
                writer.StartMessage((byte) PolusRootPackets.SetGameOption);
                writer.Write((ushort) 0);
                writer.Write(gameOption.CategoryName);
                writer.Write(gameOption.Priority);
                writer.Write(toggle.TitleText.text);
                writer.Write((byte) OptionType.Enum);
                writer.WritePacked(toggle.Selected);
                // just for the fans (not used on server, just to avoid server crashes)
                foreach (Il2CppSystem.Collections.Generic.KeyValuePair<string, int> keyValuePair in toggle.Values)
                    writer.Write(keyValuePair.key);

                PolusMod.EndSend(writer);
            }

            // private static bool _gotVeryFirst;
            // private static ushort _nextSequenceReceived;
            // private static Dictionary<ushort, GameOptionPacket> _packetQueue = new();

            public static void Reset() {
                // _packetQueue = new Dictionary<ushort, GameOptionPacket>();
                // _gotVeryFirst = false;
                // _nextSequenceReceived = 0;
                Categories = new Dictionary<string, List<GameOption>>();
                OptionMap = new Dictionary<string, GameOption>();
            }

            public static void ReceivedGameOptionPacket(GameOptionPacket packet) {
                ushort sequenceId = packet.SequenceId;
                lock (Lockable) {
                    ushort lastId = sequenceId;
                    CatchHelper.TryCatch(() => HandlePacket(packet));
                }
            }

            private static void HandlePacket(GameOptionPacket packet) {
                lock (Lockable) {
                    MessageReader reader = MessageReader.GetSized(packet.Reader.Length);
                    reader.Buffer = packet.Reader;
                    reader.Length = packet.Reader.Length;
                    reader.Tag = 0;
                    switch (packet.Type) {
                        case OptionPacketType.DeleteOption: {
                            string name = reader.ReadString();
                            // (string categoryName, List<GameOption> category) = NoCategory.Any(x => x.Title == name) ? ("", NoCategory) : ;
                            (string categoryName, List<GameOption> category) =
                                NoCategory.Any(x => x.Title == name)
                                    ? new KeyValuePair<string, List<GameOption>>("", NoCategory)
                                    : Categories.First(x => x.Value.Any(option => option.Title == name));
                            category.RemoveAll(x => x.Title == name);
                            OptionMap.Remove(name);
                            if (category.Count == 0 && categoryName != "")
                                Categories.Remove(Categories.First(y => y.Value == category).Key);

                            PolusMod.Instance.DirtyOptions();
                            break;
                        }
                        case OptionPacketType.SetOption: {
                            string cat = reader.ReadString();
                            ushort priority = reader.ReadUInt16();
                            string name = reader.ReadString();
                            OptionType optionType = (OptionType) reader.ReadByte();

                            PolusMod.Instance.DirtyOptions();

                            List<GameOption> category;
                            if (cat != "") {
                                if (Categories.All(x => x.Key != cat)) {
                                    Categories.Add(cat, new List<GameOption>());
                                }

                                category = Categories[cat];
                            } else {
                                category = NoCategory;
                            }

                            if (category.Any(x => x.Title == name)) {
                                category.Find(x => x.Title == name).Value = optionType switch {
                                    OptionType.Boolean => new BooleanValue(reader.ReadBoolean()),
                                    OptionType.Number => new FloatValue(reader.ReadSingle(), reader.ReadSingle(),
                                        reader.ReadSingle(), reader.ReadSingle(), reader.ReadBoolean(),
                                        reader.ReadString()),
                                    OptionType.Enum => EnumValue.ConstructEnumValue(reader),
                                    _ => throw new ArgumentOutOfRangeException()
                                };
                            } else {
                                GameOption option = new() {
                                    Title = name,
                                    Type = optionType,
                                    Priority = priority,
                                    Value = optionType switch {
                                        OptionType.Boolean => new BooleanValue(reader.ReadBoolean()),
                                        OptionType.Number => new FloatValue(reader.ReadSingle(), reader.ReadSingle(),
                                            reader.ReadSingle(), reader.ReadSingle(), reader.ReadBoolean(),
                                            reader.ReadString()),
                                        OptionType.Enum => EnumValue.ConstructEnumValue(reader),
                                        _ => throw new ArgumentOutOfRangeException()
                                    },
                                    CategoryName = cat
                                };
                                OptionMap[name] = option;
                                category.Add(option);
                            }

                            break;
                        }
                    }
                }
            }
        }

        public class GameOptionPacket {
            public ushort SequenceId;
            public OptionPacketType Type;
            public byte[] Reader;
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
        public class DisableRpcSyncSettings {
            [HarmonyPrefix]
            public static bool RpcSyncSettings() {
                return false;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        public class MenuDisableStart {
            [HarmonyPrefix]
            public static bool Start() {
                return false;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.RefreshChildren))]
        public class DisableRefresh {
            [HarmonyPrefix]
            public static bool RefreshChildren() {
                return false;
            }
        }

        [HarmonyPatch(typeof(KeyValueOption), nameof(KeyValueOption.Increase))]
        public class KvIncrease {
            [HarmonyPrefix]
            public static bool Increase(KeyValueOption __instance) {
                __instance.Selected = Mathf.Clamp(__instance.Selected + 1, 0, __instance.Values.Count - 1);
                OnEnablePatch.HandleStringChanged(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(KeyValueOption), nameof(KeyValueOption.Decrease))]
        public class KvDecrease {
            [HarmonyPrefix]
            public static bool Decrease(KeyValueOption __instance) {
                __instance.Selected = Mathf.Clamp(__instance.Selected - 1, 0, __instance.Values.Count - 1);
                OnEnablePatch.HandleStringChanged(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
        public class NumberIncreasePatch {
            [HarmonyPrefix]
            public static bool Increase(NumberOption __instance) {
                __instance.Value = __instance.ValidRange.Clamp(__instance.Value + __instance.Increment);
                OnEnablePatch.HandleNumberChanged(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
        public class NumberDecreasePatch {
            [HarmonyPrefix]
            public static bool Decrease(NumberOption __instance) {
                __instance.Value = __instance.ValidRange.Clamp(__instance.Value - __instance.Increment);
                OnEnablePatch.HandleNumberChanged(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
        public class TogglePatch {
            [HarmonyPrefix]
            public static bool Toggle(ToggleOption __instance) {
                __instance.CheckMark.enabled = !__instance.CheckMark.enabled;
                OnEnablePatch.HandleToggleChanged(__instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.OnEnable))]
        public class ToggleButtonDisableStartPatch {
            [HarmonyPrefix]
            public static bool OnEnable() {
                return false;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
        public class NumberButtonDisableStartPatch {
            [HarmonyPrefix]
            public static bool OnEnable() {
                return false;
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.FixedUpdate))]
        public class NumberButtonFixedUpdatePatch {
            [HarmonyPrefix]
            public static bool OnEnable(NumberOption __instance) {
                if (Math.Abs(__instance.oldValue - __instance.Value) > 0.001f) {
                    __instance.oldValue = __instance.Value;
                    __instance.ValueText.text = string.Format(__instance.FormatString, __instance.Value);
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(KeyValueOption), nameof(KeyValueOption.OnEnable))]
        public class StringButtonDisableStartPatch {
            [HarmonyPrefix]
            public static bool Prefix() {
                return false;
            }
        }

        [HarmonyPatch(typeof(KeyValueOption), nameof(KeyValueOption.FixedUpdate))]
        public class StringButtonUpdatePatch {
            [HarmonyPrefix]
            public static bool Prefix(KeyValueOption __instance) {
                if (__instance.oldValue != __instance.Selected) {
                    __instance.oldValue = __instance.Selected;
                    __instance.ValueText.text =
                        ((EnumValue) OptionMap[__instance.TitleText.text].Value).Values[
                            __instance.Selected];
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(KeyValueOption), nameof(KeyValueOption.GetInt))]
        public class StringButtonIncreasePatch {
            [HarmonyPrefix]
            public static bool Prefix(KeyValueOption __instance, out int __result) {
                __result = 0;
                return false;
            }
        }

        public class GameOption {
            public string Title { get; set; }
            public OptionType Type { get; set; }
            public IGameOptionValue Value { get; set; }
            public string CategoryName { get; set; }
            public ushort Priority { get; set; }
        }

        public interface IGameOptionValue { }

        public class BooleanValue : IGameOptionValue {
            public bool Value;

            public BooleanValue(bool value) {
                Value = value;
            }
        }

        public class EnumValue : IGameOptionValue {
            public uint OptionIndex;
            public string[] Values;

            public EnumValue(uint optionIndex, string[] values) {
                OptionIndex = optionIndex;
                Values = values;
            }

            public static EnumValue ConstructEnumValue(MessageReader reader) {
                List<string> strings = new();
                uint current = reader.ReadPackedUInt32();
                while (reader.Position < reader.Length) strings.Add(reader.ReadString());

                return new EnumValue(current, strings.ToArray());
            }
        }

        public class FloatValue : IGameOptionValue {
            public string FormatString;
            public bool IsInfinity;
            public float Lower;
            public float Step;
            public float Upper;

            public float Value;

            public FloatValue(float value, float step, float lower, float upper, bool isInfinity, string formatString) {
                Value = value;
                Step = step;
                Lower = lower;
                Upper = upper;
                IsInfinity = isInfinity;
                FormatString = formatString;
            }
        }

        public class TitleOption : MonoBehaviour {
            public TextMeshPro title;
            public TitleOption(IntPtr ptr) : base(ptr) { }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.ToHudString))]
        public class HudStringPatch {
            [HarmonyPrefix]
            public static bool ToHudString(out string __result) {
                __result = "Game Settings:\n";

                string oute = "";

                void GenerateHud(string categoryTitle, List<GameOption> options) {
                    CatchHelper.TryCatch(() => {
                        string output = "";
                        if (categoryTitle != null) output += $"\n{categoryTitle}\n";
                        foreach (GameOption option in options.OrderBy(x => x.Priority).ThenBy(x => x.Title)) {
                            if (categoryTitle != null) output += "  ";
                            output += $"{option.Title}: ";
                            output += option.Type switch {
                                OptionType.Number => string.Format(((FloatValue) option.Value).FormatString,
                                    ((FloatValue) option.Value).Value),
                                OptionType.Boolean => ((BooleanValue) option.Value).Value ? "On" : "Off",
                                OptionType.Enum => ((EnumValue) option.Value).Values
                                    [((EnumValue) option.Value).OptionIndex],
                                _ => throw new ArgumentOutOfRangeException()
                            };
                            output += '\n';
                        }

                        oute += output;
                    });
                }

                lock (Lockable) {
                    GenerateHud(null, NoCategory);

                    foreach ((string key, List<GameOption> value) in Categories.OrderBy(pair => pair.Value.Min(x => x.Priority)).ThenBy(x => x.Key)) {
                        GenerateHud(key, value);
                    }

                    __result = oute;
                }

                return false;
            }
        }
    }
}