using ClassifiedAds.Application;
using ClassifiedAds.Application.Products.Queries;
using ClassifiedAds.Application.Products.Services;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Entities;
using ClassifiedAds.WebMVC.ConfigurationOptions;
using ClassifiedAds.WebMVC.Models;
using ClassifiedAds.WebMVC.Models.Home;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClassifiedAds.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Dispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;

        public HomeController(IProductService productService, IHttpClientFactory httpClientFactory, Dispatcher dispatcher, ILogger<HomeController> logger, IOptionsSnapshot<AppSettings> appSettings)
        {
            _productService = productService;
            _httpClientFactory = httpClientFactory;
            _dispatcher = dispatcher;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Getting all products");

            _logger.LogDebug("Test LogDebug");
            _logger.LogInformation("Test LogInformation");
            _logger.LogWarning("Test LogWarning");

            var products1 = _dispatcher.Dispatch(new GetProductsQuery());
            var products2 = _productService.Get().ToList();

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            if (User.Identity.IsAuthenticated && !string.IsNullOrEmpty(accessToken))
            {
                var httpClient = _httpClientFactory.CreateClient();

                httpClient.SetBearerToken(accessToken);
                var response = await httpClient.GetAsync($"{_appSettings.ResourceServer.Endpoint}/api/products");
                var product3 = await response.Content.ReadAs<List<Product>>();
            }

            return View();
        }

        public IActionResult Privacy()
        {
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
            var model = new AuthenticationModel
            {
                User = new CurrentUserModel
                {
                    Identity = new CurrentUserIdentityModel
                    {
                        IsAuthenticated = User.Identity.IsAuthenticated,
                        Name = User.Identity.Name,
                        AuthenticationType = User.Identity.AuthenticationType,
                    },
                    Claims = User.Claims.Select(x => new ClaimModel { Type = x.Type, Value = x.Value }).ToList(),
                },
                Token = new TokenModel
                {
                    IdentityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken),
                    AccessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken),
                    RefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken),
                    ExpiresAt = await HttpContext.GetTokenAsync("expires_at"),
                },
            };

            var httpClient = _httpClientFactory.CreateClient();
            var metaDataResponse = await httpClient.GetDiscoveryDocumentAsync(_appSettings.OpenIdConnect.Authority);
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await httpClient.GetUserInfoAsync(new UserInfoRequest { Address = metaDataResponse.UserInfoEndpoint, Token = accessToken });

            if (!response.IsError)
            {
                model.User.Claims.AddRange(response.Claims.Select(x => new ClaimModel { Type = x.Type, Value = x.Value }));
            }

            return View(model);
        }

        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var metaDataResponse = await httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _appSettings.OpenIdConnect.Authority,
                Policy = { RequireHttps = _appSettings.OpenIdConnect.RequireHttpsMetadata },
            });

            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var response = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = metaDataResponse.TokenEndpoint,
                ClientId = _appSettings.OpenIdConnect.ClientId,
                ClientSecret = _appSettings.OpenIdConnect.ClientSecret,
                RefreshToken = refreshToken,
            });

            if (response.IsError)
            {
                return Json(response);
            }

            var auth = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            auth.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, response.AccessToken);
            auth.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, response.RefreshToken);
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
            auth.Properties.UpdateTokenValue("expires_at", expiresAt.ToString("o", CultureInfo.InvariantCulture));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, auth.Principal, auth.Properties);

            return RedirectToAction(nameof(AuthorizedAction));
        }

        public IActionResult TestException()
        {
            throw new Exception("Test Exception Filter");
        }

        [Authorize(Policy = "CustomPolicy")]
        public IActionResult CustomRequirement()
        {
            return Content("Inside Custom Requirement Action");
        }
    }
}
