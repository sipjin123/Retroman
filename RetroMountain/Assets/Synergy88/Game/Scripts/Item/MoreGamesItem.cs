using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Common.Signal;

// alias
using S88Const = Synergy88.Const;

namespace Synergy88 {

	public class MoreGamesItem : MonoBehaviour {

		[SerializeField]
		private Image body;

		[SerializeField]
		private RawImage avatar;

		[SerializeField]
		private Text title;

		[SerializeField]
		private Text description;

		[SerializeField]
		private MoreGamesItemData data;

		private int index;

		private void Awake() {
			Assertion.AssertNotNull(this.body);
			Assertion.AssertNotNull(this.avatar);
			Assertion.AssertNotNull(this.title);
			Assertion.AssertNotNull(this.description);

			S88Signals.ON_IMAGE_DOWNLOADED.AddListener(this.OnImageDownloaded);
		}

		private void Start() {
			this.Refresh();
		}

		private void OnDestroy() {
			S88Signals.ON_IMAGE_DOWNLOADED.RemoveListener(this.OnImageDownloaded);
		}
		
		private void Refresh() {
			// refresh visuals here
			if (this.index % 2 == 0) {
				this.body.color = Color.gray;
			}
			else {
				this.body.color = Color.gray * 1.5f;
			}

			// update labels
			this.title.text = this.data.Name;
			this.description.text = this.data.Description;

			// download image
			Signal signal = S88Signals.ON_IMAGE_DOWNLOAD;
			signal.ClearParameters();
			signal.AddParameter(S88Params.IMAGE_ID, this.data.ItemId);
			signal.AddParameter(S88Params.IMAGE_URL, this.data.Avatar);
			signal.Dispatch();
		}

		public void UpdateData(int index, MoreGamesItemData data) {
			this.index = index;
			this.data = data;
		}

		public void OpenLink() {
			Application.OpenURL(this.data.Link);
		}

		public MoreGamesItemData ItemData {
			get { return this.data; }
		}

		#region Signals

		private void OnImageDownloaded(ISignalParameters parameters) {
			string imageId = (string)parameters.GetParameter(S88Params.IMAGE_ID);
			Texture2D imageTexture = (Texture2D)parameters.GetParameter(S88Params.IMAGE_TEXTURE);
			if (this.data.ItemId.Equals(imageId)) {
				this.avatar.texture = imageTexture;
			}
		}

		#endregion

	}
}