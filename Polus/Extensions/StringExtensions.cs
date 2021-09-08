﻿using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;

namespace Polus.Extensions {
    public static class StringExtensions {
        public static string Hex(this IEnumerable<byte> value) {
            return value.Select(x => x.ToString("X2")).Join(delimiter: "");
        }
        
        public static byte[] FromHex(this string text) {
            return Enumerable.Range(0, text.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(text.Substring(x, 2), 16))
                .ToArray();
        }

        public static string SanitizeName(this string name) {
            while (true) {
                int pos;
                if ((pos = name.IndexOf("<sprite", StringComparison.Ordinal)) != -1 && name.IndexOf('>', pos) != -1) {
                    int second = name.IndexOf('>', pos) + 1;
                    while (second + 1 < name.Length && name[second] == ' ') second++;
                    name = name[..pos] + name[second..];
                    continue;
                }

                break;
            }

            return name;
        }
    }
}