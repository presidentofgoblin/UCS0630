using UCS.Core;
using UCS.Logic;
using UCS.Network;

namespace UCS.PacketProcessing
{
    internal class GameOpCommand
    {
        private byte m_vRequiredAccountPrivileges;

        public virtual void Execute(Level level)
        {
        }

        public byte GetRequiredAccountPrivileges()
        {
            return m_vRequiredAccountPrivileges;
        }

        public void SendCommandFailedMessage(Client c)
        {
            Debugger.WriteLine("GameOp command failed. Insufficient privileges", null, 5);
            var p = new GlobalChatLineMessage(c);
            p.SetChatMessage("GameOp command failed. Insufficient privileges.");
            p.SetPlayerId(0);
            p.SetPlayerName("System Manager");
            PacketManager.ProcessOutgoingPacket(p);
        }

        public void SetRequiredAccountPrivileges(byte level)
        {
            m_vRequiredAccountPrivileges = level;
        }
    }
}