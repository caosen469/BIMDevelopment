using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ClassLibrary1
{
    class Class3: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document RevitDoc = commandData.Application.ActiveUIDocument.Document;

            //创建模型元素
            //RevitDoc.Create.NewFamilyInstance();

            return Result.Succeeded;
        }
    }
}
