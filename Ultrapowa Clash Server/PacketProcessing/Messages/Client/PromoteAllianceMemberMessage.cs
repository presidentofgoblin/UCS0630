﻿using System.IO;
using UCS.Logic;

namespace UCS.PacketProcessing
{
    //Packet 14316

    internal class PromoteAllianceMemberMessage : Message
    {
        public PromoteAllianceMemberMessage(Client client, BinaryReader br) : base(client, br)
        {
            //Not sure if there should be something here o.O
        }

        public override void Decode()
        {
        }

        public override void Process(Level level)
        {
        }
    }
}