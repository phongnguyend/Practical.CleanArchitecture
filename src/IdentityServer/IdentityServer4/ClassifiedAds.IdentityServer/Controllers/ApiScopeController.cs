using ClassifiedAds.IdentityServer.Models.ApiScopeModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ClassifiedAds.IdentityServer.Controllers
{
    public class ApiScopeController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public ApiScopeController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        public IActionResult Scopes(int id)
        {
            var api = _configurationDbContext.ApiScopes
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .FirstOrDefault(x => x.Id == id);
            return View(ApiScopeModel.FromEntity(api));
        }

        public IActionResult Edit(int id)
        {
            if (id != 0)
            {
                var api = _configurationDbContext.ApiScopes
                            .Include(x => x.UserClaims)
                            .Include(x => x.Properties)
                            .FirstOrDefault(x => x.Id == id);

                return View(ApiScopeModel.FromEntity(api));
            }
            else
            {
                return View(new ApiScopeModel
                {
                });
            }
        }

        [HttpPost]
        public IActionResult Edit(ApiScopeModel model)
        {
            ApiScope api;

            if (model.Id == 0)
            {
                api = new ApiScope
                {

                };
            }
            else
            {
                api = _configurationDbContext.ApiScopes
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .FirstOrDefault(x => x.Id == model.Id);
            }

            model.UpdateEntity(api);

            if (!string.IsNullOrEmpty(model.UserClaimsItems))
            {
                var userClaims = JsonSerializer.Deserialize<List<string>>(model.UserClaimsItems);

                api.UserClaims.AddRange(userClaims.Select(x => new ApiScopeClaim
                {
                    Type = x,
                }));
            }

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Scopes), new { id = api.Id });
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var api = _configurationDbContext.ApiScopes
                                        .Include(x => x.UserClaims)
                                        .Include(x => x.Properties)
                                        .FirstOrDefault(x => x.Id == id);
            return View(ApiScopeModel.FromEntity(api));
        }

        [HttpPost]
        public IActionResult Delete(ApiScopeModel model)
        {
            var api = _configurationDbContext.ApiScopes
                                        .Include(x => x.UserClaims)
                                        .Include(x => x.Properties)
                                        .FirstOrDefault(x => x.Id == model.Id);

            _configurationDbContext.ApiScopes.Remove(api);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Scopes), new { id = api.Id });
        }
    }
}
