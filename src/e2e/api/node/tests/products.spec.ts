import { writeFile } from 'node:fs/promises';
import { test, expect, writes, json, successful } from '../support/fixtures.js';
import type { Product, AuditEntry } from '../support/models.js';

test('product CRUD, audit history and CSV export/import @writes', async ({ api, data }, info) => {
  writes();
  const values = { name: `${data.prefix}-product`, code: 'TEST', description: 'Description' };
  const created = await data.create<Product>('products', values);
  expect(created).toMatchObject(values);
  const audits = async () => json<AuditEntry[]>(await api.get(`products/${created.id}/auditlogs`));
  await expect.poll(async () => (await audits()).map(a => a.action)).toEqual(['CREATED']);
  expect(await json<Product[]>(await api.get('products'))).toEqual(expect.arrayContaining([expect.objectContaining({ id: created.id })]));
  const refreshed = await json<Product>(await api.get(`products/${created.id}`));
  expect(refreshed).toMatchObject({ ...values, id: created.id });
  const name = `${data.prefix}-Test' 2,2 "a"`;
  const updated = await json<Product>(await api.put(`products/${created.id}`, { data: { ...refreshed, name } }));
  expect(updated).toMatchObject({ ...values, name, id: created.id });
  await expect.poll(async () => (await audits()).map(a => a.action).sort()).toEqual(['CREATED', 'UPDATED']);
  const exported = await api.get('products/ExportAsCsv'); successful(exported);
  const bytes = await exported.body(); expect(bytes.length).toBeGreaterThan(0);
  await writeFile(info.outputPath('Products.csv'), bytes);
  const imported = await json<Product[]>(await api.post('products/ImportCsv', { multipart: {
    name: 'Products.csv', formFile: { name: 'Products.csv', mimeType: 'text/csv', buffer: bytes },
  } }));
  expect(imported).toEqual(expect.arrayContaining([expect.objectContaining({ name, code: values.code, description: values.description })]));
  await data.remove('products', created.id);
});

test('product PDF export', async ({ api }, info) => {
  test.skip(process.env.API_TEST_PDF === '0', 'PDF conversion explicitly disabled');
  const response = await api.get('products/ExportAsPdf'); successful(response);
  const bytes = await response.body();
  expect(bytes.subarray(0, 5).toString()).toBe('%PDF-');
  await writeFile(info.outputPath('Products.pdf'), bytes);
});
