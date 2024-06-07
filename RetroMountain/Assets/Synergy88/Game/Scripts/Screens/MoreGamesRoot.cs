﻿using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using MiniJSON;

using Common;
using Common.Signal;
using Common.Utils;

// alias
using S88Const = Synergy88.Const;

namespace Synergy88 {

	public class MoreGamesRoot : S88Scene {

		[SerializeField]
		private GameObject template;

		[SerializeField]
		private List<MoreGamesItemData> items;

		protected override void Awake() {
			base.Awake();
			Assertion.AssertNotNull(this.template);
		}

		protected override void Start() {
			base.Start();

			this.PopulateItems();

			// add button handlers
			this.AddButtonHandler(EButtonType.MoreGamesItem, (ISignalParameters parameters) => {
				MoreGamesItem item = (MoreGamesItem)parameters.GetParameter(S88Params.BUTTON_DATA);
				item.OpenLink();

				SoundControls.Instance._buttonClick.Play();
			});
		}

		protected override void OnEnable() {
			base.OnEnable();
			this.StopAllCoroutines();
			this.StartCoroutine(this.Fetch());
		}

		protected override void OnDisable() {
			base.OnDisable();
			this.StopAllCoroutines();
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			this.StopAllCoroutines();
		}

		private IEnumerator Fetch() {
			// headers
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers[S88Const.MORE_GAMES_AUTHORIZATION_KEY] = S88Const.MORE_GAMES_AUTHORIZATION_VALUE;
			headers[S88Const.MORE_GAMES_CONTENT_TYPE_KEY] = S88Const.MORE_GAMES_CONTENT_TYPE_VALUE;

			// get
			string url = S88Const.GetMoreGamesUrl();
			WWW www = new WWW(url, null, headers);

			while (!www.isDone) {
				yield return null;
			}

			if (www.error != null) {
				Debug.LogErrorFormat("MoreGamesItem::Fetch Error:{0}\n", www.error);
			}
			else {
				Debug.LogFormat("MoreGamesItem::Fetch Result:{0}\n", www.text);
				this.ParseMoreGames(www.text);
				this.PopulateItems();
			}
		}

		private void ParseMoreGames(string jsonData) {
			Dictionary<string, object> data = (Dictionary<string, object>)Json.Deserialize(jsonData);
			List<object> items = (List<object>)data[S88Const.MORE_GAMES_DATA];

			foreach (Dictionary<string, object> item in items) {
				string itemId = (string)item[S88Const.MORE_GAMES_ITEM_ID];
				if (this.items.Exists(i => i.ItemId.Equals(itemId))) {
					MoreGamesItemData itemData = this.items.Find(i => i.ItemId.Equals(itemId));
					itemData.Parse((Dictionary<string, object>)item);
				}
				else {
					MoreGamesItemData itemData = new MoreGamesItemData();
					itemData.Parse((Dictionary<string, object>)item);
					this.items.Add(itemData);
				}
			}
		}

		public void PopulateItems() {
			Transform parent = this.template.transform.parent;
			Vector3 localScale = this.template.transform.localScale;
			int len = this.items.Count;
			int children = parent.childCount - 1;;
			bool create = len == children;

			for (int i = 0; i < len; i++) {
				MoreGamesItemData item = null;
				GameObject itemObject = null;
				MoreGamesItem gameItem = null;

				try {
					item = this.items[i];
					itemObject = parent.GetChild(i + 1).gameObject;
					gameItem = itemObject.GetComponent<MoreGamesItem>();
				}
				catch (UnityException e) {
					item = this.items[i];
					itemObject = (GameObject)GameObject.Instantiate(this.template);
					gameItem = itemObject.GetComponent<MoreGamesItem>();
				}

				gameItem.UpdateData(i, item);

				// display item
				itemObject.transform.SetParent(parent);
				itemObject.transform.localScale = localScale;
				itemObject.SetActive(true);
			}
		}
	}

}