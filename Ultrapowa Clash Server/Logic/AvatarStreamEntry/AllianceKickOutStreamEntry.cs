using System.Collections.Generic;
using UCS.Helpers;

namespace UCS.Logic
{
    internal class AllianceKickOutStreamEntry : AvatarStreamEntry
    {
        private int m_vAllianceBadgeData;
        private long m_vAllianceId;
        private string m_vAllianceName;
        private string m_vMessage;

        public override byte[] Encode()
        {
            var data = new List<byte>();

            data.AddRange(base.Encode());
            data.AddInt32(2);
            data.AddString(m_vMessage);
            data.AddInt64(m_vAllianceId);
            data.AddString(m_vAllianceName);
            data.AddInt32(m_vAllianceBadgeData);
            data.Add(2);
            data.AddInt32(0x29);
            data.AddInt32(0x0084E879);

            return data.ToArray();
        }

        public override int GetStreamEntryType()
        {
            return 5;
        }

        public void SetAllianceBadgeData(int data)
        {
            m_vAllianceBadgeData = data;
        }

        //00 00 00 02
        //00 00 00 33
        //44 C3 A9 73 6F 6C C3 A9 2C 20 6E 6F 75 73 20 61 76 6F 6E 73 20 64 C3 A9 63 69 64 C3 A9 20 64 65 20 74 27 65 78 63 6C 75 72 65 20 64 75 20 63 6C 61 6E 2E
        //00 00 00 1F 00 21 7F 96
        //00 00 00 0E
        //66 6F 6C 6C 61 74 74 69 74 75 64 65 20 21
        //5B 00 1A 56
        //01
        //00 00 00 29
        public void SetAllianceId(long id)
        {
            m_vAllianceId = id;
        }

        public void SetAllianceName(string name)
        {
            m_vAllianceName = name;
        }

        public void SetMessage(string message)
        {
            m_vMessage = message;
        }
    }
}