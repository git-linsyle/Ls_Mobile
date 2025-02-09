﻿using EdgeFramework.UI;
using EdgeFramework.Sheet;
using EdgeFramework;
using EdgeFramework.Audio;

public class ProcedureMenu : ProcedureBase
{
    public ProcedureMenu(ProcedureFSM _fsm)
: base(_fsm, StateDefine.PROCEDURE_MENU)
    {

    }
    public override void OnEnter(object[] param)
    {
        UIManager.Instance.PushPanel<BaseUI>(UIPanelTypeEnum.MainMenuPanel,false);
        AudioPlayer.Instance.PlayBGM(MusicEnum.Lobby);
    }

    public override void OnExit()
    {
      
    }

    public override void OnUpdate(float step)
    {
       
    }
}
