﻿using UnityEngine;
using UnityEngine.Playables;

namespace BehaviorDesigner.Runtime.Tasks.Unity.Timeline
{
    [TaskCategory("Unity/Timeline")]
    [TaskDescription("Instatiates a Playable using the provided PlayableAsset and starts playback.")]
    public class Play : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("An asset to instantiate a playable from.")]
        public PlayableAsset playableAsset;
        [Tooltip("Should the task be stopped when the timeline has stopped playing?")]
        public SharedBool stopWhenComplete;

        private PlayableDirector playableDirector;
        private GameObject prevGameObject;
        private bool playbackStarted;

        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject)
            {
                playableDirector = currentGameObject.GetComponent<PlayableDirector>();
                prevGameObject = currentGameObject;
            }
            playbackStarted = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (playableDirector == null)
            {
                Debug.LogWarning("PlayableDirector is null");
                return TaskStatus.Failure;
            }

            if (playbackStarted)
            {
                if (stopWhenComplete.Value && playableDirector.state == PlayState.Playing)
                {
                    return TaskStatus.Running;
                }
                return TaskStatus.Success;
            }

            if (playableAsset == null)
            {
                playableDirector.Play();
            }
            else
            {
                playableDirector.Play(playableAsset);
            }
            playbackStarted = true;

            return stopWhenComplete.Value ? TaskStatus.Running : TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            playableAsset = null;
            stopWhenComplete = false;
        }
    }
}