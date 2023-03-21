//-----------------------------------------------------------------------
// <copyright file="EffortProjectRepository.cs" company="Lifeprojects.de">
//     Class: EffortProjectRepository
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
    using System.Collections.Generic;
    using System.Linq;

    using LiteDB;

    using PertNET.BaseClass;
    using PertNET.Model;

    public class EffortProjectRepository : RepositoryBase<EffortProject>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffortProjectRepository"/> class.
        /// </summary>
        public EffortProjectRepository(string databaseFile) : base(databaseFile)
        {
            if (string.IsNullOrEmpty(databaseFile) == false)
            {
                this.Database = base.DatabaseIntern;
                this.Collection = base.CollectionIntern;
            }
        }

        public LiteDatabase Database { get; private set; }

        public ILiteCollection<EffortProject> Collection { get; private set; }

        public override int Count()
        {
            return base.Count();
        }

        public override EffortProject ListById(Guid id)
        {
            EffortProject result = null;

            if (id != Guid.Empty)
            {
                result = base.ListById(id);
            }

            return result;
        }

        public IEnumerable<Attachment> ListByAttachmentId(Guid id)
        {
            IEnumerable<Attachment> result = null;

            if (id != Guid.Empty)
            {
                try
                {
                    ILiteCollection<Attachment> attachmentCollection = this.DatabaseIntern.GetCollection<Attachment>(nameof(Attachment));
                    result = attachmentCollection.Find(f => f.ObjectId == id).ToList();
                }
                catch (Exception ex)
                {
                    string errotrText = ex.Message;
                    throw;
                }
            }

            return result;
        }

        public override void Add(EffortProject entity)
        {
            try
            {
                ILiteCollection<EffortProject> entityCollection = this.DatabaseIntern.GetCollection<EffortProject>(nameof(EffortProject));

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

        public override bool Update(EffortProject entity)
        {
            try
            {
                ILiteCollection<EffortProject> entityCollection = this.DatabaseIntern.GetCollection<EffortProject>(nameof(EffortProject));

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
                ILiteCollection<EffortProject> entityCollection = this.DatabaseIntern.GetCollection<EffortProject>(nameof(EffortProject));
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

        public bool ExistChapter(int chapterA, int chapterB, int chapterC)
        {
            bool result = false;

            try
            {
                ILiteCollection<EffortProject> entityCollection = this.DatabaseIntern.GetCollection<EffortProject>(nameof(EffortProject));
                result = entityCollection.Find(f => f.ChapterA == chapterA && f.ChapterB == chapterB && f.ChapterC == chapterC).Any();
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }

            return result;
        }

        public Tuple<double, double, double> BuildFullSum()
        {
            Tuple<double, double, double> result = null;

            try
            {
                ILiteCollection<EffortProject> entityCollection = this.DatabaseIntern.GetCollection<EffortProject>(nameof(EffortProject));
                double min = entityCollection.FindAll().Sum(s => s.Min);
                double mid = entityCollection.FindAll().Sum(s => s.Mid);
                double max = entityCollection.FindAll().Sum(s => s.Max);

                result = new Tuple<double, double, double>(min, mid, max);
            }
            catch (Exception ex)
            {
                string errotrText = ex.Message;
                throw;
            }

            return result;
        }
    }
}
