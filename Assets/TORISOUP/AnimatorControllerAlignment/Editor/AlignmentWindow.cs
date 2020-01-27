using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace TORISOUP.AnimatorControllerAlignment.Editor
{
    public class AlignmentWindow : EditorWindow
    {
        private AnimatorController controller = null;
        private int selectedLayers = 1;
        private float k = -0.01f;
        private float naturalLength = 300;
        float repulsivePower = 0.01f;
        float threshold = 300.0f;
        private bool resetPosition;
        private int tryCount = 1000;
        
        //! MenuItem("メニュー名/項目名") のフォーマットで記載してね
        [MenuItem("Custom/Align AnimationController")]
        static void ShowWindow()
        {
            var w = GetWindow<AlignmentWindow>();
            w.titleContent = new GUIContent("Align AnimationController");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            controller =
                (AnimatorController) EditorGUILayout.ObjectField("Controller", controller, typeof(AnimatorController),
                    false);

            EditorGUILayout.EndHorizontal();


            if (controller == null) return;

            selectedLayers = EditorGUILayout.MaskField(
                "Target Layer",
                selectedLayers,
                controller.layers.Select(x => x.name).ToArray());

            tryCount = EditorGUILayout.IntField("計算回数", tryCount);


            if (GUILayout.Button("均等に並べる"))
            {
                foreach (var layer in TakeLayers(selectedLayers))
                {
                    if(layer >= controller.layers.Length) break;
                    AlignmentTask.GridLayout(controller.layers[layer].stateMachine);
                }

                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
            }

            k = EditorGUILayout.FloatField("ばね係数", k);
            naturalLength = EditorGUILayout.FloatField("ばねの自然長", naturalLength);
            repulsivePower = EditorGUILayout.FloatField("斥力", repulsivePower);
            threshold = EditorGUILayout.FloatField("斥力の効果範囲", threshold);

            if (GUILayout.Button("配置する"))
            {
                foreach (var layer in TakeLayers(selectedLayers))
                {
                    if(layer >= controller.layers.Length) break;
                    AlignmentTask.Align(controller.layers[layer].stateMachine,
                        tryCount, k, naturalLength,
                        repulsivePower, threshold);
                }


                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
            }
        }

        private static IEnumerable<int> TakeLayers(int n)
        {
            for (int i = 0; i < 32; i++)
            {
                if ((n & 1 << i) != 0) yield return i;
            }
        }
        
    }
}