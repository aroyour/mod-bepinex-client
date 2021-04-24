﻿using System;
using System.Collections;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Text;
using PolusGG.Extensions;
using PolusGG.Utils;
using TMPro;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PolusGG {
    public static class AccountMenu {
        private static bool _loaded;
        private static AccountMenuHolder _menu;
        private static TopAccountHolder _top;
        private static TopAccountHolder _top2;
        public static bool MenuVisible = _top2 && _top2.menuHolder && _top2.menuHolder.gameObject.active;
        public static IEnumerator LoginSequence;

        public static void InitializeAccountMenu(Scene scene) {
            GameObject gameObject = GameObject.Find("NormalMenu");
            if (!_loaded) {
                _menu = PogusPlugin.Bundle.LoadAsset("Assets/Mods/LoginMenu/PolusggAccountMenu.prefab")
                    .Cast<GameObject>().DontDestroy().AddComponent<AccountMenuHolder>();
                _top = PogusPlugin.Bundle.LoadAsset("Assets/Mods/LoginMenu/TopAccount.prefab").Cast<GameObject>()
                    .DontDestroy().AddComponent<TopAccountHolder>();

                _loaded = true;
            }

            PlayerControl.LocalPlayer.SetThickAssAndBigDumpy(true, true);

            if (PogusPlugin.ModManager.AllPatched) {
                Transform nameText = gameObject.transform.Find("NameText");
                Vector3 pos = nameText.position;
                Object.Destroy(nameText.gameObject);
                _top2 = Object.Instantiate(_top);
                _top2.transform.position = pos;
                GameObject login = _top2.gameObject.FindRecursive(x => x.name.Contains("Login"));
                GameObject create = _top2.gameObject.FindRecursive(x => x.name.Contains("Create"));
                UIMethods.CreateButton(login, () => {
                    try {
                        if (_top2.menuHolder) {
                            _top2.menuHolder.gameObject.SetActive(true);
                            return;
                        }

                        AccountMenuHolder menu2 = _top2.menuHolder = Object.Instantiate(_menu);
                        menu2.transform.localPosition = new Vector3(0f, 0f, -400f);
                        // menu2.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
                        GameObject name = menu2.gameObject.FindRecursive(x => x.name.Contains("EmailTextBox"));
                        GameObject password = menu2.gameObject.FindRecursive(x => x.name.Contains("PasswordTextBox"));
                        menu2.password = UIMethods.LinkTextBox(password);
                        //this is allowed because of a patch at bottom of file
                        menu2.password.allowAllCharacters = true;
                        menu2.password.AllowPaste = true;
                        menu2.nameField = UIMethods.LinkTextBox(name);
                        menu2.nameField.AllowEmail = true;
                        menu2.nameField.AllowPaste = true;
                        menu2.password.OnChange.AddListener(new Action(() => {
                            menu2.password.outputText.text =
                                Enumerable.Repeat("*", menu2.password.text.Length).Join(delimiter: "") +
                                "<color=#FF0000>" + menu2.password.compoText + "</color>";
                            menu2.password.outputText.ForceMeshUpdate(true, true);
                        }));

                        GameObject loginButton = menu2.gameObject.FindRecursive(x => x.name.Contains("Login"));
                        bool loading = false;
                        UIMethods.CreateButton(loginButton, () => {
                            //todo login
                            IEnumerator Load() {
                                loading = true;
                                MatchMaker.Instance.Connecting(Object.FindObjectOfType<JoinGameButton>());
                                yield return new WaitForSeconds(3f);
                                MatchMaker.Instance.NotConnecting();
                                loading = false;
                            }

                            if (!loading) menu2.StartCoroutine(Load());
                        });
                        GameObject ui = menu2.gameObject.FindRecursive(x => x.name.Contains("UI"));
                        UIMethods.CreateButton(ui, () => { }, false);
                        GameObject background = menu2.gameObject.FindRecursive(x => x.name.Contains("Background"));
                        GameObject close = menu2.gameObject.FindRecursive(x => x.name.Contains("closeButton"));

                        void Close() {
                            menu2.gameObject.SetActive(false);
                        }

                        UIMethods.CreateButton(background, Close, false);
                        UIMethods.CreateButton(close, Close, false);

                        menu2.gameObject.AddComponent<TransitionOpen>().duration = 0.2f;
                        // "work pls".Log(4);
                    } catch (Exception e) {
                        e.Log();
                    }
                });
                UIMethods.CreateButton(create, () => { Application.OpenURL("https://polus.gg/"); });
            }
        }

        public class TopAccountHolder : MonoBehaviour {
            public AccountMenuHolder menuHolder;

            static TopAccountHolder() {
                ClassInjector.RegisterTypeInIl2Cpp<TopAccountHolder>();
            }

            public TopAccountHolder(IntPtr ptr) : base(ptr) { }
        }

        public class AccountMenuHolder : MonoBehaviour {
            public TextBoxTMP nameField;
            public TextBoxTMP password;

            static AccountMenuHolder() {
                ClassInjector.RegisterTypeInIl2Cpp<AccountMenuHolder>();
            }

            public AccountMenuHolder(IntPtr ptr) : base(ptr) { }
        }

        [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.IsCharAllowed))]
        public class TextBoxAllCharSupport {
            [HarmonyPrefix]
            public static bool IsCharAllowed(TextBoxTMP __instance, out bool __result) {
                if (__instance.allowAllCharacters) {
                    __result = true;
                    return false;
                }

                __result = false;
                return true;
            }
        }
    }
}