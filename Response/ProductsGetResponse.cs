
using Daimali.ISV.Domain;
using System.Collections.Generic;

namespace Daimali.ISV.Response
{
    public class ProductsGetResponse:APIResponse
    {
        public List<ProductsGetDomain> Domains { get; set; }
    }
}