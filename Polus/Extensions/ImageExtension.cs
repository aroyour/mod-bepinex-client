﻿using System;
using UnhollowerBaseLib;
using UnityEngine;

namespace Polus.Extensions {
    public static class ImageExtension {
        private static readonly d_LoadImage i_LoadImage =
            IL2CPP.ResolveICall<d_LoadImage>("UnityEngine.ImageConversion::LoadImage");

        /// <summary>
        ///     Load image from <paramref name="data" /> to <paramref name="tex" />
        /// </summary>
        public static bool LoadImage(this Texture2D tex, byte[] data, bool markNonReadable = false) {
            return i_LoadImage(tex.Pointer, ((Il2CppStructArray<byte>) data).Pointer, markNonReadable);
        }

        private delegate bool d_LoadImage(IntPtr tex, IntPtr data, bool markNonReadable);
    }
}