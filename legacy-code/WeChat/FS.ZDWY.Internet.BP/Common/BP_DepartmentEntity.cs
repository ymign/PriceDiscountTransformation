using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using FS.ZDWY.Internet.BL;
using FS.ZDWY.Internet.Models;

namespace FS.ZDWY.Internet.BP
{
    public class BP_DepartmentEntity
    {
        public List<DepartmentEntity> QueryDepartments(string deptCode, string rank)
        {
            DepartmentEntityLogic dept = new DepartmentEntityLogic();
            List<DepartmentEntity> deptData = dept.DepartmentDataContains(deptCode, rank);
            return deptData;
        }
        public List<DepartmentEntity> QueryDepartmentAll(string rank)
        {
            DepartmentEntityLogic dept = new DepartmentEntityLogic();
            List<DepartmentEntity> deptAll = dept.DepartmentData(rank);
            return deptAll;
        }
    }
}

