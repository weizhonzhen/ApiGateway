﻿using Microsoft.AspNetCore.Mvc;
using FastData.Core;
using FastData.Core.Context;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using FastUntility.Core.Page;
using FastApiGatewayDb.Ui.Models;
using FastApiGatewayDb.DataModel;
using FastUntility.Core.Base;

namespace FastApiGatewayDb.Ui.Controllers
{
    public class HomeController : Controller
    {
        #region 加载接口
        /// <summary>
        /// 加载接口
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewData.Model = new QueryPageUrlModel();
            return View();
        }
        #endregion

        #region 下游分页
        /// <summary>
        /// 下游分页
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DownList(QueryPageUrlModel item)
        {
            using (var db = new DataContext(App.DbKey.Api))
            {
                var page = new PageModel();
                page.PageSize = item.PageSize == 0 ? 10 : item.PageSize;
                page.PageId = item.PageId == 0 ? 1 : item.PageId;
                var info = new PageResult();

                if (!string.IsNullOrEmpty(item.Key))
                {
                    var param = new List<OracleParameter>();
                    param.Add(new OracleParameter { ParameterName = "Key", Value = item.Key.ToUpper() });
                    info = FastMap.QueryPage(page, "Api.DownUrl", param.ToArray(),db);
                }
                else
                    info.list = new List<Dictionary<string, object>>();

                //是否显示下游表单
                if (item.Success == "1" && info.list.Count > 0)
                {
                    info.list.ForEach(a => a.Add("IsShowForm", item.Success));
                }

                return PartialView("DownList", info);
            }
        }
        #endregion

        #region url分页
        /// <summary>
        /// url分页
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UrlList(QueryPageUrlModel item)
        {
            using (var db = new DataContext(App.DbKey.Api))
            {
                var page = new PageModel();
                page.PageSize = item.PageSize == 0 ? 10 : item.PageSize;
                page.PageId = item.PageId == 0 ? 1 : item.PageId;

                var param = new List<OracleParameter>();
                param.Add(new OracleParameter { ParameterName = "Key", Value = item.Key });
                var info = FastMap.QueryPage(page, "Api.Url", param.ToArray(),db);

                return PartialView("UrlList", info);
            }
        }
        #endregion

        #region 加载接口
        /// <summary>
        /// 加载接口
        /// </summary>
        /// <returns></returns>
        public IActionResult Option(string key)
        {
            var model = new UrlModel();

            if(!string.IsNullOrEmpty(key))
            {
                var item = FastRead.Query<ApiGatewayUrl>(a => a.Key.ToLower() == key.ToLower(), null, App.DbKey.Api).ToItem<ApiGatewayUrl>();
                model.IsAnonymous = item.IsAnonymous;
                model.CacheTimeOut = item.CacheTimeOut;
                model.IsCache = item.IsCache;
                model.IsDbLog = item.IsDbLog;
                model.IsGetToken = item.IsGetToken;
                model.IsTxtLog = item.IsTxtLog;
                model.Key = item.Key;
                model.Schema = item.Schema;
                model.Name = item.Name;
            }

            ViewData.Model = model;
            return View();
        }
        #endregion

        #region 下游接口
        /// <summary>
        /// 下游接口
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DownForm(string key,string orderBy)
        {
            var model = new DownUrlModel();

            if (!string.IsNullOrEmpty(key))
            {
                var item = FastRead.Query<ApiGatewayDownParam>(a => a.Key.ToLower() == key.ToLower()&&a.OrderBy==orderBy.ToInt(1), null, App.DbKey.Api).ToItem<ApiGatewayDownParam>();
             
                model.Name = item.Name;
                model.IsBody = item.IsBody;
                model.IsDecode = item.IsDecode;
                model.IsResult = item.IsResult;
                model.Key = item.Key;
                model.Method = item.Method;
                model.Name = item.Name;
                model.Protocol = item.Protocol;
                model.SoapMethod = item.SoapMethod;
                model.SoapParamName = item.SoapParamName;
                model.SourceParam = item.SourceParam;
                model.Url = item.Url;
                model.WaitHour = item.WaitHour;
            }

            return PartialView("OptionLeaf", model);
        }
        #endregion

