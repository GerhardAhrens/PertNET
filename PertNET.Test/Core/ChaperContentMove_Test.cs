//-----------------------------------------------------------------------
// <copyright file="ChaperContentMove_Test.cs" company="Lifeprojects.de">
//     Class: ChaperContentMove_Test
//     Copyright © Lifeprojects.de 2023
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>09.02.2023 07:03:35</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PERT_Test.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using EasyPrototyping.Core;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using static System.Net.Mime.MediaTypeNames;

    [TestClass]
    public class ChaperContentMove_Test
    {
        [TestInitialize]
        public void Initialize()
        {
            CultureInfo culture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChaperContentMove_Test"/> class.
        /// </summary>
        public ChaperContentMove_Test()
        {
        }

        [TestMethod]
        public void InserChapterA()
        {
            List<ChapterContent> sl = new List<ChapterContent>();
            sl.Add(new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel B" });
            sl.Add(new ChapterContent() { ChapterA = 1, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel B" });
            sl.Add(new ChapterContent() { ChapterA = 3, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel B" });
            List<ChapterContent> chapters = sl.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenBy(d => d.Action).ToList();

            sl.Add(new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel B-B bfore", Action = Action.Before });
            chapters = sl.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenBy(d => d.Action).ToList();
            chapters = MoveChapter(chapters);
            Assert.IsNotNull(chapters);
            Assert.IsTrue(chapters[chapters.Count-1].ChapterA == 4);

            sl.Add(new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel 2 after", Action = Action.After });
            chapters = sl.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenBy(d => d.Action).ToList();
            chapters = MoveChapter(chapters);
            Assert.IsNotNull(chapters);
            Assert.IsTrue(chapters[chapters.Count - 1].ChapterA == 5);

            sl.Add(new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel 2 after", Action = Action.After });
            chapters = sl.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenBy(d => d.Action).ToList();
            chapters = MoveChapter(chapters);
            Assert.IsNotNull(chapters);
            Assert.IsTrue(chapters[chapters.Count - 1].ChapterA == 6);

            sl.Add(new ChapterContent() { ChapterA = 4, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel 4 after", Action = Action.After });
            chapters = sl.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenBy(d => d.Action).ToList();
            chapters = MoveChapter(chapters);
            Assert.IsNotNull(chapters);
            Assert.IsTrue(chapters[chapters.Count - 1].ChapterA == 7);

            sl.Add(new ChapterContent() { ChapterA = 4, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel 4 before", Action = Action.Before });
            chapters = sl.OrderBy(a => a.ChapterA).ThenBy(b => b.ChapterB).ThenBy(c => c.ChapterC).ThenBy(d => d.Action).ToList();
            chapters = MoveChapter(chapters);
            Assert.IsNotNull(chapters);
            Assert.IsTrue(chapters[chapters.Count - 1].ChapterA == 8);
        }

        private List<ChapterContent> MoveChapter(List<ChapterContent> chapters)
        {
            for (int all = 0; all < chapters.Count; all++)
            {
                if (chapters[all].Action == Action.Before || chapters[all].Action == Action.After)
                {
                    if (chapters[all].Action == Action.Before)
                    {
                        chapters[all].Action = Action.Current;
                        for (int next = all; next < chapters.Count - 1; next++)
                        {
                            chapters[next + 1].ChapterA = chapters[next + 1].ChapterA + 1;
                        }
                    }
                    else if (chapters[all].Action == Action.After)
                    {
                        chapters[all].Action = Action.Current;

                        for (int next = all; next < chapters.Count; next++)
                        {
                            chapters[next].ChapterA = chapters[next].ChapterA + 1;
                        }
                    }
                }
            }

            return chapters;
        }

        [TestMethod]
        public void InserChapterB()
        {
            ObservableCollection<ChapterContent> chapters = new ObservableCollection<ChapterContent>();
            //chapters.CollectionChanged += OnChaptersCollectionChanged;

            ChapterContent c1 = new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel B"};
            chapters.InsertInPlace(c1, o => o.Chapter);
            ChapterContent c2 = new ChapterContent() { ChapterA = 1, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel A" };
            chapters.InsertInPlace(c2, o => o.Chapter);
            ChapterContent c3 = new ChapterContent() { ChapterA = 3, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel B" };
            chapters.InsertInPlace(c3, o => o.Chapter);
            ChapterContent c21 = new ChapterContent() { ChapterA = 2, ChapterB = 0, ChapterC = 0, ChapterText = "Kapitel B", Action = Action.After };
            chapters.InsertInPlace(c21, o => o.Chapter);
            Assert.IsNotNull(chapters);
        }

        private void OnChaptersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<ChapterContent> items = sender as ObservableCollection<ChapterContent>;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ChapterContent content = (ChapterContent)e.NewItems[0];
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].Action == Action.After)
                        {
                            for (int move = i; move < items.Count-i; move++)
                            {
                                if (items[move].Action != Action.None)
                                {
                                    items[move].ChapterA = items[move].ChapterA + 1;
                                    items[move].Action = Action.None;
                                }
                            }
                        }
                        else if (items[i].Action == Action.Before)
                        {

                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (ChapterContent s in e.OldItems)
                    {
                    }

                    break;
            }
        }

        [DataRow("", "")]
        [TestMethod]
        public void DataRowInputTest(string input, string expected)
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

        [DebuggerDisplay("Chapter={this.Chapter}")]
        private class ChapterContent
        {
            public ChapterContent()
            {
                this.Id = Guid.NewGuid();
            }

            public Guid Id { get; set; }

            public Action Action { get; set; } = Action.Current;

            public int ChapterA { get; set; } = 0;

            public int ChapterB { get; set; } = 0;

            public int ChapterC { get; set; } = 0;

            public string ChapterText { get; set;}

            public string Chapter { get { return $"{this.ChapterA}.{this.ChapterB}.{this.ChapterC}-{this.Action}-{this.ChapterText}"; } }
        }
    }

    public static class ObservableCollectionExtenion
    {
        /// <summary>
        /// Inserts an item into a list in the correct place, based on the provided key and key comparer. Use like OrderBy(o => o.PropertyWithKey).
        /// </summary>
        public static void InsertInPlace<TItem, TKey>(this ObservableCollection<TItem> collection, TItem itemToAdd, Func<TItem, TKey> keyGetter)
        {
            int index = collection.ToList().BinarySearch(keyGetter(itemToAdd), Comparer<TKey>.Default, keyGetter);
            collection.Insert(index, itemToAdd);
        }

        /// <summary>
        /// Binary search.
        /// </summary>
        /// <returns>Index of item in collection.</returns> 
        /// <notes>This version tops out at approximately 25% faster than the equivalent recursive version. This 25% speedup is for list
        /// lengths more of than 1000 items, with less performance advantage for smaller lists.</notes>
        public static int BinarySearch<TItem, TKey>(this IList<TItem> collection, TKey keyToFind, IComparer<TKey> comparer, Func<TItem, TKey> keyGetter)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            int lower = 0;
            int upper = collection.Count - 1;

            while (lower <= upper)
            {
                int middle = lower + (upper - lower) / 2;
                int comparisonResult = comparer.Compare(keyToFind, keyGetter.Invoke(collection[middle]));
                if (comparisonResult == 0)
                {
                    return middle;
                }
                else if (comparisonResult < 0)
                {
                    upper = middle - 1;
                }
                else
                {
                    lower = middle + 1;
                }
            }

            // If we cannot find the item, return the item below it, so the new item will be inserted next.
            return lower;
        }
    }

    public enum Action
    {
        None = 0,
        Before = 1,
        Current = 2,
        After = 3
    }

}
