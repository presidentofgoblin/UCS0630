using System.IO;
using UCS.Core;
using UCS.Logic;
using UCS.Network;

namespace UCS.PacketProcessing
{
    internal class ResetHomeGameOpCommand : GameOpCommand
    {
        public ResetHomeGameOpCommand(string[] args)
        {
            SetRequiredAccountPrivileges(1);
        }

        public override void Execute(Level level)
        {
            if (level.GetPlayerAvatar().GetId() != null)
            {
                var id = level.GetPlayerAvatar().GetId();
                var l = ResourcesManager.GetPlayer(id);
                if (l != null)
                {
                    using (var sr = new StreamReader(@"gamefiles/default/home.json"))
                    {
                        level.SetHome(sr.ReadToEnd());
                    }
                    var p = new OutOfSyncMessage(l.GetClient());
                    PacketManager.ProcessOutgoingPacket(p);
                }
                else
                {
                    Debugger.WriteLine("ResetPlayer failed: id " + id + " not found");
                }
            }
            else
            {
                SendCommandFailedMessage(level.GetClient());
            }
        }
    }
}