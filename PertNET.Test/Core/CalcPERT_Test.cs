//-----------------------------------------------------------------------
// <copyright file="CalcPERT_Test.cs" company="Lifeprojects.de">
//     Class: CalcPERT_Test
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>18.07.2022 12:58:35</date>
//
// <summary>
// Klasse zum Test der PERT kalkulation
// </summary>
//-----------------------------------------------------------------------

namespace PERT_Test.Core
{
    using System;
    using System.Globalization;
    using System.Threading;

    using PertNET.Core;

    [TestClass]
    public class CalcPERT_Test
    {
        [TestInitialize]
        public void Initialize()
        {
            CultureInfo culture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalcPERT_Test"/> class.
        /// </summary>
        public CalcPERT_Test()
        {
        }

        [TestMethod]
        public void xyz_xyz()
        {
        }

        [DataRow(1.0, 2.0,3.0,1.0,"2,00")]
        [DataRow(0.5, 1.0, 1.75, 1.0, "1,04")]
        [DataRow(2, 3.5, 4, 1.25, "4,17")]
        [DataRow(1, 1, 1, 1, "1,00")]
        [DataRow(4, 5.5, 7, 0.75, "4,13")]
        [TestMethod]
        public void PERTEffort_Test(double min,double mid, double max, double factor, string expected)
        {
            string result = string.Empty;
            using (CalcPERT cp = new CalcPERT())
            {
                result = cp.PERTEffort(min, mid, max, factor).ToString("0.00");
            }

            Assert.AreEqual(expected, result);
        }

        [DataRow(1, 3, 1, "0,33")]
        [DataRow(0.5, 1.75, 1, "0,21")]
        [DataRow(2, 4, 1.25, "0,42")]
        [DataRow(1, 1, 1, "0,00")]
        [DataRow(4,7, 0.75, "0,38")]
        [TestMethod]
        public void StandardVariation_Test(double min, double max, double factor, string expected)
        {
            string result = string.Empty;
            using (CalcPERT cp = new CalcPERT())
            {
                result = cp.StandardVariation(min, max, factor).ToString("0.00");
            }

            Assert.AreEqual(expected, result);
        }

        [DataRow(1, 3, 1, "0,11")]
        [DataRow(0.5, 1.75, 1, "0,04")]
        [DataRow(2, 4, 1.25, "0,17")]
        [DataRow(1, 1, 1, "0,00")]
        [DataRow(4, 7, 0.75, "0,14")]
        [TestMethod]
        public void VarianzValue_Test(double min, double max, double factor, string expected)
        {
            string result = string.Empty;
            using (CalcPERT cp = new CalcPERT())
            {
                result = cp.VarianzValue(min, max, factor).ToString("0.00");
            }

            Assert.AreEqual(expected, result);
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
    }
}
