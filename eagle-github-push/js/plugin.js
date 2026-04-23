'use strict';

// ── Settings (localStorage) ──────────────────────────────────────────────────

const STORAGE_KEY = 'eagle_github_push_v1';

function loadSettings() {
  try { return JSON.parse(localStorage.getItem(STORAGE_KEY) || '{}'); }
  catch { return {}; }
}
function saveSettings(s) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(s));
}

// ── GitHub API ───────────────────────────────────────────────────────────────

async function ghRequest(method, apiPath, token, body) {
  const opts = {
    method,
    headers: {
      Authorization: `token ${token}`,
      Accept: 'application/vnd.github.v3+json',
      'User-Agent': 'Eagle-GitHub-Push/1.1',
    },
  };
  if (body) {
    opts.headers['Content-Type'] = 'application/json';
    opts.body = JSON.stringify(body);
  }
  const res = await fetch(`https://api.github.com${apiPath}`, opts);
  const data = await res.json().catch(() => ({}));
  return { ok: res.ok, status: res.status, data };
}

async function getFileSha(token, owner, repo, filePath, branch) {
  const { ok, data } = await ghRequest(
    'GET',
    `/repos/${owner}/${repo}/contents/${encPath(filePath)}?ref=${branch}`,
    token
  );
  return ok && data.sha ? data.sha : null;
}

async function pushFileToGH(token, owner, repo, branch, filePath, b64content, message) {
  const sha = await getFileSha(token, owner, repo, filePath, branch);
  const body = { message, content: b64content, branch };
  if (sha) body.sha = sha;
  return ghRequest('PUT', `/repos/${owner}/${repo}/contents/${encPath(filePath)}`, token, body);
}

function encPath(p) {
  return p.split('/').map(encodeURIComponent).join('/');
}

// ── File helpers ─────────────────────────────────────────────────────────────

function readFileBase64(filePath) {
  const fs = require('fs');
  return fs.readFileSync(filePath).toString('base64');
}

function fileExists(filePath) {
  try { require('fs').accessSync(filePath); return true; }
  catch { return false; }
}

function fmtBytes(b) {
  if (!b) return '';
  if (b < 1024) return `${b} B`;
  if (b < 1048576) return `${(b / 1024).toFixed(1)} KB`;
  return `${(b / 1048576).toFixed(1)} MB`;
}

// ── Helpers ──────────────────────────────────────────────────────────────────

function esc(s) {
  return String(s || '')
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;');
}

function $id(id) { return document.getElementById(id); }

function showEl(el, show) {
  if (typeof el === 'string') el = $id(el);
  el.style.display = show ? '' : 'none';
}

// ── Log ─────────────────────────────────────────────────────────────────────

function addLog(msg, type = 'info') {
  showEl('push-log', true);
  const entries = $id('log-entries');
  const t = new Date().toLocaleTimeString('zh-TW', { hour12: false });
  const row = document.createElement('div');
  row.className = 'le';
  row.innerHTML = `<span class="le-t">${t}</span><span class="le-m ${type === 'success' ? 'ok' : type === 'error' ? 'err' : 'info'}">${esc(msg)}</span>`;
  entries.appendChild(row);
  entries.scrollTop = entries.scrollHeight;
}

// ── Settings tab ─────────────────────────────────────────────────────────────

