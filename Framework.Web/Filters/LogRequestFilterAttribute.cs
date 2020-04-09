using Framework.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Web.Filters
{
    public sealed class LogRequestAttribute : TypeFilterAttribute
    {
        public LogRequestAttribute()
            : base(typeof(LogRequestFilterAttribute))
        {
        }

        private sealed class LogRequestFilterAttribute : ActionFilterAttribute
        {
            private readonly IDiagnosticContext _diagnosticContext;
            private readonly IWebHelper _webHelper;

            public LogRequestFilterAttribute(IDiagnosticContext diagnosticContext, IWebHelper webHelper)
            {
                _diagnosticContext = diagnosticContext ?? throw new ArgumentNullException(nameof(diagnosticContext));
                _webHelper = webHelper ?? throw new ArgumentNullException(nameof(webHelper));
            }

            public override void OnActionExecuting(ActionExecutingContext context)
            {
                var keyCountInput = context.HttpContext.Request.Headers["X-Key"].Count;
                var identityName = context.HttpContext.User.Identity.Name;
                if (keyCountInput != 0 || identityName != null || context.HttpContext.Response.StatusCode == 404)
                {
                    #region Commented
                    //_diagnosticContext.Set("User.Identity.Name", context.HttpContext.User.Identity.Name, true);
                    //_diagnosticContext.Set("User.Identity.IsAuthenticated", context.HttpContext.User.Identity.IsAuthenticated, true);
                    //_diagnosticContext.Set("User.Identity.Claims", context.HttpContext.User.Claims, true);
                    //_diagnosticContext.Set("User.Identity.AuthenticationType", context.HttpContext.User.Identity.AuthenticationType, true);
                    //_diagnosticContext.Set("Request.Host", context.HttpContext.Request.Host, false);
                    //_diagnosticContext.Set("Request.Query", context.HttpContext.Request.Query, true);
                    //_diagnosticContext.Set("Response.Headers", context.HttpContext.Response.Headers, true);
                    //_diagnosticContext.Set("Response.Cookies", context.HttpContext.Response.Cookies, true);
                    //_diagnosticContext.Set("Request.Cookies", context.HttpContext.Request.Cookies, true);
                    //_diagnosticContext.Set("Request.Headers", context.HttpContext.Request.Headers, true);
                    #endregion

                    _diagnosticContext.Set(nameof(LogRequestAttribute), null);
                    _diagnosticContext.Set("ModelState.IsValid", context.ModelState.IsValid);
                    var errors = context.ModelState.ToDictionary(p => p.Key, p => p.Value.Errors.Select(q => q.ErrorMessage));
                    _diagnosticContext.Set("ModelState.Errors", errors);
                    _diagnosticContext.Set("ActionArguments", context.ActionArguments, true);
                    _diagnosticContext.Set("Request.QueryString", context.HttpContext.Request.QueryString, true);
                    Microsoft.Extensions.Primitives.StringValues userAgent;
                    context.HttpContext.Request.Headers.TryGetValue("User-Agent", out userAgent);
                    _diagnosticContext.Set("Request.UserAgent", userAgent.FirstOrDefault(), true);
                    _diagnosticContext.Set("Request.RouteValues", context.HttpContext.GetRouteData()?.Values, true);
                    _diagnosticContext.Set("IpAddress", _webHelper.GetCurrentIpAddress());
                    _diagnosticContext.Set("UrlReferrer", _webHelper.GetUrlReferrer());
                    var keyCount = context.HttpContext.Request.Headers["X-Key"].Count;
                    if (keyCount == 0)
                        _diagnosticContext.Set("UserKey", context.HttpContext.User.Identity.Name, true);
                    else
                        _diagnosticContext.Set("UserKey", context.HttpContext.Request.Headers["X-Key"].FirstOrDefault(), true);
                    //_diagnosticContext.Set("RawUrl", _webHelper.GetRawUrl());

                    base.OnActionExecuting(context);
                }
            }
        }
    }
}
