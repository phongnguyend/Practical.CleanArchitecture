import { randomUUID } from 'node:crypto';
import { test, expect, writes, json, successful } from '../support/fixtures.js';
import type { User } from '../support/models.js';

test('user CRUD and password policy @writes', async ({ api, data }) => {
  writes();
  const values = { userName: `${data.prefix}-user`, email: `${data.prefix}@example.invalid` };
  const created = await data.create<User>('users', values);
  expect(created).toMatchObject(values);
  expect(await json<User[]>(await api.get('users'))).toEqual(expect.arrayContaining([expect.objectContaining({ id: created.id })]));
  const refreshed = await json<User>(await api.get(`users/${created.id}`));
  expect(refreshed).toMatchObject({ ...values, id: created.id });
  const email = `${data.prefix}-updated@example.invalid`;
  expect(await json<User>(await api.put(`users/${created.id}`, { data: { ...refreshed, email } }))).toMatchObject({ id: created.id, email });
  const weak = await api.put(`users/${created.id}/password`, { data: { id: created.id, password: 'abc', confirmPassword: 'abc' } });
  expect([400, 422], 'Weak password must be rejected as a validation error').toContain(weak.status());
  const password = `Aa1!${randomUUID()}`;
  successful(await api.put(`users/${created.id}/password`, { data: { id: created.id, password, confirmPassword: password } }));
  await data.remove('users', created.id);
});

test('user confirmation and password-reset email requests @writes @emails', async ({ api, data }) => {
  writes();
  test.skip(process.env.API_ALLOW_EMAILS !== '1', 'Requires API_ALLOW_EMAILS=1 and a test mailbox');
  if (!process.env.API_TEST_EMAIL) throw new Error('Set API_TEST_EMAIL to a test mailbox or mail sink');
  const user = await data.create<User>('users', { userName: `${data.prefix}-mail`, email: process.env.API_TEST_EMAIL });
  successful(await api.post(`users/${user.id}/emailaddressconfirmation`));
  successful(await api.post(`users/${user.id}/passwordresetemail`));
  await data.remove('users', user.id);
});
