using System;
using UCS.Logic;
using UCS.Network;

namespace UCS.PacketProcessing
{
    internal class GetIdCommand : GameOpCommand
    {
        private readonly string[] m_vArgs;

        public GetIdCommand(string[] args)
        {
            m_vArgs = args;
            SetRequiredAccountPrivileges(0);
        }

        public override void Execute(Level level)
        {
            if (level.GetAccountPrivileges() >= GetRequiredAccountPrivileges())
            {
                var l = level.GetClient();
                var pm = new GlobalChatLineMessage(l);
                pm.SetPlayerName("System Manager");
                pm.SetLeagueId(22);
                pm.SetChatMessage("Your id: " + level.GetPlayerAvatar().GetId());
                pm.SetPlayerId(0);
                PacketManager.ProcessOutgoingPacket(pm);

            }
        }
    }
}