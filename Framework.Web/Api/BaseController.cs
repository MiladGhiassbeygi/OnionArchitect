using AutoMapper;
using Framework.Core.Helpers;
using Framework.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Web.Api
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[Controller]")]
    public class BaseController : ControllerBase
    {
        protected readonly PagingMetadataHelper pagingMetadataHelper;

        public BaseController(PagingMetadataHelper pagingMetadataHelper)
        {
            this.pagingMetadataHelper = pagingMetadataHelper;
        }

        
    }
}

