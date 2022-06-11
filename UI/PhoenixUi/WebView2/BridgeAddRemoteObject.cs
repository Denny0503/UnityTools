using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixUi.WebView2
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class BridgeAddRemoteObject
    {
        public Func<string> ApiHeader_Action;
        public Func<string, bool> UpdateHY_Action;
        public Func<string, bool> UpdateEvaluationResult_Action;
        public Action<object, string> ScanScale_Action;
        public Action AddScale_Action;
        public Action<bool, string> GoBack_Action;
        public Action<string, string, string> ExportFOGExcel_Action;
        public Action<int> PrintFogOrSportReport_Action;
        public Func<string, bool> Save_Action;
        public Func<string, bool> SaveGroupReport_Action;
        public Action<bool> EditVisibility_Action;

        public BridgeAddRemoteObject() { }

        public string initApiHeader() { return ApiHeader_Action?.Invoke(); }

        public bool updateHY(string param) { return UpdateHY_Action?.Invoke(param) ?? false; }

        public bool updateEvaluationResult(string param)
        {
            return UpdateEvaluationResult_Action?.Invoke(param) ?? false;
        }

        public void scanpatientscale(object param, string index) { ScanScale_Action?.Invoke(param, index); }

        public void addpatientscale() { AddScale_Action?.Invoke(); }

        public void goBack(bool isEdit, string url) { GoBack_Action?.Invoke(isEdit, url); }

        public void exportFOGExcel(string patientInfo, string patientGaitModel, string FreezeIndex)
        {
            ExportFOGExcel_Action?.Invoke(patientInfo, patientGaitModel, FreezeIndex);
        }

        public void printFogOrSportReport(int isfog)
        {
            PrintFogOrSportReport_Action?.Invoke(isfog);
        }

        public bool save(string patientGaitModel)
        {
            return Save_Action?.Invoke(patientGaitModel) ?? false;
        }

        public bool saveGroupReport(string patientGaitModel)
        {
            return SaveGroupReport_Action?.Invoke(patientGaitModel) ?? false;
        }

        public void editVisibility(bool visibility)
        {
            EditVisibility_Action?.Invoke(!visibility);
        }
    }
}
