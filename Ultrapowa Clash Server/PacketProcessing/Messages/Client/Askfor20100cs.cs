using System.Collections.Generic;
using System.IO;
using System.Linq;
using UCS.Core;
using UCS.Helpers;
using UCS.Logic;
using UCS.Network;

namespace UCS.PacketProcessing
{
    internal class Askfor20100 : Message
    {
        public Askfor20100(Client client, BinaryReader br) : base(client, br)
        {
            //Not sure if there should be something here o.O
        }

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
            var p = new LoginFailedMessage(Client);
            p.SetErrorCode(10);
            p.RemainingTime(0);
            p.SetReason("You are connecting with 8.67 client but UCS not support it yet");
            PacketManager.ProcessOutgoingPacket(p);
            return;
        }
    }
}