using System.Collections.Generic;
using UCS.Logic;

namespace UCS.PacketProcessing
{
    //Packet 24412
    internal class AvatarStreamEntryMessage : Message
    {
        private AvatarStreamEntry m_vAvatarStreamEntry;

        public AvatarStreamEntryMessage(Client client) : base(client)
        {
            SetMessageType(24412);
        }

        //00 00 00 06
        //00 00 00 00
        //AB 00 3F FA
        //00 00 00 15 00 58 4D 94
        //00 00 00 0C 69 69 69 69 69 69 69 69 69 69 69 69
        //00 00 00 08
        //00 00 00 00
        //00
        //00 00 00 02
        //00 00 00 0E
        //63 6F 72 72 69 67 C3 A9 20 66 61 75 74 65 01 00 00 00 15 00 58 4D 94 00 00 00 00 00 1A 8E 98 00 00 00 0C 74 68 65 20 38 30 30 20 63 6C 75 62 5B 00 09 53
        public override void Encode()
        {
            var pack = new List<byte>();

            pack.AddRange(m_vAvatarStreamEntry.Encode());

            SetData(pack.ToArray());
        }

        public void SetAvatarStreamEntry(AvatarStreamEntry entry)
        {
            m_vAvatarStreamEntry = entry;
        }
    }
}