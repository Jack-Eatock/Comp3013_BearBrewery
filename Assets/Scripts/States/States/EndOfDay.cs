using UnityEngine;

namespace DistilledGames.States
{
    public class EndOfDay : BaseState
    {
        public override void StateEnter()
        {
            base.StateEnter();

            gameManager.SetBearActive(false);
            gameManager.SetItemsActive(false);
            // Any other end of day initializations
        }

        public override void StateExit()
        {
            base.StateExit();

            gameManager.SetBearActive(true);
            gameManager.SetItemsActive(true);
            // Any cleanup needed
        }

        public override void StateUpdate()
        {
            base.StateUpdate();
        }

    }
}