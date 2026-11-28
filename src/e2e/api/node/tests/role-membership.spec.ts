import { randomUUID } from 'node:crypto';
import { test, expect, writes, json, successful } from '../support/fixtures.js';

test('role membership assignment, removal and user roles are consistent @writes', async ({ api, data }) => {
  writes();
  const user = await data.create('users', { userName: `${data.prefix}-member`, email: `${data.prefix}@example.invalid` });
  const role = await data.create('roles', { name: `${data.prefix}-role` });
  const other = await data.create('roles', { name: `${data.prefix}-other` });
  expect(await json(await api.get(`roles/${role.id}/users`))).toEqual([]);
  expect(await json(await api.get(`users/${user.id}/roles`))).toEqual([]);
  for (let attempt = 0; attempt < 2; attempt++) {
    successful(await api.put(`roles/${role.id}/users/${user.id}`));
  }
  const assigned = await json<{ id: string; userName: string }[]>(await api.get(`roles/${role.id}/users`));
  expect(assigned).toEqual([expect.objectContaining({ id: user.id, userName: user.userName })]);
  expect(Object.keys(assigned[0]).sort()).toEqual(['email', 'id', 'userName']);
  successful(await api.put(`roles/${other.id}/users/${user.id}`));
  expect(await json(await api.get(`users/${user.id}/roles`))).toEqual(expect.arrayContaining([
    expect.objectContaining({ id: role.id, name: role.name }), expect.objectContaining({ id: other.id, name: other.name }),
  ]));
  for (let attempt = 0; attempt < 2; attempt++) {
    successful(await api.delete(`roles/${role.id}/users/${user.id}`));
  }
  expect(await json(await api.get(`roles/${role.id}/users`))).toEqual([]);
  expect(await json(await api.get(`users/${user.id}/roles`))).toEqual([expect.objectContaining({ id: other.id })]);
  expect((await api.put(`roles/${role.id}/users/${randomUUID()}`)).status()).toBe(404);
  expect((await api.put(`roles/${randomUUID()}/users/${user.id}`)).status()).toBe(404);
  expect((await api.get(`users/${randomUUID()}/roles`)).status()).toBe(404);
  expect((await api.get(`roles/${randomUUID()}/users`)).status()).toBe(404);
});
