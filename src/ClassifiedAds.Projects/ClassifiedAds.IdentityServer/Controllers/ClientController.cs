using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassifiedAds.IdentityServer.Models.ClientModels;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClassifiedAds.IdentityServer.Controllers
{
    public class ClientController : Controller
    {
        private readonly ConfigurationDbContext _configurationDbContext;

        public ClientController(ConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        public IActionResult Index()
        {
            var clients = _configurationDbContext.Clients
            .Include(x => x.AllowedGrantTypes)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .Include(x => x.ClientSecrets)
            .Include(x => x.Claims)
            .Include(x => x.IdentityProviderRestrictions)
            .Include(x => x.AllowedCorsOrigins)
            .Include(x => x.Properties)
            .AsNoTracking()
            .ToList();

            var models = clients.Select(x => ClientModel.FromEntity(x)).ToList();

            return View(models);
        }

        public IActionResult Add()
        {
            var client = new ClientModel();
            return View(nameof(Edit), client);
        }

        public IActionResult Edit(int id)
        {
            var client = _configurationDbContext.Clients
            .Include(x => x.AllowedGrantTypes)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .Include(x => x.ClientSecrets)
            .Include(x => x.Claims)
            .Include(x => x.IdentityProviderRestrictions)
            .Include(x => x.AllowedCorsOrigins)
            .Include(x => x.Properties)
            .Where(x => x.Id == id)
            .AsNoTracking()
            .FirstOrDefault();

            var model = ClientModel.FromEntity(client);

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(ClientModel model)
        {
            Client client;
            if (model.Id == 0)
            {
                model.SetDefaultValues();
                client = new Client();
                _configurationDbContext.Clients.Add(client);
            }
            else
            {
                model.ConvertItemsToList();

                client = _configurationDbContext.Clients
                        .Include(x => x.AllowedGrantTypes)
                        .Include(x => x.RedirectUris)
                        .Include(x => x.PostLogoutRedirectUris)
                        .Include(x => x.AllowedScopes)
                        .Include(x => x.ClientSecrets)
                        .Include(x => x.Claims)
                        .Include(x => x.IdentityProviderRestrictions)
                        .Include(x => x.AllowedCorsOrigins)
                        .Include(x => x.Properties)
                        .Where(x => x.Id == model.Id)
                        .FirstOrDefault();
                client.Updated = DateTime.Now;
            }

            model.UpdateEntity(client);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = client.Id });
        }

        public IActionResult Clone(int id)
        {
            var client = _configurationDbContext.Clients
            .Include(x => x.AllowedGrantTypes)
            .Include(x => x.RedirectUris)
            .Include(x => x.PostLogoutRedirectUris)
            .Include(x => x.AllowedScopes)
            .Include(x => x.ClientSecrets)
            .Include(x => x.Claims)
            .Include(x => x.IdentityProviderRestrictions)
            .Include(x => x.AllowedCorsOrigins)
            .Include(x => x.Properties)
            .Where(x => x.Id == id)
            .AsNoTracking()
            .FirstOrDefault();

            var model = ClientModel.FromEntity(client);
            model.OriginalClientId = model.ClientId;
            model.ClientId = $"Clone_From_{model.ClientId}_{DateTime.Now.ToString("yyyyMMddhhmmssfff")}";

            return View(model);
        }

        [HttpPost]
        public IActionResult Clone(ClientModel model)
        {
            Client client = new Client();
            model.ConvertItemsToList();
            model.UpdateEntity(client);

            _configurationDbContext.Clients.Add(client);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = client.Id });
        }

        public IActionResult Delete(int id)
        {
            var client = _configurationDbContext.Clients
            .Where(x => x.Id == id)
            .AsNoTracking()
            .FirstOrDefault();

            var model = ClientModel.FromEntity(client);

            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(ClientModel model)
        {
            var client = _configurationDbContext.Clients
            .FirstOrDefault(x => x.Id == model.Id);

            _configurationDbContext.Clients.Remove(client);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetScopes()
        {
            var identityResources = _configurationDbContext.IdentityResources
                .Select(x => x.Name).ToList();

            var apiScopes = _configurationDbContext.ApiResources
                .Select(x => x.Name).ToList();

            var scopes = identityResources.Concat(apiScopes).ToList();

            return Ok(scopes);
        }

        public IActionResult GetGrantTypes()
        {
            var allowedGrantypes = new List<string>
                {
                    "implicit",
                    "client_credentials",
                    "authorization_code",
                    "hybrid",
                    "password",
                    "urn:ietf:params:oauth:grant-type:device_code",
                };

            return Ok(allowedGrantypes);
        }

        public IActionResult Properties(int id)
        {
            var client = _configurationDbContext.Clients
                .Include(x => x.Properties)
                .FirstOrDefault(x => x.Id == id);
            return View(PropertiesModel.FromEntity(client));
        }

        [HttpPost]
        public IActionResult AddProperty(PropertiesModel model)
        {
            var client = _configurationDbContext.Clients
                .Include(x => x.Properties)
                .FirstOrDefault(x => x.Id == model.Client.Id);

            client.Properties.Add(new ClientProperty
            {
                Key = model.Key,
                Value = model.Value,
            });

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Properties), new { id = client.Id });
        }

        [HttpGet]
        public IActionResult DeleteProperty(int id)
        {
            var prop = _configurationDbContext.Set<ClientProperty>()
                .Include(x => x.Client)
                .FirstOrDefault(x => x.Id == id);
            return View(PropertyModel.FromEntity(prop));
        }

        [HttpPost]
        public IActionResult DeleteProperty(PropertyModel model)
        {
            var client = _configurationDbContext.Clients
                            .Include(x => x.Properties)
                            .FirstOrDefault(x => x.Id == model.Client.Id);
            var prop = client.Properties.FirstOrDefault(x => x.Id == model.Id);
            client.Properties.Remove(prop);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Properties), new { id = client.Id });
        }

        public IActionResult Secrets(int id)
        {
            var client = _configurationDbContext.Clients
                .Include(x => x.ClientSecrets)
                .FirstOrDefault(x => x.Id == id);
            return View(SecretsModel.FromEntity(client));
        }

        [HttpPost]
        public IActionResult Secrets(SecretsModel model)
        {
            var client = _configurationDbContext.Clients
                .Include(x => x.ClientSecrets)
                .FirstOrDefault(x => x.Id == model.Client.Id);

            var secret = new ClientSecret
            {
                Created = DateTime.Now,
            };

            model.HashSecret();
            model.UpdateEntity(secret);
            client.ClientSecrets.Add(secret);

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Secrets), new { id = client.Id });
        }

        [HttpGet]
        public IActionResult DeleteSecret(int id)
        {
            var secret = _configurationDbContext.Set<ClientSecret>()
                .Include(x => x.Client)
                .FirstOrDefault(x => x.Id == id);
            return View(SecretModel.FromEntity(secret));
        }

        [HttpPost]
        public IActionResult DeleteSecret(SecretModel model)
        {
            var client = _configurationDbContext.Clients
                            .Include(x => x.ClientSecrets)
                            .FirstOrDefault(x => x.Id == model.Client.Id);
            var secret = client.ClientSecrets.FirstOrDefault(x => x.Id == model.Id);
            client.ClientSecrets.Remove(secret);
            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Secrets), new { id = client.Id });
        }

        public IActionResult Claims(int id)
        {
            var client = _configurationDbContext.Clients
                .Include(x => x.Claims)
                .FirstOrDefault(x => x.Id == id);

            return View(ClaimsModel.FromEntity(client));
        }

        [HttpPost]
        public IActionResult Claims(ClaimModel model)
        {
            var client = _configurationDbContext.Clients
                .Include(x => x.Claims)
                .FirstOrDefault(x => x.Id == model.Client.Id);

            client.Claims.Add(new ClientClaim
            {
                Type = model.Type,
                Value = model.Value,
            });

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Claims), new { id = client.Id });
        }

        public IActionResult DeleteClaim(int id)
        {
            var claim = _configurationDbContext.Set<ClientClaim>()
                .Include(x => x.Client)
                .FirstOrDefault(x => x.Id == id);

            return View(ClaimModel.FromEntity(claim));
        }

        [HttpPost]
        public IActionResult DeleteClaim(ClaimModel model)
        {
            var client = _configurationDbContext.Clients
                .Include(x => x.Claims)
                .FirstOrDefault(x => x.Id == model.Client.Id);

            var claim = client.Claims.FirstOrDefault(x => x.Id == model.Id);

            client.Claims.Remove(claim);

            _configurationDbContext.SaveChanges();

            return RedirectToAction(nameof(Claims), new { id = client.Id });
        }
    }
}