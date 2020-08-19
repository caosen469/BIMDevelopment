using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //收集器构造
            //FilteredElementCollector collector = new FilteredElementCollector(RevitDoc);
            
            //FilteredElementCollector collector2 = new FilteredElementCollector(RevitDoc, ICollection<ElementId> elementIds); //文档+ElememtID集合构造

            //FilteredElementCollector collector3 =  new FilteredElementCollector(RevitDoc, ElementId viewId); //文档+视图构造

            // collector + filter => collector.WherePasses(filter)

            FilteredElementCollector collection = new FilteredElementCollector(RevitDoc);
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StackedWalls);
            collection.OfClass(typeof(Wall)).WherePasses(filter);
            ICollection<ElementId> foundIds = collection.ToElementIds();

            //
            FilteredElementCollector collector3 = new FilteredElementCollector(RevitDoc);

            //查询并遍历文档中所有的Level
            collector3.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Levels))
                .WhereElementIsNotElementType();
            foreach(Level level in collector3)
            {
                TaskDialog.Show("Level Name", level.Name);
            }

            // Linq方法
            FilteredElementCollector collector4 = new FilteredElementCollector(RevitDoc);

            //首先使用一个内建的过滤器来减少后面使用LINQ查询的元素数量
            collector4.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Levels));


            //LINQ查询：找到名字为"Level 1"的标高
            var levelElements = from element in collector4
                where element.Name == "Level 1"
                select element;
            List<Autodesk.Revit.DB.Element> levels = levelElements.ToList<Autodesk.Revit.DB.Element>();

            ElementId level1Id = levels[0].Id;

            //-------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------

            ///<summary>
            ///使用ElementCategoryFIlter过滤元素
            ///</summary>
            void TestElementCategory



            return Result.Succeeded;
        }
    }
}
