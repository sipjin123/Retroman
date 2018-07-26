using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Sirenix.OdinInspector;

using Common;

using Framework;

namespace Sandbox.ZipDownloader
{
    using UniRx;
    using UniRx.Triggers;

    public class UniRxZipDownloader : MonoBehaviour, IProgress<float>
    {
        public MeshRenderer Renderer000;
        public MeshRenderer Renderer001;

        [SerializeField, ShowInInspector]
        private List<DownloadData> Downloads;
        private DownloadData DownloadCue = null;

        private void Start()
        {
            this.UpdateAsObservable()
                .Where(_ => Downloads.Count > 0)
                .Where(_ => DownloadCue == null)
                .Subscribe(_ => Download())
                .AddTo(this);
        }

        private void Download()
        {
            DownloadCue = Downloads.FirstOrDefault();

            ObservableWWW
                .GetWWW(DownloadCue.DownloadUrl, null, this)
                .Subscribe(
                    www =>
                    {
                        DownloadCue.ProcessDownload(www.bytes);

                        // test
                        if (DownloadCue.ZipContent.Count > 0)
                        {
                            Renderer000.material.mainTexture = DownloadCue.GetTextureFile(DownloadCue.ZipContent[0]);
                            Renderer001.material.mainTexture = DownloadCue.GetTextureFile(DownloadCue.ZipContent[1]);
                        }
                    },
                    error =>
                    {
                        Debug.LogErrorFormat("Error! Id:{0} Progress:{1} Error:{2} InnerError:{3}\n", DownloadCue.DownloadUrl, DownloadCue.DownloadProgress, error.Message, error.InnerException.Message);
                    },
                    delegate
                    {
                        Debug.LogErrorFormat("Done! Id:{0} Progress:{1}\n", DownloadCue.DownloadUrl, DownloadCue.DownloadProgress);
                    });
        }

        #region IProgress<float> Implements
        public void Report(float progress)
        {
            //Debug.LogErrorFormat("Progress {0}\n", value);
            DownloadCue.DownloadProgress = progress;
        }
        #endregion
    }
}
