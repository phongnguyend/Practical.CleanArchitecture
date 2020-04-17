using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassifiedAds.IdentityServer.Models.IdentityResourceModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ClassifiedAds.IdentityServer.Controllers
{
    public class IdentityResourceController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public IdentityResourceController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        public IActionResult Index()
        {
            var itentities = _configurationDbContext.IdentityResources.ToList();
            return View(itentities.ToModels().ToList());
        }

        public IActionResult Add()
        {
            return View(nameof(Edit), new IdentityResourceModel());
        }

        public IActionResult Edit(int id)
        {
            var identity = _configurationDbContext.IdentityResources
                .Include(x => x.UserClaims)
                .FirstOrDefault(x => x.Id == id);
            return View(identity.ToModel());
        }

        [HttpPost]
        public IActionResult Edit(IdentityResourceModel model)
        {
            IdentityResource identity;
            if (model.Id == 0)
            {
                identity = new IdentityResource
                {
                    UserClaims = new List<IdentityClaim>(),
                };
                _configurationDbContext.IdentityResources.Add(identity);
            }
            else
            {
                identity = _configurationDbContext.IdentityResources
                    .Include(x => x.UserClaims)
                    .FirstOrDefault(x => x.Id == model.Id);
                identity.UserClaims.Clear();
            }

            model.UpdateEntity(identity);

            if (!string.IsNullOrEmpty(model.UserClaimsItems))
            {
                model.UserClaims = JsonConvert.DeserializeObject<List<string>>(model.UserClaimsItems);

                identity.UserClaims.AddRange(model.UserClaims.Select(x => new IdentityClaim
                {
                    Type = x,
                }));
            }

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = identity.Id });
        }

        public IActionResult Delete(int id)
        {
            var identity = _configurationDbContext.IdentityResources
                .Include(x => x.UserClaims)
                .FirstOrDefault(x => x.Id == id);
            return View(identity.ToModel());
        }

        [HttpPost]
        public IActionResult Delete(IdentityResourceModel model)
        {
            var identity = _configurationDbContext.IdentityResources
                .FirstOrDefault(x => x.Id == model.Id);

            _configurationDbContext.IdentityResources.Remove(identity);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Properties(int id)
        {
            var identity = _configurationDbContext.IdentityResources
                .Include(x => x.Properties)
                .FirstOrDefault(x => x.Id == id);
            return View(PropertiesModel.FromEntity(identity));
        }

        [HttpPost]
        public IActionResult AddProperty(PropertiesModel model)
        {
            var identity = _configurationDbContext.IdentityResources
                .Include(x => x.Properties)
                .FirstOrDefault(x => x.Id == model.IdentityResourceId);

            identity.Properties.Add(new IdentityResourceProperty
            {
                Key = model.Key,
                Value = model.Value,
            });

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Properties), new { id = identity.Id });
        }

        [HttpGet]
        public IActionResult DeleteProperty(int id)
        {
            var prop = _configurationDbContext.Set<IdentityResourceProperty>()
                .Include(x => x.IdentityResource)
                .FirstOrDefault(x => x.Id == id);
            return View(IdentityResourcePropertyModel.FromEntity(prop));
        }

        [HttpPost]
        public IActionResult DeleteProperty(IdentityResourcePropertyModel model)
        {
            var identity = _configurationDbContext.IdentityResources
                            .Include(x => x.Properties)
                            .FirstOrDefault(x => x.Id == model.IdentityResource.Id);
            var prop = identity.Properties.FirstOrDefault(x => x.Id == model.Id);
            identity.Properties.Remove(prop);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Properties), new { id = identity.Id });
        }

        public IActionResult GetClaims()
        {
            // http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
            var standardClaims = new List<string>
            {
                "name",
                "given_name",
                "family_name",
                "middle_name",
                "nickname",
                "preferred_username",
                "profile",
                "picture",
                "website",
                "gender",
                "birthdate",
                "zoneinfo",
                "locale",
                "address",
                "updated_at",
            };

            return Ok(standardClaims);
        }
    }
}