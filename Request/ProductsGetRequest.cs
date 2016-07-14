using Daimali.ISV.Response;
using System;
using System.Collections.Generic;

namespace Daimali.ISV.Request
{
    /// <summary>
    /// 请求参数类
    /// </summary>
    public class ProductsGetRequest : IRequest<ProductsGetResponse>
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页显示数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }

        public string GetApiURL()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> GetParameters()
        {
            throw new NotImplementedException();
        }

        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}