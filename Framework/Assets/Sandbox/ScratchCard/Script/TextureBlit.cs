using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Sirenix.OdinInspector;

using UniRx;
using UniRx.Triggers;

using Framework;

namespace Sandbox.ScratchCard
{
    using UnityEngine;

    public class TextureBlit : MonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private Camera Camera;

        [SerializeField, ShowInInspector]
        private Renderer Renderer;

        [SerializeField, ShowInInspector]
        private TextureBlitData ScratchData;

        [SerializeField, ShowInInspector]
        [MinMaxSlider(0f ,1f)]
        private float _Percentage;
        public float Percentage
        {
            get { return _Percentage; }
            private set { _Percentage = value; }
        }

        private Texture2D TextureInstance;
        private RaycastHit RayHit;
        private Ray Ray;
        private Color Transparent = Color.clear;

        private List<Color> BrushData;
        private List<Color> TextureData;

        private int ColorIndex;
        private int TextureXCoord;
        private int TextureYCoord;
        private int TargetBrushWidth;
        private int TargetBrushHeight;
        private int TargetBrushHalf;
        private int TargetTextureWidth;
        private int TargetTextureHeight;
        private int TargetTextureHalf;

        private float DustParticleXPercentage;
        private float DustParticleYPercentage;
        
        private Ray TouchRay
        {
            get { return Camera.ScreenPointToRay(Input.mousePosition); }
        }

        private void Start()
        {
            StartTextureBlitting();
        }

        [Button(25)]
        public void StartTextureBlitting()
        {
            Initialize(ScratchData);
        }

        public void Initialize(TextureBlitData data)
        {
            ScratchData = data;
            Renderer = GetComponent<Renderer>();
            Renderer.material = new Material(ScratchData.Material);
            Renderer.material.enableInstancing = true;

            TextureInstance = Instantiate(ScratchData.Texture) as Texture2D;
            Renderer.material.mainTexture = TextureInstance;

            BrushData = ScratchData.Brush.GetPixels().ToList();
            TextureData = ScratchData.Texture.GetPixels().ToList();

            TargetBrushWidth = ScratchData.Brush.width;
            TargetBrushHeight = ScratchData.Brush.height;
            TargetBrushHalf = TargetBrushWidth / 2;

            TargetTextureWidth = TextureInstance.width;
            TargetTextureHeight = TextureInstance.height;
            TargetTextureHalf = TargetTextureWidth / 2;
        }

        private void UpdateScratchTexture()
        {
            Ray = TouchRay;

            if (!Physics.Raycast(Ray, out RayHit, 1000))
            {
                return;
            }

            if (RayHit.transform.GetInstanceID() != transform.GetInstanceID())
            {
                return;
            }

            TextureXCoord = (int)(RayHit.textureCoord.x * TargetTextureWidth);
            TextureYCoord = (int)(RayHit.textureCoord.y * TargetTextureHeight);

            if (TextureData[TextureXCoord + TextureYCoord * TargetBrushWidth].a == 1)
            {
                DustParticleXPercentage = -(((TextureXCoord * 0.25f) / (TargetTextureWidth * 0.25f)) - 0.5f);
                DustParticleYPercentage = -(((TextureYCoord * 0.25f) / (TargetTextureHeight * 0.25f)) - 0.5f);
            }

            for (int textWidth = 0; textWidth < TargetBrushWidth; textWidth++)
            {
                for (int textHeight = 0; textHeight < TargetBrushHeight; textHeight++)
                {
                    ApplyBrushTexture(textWidth, textHeight);
                }
            }

            TextureInstance.SetPixels(TextureData.ToArray());
            TextureInstance.Apply();
            CalculatePercentage();
        }

        private void ApplyBrushTexture(int textureWidth, int textureHeight)
        {
            //Check if brushpixel for this index is not transparent
            if (BrushData[textureWidth + textureHeight * TargetBrushWidth].a == 0)
            {
                return;
            }

            ColorIndex = (TextureXCoord + textureWidth - TargetBrushHalf) + (TextureYCoord + textureHeight - TargetBrushHalf) * TargetTextureWidth;

            if (TextureXCoord + TargetBrushHalf >= TargetTextureWidth || TextureXCoord - TargetBrushHalf <= 0.0f)
            {
                if (TextureXCoord + TargetBrushHalf > TargetTextureWidth && ColorIndex % TargetTextureWidth >= TargetTextureHalf)
                {
                    TextureData[Mathf.Clamp(ColorIndex, 0, TextureData.Count - 1)] = Transparent;
                }
                else if (TextureXCoord - TargetBrushHalf < 0 && ColorIndex % TargetTextureWidth <= TargetTextureHalf)
                {
                    TextureData[Mathf.Clamp(ColorIndex, 0, TextureData.Count - 1)] = Transparent;
                }
            }
            else
            {
                TextureData[Mathf.Clamp(ColorIndex, 0, TextureData.Count - 1)] = Transparent;
            }
        }

        private void CalculatePercentage()
        {
            Percentage = (float)(TextureData.FindAll(_ => _.a <= 0f).Count / TextureData.Count);
        }

        [Button(25)]
        public void ClearScratch()
        {
            GetComponent<MeshRenderer>().enabled = false;
            CalculatePercentage();
        }
        
        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                UpdateScratchTexture();
            }
        }
    }
}
