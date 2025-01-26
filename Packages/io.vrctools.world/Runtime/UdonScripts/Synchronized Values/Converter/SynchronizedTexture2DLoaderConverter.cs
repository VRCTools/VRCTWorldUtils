// Copyright 2025 .start <https://dotstart.tv>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDK3.Image;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRCTools.World.SynchronizedValues;

namespace VRCTools.World.LocalValues.Converters {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Synchronized Values/Converter/Synchronized Texture2D Loader Converter")]
  public class SynchronizedTexture2DLoaderConverter : UdonSharpBehaviour {
    [FormerlySerializedAs("localUrl")]
    public SynchronizedUrl synchronizedUrl;

    public LocalTexture2D localTexture2D;

    public Texture2D defaultTexture;
    public Texture2D loadingTexture;
    public Texture2D errorTexture;
    public Texture2D accessDeniedErrorTexture;
    public Texture2D downloadErrorTexture;
    public Texture2D invalidErrorTexture;
    public Texture2D tooManyRequestsErrorTexture;
    public Texture2D invalidUrlErrorTexture;

    public float retryPeriod = 2f;

    private VRCImageDownloader _imageDownloader;

    private bool _loading;
    private bool _downloadQueued;

    private float _retryTimer = -1;

    private void Start() {
      if (!Utilities.IsValid(this.synchronizedUrl)) {
        Debug.LogError("[Texture2D Loader Converter] Invalid synchronized url reference - Disabled", this);
        this.enabled = false;
        return;
      }

      if (!Utilities.IsValid(this.localTexture2D)) {
        Debug.LogError("[Texture2D Loader Converter] Invalid local texture reference - Disabled", this);
        this.enabled = false;
        return;
      }

      this._imageDownloader = new VRCImageDownloader();

      this.synchronizedUrl._RegisterHandler(LocalUrl.EVENT_STATE_UPDATED, this, nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void Update() {
      if (this._retryTimer <= 0) {
        return;
      }

      this._retryTimer -= Time.deltaTime;
      if (this._retryTimer > 0) {
        return;
      }

      this._OnStateUpdated();
    }

    private void OnDestroy() {
      // before we destroy the image downloader, we'll swap the texture to the configured default as the texture should
      // be destroyed as a result of the image downloader being destroyed
      if (Utilities.IsValid(this.localTexture2D)) {
        this.localTexture2D.State = this.defaultTexture;
      }

      // dispose of our image downloader instance if it was allocated on script startup
      if (Utilities.IsValid(this._imageDownloader)) {
        this._imageDownloader.Dispose();
      }

      if (!Utilities.IsValid(this.synchronizedUrl)) {
        return;
      }

      this.synchronizedUrl._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      if (this._loading) {
        Debug.LogWarning("[Texture2D Loader Converter] Image download in process - Queued update");
        this._downloadQueued = true;
        return;
      }

      this._loading = true;
      this._retryTimer = -1;

      var url = this.synchronizedUrl.State;
      if (VRCUrl.IsNullOrEmpty(url)) {
        this._loading = false;

        this.localTexture2D.State = this.defaultTexture;
        return;
      }

      var texture = this.loadingTexture;
      if (texture != null) {
        this.localTexture2D.State = texture;
      }

      this._imageDownloader.DownloadImage(url, null, (IUdonEventReceiver)this);
    }

    public override void OnImageLoadSuccess(IVRCImageDownload result) {
      Debug.Log($"[Texture2D Loader Converter] Image download from \"{result.Url}\" successful", this);
      this.localTexture2D.State = result.Result;

      this._OnImageLoadFinished();
    }

    public override void OnImageLoadError(IVRCImageDownload result) {
      Debug.LogWarning(
        $"[Texture2D Loader Converter] Failed to load texture from \"{result.Url}\": {result.ErrorMessage}", this);

      var texture = this.errorTexture;
      switch (result.Error) {
        case VRCImageDownloadError.AccessDenied:
          texture = this.accessDeniedErrorTexture;
          break;
        case VRCImageDownloadError.DownloadError:
          texture = this.downloadErrorTexture;
          break;
        case VRCImageDownloadError.InvalidImage:
          texture = this.invalidErrorTexture;
          break;
        case VRCImageDownloadError.TooManyRequests:
          texture = this.tooManyRequestsErrorTexture;
          break;
        case VRCImageDownloadError.InvalidURL:
          texture = this.invalidUrlErrorTexture;
          break;
      }

      if (texture == null) {
        texture = this.errorTexture;
      }

      this.localTexture2D.State = texture;

      this._OnImageLoadFinished();

      if (result.Error == VRCImageDownloadError.TooManyRequests) {
        this._retryTimer = this.retryPeriod;
        Debug.Log($"[Texture2D Loader Converter] Scheduling retry in {this.retryPeriod} seconds", this);
      }
    }

    private void _OnImageLoadFinished() {
      this._loading = false;
      this._retryTimer = -1;

      if (!this._downloadQueued) {
        return;
      }

      Debug.Log("[Texture2D Loader Converter] Dispatching queued image download", this);
      this._downloadQueued = false;
      this._OnStateUpdated();
    }
  }
}
