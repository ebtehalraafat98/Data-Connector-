using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitWithWPF.Model
{
  public class CategoryObj
  {
    public string CategoryName { get; set; }

    public BuiltInCategory Category { get; set; } 

  
    public override string ToString()
    {
      return CategoryName;
    }


  }
}
