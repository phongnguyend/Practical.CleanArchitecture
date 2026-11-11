import { writeFile } from 'node:fs/promises';
import { test, expect, writes, json, successful } from '../support/fixtures.js';
import type { FileEntry, AuditEntry } from '../support/models.js';

for (const encrypted of [true, false]) {
  test(`file upload, CRUD, audit history and byte-exact download (encrypted=${encrypted}) @writes`, async ({ api, data }, info) => {
    writes();
    const name = `${data.prefix}-file`;
    const bytes = Buffer.concat([Buffer.from('Test'), Buffer.from([0, 128, 255])]);
    data.watch('files');
    const created = await json<FileEntry>(await api.post('files', { multipart: {
      name, description: 'Description', encrypted: String(encrypted),
      formFile: { name: 'IntegrationTest.bin', mimeType: 'application/octet-stream', buffer: bytes },
    } }));
    expect(created.id).toMatch(/^[0-9a-f-]{36}$/i);
    expect(created).toMatchObject({ name, description: 'Description', encrypted });
    const audits = async () => json<AuditEntry[]>(await api.get(`files/${created.id}/auditlogs`));
    // The .NET encrypted-file scenario expects creation plus two storage updates.
    await expect.poll(async () => (await audits()).map(a => a.action).sort()).toEqual(['CREATED', 'UPDATED', 'UPDATED']);
    expect(await json<FileEntry[]>(await api.get('files'))).toEqual(expect.arrayContaining([expect.objectContaining({ id: created.id })]));
    const refreshed = await json<FileEntry>(await api.get(`files/${created.id}`));
    expect(refreshed).toMatchObject({ id: created.id, name, description: 'Description' });
    const updated = await json<FileEntry>(await api.put(`files/${created.id}`, { data: { ...refreshed, name: `${name}-2` } }));
    expect(updated).toMatchObject({ id: created.id, name: `${name}-2`, description: 'Description' });
    await expect.poll(async () => (await audits()).map(a => a.action).sort()).toEqual(['CREATED', 'UPDATED', 'UPDATED', 'UPDATED']);
    const download = await api.get(`files/${created.id}/download`); successful(download);
    expect(await download.body()).toEqual(bytes);
    await writeFile(info.outputPath(created.fileName), await download.body());
    await data.remove('files', created.id);
  });
}
