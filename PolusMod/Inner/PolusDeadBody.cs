﻿using System;
using Hazel;
using PolusApi.Net;
using PowerTools;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace PolusMod.Inner {
	public class PolusDeadBody : PnoBehaviour {
		public SpriteAnim anim;
		public DeadBody deadBody;
		public SpriteRenderer rend;
		public PolusNetworkTransform netTransform;
		public PolusClickBehaviour clickBehaviour;
		private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
		private static readonly int BackColor = Shader.PropertyToID("_BackColor");

		// todo recreate dead body lmao
		// todo patch murder player to not show dead body lmao
		// todo patch reporting to correctly allow reporting custom dead bodies

		public PolusDeadBody(IntPtr ptr) : base(ptr) { }

		static PolusDeadBody() {
			ClassInjector.RegisterTypeInIl2Cpp<PolusDeadBody>();
		}

		private void Start() {
			pno = IObjectManager.Instance.LocateNetObject(this);
			pno.OnData = Deserialize;
			rend = GetComponent<SpriteRenderer>();
			anim = GetComponent<SpriteAnim>();
			deadBody = GetComponent<DeadBody>();
			netTransform = GetComponent<PolusNetworkTransform>();
			clickBehaviour = GetComponent<PolusClickBehaviour>();
		}

		private void FixedUpdate() {
			if (pno.HasSpawnData()) Deserialize(pno.GetSpawnData());
		}

		public void Deserialize(MessageReader reader) {
			anim.SetNormalizedTime(reader.ReadBoolean() ? 1 : 0);
			// reader.ReadBoolean();
			rend.flipX = reader.ReadBoolean();
			// transform.localScale = new Vector3(reader.ReadBoolean() ? -0.7f : 0.7f, 0.7f, 0.7f);
			rend.material.SetColor(BackColor, new Color32(reader.ReadByte(),reader.ReadByte(),reader.ReadByte(),reader.ReadByte()));
			rend.material.SetColor(BodyColor, new Color32(reader.ReadByte(),reader.ReadByte(),reader.ReadByte(),reader.ReadByte()));
		}
	}
}