using System.IO;
using UCS.Core;
using UCS.GameFiles;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing
{
    //Commande 0x20B
    internal class ClaimAchievementRewardCommand : Command
    {
        public ClaimAchievementRewardCommand(BinaryReader br)
        {
            AchievementId = br.ReadInt32WithEndian(); //= achievementId - 0x015EF3C0;
            Unknown1 = br.ReadUInt32WithEndian();
        }

        public int AchievementId { get; set; }

        //00 00 02 0B 01 5E F3 C6 00 00 06 53
        public uint Unknown1 { get; set; }

        public override void Execute(Level level)
        {
            var ca = level.GetPlayerAvatar();

            var ad = (AchievementData) ObjectManager.DataTables.GetDataById(AchievementId);

            ca.AddDiamonds(ad.DiamondReward);
            ca.AddExperience(ad.ExpReward);

            ca.SetAchievment(ad, true);
        }
    }
}