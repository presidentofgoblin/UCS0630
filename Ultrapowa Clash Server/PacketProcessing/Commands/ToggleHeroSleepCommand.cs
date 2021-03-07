using System.IO;
using UCS.Helpers;
using UCS.Logic;

namespace UCS.PacketProcessing
{
    //Commande 0x0211
    internal class ToggleHeroSleepCommand : Command
    {
        public ToggleHeroSleepCommand(BinaryReader br)
        {
            BuildingId = br.ReadUInt32WithEndian(); //buildingId - 0x1DCD6500;
            FlagSleep = br.ReadByte();
            Unknown1 = br.ReadUInt32WithEndian();
        }

        //00 00 02 11 1D CD 65 06 00 00 01 04 CA
        public override void Execute(Level level)
        {
            System.Console.WriteLine(BuildingId);
            System.Console.WriteLine(FlagSleep);
            System.Console.WriteLine(Unknown1);
        }
        public uint BuildingId { get; set; }

        public byte FlagSleep { get; set; }

        public uint Unknown1 { get; set; }
    }
}