//-----------------------------------------------------------------------
// <copyright file="ChapterReNumber_Test.cs" company="Lifeprojects.de">
//     Class: ChapterReNumber_Test
//     Copyright © Lifeprojects.de 2023
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>05.04.2023</date>
//
// <summary>
// Klasse zum Test der PERT kalkulation
// </summary>
//-----------------------------------------------------------------------

namespace PERT_Test.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Shell;
    using DocumentFormat.OpenXml.Spreadsheet;
    using DocumentFormat.OpenXml.Wordprocessing;

    using PertNET.Core;

    [TestClass]
    public class ChapterReNumber_Test
    {
        [TestInitialize]
        public void Initialize()
        {
            CultureInfo culture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChapterReNumber_Test"/> class.
        /// </summary>
        public ChapterReNumber_Test()
        {
        }

        [TestMethod]
        public void InserNewChapter()
        {
            List<ChapterContent> chapters = CreateChapterData();
            Assert.IsTrue(chapters.Count == 11);

            Assert.AreEqual(chapters[0].ToString(), "1.0.0");
            Assert.AreEqual(chapters[1].ToString(), "2.0.0");
            Assert.AreEqual(chapters[2].ToString(), "2.1.0");
            Assert.AreEqual(chapters[3].ToString(), "3.0.0");
            Assert.AreEqual(chapters[4].ToString(), "3.1.0");
            Assert.AreEqual(chapters[5].ToString(), "3.2.0");
            Assert.AreEqual(chapters[6].ToString(), "4.0.0");
            Assert.AreEqual(chapters[7].ToString(), "4.1.0");
            Assert.AreEqual(chapters[8].ToString(), "4.2.0");
            Assert.AreEqual(chapters[9].ToString(), "5.0.0");
            Assert.AreEqual(chapters[10].ToString(), "6.0.0");

            Assert.IsFalse(IsDuplicateChapter(chapters));

            ChapterContent insert1 = new ChapterContent() { ChapterA = 4, ChapterB = 1, ChapterC = 1, ChapterText = "Kapitel(1)-Neu", ChapterInsert = true };

            List<ChapterContent> resultReNumber = ReNumberChapterInsert(chapters, insert1);
            Assert.IsTrue(resultReNumber.Count == 12);

            Assert.AreEqual(resultReNumber[0].ToString(), "1.0.0");
            Assert.AreEqual(resultReNumber[1].ToString(), "2.0.0");
            Assert.AreEqual(resultReNumber[2].ToString(), "2.1.0");
            Assert.AreEqual(resultReNumber[3].ToString(), "3.0.0");
            Assert.AreEqual(resultReNumber[4].ToString(), "3.1.0");
            Assert.AreEqual(resultReNumber[5].ToString(), "3.2.0");
            Assert.AreEqual(resultReNumber[6].ToString(), "4.0.0");
            Assert.AreEqual(resultReNumber[7].ToString(), "4.1.0");
            Assert.AreEqual(resultReNumber[8].ToString(), "4.1.1"); // Neuer Eintrag
            Assert.AreEqual(resultReNumber[9].ToString(), "4.2.0");
            Assert.AreEqual(resultReNumber[10].ToString(), "5.0.0");
            Assert.AreEqual(resultReNumber[11].ToString(), "6.0.0");

            Assert.IsFalse(IsDuplicateChapter(resultReNumber));
            ChapterContent insert2 = new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel(2)-Neu", ChapterInsert = true };

            List<ChapterContent> resultReNumber1 = ReNumberChapterInsert(resultReNumber, insert2);

            Assert.IsTrue(resultReNumber1.Count == 13);

            Assert.AreEqual(resultReNumber1[0].ToString(), "1.0.0");
            Assert.AreEqual(resultReNumber1[1].ToString(), "2.0.0"); // Neuer Eintrag
            Assert.AreEqual(resultReNumber1[2].ToString(), "3.0.0");
            Assert.AreEqual(resultReNumber1[3].ToString(), "3.1.0");
            Assert.AreEqual(resultReNumber1[4].ToString(), "4.0.0");
            Assert.AreEqual(resultReNumber1[5].ToString(), "4.1.0");
            Assert.AreEqual(resultReNumber1[6].ToString(), "4.2.0");
            Assert.AreEqual(resultReNumber1[7].ToString(), "5.0.0");
            Assert.AreEqual(resultReNumber1[8].ToString(), "5.1.0");
            Assert.AreEqual(resultReNumber1[9].ToString(), "5.1.1");
            Assert.AreEqual(resultReNumber1[10].ToString(), "5.2.0");
            Assert.AreEqual(resultReNumber1[11].ToString(), "6.0.0");
            Assert.AreEqual(resultReNumber1[12].ToString(), "7.0.0");

            Assert.IsFalse(IsDuplicateChapter(resultReNumber1));
        }

        [TestMethod]
        public void DeleteCurrentChapterA2()
        {
            List<ChapterContent> chapters = CreateChapterData();
            Assert.IsTrue(chapters.Count == 11);

            ChapterContent deleteChapter = new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel(1)-Gelöscht"};

            List<ChapterContent> resultReNumber1 = ReNumberChapterDelete(chapters, deleteChapter);

        }

        [TestMethod]
        public void DeleteCurrentChapterA21()
        {
            List<ChapterContent> chapters = CreateChapterData();
            Assert.IsTrue(chapters.Count == 11);

            ChapterContent deleteChapter = new ChapterContent() { ChapterA = 2, ChapterB = 1, ChapterC = 0, ChapterText = "Kapitel(1)-Gelöscht" };

            List<ChapterContent> resultReNumber1 = ReNumberChapterDelete(chapters, deleteChapter);

        }

        [TestMethod]
        public void ExceptionTest()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.GetType() == typeof(Exception));
            }
        }

        private List<ChapterContent> CreateChapterData()
        {
            List<ChapterContent> sl = new List<ChapterContent>();
            sl.Add(new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel A" });
            sl.Add(new ChapterContent() { ChapterA = 2, ChapterB = 1, ChapterC = 0, ChapterText = "Kapitel A-A" });
            sl.Add(new ChapterContent() { ChapterA = 1, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel B" });
            sl.Add(new ChapterContent() { ChapterA = 3, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel C" });
            sl.Add(new ChapterContent() { ChapterA = 3, ChapterB = 1, ChapterC = 0, ChapterText = "Kapitel C-A" });
            sl.Add(new ChapterContent() { ChapterA = 3, ChapterB = 2, ChapterC = 0, ChapterText = "Kapitel C-B" });
            sl.Add(new ChapterContent() { ChapterA = 4, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel D" });
            sl.Add(new ChapterContent() { ChapterA = 4, ChapterB = 1, ChapterC = 0, ChapterText = "Kapitel D-A" });
            sl.Add(new ChapterContent() { ChapterA = 4, ChapterB = 2, ChapterC = 0, ChapterText = "Kapitel D-B" });
            sl.Add(new ChapterContent() { ChapterA = 5, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel E" });
            sl.Add(new ChapterContent() { ChapterA = 6, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel F" });

            List<ChapterContent> chapters = sl.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ToList();

            return chapters;
        }

        private bool IsDuplicateChapter(List<ChapterContent> chapters)
        {
            return chapters.Select((x, i) => new { Index = i, Value = x })
                   .GroupBy(x => new { x.Value.ChapterA, x.Value.ChapterB, x.Value.ChapterC })
                   .Where(x => x.Skip(1).Any()).Any();

        }

        private List<ChapterContent> ReNumberChapterInsert(List<ChapterContent> chapters, ChapterContent insert)
        {
            chapters.Add(insert);
            chapters = chapters.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenByDescending(i => i.ChapterInsert).ToList();

            var query = chapters.Select((x, i) => new { Index = i, Value = x })
                   .GroupBy(x => new { x.Value.ChapterA, x.Value.ChapterB, x.Value.ChapterC })
                   .Where(x => x.Skip(1).Any()).ToList().FirstOrDefault();

            if (query != null)
            {
                int startIndex = 0;
                foreach (var group in query.Where(w => w.Value.ChapterInsert == true))
                {
                    startIndex = group.Index;
                }

                for (int i = startIndex + 1; i < chapters.Count; i++)
                {
                    ChapterContent insertItem = chapters[startIndex];
                    ChapterContent currentItem = chapters[i];

                    if (insertItem.ChapterA > 0 && insertItem.ChapterB == 0 && insertItem.ChapterC == 0)
                    {
                        chapters[i].ChapterA = chapters[i].ChapterA + 1;
                    }
                    else if (insertItem.ChapterA == currentItem.ChapterA && insertItem.ChapterB > 0 && insertItem.ChapterC == 0)
                    {
                        chapters[i].ChapterB = chapters[i].ChapterB + 1;
                    }
                    else if (insertItem.ChapterA == currentItem.ChapterA && insertItem.ChapterB == currentItem.ChapterB && insertItem.ChapterC > 0)
                    {
                        chapters[i].ChapterB = chapters[i].ChapterB + 1;
                    }
                }

                chapters.ForEach(f => f.ChapterInsert = false);
            }

            return chapters;
        }

        private List<ChapterContent> ReNumberChapterDelete(List<ChapterContent> chapters, ChapterContent delete)
        {
            int startIndex = chapters.IndexOf(i => i.ChapterA == delete.ChapterA && i.ChapterB== delete.ChapterB && i.ChapterC == delete.ChapterC);
            chapters.RemoveAt(startIndex);
            chapters = chapters.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenByDescending(i => i.ChapterDelete).ToList();

            for (int i = startIndex+1; i < chapters.Count; i++)
            {
                ChapterContent beforeItem = chapters[i];
                ChapterContent currentItem = chapters[startIndex];

                if (currentItem.ChapterA > 0 && beforeItem.ChapterB == 0 && beforeItem.ChapterC == 0)
                {
                    chapters[i].ChapterA = chapters[i].ChapterA - 1;
                }
                else if (beforeItem.ChapterA == currentItem.ChapterA && beforeItem.ChapterB > 0 && beforeItem.ChapterC == 0)
                {
                    chapters[i].ChapterB = chapters[i].ChapterB - 1;
                }
                else if (beforeItem.ChapterA == currentItem.ChapterA && beforeItem.ChapterB == currentItem.ChapterB && beforeItem.ChapterC > 0)
                {
                    chapters[i].ChapterB = chapters[i].ChapterB - 1;
                }
            }

            return chapters.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ToList();
        }

        [DebuggerDisplay("FullName={this.FullName}")]
        private class ChapterContent
        {
            public ChapterContent()
            {
                this.Id = Guid.NewGuid();
            }

            public Guid Id { get; set; }

            public int ChapterA { get; set; } = 0;

            public int ChapterB { get; set; } = 0;

            public int ChapterC { get; set; } = 0;

            public string ChapterText { get; set; }

            public bool ChapterInsert { get; set; }

            public bool ChapterDelete { get; set; }

            public string Chapter { get { return $"{this.ChapterA}.{this.ChapterB}.{this.ChapterC}"; } }

            public string FullName { get { return $"{this.ChapterA}.{this.ChapterB}.{this.ChapterC}-{this.ChapterInsert}:{this.ChapterDelete}-{this.ChapterText}"; } }

            public override string ToString()
            {
                return $"{this.ChapterA}.{this.ChapterB}.{this.ChapterC}"; ;
            }
        }
    }
}
