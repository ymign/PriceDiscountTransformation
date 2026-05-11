using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace His.Models.Pha
{
   public class RecipeBase
    {
      public string OPSYSTEM { get; set; }
      public string OPWINID { get; set; }
      public string OPTYPE { get; set; }
      public string OPIP { get; set; }
      public string OPMANNO { get; set; }
      public string OPMANNAME { get; set; }
      public List<RecipeStatusInfo> Details { get; set; }
    }

   public class RecipeStatusInfo
   {
       public string RecipeNo { get; set; }
       public string PresDetailID { get; set; }
       public string PrescriptionID { get; set; }
       public string SendStatus { get; set; }
   }

   public class ROOT
   {
        public string RETVAL { get; set; }
      public string RETMSG { get; set; }
      public string RETCODE { get; set; }
   }
}
