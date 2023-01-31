using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using BIM360FileTransfer.Views;

namespace BIM360FileTransfer
{

    [Serializable()]
    [Transaction(TransactionMode.Manual)]
    public class BIM360FileTransferTool : IExternalCommand
    {
        private ExternalCommandData cmdData;
        public static int success;
        public static int failed;
        public static IList<String> failures = new List<String>();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication UIApp = commandData.Application;
            UIApp.DialogBoxShowing += new EventHandler<DialogBoxShowingEventArgs>(OnDialogShowing);
            UIApp.Application.FailuresProcessing += new EventHandler<FailuresProcessingEventArgs>(OnFailuresProcessing);
            cmdData = commandData;

            var instance = new MainWindow(UIApp.Application)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            instance.Show();
            

            UIApp.Application.FailuresProcessing -= OnFailuresProcessing;
            UIApp.DialogBoxShowing -= OnDialogShowing;
            return Result.Succeeded;
        }

        private static void OnFailuresProcessing(object sender, FailuresProcessingEventArgs e)
        {
            FailuresAccessor failuresAccessor = e.GetFailuresAccessor();
            IEnumerable<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
            foreach (FailureMessageAccessor failureMessage in failureMessages)
            {
                if (failureMessage.GetSeverity() == FailureSeverity.Warning)
                {
                    failuresAccessor.DeleteWarning(failureMessage);
                }
            }
            e.SetProcessingResult(FailureProcessingResult.Continue);
        }

        private static void OnDialogShowing(object o, DialogBoxShowingEventArgs e)
        {
            if (e.Cancellable)
            {
                e.Cancel();
            }
            //worry about this later - 1002 = cancel
            if (e.DialogId == "TaskDialog_Unresolved_References")
            {
                e.OverrideResult(1002);
            }
            //Don't sync newly created files. 1003 = close
            if (e.DialogId == "TaskDialog_Local_Changes_Not_Synchronized_With_Central")
            {
                e.OverrideResult(1003);
            }
            if (e.DialogId == "TaskDialog_Save_Changes_To_Local_File")
            {
                //Relinquish unmodified elements and worksets
                e.OverrideResult(1001);
            }
        }
    }
}
