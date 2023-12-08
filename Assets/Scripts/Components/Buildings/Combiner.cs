using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace DistilledGames
{
    public class Combiner : BaseMachine
    {
        [SerializeReference]
        private AudioClip runningSound, outputSound, inputSound;
        private SFXInGame sfxController;


        protected override void Awake()
        {
            base.Awake();
            sfxController = GetComponent<SFXInGame>();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void UpdateSprite()
        {
            base.UpdateSprite();

            // Check if there are items in the input or output 
            bool isActive = CanWeProcess();

            if (isActive)
                sfxController.LoopingClipPlay(runningSound, 1f);
            else
                sfxController.LoopingClipStop();
        }

        protected override void InputtedFromConveyer()
        {
            base.InputtedFromConveyer();
            sfxController.PlayOneClip(inputSound, 1f);
        }

        protected override void OutputedToConveyer()
        {
            base.OutputedToConveyer();
            sfxController.PlayOneClip(outputSound, 1f);
        }
    }
}
