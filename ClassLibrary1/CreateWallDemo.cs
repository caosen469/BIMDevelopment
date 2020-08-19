using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    public class CreateWallDemo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // [1] 获取当前文档
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // [2] 获取 CW 102-50-100p 类型的墙
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            Element ele = collector.OfCategory(BuiltInCategory.OST_Walls).OfClass(typeof(WallType)).FirstOrDefault(x => x.Name == "CW 102-50-100P");

            // [3] class transfer
            WallType wallType = ele as WallType;

            // [4] obtain the level
            Level level = new FilteredElementCollector(doc).OfClass(typeof(Level)).FirstOrDefault(x => x.Name == "标高 1") as Level;

            // [5] create control line
            XYZ start = new XYZ(0, 0, 0);
            XYZ end = new XYZ(10, 10, 0);
            Line geomline = Line.CreateBound(start, end);

            // [6] Assign the height of the wall
            double height = 15 / 0.3048; // inch to meters
            double offset = 0;

            // [7] Using Transaction to Create the wall
            // Command that will edit the document should be included in a transcation
            Transaction trans = new Transaction(doc, "Create the wall");

            trans.Start();
            Wall wall = Wall.Create(doc, geomline, wallType.Id, level.Id, height, offset, false, false);
            trans.Commit();

            return Result.Succeeded;
        }
    }
}
