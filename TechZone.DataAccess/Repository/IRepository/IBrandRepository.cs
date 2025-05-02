using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechZone.Models.Models;
using TechZone.DataAccess.Data;

namespace TechZone.DataAccess.Repository.IRepository
{
    public interface IBrandRepository:IRepository<Brand>
    {
        void Update(Brand obj);
       
    }
}
