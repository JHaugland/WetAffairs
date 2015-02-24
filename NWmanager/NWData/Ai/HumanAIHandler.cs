using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData.Ai
{
    [Serializable]
    public class HumanAIHandler : BaseAIHandler
    {
        #region "Constructors"

        public HumanAIHandler(): base()
        {

        }

        #endregion


        #region "Public properties"

        #endregion

        public override void GamePlayHasStarted()
        {
            base.GamePlayHasStarted();
            SetLandInstallationsActiveRadar();
        }



        #region "Public methods"

        public override void BattleDamageReportReceived(BattleDamageReport battleDamageReport)
        {
            base.BattleDamageReportReceived(battleDamageReport);
            HandleIncomingBattleDamageReport(battleDamageReport, true, true, false, false, false);
        }

        public override void TickEveryMinute(DateTime gameTime)
        {
            base.TickEveryMinute(gameTime);
            // GameManager.Instance.Log.LogDebug("HumanAIHandler->TickEveryMinite  GameTime=" + gameTime);
            EngageDetectedAirTargets();
        }

        #endregion


    }
}
