using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

namespace TORISOUP.AnimatorControllerAlignment.Editor
{
    public static class AlignmentTask
    {
        private static int FindState(ChildAnimatorState[] states, AnimatorState state)
        {
            for (int i = 0; i < states.Length; i++)
            {
                if (state == states[i].state)
                {
                    return i;
                }
            }

            return -1;
        }

        public static void GridLayout(AnimatorStateMachine stateMachine)
        {
            var max = stateMachine.states.Length;
            var positions = new Vector3[max];

            // 初期位置
            for (int i = 0; i < max; i++)
            {
                positions[i] = new Vector3((i % 5) * 300, 100 * (i / 5));
            }

            //-----   新しい配置に上書き  ---------
            var newStates = new ChildAnimatorState[stateMachine.states.Length];
            for (int i = 0; i < max; i++)
            {
                var c = stateMachine.states[i];
                newStates[i] = new ChildAnimatorState
                {
                    position = positions[i],
                    state = c.state
                };
            }

            stateMachine.states = newStates;
        }


        public static void Align(
            AnimatorStateMachine stateMachine,
            int tryCount,
            float relationK,
            float relationNaturalLength,
            float repulsivePower,
            float threshold
        )
        {
            var max = stateMachine.states.Length;
            var positions = new Vector3[max];

            // 初期位置
            for (int i = 0; i < max; i++)
            {
                positions[i] = stateMachine.states[i].position;
            }

            var defaultStateIndex = 0;

            // デフォルトノード
            for (int i = 0; i < max; i++)
            {
                var s = stateMachine.states[i];
                if (stateMachine.defaultState != s.state) continue;
                defaultStateIndex = i;
                break;
            }

            //----- 関係性グラフを作成 ----
            var relations = new List<int>[max];
            for (int i = 0; i < max; i++)
            {
                var s = stateMachine.states[i].state;
                relations[i] = new List<int>();

                foreach (var transition in s.transitions)
                {
                    // ノードがつながっている場合は互いに連動する
                    var target = FindState(stateMachine.states, transition.destinationState);
                    relations[i].Add(target);
                    if (relations[target] == null) relations[target] = new List<int>();
                    relations[target].Add(i);
                }
            }

            while (tryCount-- > 0)
            {
                for (int i = 0; i < max; i++)
                {

                    // default stateを固定
                    if (i == defaultStateIndex) continue;

                    var target = positions[i];
                    var force = Vector3.zero;

                    for (int j = 0; j < max; j++)
                    {
                        if (j == i) continue;
                        {
                            var other = positions[j];

                            // ばねの計算
                            // 接続したノード同士はばねによって引き合う
                            var isConnectedNode = relations[i].Contains(j);
                            if (isConnectedNode)
                            {
                                var k = relationK;
                                var nl = relationNaturalLength;

                                var l = (target - other).magnitude;
                                var delta = l - nl;

                                force += -(delta * k * (other - target).normalized);
                            }

                            // 全ノードは互いに斥力が発生する
                            {
                                var l = (other - target).magnitude;
                                if (l < threshold)
                                {
                                    force += -(other - target).normalized * ((threshold - l) * repulsivePower);
                                }
                            }
                        }
                    }


                    positions[i] = target + force * 1.0f;
                }
            }

            //-----   新しい配置に上書き  ---------
            var newStates = new ChildAnimatorState[stateMachine.states.Length];
            for (int i = 0; i < max; i++)
            {
                var c = stateMachine.states[i];
                newStates[i] = new ChildAnimatorState
                {
                    position = positions[i],
                    state = c.state
                };
            }

            stateMachine.states = newStates;


        }
    }
}