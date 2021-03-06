﻿using System;
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

            //[3-2]转为list处理
            List<Element> elementList2 = collector.ToList<Element>();

            //[3-3]linq表达式
            var wallElement = from element in collector where element.Name == "CL_W1" select element;
            //Element wallInstance = wallElement.LastOrDefault<Element>();

            //[4]某个族实例的获取
            //[4-1]确定只有一个实例
            //[4-1-1]list获取
            //Element wallInstance = elementList[0];

            //[4-1-2]IEnumberable获取
            //虽然wallElement是一个IEnumberable对象，但是不能直接wallElement[0]
            Element wallInstance = wallElement.FirstOrDefault<Element>();

            //[4-1-3]lambda表达式的写法
            Element ele = collector.OfCategory(BuiltInCategory.OST_Walls).OfClass(typeof(Wall))
                .FirstOrDefault<Element>(x => x.Name == "CL_W1");

            //[4-2]有多个实例，但是只想获取其中一个，可以使用ElementId,或者根据一些特征
            Element wallInstance2 = doc.GetElement(new ElementId(237473));
            
            //[5]类型判断与转换

            foreach (var item in collector)
            {
                if (item is Wall)
                {
                    Wall wallInstance3 = item as Wall;
                    //Wall wallInstance3 = (Wall)item;
                }
                else if ((item is WallType))
                {
                    WallType wallInstance3 = item as WallType;
                }
            }


            //[6]高亮显示实例
            var sel = uiDoc.Selection.GetElementIds();
            foreach (var item in elementList)
            {
                //TaskDialog.Show("查看结果", item.Name);
                sel.Add(item.Id);
            }

            //[7]找到族Familiy
            var famType = new FilteredElementCollector(doc).OfClass(typeof(Family)).FirstOrDefault(x=>x.Name == "FamilyName");

            uiDoc.Selection.SetElementIds(sel);
            return Result.Succeeded;

        }
    }
}
