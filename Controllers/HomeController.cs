
using Daimali.ISV.Request;
using Daimali.ISV.Response;
using System.Collections.Generic;

namespace Daimali.ISV.Controllers
{
    /// <summary>
    /// api接口方法类
    /// </summary>
    public class HomeController : APIController
    {

        /*
         * 下面三个属性都继承自APIController，根据对应操作灵活调用进行相关权限判断，需要登录才能操作时就进行用户授权验证，不需要时就只需要应用和签名验证
         * if (!APPAuthorization) return APP_AUTH_ERROR;//应用认证
         * if (!SignSuccess) return SIGN_ERROR;         //签名验证
         * if (!UserAuthorization) return USER_AUTH_ERROR;//用户授权验证
         */

        public APIResponse GetProducts(ProductsGetRequest request)
        {
            if (!APPAuthorization) return APP_AUTH_ERROR;//应用认证
            if (!SignSuccess) return SIGN_ERROR;//签名验证
            ProductsGetResponse response = new ProductsGetResponse();
            response.Domains = new List<Domain.ProductsGetDomain>();
            return response;
        }

        public APIResponse SaveProduct()
        {
            if (!APPAuthorization) return APP_AUTH_ERROR;//应用认证
            if (!SignSuccess) return SIGN_ERROR;//签名验证
            if (!UserAuthorization) return USER_AUTH_ERROR;//用户授权验证

            return new APIResponse();
        }
    }
}
