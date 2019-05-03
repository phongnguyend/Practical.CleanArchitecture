using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ClassifiedAds.WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using ClassifiedAds.DomainServices;
using IdentityModel.Client;
using System.Net.Http;

namespace ClassifiedAds.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IProductService productService, IHttpClientFactory httpClientFactory)
        {
            _productService = productService;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var metaDataResponse = await httpClient.GetDiscoveryDocumentAsync("https://localhost:44367/");
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var response = await httpClient.GetUserInfoAsync(new UserInfoRequest { Address = metaDataResponse.UserInfoEndpoint, Token = accessToken });

            var products = _productService.GetProducts().ToList();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> AuthorizedAction()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            var claims = User.Claims.Select(x => new { x.Type, x.Value });
            return Json(claims);
        }

        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        [Authorize]
        public async Task<IActionResult> UserInfoClient()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var metaDataResponse = await httpClient.GetDiscoveryDocumentAsync("https://localhost:44367/");
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var response = await httpClient.GetUserInfoAsync(new UserInfoRequest { Address = metaDataResponse.UserInfoEndpoint, Token = accessToken });

            if (response.IsError)
            {
                throw new Exception("Problem accessing the UserInfo endpoint.", response.Exception);
            }

            return Json(response.Claims);
        }
    }
}
