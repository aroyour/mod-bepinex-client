﻿using UnityEngine;

namespace Polus.Extensions {
    public static class PlayerColorExtensions {
        private static readonly Color32 DefaultVisorColor = new(149, 202, 220, byte.MaxValue);
        private static readonly int BackColor = Shader.PropertyToID("_BackColor");
        private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
        private static readonly int VisorColor = Shader.PropertyToID("_VisorColor");

        public static void SetPlayerMaterialColors(this SpriteRenderer rend, Color back, Color body, Color? visor = null) {
            if (!rend)
                return;

            rend.material.SetColor(BackColor, back);
            rend.material.SetColor(BodyColor, body);
            rend.material.SetColor(VisorColor, visor ?? DefaultVisorColor);
        }

        public static void SetPetImage(this SpriteRenderer rend, uint petId, Color backColor, Color bodyColor) {
            rend.sprite = HatManager.Instance.GetPetById(petId).rend.sprite;
            rend.material = new Material(rend.sharedMaterial);
            rend.SetPlayerMaterialColors(backColor, bodyColor);
        }
    }
}