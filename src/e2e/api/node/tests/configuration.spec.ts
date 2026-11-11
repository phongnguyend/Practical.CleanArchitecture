import { writeFile } from 'node:fs/promises';
import { test, expect, writes, json, successful } from '../support/fixtures.js';
import type { ConfigurationEntry } from '../support/models.js';

for (const sensitive of [false, true]) {
  test(`configuration CRUD, sensitivity toggle and Excel export/import (sensitive=${sensitive}) @writes`, async ({ api, data }, info) => {
    writes();
    const values = { key: `${data.prefix}-setting`, value: 'VALUE1', description: 'Description', isSensitive: sensitive };
    const created = await data.create<ConfigurationEntry>('configurationentries', values);
    expect(created).toMatchObject({ key: values.key, description: values.description, isSensitive: sensitive });
    if (sensitive) expect(created.value).not.toBe(values.value); else expect(created.value).toBe(values.value);
    expect(await json<ConfigurationEntry[]>(await api.get('configurationentries'))).toEqual(expect.arrayContaining([expect.objectContaining({ id: created.id })]));
    const refreshed = await json<ConfigurationEntry>(await api.get(`configurationentries/${created.id}`));
    expect(refreshed).toMatchObject({ ...created });
    const update = async (value: string, isSensitive: boolean) => json<ConfigurationEntry>(await api.put(`configurationentries/${created.id}`, { data: { ...refreshed, value, isSensitive } }));
    const updated = await update('VALUE2', sensitive);
    expect(updated).toMatchObject({ id: created.id, key: values.key, description: values.description });
    if (sensitive) expect(updated.value).not.toBe('VALUE2'); else expect(updated.value).toBe('VALUE2');
    const toggled = await update('VALUE3', !sensitive);
    expect(toggled).toMatchObject({ id: created.id, key: values.key, description: values.description, isSensitive: !sensitive });
    if (sensitive) expect(toggled.value).toBe('VALUE3'); else expect(toggled.value).not.toBe('VALUE3');
    const exported = await api.get('configurationentries/ExportAsExcel'); successful(exported);
    const bytes = await exported.body(); expect(bytes.subarray(0, 2).toString()).toBe('PK');
    await writeFile(info.outputPath('ConfigurationEntries.xlsx'), bytes);
    const imported = await json<ConfigurationEntry[]>(await api.post('configurationentries/ImportExcel', { multipart: {
      name: 'ConfigurationEntries.xlsx', formFile: { name: 'ConfigurationEntries.xlsx', mimeType: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', buffer: bytes },
    } }));
    expect(imported).toEqual(expect.arrayContaining([expect.objectContaining({ key: values.key })]));
    await data.remove('configurationentries', created.id);
  });
}
