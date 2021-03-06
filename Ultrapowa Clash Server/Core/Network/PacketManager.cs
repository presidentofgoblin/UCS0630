using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using UCS.Core;
using UCS.PacketProcessing;

namespace UCS.Network
{
    internal class PacketManager
    {
        private static readonly EventWaitHandle m_vIncomingWaitHandle = new AutoResetEvent(false);

        private static readonly EventWaitHandle m_vOutgoingWaitHandle = new AutoResetEvent(false);

        private static ConcurrentQueue<Message> m_vIncomingPackets = new ConcurrentQueue<Message>();

        private static ConcurrentQueue<Message> m_vOutgoingPackets = new ConcurrentQueue<Message>();

        private bool m_vIsRunning;

        public PacketManager()
        {
            m_vIsRunning = false;
        }

        public static void ProcessIncomingPacket(Message p)
        {
            m_vIncomingPackets.Enqueue(p);
            m_vIncomingWaitHandle.Set();
        }

        public static void ProcessOutgoingPacket(Message p)
        {
            p.Encode();
            try
            {
                var pl = p.Client.GetLevel();
                var player = "";
                if (pl != null)
                    player += string.Format(" ({0}, {1})", pl.GetPlayerAvatar().GetId(),
                        pl.GetPlayerAvatar().GetAvatarName());
                Debugger.WriteLine(string.Format("[S] {0} {1}{2}", p.GetMessageType(), p.GetType().Name, player));
                GuiConsoleWrite.Write(string.Format("[S] {0} {1}{2}", p.GetMessageType(), p.GetType().Name, player));
                m_vOutgoingPackets.Enqueue(p);
                m_vOutgoingWaitHandle.Set();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error with packet manager : " + ex);
            }
        }

        public void Start()
        {
            m_vIsRunning = true;

            IncomingProcessingDelegate incomingProcessing = IncomingProcessing;
            incomingProcessing.BeginInvoke(null, null);

            OutgoingProcessingDelegate outgoingProcessing = OutgoingProcessing;
            outgoingProcessing.BeginInvoke(null, null);
        }

        public void Stop()
        {
            m_vIsRunning = false;
        }

        private void IncomingProcessing()
        {
            while (m_vIsRunning)
            {
                m_vIncomingWaitHandle.WaitOne();
                Message p;
                while (m_vIncomingPackets.TryDequeue(out p))
                {
                    p.Client.Decrypt(p.GetData());
                    Logger.WriteLine(p, "R");
                    MessageManager.ProcessPacket(p);
                }
            }
        }

        private void OutgoingProcessing()
        {
            while (m_vIsRunning)
            {
                m_vOutgoingWaitHandle.WaitOne();
                Message p;
                while (m_vOutgoingPackets.TryDequeue(out p))
                {
                    Logger.WriteLine(p, "S");
                    if (p.GetMessageType() == 20000)
                    {
                        var sessionKey = ((SessionKeyMessage) p).Key;
                        p.Client.Encrypt(p.GetData());
                        p.Client.UpdateKey(sessionKey);
                    }
                    else
                    {
                        p.Client.Encrypt(p.GetData());
                    }

                    try
                    {
                        if (p.Client.Socket != null)
                        {
                            p.Client.Socket.Send(p.GetRawData());
                        }
                        else
                        {
                            ResourcesManager.DropClient(p.Client.GetSocketHandle());
                        }
                    }
                    catch (Exception)
                    {
                        try
                        {
                            ResourcesManager.DropClient(p.Client.GetSocketHandle());
                            p.Client.Socket.Shutdown(SocketShutdown.Both);
                            p.Client.Socket.Close();
                        }
                        catch (Exception ex)
                        {
                            Debugger.WriteLine("Error when closing the connection from client : ", ex, 4, ConsoleColor.Cyan);
                        }
                    }
                }
            }
        }

        private delegate void IncomingProcessingDelegate();

        private delegate void OutgoingProcessingDelegate();
    }
}