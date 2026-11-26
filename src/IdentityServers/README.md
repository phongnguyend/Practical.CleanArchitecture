# Identity Server Docker stacks

Duende and OpenIddict have independent Compose stacks. Choose one provider; both
use port 9000 by default. Application Compose stacks no longer start Identity Server.

1. Start the chosen application stack and run its database migrations first.
   SQL Server must be reachable from Docker (the application stacks publish port 1433).
2. In `Duende` or `OpenIddict`, copy `.env.example` to `.env`.
3. Set `CLASSIFIEDADS_CONNECTION_STRING` to the same user database used by your
   application: Monolith's `ConnectionStrings__ClassifiedAds`, ModularMonolith's
   `Modules__Identity__ConnectionStrings__Default`, or the Microservices Identity
   service database. Replace container-local `Server=db` with an address reachable
   from this separate stack, such as `Server=host.docker.internal`.
4. Configure `IDENTITYSERVER_CONNECTION_STRING` and `IDENTITYSERVER_DB_HOST` for
   the provider's identity database. The migrator creates/migrates this database;
   it does not migrate the application user database.

From the selected provider directory:

```powershell
# Build locally and start:
docker compose up --build -d
# Or use images published by the dedicated CI workflow:
docker compose pull
docker compose up --no-build -d
```

The identity server starts after its migrator succeeds. View logs with
`docker compose logs -f migrator identityserver`. Each provider has separate
Compose-managed log and data-protection-key volumes.

The application stacks already use `http://host.docker.internal:9000` as their
Identity Server authority. If you change `IDENTITYSERVER_PORT`, update those
consumer URLs and the registered client redirect URLs as appropriate. Choose
one provider at a time unless you configure distinct ports and clients.

These settings retain the existing local HTTP development configuration.
`IDENTITYSERVER_IMAGE` and `IDENTITYSERVER_MIGRATOR_IMAGE` can override the GHCR
images, for example to select the `build` tag or a specific commit SHA.

Dedicated workflows are `docker-build-identityserver-duende.yml` and
`docker-build-identityserver-openiddict.yml`. They build both server and migrator;
only `master` and `build` publish images.
