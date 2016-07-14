using Daimali.ISV.Response;
using System.Collections.Generic;
namespace Daimali.ISV.Request
{
    public interface IRequest<T> where T: APIResponse
    {
        //string APP_Key { get; set; }
        //string APP_Secrett { get; set; }
        //string Sign { get; set; }
        //string Timestamp { get; set; }
        //string Version { get; set; }
        //string Token { get; set; }

        /// <summary>
        /// 获取API地址。
        /// </summary>
        /// <returns>API地址</returns>
        string GetApiURL();

        /// <summary>
        /// 获取所有的Key-Value形式的文本请求参数字典。其中：
        /// Key: 请求参数名
        /// Value: 请求参数文本值
        /// </summary>
        /// <returns>文本请求参数字典</returns>
        IDictionary<string, string> GetParameters();

        /// <summary>
        /// 提前验证参数。
        /// </summary>
        void Validate();
    }
}