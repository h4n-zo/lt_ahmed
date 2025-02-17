﻿#if GLEY_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("https://gley.gitbook.io/easy-achievements/")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Login to GameServices")]
    public class Login : FsmStateAction
    {
        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("Event sent when Login was successful")]
        public FsmEvent loginSuccess;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("Event sent when Login failed")]
        public FsmEvent loginFailed;


        public override void Reset()
        {
            base.Reset();
            loginSuccess = null;
            loginFailed = null;
            eventTarget = FsmEventTarget.Self;
        }

        public override void OnEnter()
        {
            if (!Gley.GameServices.API.IsLoggedIn())
            {
                Gley.GameServices.API.LogIn(LoginComplete);
            }
            else
            {
                Fsm.Event(eventTarget, loginSuccess);
                Finish();
            }
        }

        private void LoginComplete(bool success)
        {
            if(success)
            {
                Fsm.Event(eventTarget, loginSuccess);
            }
            else
            {
                Fsm.Event(eventTarget, loginFailed);
            }
            Finish();
        }
    }
}
#endif
