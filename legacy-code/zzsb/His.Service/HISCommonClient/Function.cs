using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HISCommonClient
{
  public  class Function
    {
      public static void EntityToEntity<T>(T pTargetObjSrc, T pTargetObjDest)
      {
          try
          {
              foreach (var mItem in typeof(T).GetProperties())
              {
                  mItem.SetValue(pTargetObjDest, mItem.GetValue(pTargetObjSrc, new object[] { }), null);
              }
          }
          catch (NullReferenceException NullEx)
          {
             // throw NullEx;
          }
          catch (Exception Ex)
          {
            //  throw Ex;
          }
      }
    }
}
 
