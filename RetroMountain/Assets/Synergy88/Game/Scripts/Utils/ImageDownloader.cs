using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Signal;
using Common.Utils;

namespace Synergy88 {
	
	public class ImageDownloader : MonoBehaviour {

		private Dictionary<string, Texture2D> cachedTextures = new Dictionary<string, Texture2D>();

		private void Awake() {
			//Factory.Register<ImageDownloader>(this);
			S88Signals.ON_IMAGE_DOWNLOAD.AddListener(this.OnDownloadImage);
		}

		private void OnDestroy() {
			//Factory.Clean<ImageDownloader>();
			S88Signals.ON_IMAGE_DOWNLOAD.RemoveListener(this.OnDownloadImage);
		}


		private void OnDownloadImage(ISignalParameters parameters) {
			string imageId = (string)parameters.GetParameter(S88Params.IMAGE_ID);
			string imageUrl = (string)parameters.GetParameter(S88Params.IMAGE_URL);

			// TODO: 
			//	Please use uniRx instead
			//	Add download progress checking
			this.StartCoroutine(this.DownloadImage(imageId, imageUrl));

			// uniRx
			//ObservableWWW.Get(imageId, null, this).Subscribe(_ => {
			//	Debug.LogFormat("Downloaded:{0}\n", _);
			//});
		}

		private IEnumerator DownloadImage(string imageId, string imageUrl) {
			// check local images
			string localPath = imageUrl;
			Texture2D texture;
			byte[] textureData;

			// check if texture is cached, then use the cached instead
			if (this.cachedTextures.ContainsKey(imageId)) {
				texture = this.cachedTextures[imageId];
			}
			else {
				localPath = imageUrl;

				// wait for the image to be downloaded
				WWW www = new WWW(imageUrl);
				yield return www;	

				if (www.error != null) {
					Debug.LogErrorFormat("Error downloading image! id:{0} url:{1}\n", imageId, imageUrl);
					yield break;
				}

				// get the downloaded texture
				texture = (Texture2D)www.texture;
				textureData = www.bytes;

				// cache the texture
				this.cachedTextures[imageId] = texture;
			}

			Signal signal = S88Signals.ON_IMAGE_DOWNLOADED;
			signal.AddParameter(S88Params.IMAGE_ID, imageId);
			signal.AddParameter(S88Params.IMAGE_TEXTURE, texture);
			signal.Dispatch();
		}
	}

}