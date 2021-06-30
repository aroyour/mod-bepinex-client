﻿using System;
using System.Linq;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using PolusGG.Extensions;
using PolusGG.Mods.Patching;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace PolusGG.Patches.Permanent {
    public class SyncPatches {
        private static ObjectPoolBehavior _globalPool;
        private static StarGen _starGen;
        private static readonly string[] Scenes = {"MainMenu", "MMOnline", "MatchMaking"};

        public static void OnSceneChanged(Scene scene, LoadSceneMode _) {
            _starGen.gameObject.SetActive(Scenes.Contains(scene.name));
            _globalPool.gameObject.SetActive(Scenes.Contains(scene.name));
        }

        [HarmonyPatch(typeof(PlayerParticles), nameof(PlayerParticles.Start))]
        public class PlayerSyncStart {
            [PermanentPatch]
            [HarmonyPrefix]
            public static void Start(PlayerParticles __instance) {
                if (_globalPool == null) {
                    _globalPool = Object.Instantiate(__instance.pool).DontDestroy();
                    Object.Destroy(_globalPool.GetComponent<PlayerParticles>());
                    _globalPool.poolSize = Palette.ColorNames.Length;
                    foreach (PoolableBehavior pb in _globalPool.inactiveChildren) Object.DestroyImmediate(pb);
                    foreach (PoolableBehavior pb in _globalPool.activeChildren) Object.DestroyImmediate(pb);
                    _globalPool.activeChildren = new List<PoolableBehavior>();
                    _globalPool.inactiveChildren = new List<PoolableBehavior>();
                    _globalPool.InitPool(_globalPool.Prefab);
                    SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>(OnSceneChanged));
                    // _globalPool.ReclaimAll();
                }

                Object.Destroy(__instance.pool);
                __instance.pool = _globalPool;
            }
        }

        [HarmonyPatch(typeof(PlayerParticle), nameof(PlayerParticle.Update))]
        public class PlayerSyncUpdate {
            [PermanentPatch]
            [HarmonyPrefix]
            public static void Update(PlayerParticle __instance) {
                __instance.OwnerPool = _globalPool;
                __instance.velocity *= new Vector2(1.01f, 1.01f);
            }
        }

        [HarmonyPatch(typeof(StarGen), nameof(StarGen.Start))]
        public class StarSyncStart {
            [PermanentPatch]
            [HarmonyPrefix]
            public static bool Start(StarGen __instance) {
                if (_starGen) {
                    if (!Scenes.Contains(SceneManager.GetActiveScene().name)) return true;
                    Object.Destroy(__instance);
                    return false;
                }

                _starGen = __instance.DontDestroy();
                return true;
            }
        }
    }
}