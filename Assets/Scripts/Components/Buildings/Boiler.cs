using System.Collections;
using UnityEngine;

namespace DistilledGames
{
    public class Boiler : BaseMachine
    {
        [SerializeField] private SpriteRenderer boilerSpriteRenderer; // Make sure to assign this in the Inspector.
        [SerializeField] private Sprite idleSprite; // Sprite when the boiler is not in use
        [SerializeField] private Sprite activeSprite; // Sprite when the boiler is in use

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
           
            if (boilerSpriteRenderer == null)
                boilerSpriteRenderer = GetComponent<SpriteRenderer>();
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

            // Update the sprite
            boilerSpriteRenderer.sprite = isActive ? activeSprite : idleSprite;
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
