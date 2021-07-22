﻿//stolen from reactor, which stole from unityexplorer :)
//https://github.com/NuclearPowered/Reactor/blob/master/Reactor/Unstrip/AssetBundle.cs

namespace Polus {
    // public class AssetBundle : IDisposable {
    //     private delegate IntPtr d_LoadFromFile(IntPtr path, uint crc, ulong offset);
    //
    //     private static readonly d_LoadFromFile i_LoadFromFile =
    //         IL2CPP.ResolveICall<d_LoadFromFile>("UnityEngine.AssetBundle::LoadFromFile_Internal");
    //
    //     public static AssetBundle LoadFromFile(string path, uint crc = 0, ulong offset = 0) {
    //         return new AssetBundle(i_LoadFromFile.Invoke(IL2CPP.ManagedStringToIl2Cpp(path), crc, offset));
    //     }
    //
    //     private delegate IntPtr d_LoadFromMemory(IntPtr binary, uint crc);
    //
    //     private static readonly d_LoadFromMemory i_LoadFromMemory =
    //         IL2CPP.ResolveICall<d_LoadFromMemory>("UnityEngine.AssetBundle::LoadFromMemory_Internal");
    //
    //     public static AssetBundle LoadFromMemory(byte[] binary, uint crc = 0) {
    //         return new AssetBundle(i_LoadFromMemory(((Il2CppStructArray<byte>) binary).Pointer, crc));
    //     }
    //
    //     public IntPtr Pointer { get; }
    //
    //     public AssetBundle(IntPtr ptr) {
    //         Pointer = ptr;
    //     }
    //
    //     private delegate IntPtr d_LoadAssetWithSubAssets_Internal(IntPtr __instance, IntPtr name, IntPtr type);
    //
    //     private static readonly d_LoadAssetWithSubAssets_Internal i_LoadAssetWithSubAssets_Internal =
    //         IL2CPP.ResolveICall<d_LoadAssetWithSubAssets_Internal>(
    //             "UnityEngine.AssetBundle::LoadAssetWithSubAssets_Internal");
    //
    //     public T[] LoadAllAssets<T>() where T : UnityEngine.Object {
    //         var ptr = i_LoadAssetWithSubAssets_Internal.Invoke(Pointer, IL2CPP.ManagedStringToIl2Cpp(string.Empty),
    //             Il2CppType.Of<T>().Pointer);
    //
    //         if (ptr == IntPtr.Zero) {
    //             return new T[0];
    //         }
    //
    //         return new Il2CppReferenceArray<T>(ptr);
    //     }
    //     
    //     private delegate IntPtr d_GetAllAssetNames_Internal(IntPtr __instance, IntPtr name, IntPtr type);
    //
    //     private static readonly d_GetAllAssetNames_Internal i_GetAllAssetNames_Internal =
    //         IL2CPP.ResolveICall<d_GetAllAssetNames_Internal>(
    //             "UnityEngine.AssetBundle::GetAllAssetNames");
    //
    //     public string[] GetAllAssetNames() {
    //         var ptr = i_GetAllAssetNames_Internal.Invoke(Pointer, IL2CPP.ManagedStringToIl2Cpp(string.Empty),
    //             Il2CppType.Of<string>().Pointer);
    //
    //         if (ptr == IntPtr.Zero) {
    //             return new string[0];
    //         }
    //
    //         return new Il2CppReferenceArray<Il2CppSystem.String>(ptr).Select(s => (string) s).ToArray();
    //     }
    //
    //     private delegate IntPtr d_LoadAsset_Internal(IntPtr __instance, IntPtr name, IntPtr type);
    //
    //     private static readonly d_LoadAsset_Internal i_LoadAsset_Internal =
    //         IL2CPP.ResolveICall<d_LoadAsset_Internal>("UnityEngine.AssetBundle::LoadAsset_Internal");
    //
    //     public T LoadAsset<T>(string name) where T : UnityEngine.Object {
    //         "test1".Log();
    //         var ptr = i_LoadAsset_Internal.Invoke(Pointer, IL2CPP.ManagedStringToIl2Cpp(name),
    //             Il2CppType.Of<T>().Pointer);
    //         "test2".Log();
    //
    //         return ptr == IntPtr.Zero ? null : new UnityEngine.Object(ptr).TryCast<T>();
    //     }
    //
    //     private delegate IntPtr d_Contains_Internal(IntPtr __instance, IntPtr name, IntPtr type);
    //
    //     private static readonly d_Contains_Internal i_Contains_Internal =
    //         IL2CPP.ResolveICall<d_Contains_Internal>("UnityEngine.AssetBundle::Contains");
    //
    //     public bool ContainsAsset(string name) {
    //         var ptr = i_Contains_Internal.Invoke(Pointer, IL2CPP.ManagedStringToIl2Cpp(name),
    //             Il2CppType.Of<bool>().Pointer);
    //
    //         return ptr != IntPtr.Zero;
    //     }
    //
    //     private delegate IntPtr d_Unload(IntPtr __instance, bool unloadAllLoadedObjects);
    //
    //     private static readonly d_Unload i_Unload = IL2CPP.ResolveICall<d_Unload>("UnityEngine.AssetBundle::Unload");
    //
    //     public void Unload(bool unloadAllLoadedObjects = false) {
    //         i_Unload.Invoke(Pointer, unloadAllLoadedObjects);
    //     }
    //
    //     public void Dispose() {
    //         Unload();
    //     }
    // }
}