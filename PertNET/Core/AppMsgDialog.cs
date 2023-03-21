//-----------------------------------------------------------------------
// <copyright file="AppMsgDialog.cs" company="Lifeprojects.de">
//     Class: AppMsgDialog
//     Copyright © Lifeprojects.de 2020
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>14.07.2020</date>
//
// <summary>Class with MessageBox Dialogs Texts</summary>
//-----------------------------------------------------------------------

namespace PertNET.Core
{
    using System.Runtime.Versioning;
    using System.Windows;
    using System.Windows.Input;

    using EasyPrototypingNET.WPF;

    [SupportedOSPlatform("windows")]
    public class AppMsgDialog
    {
        private const string APPLICATIONNAME = "PERT Tool";
        public static DialogResultsEx ApplicationExit()
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.Show(APPLICATIONNAME, "Wollen Sie das Programm beenden?", string.Empty, MessageBoxButton.YesNo, InstructionIcon.Question, DialogResultsEx.No);

            return dialogResult;
        }

        public static DialogResultsEx FuncNotImplementation(string addText = "")
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.Show("Fehlende Funktion",
                $"Die Funktion '{addText}' ist nicht vorhanden!!",
                "Die aufgerufene Funktion wurde noch nicht implementiert. Versuchen Sie es zu einem späteren Zeitpunkt.",
                MessageBoxButton.OK, InstructionIcon.Information);

            return dialogResult;
        }

        public static DialogResultsEx LastChangedNotSave()
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.Show(APPLICATIONNAME, "Die letzten Änderung wurde nicht gespeichert. Trotzdem ohne Speichern schließen?", "Die eingegebene Änderungen werden nicht gespeichert.", MessageBoxButton.YesNo, InstructionIcon.Question, DialogResultsEx.No);

            return dialogResult;
        }

        public static DialogResultsEx DeleteItem(string value)
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.Show(APPLICATIONNAME, $"Wollen Sie den Eintrag '{value}' löschen?", string.Empty, MessageBoxButton.YesNo, InstructionIcon.Question, DialogResultsEx.No);

            return dialogResult;
        }

        public static DialogResultsEx NoSelectedItem()
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.ShowWithOwner(Application.Current.MainWindow, APPLICATIONNAME,
                "Sie haben kein Eintrag zur Bearbeitung ausgewählt",
                string.Empty,
                MessageBoxButton.OK, InstructionIcon.Information, DialogResultsEx.Ok);

            return dialogResult;
        }

        public static DialogResultsEx CopySelectedItem(string value)
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.Show(APPLICATIONNAME, "Wollen Sie den ausgewählten Eintrag kopieren?", $"Der Eintrag '{value}' wird kopiert und kann danach geändert werden.", MessageBoxButton.YesNo, InstructionIcon.Question, DialogResultsEx.No);

            return dialogResult;
        }

        public static DialogResultsEx ChapterIsFound(string chapter)
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.ShowWithOwner(Application.Current.MainWindow, APPLICATIONNAME,
                $"Die Aufwandsposition '{chapter}' ist bereits vorhanden.",
                "Geben Sie eine andere Aufwandspoistion an.",
                MessageBoxButton.OK, InstructionIcon.Information, DialogResultsEx.Ok);

            return dialogResult;
        }

        public static DialogResultsEx DataNotFound()
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.ShowWithOwner(Application.Current.MainWindow, APPLICATIONNAME,
                $"Keine Daten zum Export vorhanden.",
                "Prüfen Sie ob Sie die richtige Datenbank mit Aufwandsdaten geladen haben.",
                MessageBoxButton.OK, InstructionIcon.Information, DialogResultsEx.Ok);

            return dialogResult;
        }

        public static DialogResultsEx NoMarkedColumns()
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.ShowWithOwner(Application.Current.MainWindow, APPLICATIONNAME,
                $"Es wurden keine Spalten für einen Export gefunden.",
                "Wenden Sie sich an Ihren Administrator. Es kann eine Fehler im Programm vorliegen.",
                MessageBoxButton.OK, InstructionIcon.Information, DialogResultsEx.Ok);

            return dialogResult;
        }

        public static DialogResultsEx NoMarkedRows()
        {
            DialogResultsEx dialogResult = DialogResultsEx.None;

            dialogResult = MessageBoxEx.ShowWithOwner(Application.Current.MainWindow, APPLICATIONNAME,
                $"Es wurden keine Datenzeilen für einen Export gefunden.",
                "Entweder es wurden noch keine Daten eingeben, der Filter passt nicht oder es wurden keine Zeilen selektiert.",
                MessageBoxButton.OK, InstructionIcon.Information, DialogResultsEx.Ok);

            return dialogResult;
        }

        private static void CleanUp()
        {
            Mouse.OverrideCursor = null;
        }
    }
}
