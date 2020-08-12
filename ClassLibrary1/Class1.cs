using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //界面交互的doc
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            //实际内容的doc
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //[1]创建收集器 -- 收集文档中element的数据
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            //如果想要某种元素，比如墙，就需要过滤的操作
            //一般有两种 ofCategory, ofClass

            //[2]过滤，获取墙元素
            //[2-1]快速过滤--- lookup->built-in category
            collector.OfCategory(BuiltInCategory.OST_Walls).OfClass(typeof(Wall));

            //[2-2]通用过滤方法
            //ElementCategoryFilter elementCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            //ElementClassFilter elementClassFilter = new ElementClassFilter(typeof(Wall));
            //collector.WherePasses(elementCategoryFilter).WherePasses(elementClassFilter);

            //[3]某种墙族类型下族实例的获取
            //[3-1] foreach的获取方式
            List<Element> elementList = new List<Element>(); 

            foreach (var item in collector)
            {
                if (item.Name == "CL_W1")
                {
                    elementList.Add(item);
                }
            }
            
            //[6]高亮显示实例
            var sel = uiDoc.Selection.GetElementIds();
            foreach (var item in collector)
            {
                //TaskDialog.Show("查看结果", item.Name);
                sel.Add(item.Id);
            }

            uiDoc.Selection.SetElementIds(sel);
            return Result.Succeeded;

        }
    }
}
