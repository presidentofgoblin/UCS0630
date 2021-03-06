using System;
using System.Collections.Generic;
using System.Data.Entity;
using UCS.Database;
using UCS.Logic;

namespace UCS.Core
{
    internal class SaveAllianceThread
    {
        private readonly string m_vConnectionString;

        private readonly List<Alliance> m_vList;

        public SaveAllianceThread(List<Alliance> list, string connectionString)
        {
            m_vList = list;
            m_vConnectionString = connectionString;
        }

        public void DoSaveWork()
        {
            SaveProcess(m_vList);
        }

        private void SaveProcess(List<Alliance> alliances)
        {
            using (var context = new ucsdbEntities(m_vConnectionString))
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Configuration.ValidateOnSaveEnabled = false;
                var transactionCount = 0;
                foreach (var alliance in alliances)
                {
                    lock (alliance)
                    {
                        var c = context.clan.Find((int) alliance.GetAllianceId());
                        if (c != null)
                        {
                            c.LastUpdateTime = DateTime.Now;
                            c.Data = alliance.SaveToJSON();
                            context.Entry(c).State = EntityState.Modified;
                        }
                        else
                        {
                            context.clan.Add(
                                new clan
                                {
                                    ClanId = alliance.GetAllianceId(),
                                    LastUpdateTime = DateTime.Now,
                                    Data = alliance.SaveToJSON()
                                }
                                );
                        }
                    }
                    transactionCount++;
                    if (transactionCount >= 500)
                    {
                        context.SaveChanges();
                        transactionCount = 0;
                    }
                }
                context.SaveChanges();
                context.Dispose();
            }
        }
    }

    internal class SaveLevelThread
    {
        private readonly string m_vConnectionString;

        private readonly List<Level> m_vList;

        public SaveLevelThread(List<Level> list, string connectionString)
        {
            m_vList = list;
            m_vConnectionString = connectionString;
        }

        public void DoSaveWork()
        {
            SaveProcess(m_vList);
        }

        private void SaveProcess(List<Level> avatars)
        {
            var context = new ucsdbEntities(m_vConnectionString);

            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;
            var transactionCount = 0;
            foreach (var pl in avatars)
            {
                lock (pl)
                {
                    context = pl.SaveToDatabse(context);
                }
                transactionCount++;
                if (transactionCount >= 100)
                {
                    context.SaveChanges();
                    transactionCount = 0;
                }
            }
            context.SaveChanges();
            context.Dispose();
        }
    }
}