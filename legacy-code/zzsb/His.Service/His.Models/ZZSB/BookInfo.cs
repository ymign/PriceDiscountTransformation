using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.ZZSB
{
  public  class BookInfo
    {
      public string ClinicCode { get; set; }
      public string Source{ get; set; }
      public string SchemaNo { get; set; }
      public string SeeFlag { get; set; }

      public string  DoctCode { get; set; }
      public string DoctName { get; set; }
      public string DeptCode { get; set; }
      public string DeptName { get; set; }

      public string LevelCode { get; set; }
      public string BeginTime { get; set; }
      public string SeeDate { get; set; }

      public bool IsBook { get; set; }
      public string OperDate { get; set; }
      public string EndTime { get; set; }
      //public string SeeFlag { get; set; }
    }
}