function initSettings() {
  const s = loadSettings();
  if (s.token)  $id('github-token').value  = s.token;
  if (s.repo)   $id('github-repo').value   = s.repo;
  $id('github-branch').value = s.branch || 'main';
  $id('github-path').value   = s.targetPath || 'images/';
  if (s.useSubfolder) $id('use-subfolder').checked = true;

  // Show/hide token
  $id('btn-show-token').addEventListener('click', () => {
    const inp = $id('github-token');
    inp.type = inp.type === 'password' ? 'text' : 'password';
  });

  // Open token creation page
  $id('token-help-link').addEventListener('click', (e) => {
    e.preventDefault();
    if (typeof eagle !== 'undefined' && eagle.shell) {
      eagle.shell.openExternal('https://github.com/settings/tokens/new?scopes=repo');
    }
  });

  // Save
  $id('btn-save').addEventListener('click', () => {
    const s = collectSettings();
    if (!s.token) return showMsg('請填入 GitHub Token', 'err');
    if (!s.repo.includes('/')) return showMsg('Repository 格式應為 owner/repo', 'err');
    saveSettings(s);
    showMsg('設定已儲存 ✓', 'ok');
  });

  // Test connection
  $id('btn-test').addEventListener('click', async () => {
    const s = collectSettings();
    if (!s.token) return showMsg('請先填入 Token', 'err');
    if (!s.repo.includes('/')) return showMsg('請先填入 Repository', 'err');

    const btn = $id('btn-test');
    btn.disabled = true; btn.textContent = '測試中…';
    showMsg('連線中…', 'inf');

    const [owner, repo] = s.repo.split('/');
    const { ok, data } = await ghRequest('GET', `/repos/${owner}/${repo}`, s.token);

    btn.disabled = false; btn.textContent = '測試連線';

    if (ok) {
      showMsg(`✓ 連線成功！${data.full_name}（${data.private ? '私有' : '公開'}）`, 'ok');
    } else {
      showMsg(`✗ 連線失敗：${data.message || '未知錯誤'} (HTTP ${data.status || ''})`, 'err');
    }
  });
}

function collectSettings() {
  return {
    token:       $id('github-token').value.trim(),
    repo:        $id('github-repo').value.trim(),
    branch:      $id('github-branch').value.trim() || 'main',
    targetPath:  $id('github-path').value.trim(),
    useSubfolder: $id('use-subfolder').checked,
  };
}

function showMsg(text, type) {
  const el = $id('settings-msg');
  el.textContent = text;
  el.className = `msg ${type}`;
  showEl(el, true);
  clearTimeout(el._t);
  el._t = setTimeout(() => showEl(el, false), 5000);
}

// ── Push tab ─────────────────────────────────────────────────────────────────

let selectedItems = [];

async function refreshItems() {
  selectedItems = [];

  if (typeof eagle === 'undefined') {
    // Not in Eagle context (browser preview etc.)
    showEl('no-items', true);
    showEl('items-view', false);
    $id('no-items').querySelector('p').textContent = 'Eagle API 未就緒，請稍後再試';
    return;
  }

  let items = [];
  try { items = await eagle.item.getSelected(); } catch (e) { console.error(e); }

  if (!items || items.length === 0) {
    showEl('no-items', true);
    showEl('items-view', false);
    return;
  }

  selectedItems = items;
  showEl('no-items', false);
  showEl('items-view', true);

  $id('item-count').textContent = `已選取 ${items.length} 個項目`;

  const list = $id('items-list');
  list.innerHTML = '';
  items.forEach((item, i) => list.appendChild(buildRow(item, i)));
}

function buildRow(item, idx) {
  const ext  = item.ext || '';
  const size = item.fileSize || item.size || 0;

  const row = document.createElement('div');
  row.className = 'item-row';
  row.dataset.idx = idx;

  const thumb = item.thumbnailUrl
    ? `<img class="item-thumb" src="${esc(item.thumbnailUrl)}" alt="" onerror="this.outerHTML='<div class=item-thumb-ph>${esc(ext)}</div>'">`
    : `<div class="item-thumb-ph">${esc(ext)}</div>`;

  row.innerHTML = `
    ${thumb}
    <div class="item-info">
      <div class="item-name" title="${esc(item.name)}.${esc(ext)}">${esc(item.name)}.${esc(ext)}</div>
      <div class="item-meta">${esc(fmtBytes(size))}</div>
    </div>
    <span class="item-stat" data-stat></span>
    <button class="item-btn" data-push-idx="${idx}">推送</button>
  `;

  row.querySelector('[data-push-idx]').addEventListener('click', () => pushOne(idx));
  return row;
}

function setRowState(idx, state, label) {
  const row = document.querySelector(`.item-row[data-idx="${idx}"]`);
  if (!row) return;
  row.className = `item-row ${state}`;

  const stat = row.querySelector('[data-stat]');
  stat.className = `item-stat ${state}`;
  stat.textContent = label || { pushing: '…', done: '✓', failed: '✗' }[state] || '';

  const btn = row.querySelector('.item-btn');
  if (state === 'pushing') { btn.disabled = true; btn.textContent = '…'; }
  else if (state === 'done') { btn.disabled = true; btn.textContent = '✓'; }
  else { btn.disabled = false; btn.textContent = '推送'; }
}

