﻿using Microsoft.AspNetCore.Http;

namespace Api.Gateway
{
    public interface IResponse
    {
        void Content(HttpContext content);
    }
}