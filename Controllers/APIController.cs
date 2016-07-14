using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using Daimali.ISV.Response;

namespace Daimali.ISV.Controllers
{
    // REST POST（新增数据），GET（取得数据），PUT（更新数据），DELETE（删除数据）
    /// <summary>
    /// 签名验证类
    /// 所有Controller都继承自这个控制器
    /// </summary>
    public class APIController : ApiController
    {
        #region 属性
        protected int user_id = -1;
        protected int application_type = -1;
        protected string app_key = "";
        protected string app_secret = "";
        //签名
        protected string sign = "";
        protected string timestamp = "";//统一格式yyyyMMddHHmmss
        protected string version = "";
        protected string token = "";
        protected int token_time = 60 * 24 * 30;//token有效期30天
        #endregion

        #region Response
        public APIResponse NULL = new APIResponse() { Code = "100000", Message = "NULL" };
        public APIResponse SIGN_ERROR = new APIResponse() { Code = "100001", Message = "SIGN_ERROR" };
        public APIResponse APP_AUTH_ERROR = new APIResponse() { Code = "100002", Message = "APP_AUTH_ERROR" };
        public APIResponse USER_AUTH_ERROR = new APIResponse() { Code = "100003", Message = "USER_AUTH_ERROR" };
         
        #endregion

        /// <summary>
        /// 签名是否验证成功
        /// </summary>
        public bool SignSuccess
        {
            get
            {
                if (CheckPramers())
                {
                    string method = HttpContext.Current.Request.HttpMethod;
                    System.Collections.Specialized.NameValueCollection form = HttpContext.Current.Request.QueryString;
                    switch (method)
                    {
                        case "POST":
                            form = HttpContext.Current.Request.Form;
                            break;
                        case "GET":
                            form = HttpContext.Current.Request.QueryString;
                            break;
                        default:
                            return false;
                    }
                    IDictionary<string, string> parameters = new Dictionary<string, string>();
                    for (int f = 0; f < form.Count; f++)
                    {
                        string key = form.Keys[f];
                        if (key.ToLower() == "sign") continue;
                        parameters.Add(key, form[key]);
                    }

                    return SignRequest(parameters, app_secret, false) == sign;//将服务端sign和客户端传过来的sign进行比较
                    /*
                     * 上面签名验签使用的是MD5方式，如果要使用RSA的方式用下面这个
                     * 上面使用的是MD5加密，如果使用RSA的方式如下
                    */
                    //SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                    //for (int f = 0; f < form.Count; f++)
                    //{
                    //    string key = form.Keys[f];
                    //    if (key.ToLower() == "sign") continue;
                    //    sParaTemp.Add(key, form[key]);
                    //}
                    //return RSAFromPkcs8.verify(sParaTemp.ToString(),sign,"存在服务端公钥","utf-8");
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 应用认证，匹配app_key+app_secret
        /// </summary>
        public bool APPAuthorization
        {
            get
            {
                //查询数据库中是否存在，app_key对应的app_secret  如无，则返回应用授权失败
                //检测是否存在此版本的应用
                if (string.IsNullOrEmpty(app_key)) return false;
                //Application app= CoreService.Instance.GetAppSecret(app_key,version);
                //if(app==null)
                //{
                //    return false;
                //}
                //application_type = app.Type;
                //app_secret = app.App_Secret;
                return true;
            }
        }

        /// <summary>
        /// 用户授权认证，匹配app_key+token
        /// </summary>
        public bool UserAuthorization
        {
            get
            {
                //查询数据库中是否有匹配的，app_key+token  如无，则返回授权失败
                //bool result = CoreService.Instance.ExistsUserToken(app_key, token, token_time,ref user_id);
                //if (result)
                //{
                //    //判断用户是否被禁用
                //    Rong.Framework.Entity.Core.User user = CoreService.Instance.GetUser(user_id);
                //    if (user == null)
                //    {
                //        USER_AUTH_ERROR = new APIResponse() { Code = "100003", Message = "USER_AUTH_ERROR", SubMessage = "用户不存在" };
                //        return false;
                //    }
                //    if (user.UserStatusInt == 0)
                //    {
                //        USER_AUTH_ERROR = new APIResponse() { Code = "100003", Message = "USER_AUTH_ERROR", SubCode = "200002", SubMessage = "您的账户已经被禁用，请联系管理员" };
                //        return false;
                //    }
                //}
                //return result;
                return true;
            }
        }

        public APIController()
        {
            app_key =HttpContext.Current.Request["app_key"];// RequestHelper.GetString("app_key");
            //app_secret = RequestHelper.GetString("app_secret");   //app_secret仅作加密使用，不在提交中使用
            sign = HttpContext.Current.Request["sign"];//RequestHelper.GetString("sign");
            timestamp = HttpContext.Current.Request["timestamp"];//RequestHelper.GetString("timestamp");
            version = HttpContext.Current.Request["version"];//RequestHelper.GetString("version");
            token = HttpContext.Current.Request["token"]; //RequestHelper.GetString("token");     
        }

        /// <summary>
        /// 给请求签名。
        /// </summary>
        /// <param name="parameters">所有字符型的请求参数</param>
        /// <param name="secret">签名密钥</param>
        /// <param name="qhs">是否前后都加密钥进行签名</param>
        /// <returns>签名</returns>
        string SignRequest(IDictionary<string, string> parameters, string secret, bool qhs)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parameters);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder(secret);
            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key))// && !string.IsNullOrEmpty(value) 空值也加入计算！！
                {
                    query.Append(key).Append(value);
                }
            }
            if (qhs)
            {
                query.Append(secret);
            }

            // 第三步：使用MD5加密
            MD5 md5 = MD5.Create();
            byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query.ToString()));

            // 第四步：把二进制转化为大写的十六进制
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            return result.ToString();
        }
        /// <summary>
        /// 检测敏感参数（安全机制）
        /// </summary>
        /// <returns></returns>
        bool CheckPramers()
        {
            //一：检测时间戳
            if (!string.IsNullOrEmpty(timestamp) && timestamp.Length == 14)
            {
                DateTime dtimestamp;
                try
                {
                    dtimestamp = DateTime.ParseExact(timestamp, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                    SIGN_ERROR = new APIResponse() { Code = "100001", Message = "SIGN_ERROR", SubMessage = "时间戳格式不正确" };
                    return false;
                }

                //判断签名是否已过期
                if (dtimestamp < DateTime.Now.AddSeconds(-15))//请求有效时间15秒
                {
                    SIGN_ERROR = new APIResponse() { Code = "100001", Message = "SIGN_ERROR", SubMessage = "当前请求已过期" };
                    return false;
                }
            }
            else
            {
                SIGN_ERROR = new APIResponse() { Code = "100001", Message = "SIGN_ERROR", SubMessage = "时间戳格式不正确" };
                return false;
            }
            //二：检测敏感参数
            string secret = HttpContext.Current.Request["app_secret"];//RequestHelper.GetString("app_secret");
            if (!string.IsNullOrEmpty(secret))
            {
                SIGN_ERROR = new APIResponse() { Code = "100001", Message = "SIGN_ERROR", SubMessage = "存在不安全的敏感参数" };
                return false;
            }
            return true;
        }
    }
}
