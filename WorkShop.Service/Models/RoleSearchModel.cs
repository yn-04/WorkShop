using System.Collections.Generic;

namespace WorkShop.Service.Models
{
    public class RoleSearchModel : SearchBaseModel
    {
        public List<RoleViewModel> Results { get; set; } = new List<RoleViewModel>();
    }
}