
namespace Daimali.ISV.Domain
{
    /// <summary>
    /// 产品列表
    /// </summary>
    public class ProductsGetDomain : APIDomain
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }
    }
}