//-----------------------------------------------------------------------
// <copyright file="KeylistCollection.cs" company="www.pta.de">
//     Class: KeylistCollection
//     Copyright © www.pta.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - www.pta.de</author>
// <email>gerhard.ahrens@pta.de</email>
// <date>26.07.2022 17:47:31</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace PertNET.Core.Collection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EasyPrototyping.Core;

    using EasyPrototypingNET.Core.Collection;

    public class KeyListCollection
    {
        private static LexiconCollection<string, string> keyCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyListCollection"/> class.
        /// </summary>
        static KeyListCollection()
        {
            keyCollection = new LexiconCollection<string, string>();
        }

        public static void Add(string key, string value )
        {

            if (keyCollection.Contains(key, value) == false)
            {
                if (value.Contains("none", StringComparison.OrdinalIgnoreCase) == true)
                {
                    value = value.Replace("None", string.Empty);
                }

                keyCollection.Add(key, value);
            }
        }

        public static LexiconCollection<string, string> Gets()
        {
            return keyCollection;
        }

        public static List<Tuple<string,string>> ToList()
        {
            List<Tuple<string, string>> internalList = new List<Tuple<string, string>>();
            foreach (var item in keyCollection)
            {
                Tuple<string, string> itemContent = new Tuple<string, string>(item.Key, item.Value);
                internalList.Add(itemContent);
            }

            return internalList;
        }
    }
}
