using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassifiedAds.IdentityServer.Models.ApiResourceModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ClassifiedAds.IdentityServer.Controllers
{
    public class ApiResourceController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public ApiResourceController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        public IActionResult Index()
        {
            var apis = _configurationDbContext.ApiResources.ToList();
            return View(apis);
        }

        public IActionResult Add()
        {
            return View(nameof(Edit), new ApiResourceModel());
        }

        public IActionResult Edit(int id)
        {
            var identity = _configurationDbContext.ApiResources
                .Include(x => x.UserClaims)
                .FirstOrDefault(x => x.Id == id);
            return View(ApiResourceModel.FromEntity(identity));
        }

        [HttpPost]
        public IActionResult Edit(ApiResourceModel model)
        {
            ApiResource api;
            if (model.Id == 0)
            {
                api = new ApiResource
                {
                    UserClaims = new List<ApiResourceClaim>(),
                };
                _configurationDbContext.ApiResources.Add(api);
            }
            else
            {
                api = _configurationDbContext.ApiResources
                    .Include(x => x.UserClaims)
                    .FirstOrDefault(x => x.Id == model.Id);
                api.UserClaims.Clear();
            }

            model.UpdateEntity(api);

            if (!string.IsNullOrEmpty(model.UserClaimsItems))
            {
                var userClaims = JsonConvert.DeserializeObject<List<string>>(model.UserClaimsItems);

                api.UserClaims.AddRange(userClaims.Select(x => new ApiResourceClaim
                {
                    Type = x,
                }));
            }

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = api.Id });
        }

        public IActionResult Delete(int id)
        {
            var api = _configurationDbContext.ApiResources
                .FirstOrDefault(x => x.Id == id);
            return View(ApiResourceModel.FromEntity(api));
        }

        [HttpPost]
        public IActionResult Delete(ApiResourceModel model)
        {
            var api = _configurationDbContext.ApiResources
                .FirstOrDefault(x => x.Id == model.Id);

            _configurationDbContext.ApiResources.Remove(api);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Properties(int id)
        {
            var api = _configurationDbContext.ApiResources
                .Include(x => x.Properties)
                .FirstOrDefault(x => x.Id == id);
            return View(PropertiesModel.FromEntity(api));
        }

        [HttpPost]
        public IActionResult AddProperty(PropertiesModel model)
        {
            var api = _configurationDbContext.ApiResources
                .Include(x => x.Properties)
                .FirstOrDefault(x => x.Id == model.ApiResourceId);

            api.Properties.Add(new ApiResourceProperty
            {
                Key = model.Key,
                Value = model.Value,
            });

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Properties), new { id = api.Id });
        }

        [HttpGet]
        public IActionResult DeleteProperty(int id)
        {
            var prop = _configurationDbContext.Set<ApiResourceProperty>()
                .Include(x => x.ApiResource)
                .FirstOrDefault(x => x.Id == id);
            return View(ApiResourcePropertyModel.FromEntity(prop));
        }

        [HttpPost]
        public IActionResult DeleteProperty(ApiResourcePropertyModel model)
        {
            var api = _configurationDbContext.ApiResources
                            .Include(x => x.Properties)
                            .FirstOrDefault(x => x.Id == model.ApiResource.Id);
            var prop = api.Properties.FirstOrDefault(x => x.Id == model.Id);
            api.Properties.Remove(prop);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Properties), new { id = api.Id });
        }

        public IActionResult Secrets(int id)
        {
            var api = _configurationDbContext.ApiResources
                .Include(x => x.Secrets)
                .FirstOrDefault(x => x.Id == id);
            return View(SecretsModel.FromEntity(api));
        }

        [HttpPost]
        public IActionResult Secrets(SecretsModel model)
        {
            var api = _configurationDbContext.ApiResources
                .Include(x => x.Secrets)
                .FirstOrDefault(x => x.Id == model.ApiResourceId);

            var secret = new ApiSecret
            {
                Created = DateTime.Now,
            };

            model.HashSecret();
            model.UpdateEntity(secret);
            api.Secrets.Add(secret);

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Secrets), new { id = api.Id });
        }

        [HttpGet]
        public IActionResult DeleteSecret(int id)
        {
            var secret = _configurationDbContext.Set<ApiSecret>()
                .Include(x => x.ApiResource)
                .FirstOrDefault(x => x.Id == id);
            return View(SecretModel.FromEntity(secret));
        }

        [HttpPost]
        public IActionResult DeleteSecret(SecretModel model)
        {
            var api = _configurationDbContext.ApiResources
                            .Include(x => x.Secrets)
                            .FirstOrDefault(x => x.Id == model.ApiResourceId);
            var secret = api.Secrets.FirstOrDefault(x => x.Id == model.Id);
            api.Secrets.Remove(secret);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Secrets), new { id = api.Id });
        }

        public IActionResult Scopes(int id)
        {
            var api = _configurationDbContext.ApiResources
                .Include(x => x.Scopes)
                .FirstOrDefault(x => x.Id == id);
            return View(ScopesModel.FromEntity(api));
        }

        public IActionResult EditScope(int apiId, int scopeId)
        {
            var api = _configurationDbContext.ApiResources
                .Include("Scopes.UserClaims")
                .FirstOrDefault(x => x.Id == apiId);

            if (scopeId != 0)
            {
                var scopeEntity = api.Scopes.FirstOrDefault(x => x.Id == scopeId);
                return View(ScopeModel.FromEntity(scopeEntity));
            }
            else
            {
                return View(new ScopeModel
                {
                    ApiResourceId = api.Id,
                    ApiResourceName = api.Name,
                    ApiResource = api,
                });
            }
        }

        [HttpPost]
        public IActionResult EditScope(ScopeModel model)
        {
            var api = _configurationDbContext.ApiResources
                .Include("Scopes.UserClaims")
                .FirstOrDefault(x => x.Id == model.ApiResourceId);

            ApiScope scope;
            if (model.Id == 0)
            {
                scope = new ApiScope
                {
                    UserClaims = new List<ApiScopeClaim>(),
                };
                api.Scopes.Add(scope);
            }
            else
            {
                scope = api.Scopes.FirstOrDefault(x => x.Id == model.Id);
                scope.UserClaims.Clear();
            }

            model.UpdateEntity(scope);

            if (!string.IsNullOrEmpty(model.UserClaimsItems))
            {
                var userClaims = JsonConvert.DeserializeObject<List<string>>(model.UserClaimsItems);

                scope.UserClaims.AddRange(userClaims.Select(x => new ApiScopeClaim
                {
                    Type = x,
                }));
            }

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Scopes), new { id = api.Id });
        }

        [HttpGet]
        public IActionResult DeleteScope(int apiId, int scopeId)
        {
            var scope = _configurationDbContext.Set<ApiScope>()
                .Include(x => x.ApiResource)
                .FirstOrDefault(x => x.Id == scopeId);
            return View(ScopeModel.FromEntity(scope));
        }

        [HttpPost]
        public IActionResult DeleteScope(ScopeModel model)
        {
            var api = _configurationDbContext.ApiResources
                            .Include(x => x.Scopes)
                            .FirstOrDefault(x => x.Id == model.ApiResourceId);
            var scope = api.Scopes.FirstOrDefault(x => x.Id == model.Id);
            api.Scopes.Remove(scope);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Scopes), new { id = api.Id });
        }
    }
}