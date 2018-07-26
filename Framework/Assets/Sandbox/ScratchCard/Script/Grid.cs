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

    public class Grid : MonoBehaviour
    {
        [SerializeField, ShowInInspector]
        private GridData GridData;

        [Button(25)]
        public void Arrange()
        {
            int row = GridData.Row;
            int col = GridData.Col;
            bool hasValidChildCount = row * col == transform.childCount;

            //Assertion.Assert(hasValidChildCount, D.ERROR + "Grid::Arrange Grid Size:{0} is not equal to ChildCount:{1}\n", row * col, transform.childCount);
            
            if (!hasValidChildCount)
            {
                Debug.LogWarningFormat(D.WARNING + "Grid::Arrange Grid Size:{0} is not equal to ChildCount:{1}\n", row * col, transform.childCount);
            }

            int childCount = transform.childCount;
            float w = GridData.Width;
            float h = GridData.Height;
            float initX = (float)(col-1) * w * 0.5f * -1f;
            float initY = (float)(row-1) * h * 0.5f;
            Vector2 localScale = Vector2.one;
            localScale.x = w;
            localScale.y = h;

            for (int r = 0; r < row; r++)
            {
                for (int c = 0; c < col; c++)
                {
                    int index = (r * col) + c;

                    if (index >= childCount)
                    {
                        break;
                    }

                    Transform child = transform.GetChild(index);
                    Vector3 origin = Vector3.zero;
                    origin.x = initX + (w * (float)c);
                    origin.y = initY - (h * (float)r);
                    child.localPosition = origin;
                    child.localScale = localScale;
                }
            }
        }
    }
}
