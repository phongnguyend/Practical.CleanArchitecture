import { Pagination } from "./Pagination";
import React, { useCallback, useEffect, useRef, useState } from "react";
import { ActivityIndicator, Text, TextInput, View } from "react-native";
import type { PickedFile } from "../../core/services";
import { auditSnapshot, Row } from "../models/record";
import { JsonTools } from "./record-ui";
import { Button, Sheet, useStyles } from "./ui";
export type Entity = { id: string };
export type EditorProps<T> = {
  value: T;
  change: (value: T) => void;
  busy: boolean;
  file: PickedFile | null;
  pick: (file: PickedFile | null) => void;
};
export type Repository<T> = {
  list: () => Promise<T[]>;
  get: (id: string) => Promise<T>;
  save: (value: T, file: PickedFile | null) => Promise<void>;
  remove: (id: string) => Promise<void>;
  history?: (id: string) => Promise<Row[]>;
};
export type Actions = {
  run: (action: () => Promise<void>) => Promise<void>;
  reload: () => Promise<void>;
  notify: (text: string) => void;
};
export function RecordPage<T extends Entity>({
  title,
  repository,
  empty,
  label,
  searchText,
  validate,
  Form,
  Summary,
  Details,
  History,
  toolbar,
  actions,
  createTitle = "Add new",
}: {
  title: string;
  repository: Repository<T>;
  empty: () => T;
  label: (value: T) => string;
  searchText: (value: T) => string;
  validate: (value: T) => string | undefined;
  Form: React.ComponentType<EditorProps<T>>;
  Summary: React.ComponentType<{ value: T }>;
  Details: React.ComponentType<{ value: T }>;
  History?: (entry: Row) => React.ReactNode;
  toolbar?: React.ComponentType<Actions>;
  actions?: React.ComponentType<{ value: T; actions: Actions }>;
  createTitle?: string;
}) {
  const s = useStyles();
  const [rows, setRows] = useState<T[]>([]),
    [loading, setLoading] = useState(true),
    [working, setWorking] = useState(false),
    [error, setError] = useState(""),
    [toast, setToast] = useState(""),
    [search, setSearch] = useState(""),
    [page, setPage] = useState(1);
  const [editor, setEditor] = useState<T | null>(null),
    [draft, setDraft] = useState<T | null>(null),
    [file, setFile] = useState<PickedFile | null>(null),
    [detail, setDetail] = useState<T | null>(null),
    [history, setHistory] = useState<Row[] | null>(null),
    [confirm, setConfirm] = useState<{
      text: string;
      destructive?: boolean;
      action: () => Promise<void>;
    } | null>(null);
  const lock = useRef(false),
    generation = useRef(0);
  const busy = working || loading;
  const Toolbar = toolbar;
  const RowActions = actions;
  const reload = useCallback(() => {
    const ticket = ++generation.current;
    return repository
      .list()
      .then((items) => {
        if (ticket === generation.current) {
          setRows(items);
          setError("");
        }
      })
      .catch((e) => {
        if (ticket === generation.current) setError(String(e));
      })
      .finally(() => {
        if (ticket === generation.current) setLoading(false);
      });
  }, [repository]);
  const invalidate = useCallback(() => {
    generation.current++;
  }, []);
  useEffect(() => {
    void reload();
    return invalidate;
  }, [reload, invalidate]);
  useEffect(() => {
    if (!toast) return;
    const timer = setTimeout(() => setToast(""), 6000);
    return () => clearTimeout(timer);
  }, [toast]);
  async function run(action: () => Promise<void>) {
    if (lock.current) return;
    lock.current = true;
    setWorking(true);
    setError("");
    try {
      await action();
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e));
    } finally {
      lock.current = false;
      setWorking(false);
    }
  }
  const controls = { run, reload, notify: setToast };
  function edit(value: T) {
    setEditor(value);
    setDraft({ ...value });
    setFile(null);
    setError("");
  }
  function close() {
    if (busy) return;
    if (draft && (file || JSON.stringify(draft) !== JSON.stringify(editor))) {
      setConfirm({
        text: "Discard unsaved changes?",
        action: async () => {
          setEditor(null);
          setDraft(null);
        },
      });
    } else {
      setEditor(null);
      setDraft(null);
    }
  }
  const filtered = rows.filter((row) =>
    searchText(row).toLowerCase().includes(search.toLowerCase()),
  );
  return (
    <>
      <Text accessibilityRole="header" style={s.sectionTitle}>{title}</Text>
      <TextInput
        accessibilityLabel="Search current records"
        placeholder="Search records"
        value={search}
        onChangeText={(text) => {
          setSearch(text);
          setPage(1);
        }}
        style={s.input}
      />
      <View style={s.actions}>
        <Button
          title="Refresh"
          disabled={busy}
          onPress={() => void run(reload)}
        />
        <Button
          title={createTitle}
          variant="primary"
          disabled={busy}
          onPress={() => edit(empty())}
        />
        {Toolbar && <Toolbar {...controls} />}
      </View>
      {busy && <ActivityIndicator />}
      {!filtered.length && !busy && (
        <Text style={s.muted}>No records found.</Text>
      )}
      {filtered.slice((page - 1) * 20, page * 20).map((row) => (
        <View key={row.id} style={s.card}>
          <Text style={s.heading}>{label(row)}</Text>
          <Summary value={row} />
          <View style={s.actions}>
            <Button
              title="View"
              disabled={busy}
              onPress={() =>
                void run(async () => setDetail(await repository.get(row.id)))
              }
            />
            <Button
              title="Edit"
              disabled={busy}
              onPress={() =>
                void run(async () => edit(await repository.get(row.id)))
              }
            />
            <Button
              title="Delete"
              danger
              disabled={busy}
              onPress={() =>
                setConfirm({
                  text: `Delete ${label(row)}? This cannot be undone.`,
                  destructive: true,
                  action: async () => {
                    await repository.remove(row.id);
                    await reload();
                    setToast("Deleted");
                  },
                })
              }
            />
            {repository.history && (
              <Button
                title="History"
                disabled={busy}
                onPress={() =>
                  void run(async () =>
                    setHistory(await repository.history!(row.id)),
                  )
                }
              />
            )}
            {RowActions && <RowActions value={row} actions={controls} />}
          </View>
        </View>
      ))}
      <Pagination page={page} total={filtered.length} pageSize={20} busy={busy} onPage={setPage} />
      {!!error && (
        <Text accessibilityRole="alert" style={s.error}>
          {error}
        </Text>
      )}
      {!!toast && (
        <Text accessibilityRole="alert" style={s.text}>
          {toast}
        </Text>
      )}
      {(confirm || draft || detail || history) && (
        <Sheet
          title={
            confirm
              ? "Confirm action"
              : draft
                ? `${draft.id ? "Edit" : "Add"} ${title}`
                : history
                  ? "Audit history"
                  : "Details"
          }
          close={() => {
            if (busy) return;
            if (confirm) setConfirm(null);
            else if (draft) close();
            else {
              setDetail(null);
              setHistory(null);
            }
          }}
          footer={
            confirm ? (
              <>
                <Button
                  title="Cancel"
                  disabled={busy}
                  onPress={() => setConfirm(null)}
                />
                <Button
                  title="Confirm"
                  variant="primary"
                  danger={confirm.destructive}
                  disabled={busy}
                  onPress={() => {
                    const action = confirm.action;
                    setConfirm(null);
                    void run(action);
                  }}
                />
              </>
            ) : draft ? (
              <>
                <Button title="Cancel" disabled={busy} onPress={close} />
                <Button
                  title="Save"
                  variant="primary"
                  disabled={busy}
                  onPress={() =>
                    void run(async () => {
                      const message = validate(draft);
                      if (message) throw new Error(message);
                      await repository.save(draft, file);
                      setDraft(null);
                      setEditor(null);
                      setToast("Saved successfully");
                      await reload();
                    })
                  }
                />
              </>
            ) : (
              <>
                <Button
                  title="Back"
                  onPress={() => {
                    setDetail(null);
                    setHistory(null);
                  }}
                />
                {detail && (
                  <Button
                    title="Edit"
                    onPress={() => {
                      edit(detail);
                      setDetail(null);
                    }}
                  />
                )}
              </>
            )
          }
        >
          {confirm ? (
            <Text style={s.text}>{confirm.text}</Text>
          ) : draft ? (
            <Form
              value={draft}
              change={setDraft}
              busy={busy}
              file={file}
              pick={setFile}
            />
          ) : detail ? (
            <Details value={detail} />
          ) : history?.length === 0 ? (
            <Text style={s.muted}>No audit entries.</Text>
          ) : (
            history?.map((entry, index) => (
              <View key={entry.id || index} style={s.card}>
                <Text style={s.text}>
                  {entry.createdDateTime} · {entry.userName} · {entry.action}
                </Text>
                {History?.(entry)}
                <JsonTools value={auditSnapshot(entry)} />
              </View>
            ))
          )}
          {!!error && (
            <Text accessibilityRole="alert" style={s.error}>
              {error}
            </Text>
          )}
          {busy && <ActivityIndicator />}
        </Sheet>
      )}
    </>
  );
}