        #region 下游接口操作
        /// <summary>
        /// 下游接口操作
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DownUrlOption(DownUrlModel item)
        {
            using (var db = new DataContext(App.DbKey.Api))
            {
                var success = true;
                var model = new ApiGatewayDownParam();
                model.Key = item.Key;
                model.IsBody = item.IsBody;
                model.IsDecode = item.IsDecode;
                model.IsResult = item.IsResult;
                model.Method = item.Method;
                model.Name = item.Name;
                model.OrderBy = item.OrderBy;
                model.Protocol = item.Protocol;
                model.SoapMethod = item.SoapMethod;
                model.SoapParamName = item.SoapParamName;
                model.SourceParam = item.SourceParam;
                model.Url = item.Url;
                model.WaitHour = item.WaitHour;

                if (FastRead.Query<ApiGatewayDownParam>(a => a.Key.ToLower() == item.Key.ToLower() && a.OrderBy == item.OrderBy).ToCount(db) > 0)
                    success = db.Update<ApiGatewayDownParam>(model, a => a.Key.ToLower() == a.Key.ToLower() && a.OrderBy == item.OrderBy).writeReturn.IsSuccess;
                else
                    success = db.Add(model).writeReturn.IsSuccess;

                if (success)
                    return Json(new { success = true, msg = "操作成功" });
                else
                    return Json(new { success = false, msg = "操作失败" });
            }
        }
        #endregion

        #region 接口操作
        /// <summary>
        /// 接口操作
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UrlOption(UrlModel item)
        {
            using (var db = new DataContext(App.DbKey.Api))
            {
                var success = true;
                var model = new ApiGatewayUrl();

                model.CacheTimeOut = item.CacheTimeOut;
                model.IsAnonymous = item.IsAnonymous;
                model.IsCache = item.IsCache;
                model.IsDbLog = item.IsDbLog;
                model.IsGetToken = item.IsGetToken;
                model.IsTxtLog = item.IsTxtLog;
                model.Key = item.Key;
                model.Name = item.Name;
                model.Schema = item.Schema;
                
                if (FastRead.Query<ApiGatewayUrl>(a => a.Key.ToLower() == item.Key.ToLower()).ToCount(db) > 0)
                    success = db.Update<ApiGatewayUrl>(model, a => a.Key.ToLower() == a.Key.ToLower()).writeReturn.IsSuccess;
                else
                    success = db.Add(model).writeReturn.IsSuccess;

                if (success)
                    return Json(new { success = true, msg = "操作成功" });
                else
                    return Json(new { success = false, msg = "操作失败" });
            }
        }

        #endregion

        #region 接口删除
        /// <summary>
        /// 接口删除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DelUrl(QueryPageUrlModel item)
        {
            using (var db = new DataContext(App.DbKey.Api))
            {
                db.BeginTrans();
                var isSuccess = db.Delete<ApiGatewayUrl>(a => a.Key == item.Key).writeReturn.IsSuccess;

                if (isSuccess)
                    isSuccess = db.Delete<ApiGatewayDownParam>(a => a.Key == item.Key).writeReturn.IsSuccess;

                if (isSuccess)
                    isSuccess = db.Delete<ApiGatewayCache>(a => a.Key == item.Key).writeReturn.IsSuccess;

                if (isSuccess)
                    isSuccess = db.Delete<ApiGatewayLog>(a => a.Key == item.Key).writeReturn.IsSuccess;

                if (isSuccess)
                    isSuccess = db.Delete<ApiGatewayWait>(a => a.Key == item.Key).writeReturn.IsSuccess;

                if (isSuccess)
                {
                    db.SubmitTrans();
                    return Json(new { success = isSuccess, msg = "删除成功" });
                }
                else
                {
                    db.RollbackTrans();
                    return Json(new { success = isSuccess, msg = "删除失败" });
                }
            }
        }
        #endregion
    }
}
