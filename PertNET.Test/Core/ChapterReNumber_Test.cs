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
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Shell;

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

            ChapterContent insert = new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel A", ChapterInsert = true };

            List<ChapterContent> resultReNumber = ReNumberChapter(chapters, insert);
        }

        [TestMethod]
        public void DeleteCurrentChapter()
        {
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
            sl.Add(new ChapterContent() { ChapterA = 5, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel F" });
            List<ChapterContent> chapters = sl.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ToList();

            return chapters;
        }

        private List<ChapterContent> ReNumberChapter(List<ChapterContent> chapters, ChapterContent insert)
        {
            chapters.Add(insert);
            chapters = chapters.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenByDescending(i => i.ChapterInsert).ToList();

            var query = chapters.Select((x, i) => new { index = i, value = x })
                   .GroupBy(x => new { x.value.ChapterA, x.value.ChapterB, x.value.ChapterC })
                   .Where(x => x.Skip(1).Any()).ToList().FirstOrDefault();

            if (query != null)
            {
                foreach (ChapterContent item in chapters)
                {
                    if (item.ChapterInsert == true)
                    {
                        int index = chapters.IndexOf(item);

                    }
                }
            }

            return default;
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
        }
    }
}
