﻿using System;
using System.Linq;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Polus.Behaviours {
    public class PvaManager : MonoBehaviour {
        static PvaManager() {
            ClassInjector.RegisterTypeInIl2Cpp<PvaManager>();
        }

        public PvaManager(IntPtr ptr) : base(ptr) { }

        public PlayerControl player;
        public PlayerVoteArea pva;
        public SpriteRenderer renderer;
        public bool dead;
        public bool disabled;
        public bool reported;

        public void Initialize(PlayerVoteArea voteArea, byte targetId) {
            pva = voteArea;
            renderer = GetComponent<SpriteRenderer>();
            if (targetId < 252) player = PlayerControl.AllPlayerControls.Find((Func<PlayerControl, bool>)(pc => pc.PlayerId == targetId));
        }

        public void Update() {
            bool disabledState = pva.Parent.state is
                MeetingHud.VoteStates.Animating or
                MeetingHud.VoteStates.Discussion or
                MeetingHud.VoteStates.Proceeding;
            if (pva.NameText && player) pva.NameText.text = player.nameText.text;
            bool disable = dead || disabled || disabledState;
            if (disable) ControllerManager.Instance.RemoveSelectableUiElement(pva.PlayerButton);
            else ControllerManager.Instance.AddSelectableUiElement(pva.PlayerButton);
            pva.AmDead = dead;
            pva.DidReport = reported;
            if (pva.TargetPlayerId == PlayerVoteArea.SkippedVote)
                renderer.enabled = !disabled;
            else {
                pva.Flag.enabled = pva.DidVote && !pva.resultsShowing;
                pva.Megaphone.enabled = reported;
                // if (!disable) return;
                pva.Overlay.gameObject.SetActive(disable);
                pva.XMark.gameObject.SetActive(dead);
            }
        }

        public void SetState(bool dead, bool disabled, bool reported) {
            this.dead = dead;
            this.disabled = disabled;
            this.reported = reported;
        }
    }
}