function buildDestPath(s, item) {
  const name = `${item.name}.${item.ext || ''}`;
  let base = (s.targetPath || '').replace(/\/+$/, '');
  if (s.useSubfolder && item.folderName) base += `/${item.folderName}`;
  return base ? `${base}/${name}` : name;
}

function autoCommitMsg(items) {
  const custom = $id('commit-message').value.trim();
  if (custom) return custom;
  const date = new Date().toISOString().slice(0, 10);
  const names = items.map(i => `${i.name}.${i.ext}`).join(', ');
  return names.length > 80
    ? `Add ${items.length} images via Eagle [${date}]`
    : `Add ${names} via Eagle [${date}]`;
}

async function pushOne(idx) {
  await doPush([{ item: selectedItems[idx], idx }]);
}

async function pushAll() {
  await doPush(selectedItems.map((item, idx) => ({ item, idx })));
}

async function doPush(jobs) {
  const s = loadSettings();

  if (!s.token) {
    addLog('尚未設定 GitHub Token，請切換到「設定」頁面', 'error');
    switchTab('settings');
    return;
  }
  if (!s.repo || !s.repo.includes('/')) {
    addLog('尚未設定 Repository，請切換到「設定」頁面', 'error');
    switchTab('settings');
    return;
  }

  const [owner, repo] = s.repo.split('/');
  const branch = s.branch || 'main';
  const msg = autoCommitMsg(jobs.map(j => j.item));
  const allBtn = $id('btn-push-all');
  allBtn.disabled = true;

  for (const { item, idx } of jobs) {
    setRowState(idx, 'pushing');

    const filePath = item.filePath || item.path;

    if (!filePath || !fileExists(filePath)) {
      setRowState(idx, 'failed');
      addLog(`✗ ${item.name}: 找不到本地檔案`, 'error');
      continue;
    }

    let b64;
    try {
      b64 = readFileBase64(filePath);
    } catch (e) {
      setRowState(idx, 'failed');
      addLog(`✗ ${item.name}: 讀取失敗 – ${e.message}`, 'error');
      continue;
    }

    const dest = buildDestPath(s, item);
    addLog(`⟳ 上傳 ${item.name}.${item.ext} → ${dest}`, 'info');

    try {
      const { ok, status, data } = await pushFileToGH(s.token, owner, repo, branch, dest, b64, msg);

      if (ok) {
        setRowState(idx, 'done');
        const url = data.content && data.content.html_url ? data.content.html_url : '';
        addLog(`✓ 成功${url ? `：${url}` : ''}`, 'success');
      } else {
        setRowState(idx, 'failed');
        addLog(`✗ ${item.name}: ${data.message || '未知錯誤'} (HTTP ${status})`, 'error');
      }
    } catch (e) {
      setRowState(idx, 'failed');
      addLog(`✗ ${item.name}: 網路錯誤 – ${e.message}`, 'error');
    }
  }

  allBtn.disabled = false;
}

// ── Tab switching ─────────────────────────────────────────────────────────────

function switchTab(name) {
  document.querySelectorAll('.tab').forEach(t =>
    t.classList.toggle('active', t.dataset.tab === name)
  );
  showEl('tab-push',     name === 'push');
  showEl('tab-settings', name === 'settings');
}

// ── Boot ─────────────────────────────────────────────────────────────────────

function boot() {
  // Tab clicks
  document.querySelectorAll('.tab').forEach(t =>
    t.addEventListener('click', () => switchTab(t.dataset.tab))
  );

  // Refresh
  $id('btn-refresh').addEventListener('click', refreshItems);
  $id('btn-refresh-list').addEventListener('click', refreshItems);

  // Push all
  $id('btn-push-all').addEventListener('click', pushAll);

  // Clear log
  $id('btn-clear-log').addEventListener('click', () => {
    $id('log-entries').innerHTML = '';
    showEl('push-log', false);
  });

  initSettings();

  // Initial load
  refreshItems();
}

// ── Eagle lifecycle ──────────────────────────────────────────────────────────

// DOMContentLoaded ensures DOM is ready before we touch it
window.addEventListener('DOMContentLoaded', () => {
  boot();

  if (typeof eagle !== 'undefined') {
    // Refresh whenever the plugin window regains focus
    eagle.onPluginShow(() => refreshItems());
  }
});
