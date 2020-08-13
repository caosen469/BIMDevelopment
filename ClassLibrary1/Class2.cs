using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    class Class2 : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document RevitDoc = commandData.Application.ActiveUIDocument.Document;

            //[3-12]放置类型为 0762*2032 mm的门
            string doorTypeName = "0762 * 2032 mm";
            FamilySymbol doorType = null;

            //在文档中找到名字为 "0762*2032 mm"的门类型
            ElementFilter doorCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            ElementFilter familySymbolFilter = new ElementClassFilter(typeof(FamilySymbol));
            LogicalAndFilter andFilter = new LogicalAndFilter(doorCategoryFilter, familySymbolFilter);
            FilteredElementCollector doorSymbols = new FilteredElementCollector(RevitDoc);

            doorSymbols = doorSymbols.WherePasses(andFilter);

            bool symbolFound = false;

            foreach (FamilySymbol element in doorSymbols)
            {
                if (element.Name == doorTypeName)
                {
                    symbolFound = true;
                    doorType = element;
                    break;
                }
            }

            //如果没有找到就加载一个族文件
            if (!symbolFound)
            {
                string file = @"C:\ProgramData\Autodesk\RVT2014\Libraries\Chinese_INTL\门\M_单一嵌板4rfa";
                Family family;
                bool loadSuccess = RevitDoc.LoadFamily(file, out family);

                if (loadSuccess)
                {
                    foreach (ElementId doorTypeId in family.GetValidTypes())
                    {
                        doorType = RevitDoc.GetElement(doorTypeId) as FamilySymbol;
                        if (doorType.Name == doorTypeName)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Autodesk.Revit.UI.TaskDialog.Show("Load family failed", "Could not load family file"+"'");

                } 
            }

            //使用族类性创建门
            if (doorType != null)
            {
                //首先找到线性的墙
                ElementFilter wallFilter = new ElementClassFilter(typeof(Wall));
                FilteredElementCollector filteredElements = new FilteredElementCollector(RevitDoc);
                filteredElements = filteredElements.WherePasses(wallFilter);
                Wall wall = null;
                Line line = null;

                foreach (Wall element in filteredElements)
                {
                    LocationCurve locationCurve = element.Location as LocationCurve;
                    if (locationCurve != null)
                    {
                        line = locationCurve.Curve as Line;
                        if (line != null)
                        {
                            wall = element;
                            break;
                        }
                    }
                }



                //再墙的中心位置处创造一个门
                if (wall != null)
                {
                    XYZ midPoint = (line.GetEndPoint(0) + line.GetEndPoint(1)) / 2;
                    Level wallLevel = RevitDoc.GetElement(wall.LevelId) as Level;

                    //创建门；传入标高参数，作为门的默认标高
                    FamilyInstance door = RevitDoc.Create.NewFamilyInstance(midPoint, doorType, wall, wallLevel,
                        Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                    Autodesk.Revit.UI.TaskDialog.Show("Succeed", door.Id.ToString());
                    Trace.WriteLine("Door Created: " + door.Id.ToString());
                }
                else
                {
                    Autodesk.Revit.UI.TaskDialog.Show("元素不存在", "没有找到符合条件的墙");
                }
            }
            else
            {
                Autodesk.Revit.UI.TaskDialog.Show("族类性不存在", "没有找到族类性" + doorTypeName + "");
            }

            return Result.Succeeded;
        }
    }
}
