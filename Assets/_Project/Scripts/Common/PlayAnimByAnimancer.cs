using System;
using Animancer;
using Pancake;
using UnityEngine;

[RequireComponent(typeof(AnimancerComponent))]
public class PlayAnimByAnimancer : MonoBehaviour
{
    public AnimancerComponent animancerComponent;

    public void PlayAnim(ClipTransition clip, float _timeFade = .2f, Action _endAnim = null)
    {
        if (!animancerComponent.IsPlaying(clip))
        {
            animancerComponent.Play(clip, clip.Clip.length * _timeFade).Events.OnEnd = () => { _endAnim?.Invoke(); };
        }
    }
#if UNITY_EDITOR
    [Button("Setup Animancer Component")]
    private void GetAnimancerComponent()
    {
        if (animancerComponent == null)
        {
            animancerComponent = GetComponent<AnimancerComponent>();
        }
    }
#endif
}
