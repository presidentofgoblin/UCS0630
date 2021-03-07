using System.Collections.Generic;
using UCS.Core;
using UCS.Logic;
using UCS.Network;

namespace UCS
{
    internal class ProgramThread
    {
        private readonly MessageManager mm;
        private readonly PacketManager pm;
        private List<Level> list;

        public bool m_vRunning = false;
        private ObjectManager om;
        private ResourcesManager rm;

        public ProgramThread()
        {
            //rm = new ResourcesManager();
            //om = new ObjectManager();
            pm = new PacketManager();
            mm = new MessageManager();
        }

        public ProgramThread(List<Level> list)
        {
            this.list = list;
        }

        public void Start()
        {
            pm.Start();
            mm.Start();
        }

        public void Stop()
        {
            pm.Stop();
            mm.Stop();
        }
    }
}