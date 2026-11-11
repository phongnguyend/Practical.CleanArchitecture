import { existsSync, readFileSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

export const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
if (existsSync(path.join(root, '.env'))) process.loadEnvFile(path.join(root, '.env'));
type Settings = { WebAPI?: { Endpoint?: string }; OpenIdConnect?: { Authority?: string; ClientId?: string; ClientSecret?: string }; Login?: { UserName?: string; Password?: string; Scope?: string } };
const legacy: Settings = process.env.API_DOTNET_SETTINGS
  ? JSON.parse(readFileSync(path.resolve(root, process.env.API_DOTNET_SETTINGS), 'utf8')) : {};
export const settings = {
  baseURL: process.env.API_BASE_URL || `${(legacy.WebAPI?.Endpoint || 'https://localhost:44312').replace(/\/$/, '')}/api/`,
  authority: process.env.API_AUTHORITY || legacy.OpenIdConnect?.Authority || 'https://localhost:44367',
  clientId: process.env.API_CLIENT_ID || legacy.OpenIdConnect?.ClientId || 'ClassifiedAds.WebMVC',
  clientSecret: process.env.API_CLIENT_SECRET || legacy.OpenIdConnect?.ClientSecret || '',
  username: process.env.API_USERNAME || legacy.Login?.UserName || '',
  password: process.env.API_PASSWORD || legacy.Login?.Password || '',
  scope: process.env.API_SCOPE || legacy.Login?.Scope || 'ClassifiedAds.WebAPI',
  ignoreHTTPSErrors: process.env.API_IGNORE_HTTPS_ERRORS === '1',
};
settings.baseURL = settings.baseURL.replace(/\/?$/, '/');
