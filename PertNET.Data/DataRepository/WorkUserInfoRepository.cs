//-----------------------------------------------------------------------
// <copyright file="WorkUserInfoRepository.cs" company="Lifeprojects.de">
//     Class: WorkUserInfoRepository
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>06.07.2022 14:33:35</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.DataRepository
{
    using System;

    using LiteDB;

    using PertNET.BaseClass;
    using PertNET.Model;

    public class WorkUserInfoRepository : RepositoryBase<WorkUserInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkUserInfoRepository"/> class.
        /// </summary>
        public WorkUserInfoRepository(string databaseFile) : base(databaseFile)
        {
            this.Database = base.DatabaseIntern;
            this.Collection = base.CollectionIntern;
        }

        public LiteDatabase Database { get; private set; }

        public ILiteCollection<WorkUserInfo> Collection { get; private set; }

        public override int Count()
        {
            return base.Count();
        }

        public override WorkUserInfo ListById(Guid id)
        {
            WorkUserInfo result = null;

            if (id != Guid.Empty)
            {
                result = base.ListById(id);
            }

            return result;
        }

        public override void Add(WorkUserInfo entity)
        {
            try
            {
                ILiteCollection<WorkUserInfo> entityCollection = this.DatabaseIntern.GetCollection<WorkUserInfo>(nameof(WorkUserInfo));

                if (entityCollection != null)
                {
                    BsonValue resultEntity = entityCollection.Insert(entity);
                }
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }
        }

        public override bool Update(WorkUserInfo entity)
        {
            try
            {
                ILiteCollection<WorkUserInfo> entityCollection = this.DatabaseIntern.GetCollection<WorkUserInfo>(nameof(WorkUserInfo));

                if (entityCollection != null)
                {
                    BsonValue resultEntity = entityCollection.Update(entity);
                    return true;
                }
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }

            return false;
        }

        public override bool Delete(Guid id)
        {
            try
            {
                ILiteCollection<WorkUserInfo> entityCollection = this.DatabaseIntern.GetCollection<WorkUserInfo>(nameof(WorkUserInfo));
                if (id != Guid.Empty)
                {
                    bool result = entityCollection.Delete(id);

                    return result;
                }

            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }

            return false;
        }
    }
}
