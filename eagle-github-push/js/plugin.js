/* Eagle GitHub Push Plugin – plugin.js
 * Reads selected Eagle items and pushes them to a GitHub repository
 * via the GitHub Contents REST API.
 */

'use strict';

const fs = require('fs');
const path = require('path');

// ─── Settings storage (localStorage) ────────────────────────────────────────

const STORAGE_KEY = 'eagle_github_push_settings';

function loadSettings() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : {};
  } catch {
    return {};
  }
}

function saveSettings(settings) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(settings));
}

// ─── GitHub API ──────────────────────────────────────────────────────────────

async function githubRequest(method, apiPath, token, body = null) {
  const opts = {
    method,
    headers: {
      Authorization: `token ${token}`,
      Accept: 'application/vnd.github.v3+json',
      'User-Agent': 'Eagle-GitHub-Push-Plugin/1.0',
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
  const { ok, data } = await githubRequest(
    'GET',
    `/repos/${owner}/${repo}/contents/${encodeURIPath(filePath)}?ref=${branch}`,
    token
  );
  return ok && data.sha ? data.sha : null;
}

async function pushFile(token, owner, repo, branch, filePath, content, message) {
  const sha = await getFileSha(token, owner, repo, filePath, branch);
  const body = { message, content, branch };
  if (sha) body.sha = sha;

  return githubRequest(
    'PUT',
    `/repos/${owner}/${repo}/contents/${encodeURIPath(filePath)}`,
    token,
    body
  );
}

async function testConnection(token, owner, repo) {
  return githubRequest('GET', `/repos/${owner}/${repo}`, token);
}

function encodeURIPath(p) {
  return p.split('/').map(encodeURIComponent).join('/');
}

// ─── File helpers ─────────────────────────────────────────────────────────────

function fileToBase64(filePath) {
  const buf = fs.readFileSync(filePath);
  return buf.toString('base64');
}

function formatBytes(bytes) {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

// ─── Log ─────────────────────────────────────────────────────────────────────

function addLog(msg, type = 'info') {
  const logEl = document.getElementById('push-log');
  const entries = document.getElementById('log-entries');
  logEl.classList.remove('hidden');

  const now = new Date().toLocaleTimeString('zh-TW', { hour12: false });
  const div = document.createElement('div');
  div.className = 'log-entry';
  div.innerHTML = `<span class="log-time">${now}</span><span class="log-msg ${type}">${escHtml(msg)}</span>`;
  entries.appendChild(div);
  entries.scrollTop = entries.scrollHeight;
}

function escHtml(str) {
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;');
}

// ─── Settings tab ─────────────────────────────────────────────────────────────

function initSettingsTab() {
  const settings = loadSettings();
  if (settings.token)  document.getElementById('github-token').value  = settings.token;
  if (settings.repo)   document.getElementById('github-repo').value   = settings.repo;
  if (settings.branch) document.getElementById('github-branch').value = settings.branch;
  if (settings.targetPath !== undefined) document.getElementById('github-path').value = settings.targetPath;
  if (settings.useSubfolder) document.getElementById('use-subfolder').checked = true;

  // Toggle password visibility
  document.querySelectorAll('.btn-toggle-pass').forEach(btn => {
    btn.addEventListener('click', () => {
      const input = document.getElementById(btn.dataset.target);
      input.type = input.type === 'password' ? 'text' : 'password';
    });
  });

  // Save settings
  document.getElementById('settings-form').addEventListener('submit', (e) => {
    e.preventDefault();
    const s = collectSettings();
    if (!s.token) return showSettingsStatus('請填入 GitHub Token', 'error');
    if (!s.repo || !s.repo.includes('/')) return showSettingsStatus('請填入正確的 Repository (owner/repo)', 'error');
    saveSettings(s);
    showSettingsStatus('設定已儲存', 'success');
  });

  // Test connection
  document.getElementById('btn-test-connection').addEventListener('click', async () => {
    const s = collectSettings();
    if (!s.token) return showSettingsStatus('請先填入 GitHub Token', 'error');
    if (!s.repo || !s.repo.includes('/')) return showSettingsStatus('請先填入 Repository', 'error');

    const btn = document.getElementById('btn-test-connection');
    btn.disabled = true;
    btn.textContent = '測試中…';
    showSettingsStatus('正在連線到 GitHub…', 'info');

    const [owner, repo] = s.repo.split('/');
    const { ok, data } = await testConnection(s.token, owner, repo);

    btn.disabled = false;
    btn.textContent = '測試連線';

    if (ok) {
      showSettingsStatus(`連線成功！${data.full_name}（${data.private ? '私有' : '公開'}）`, 'success');
    } else {
      showSettingsStatus(`連線失敗：${data.message || '未知錯誤'} (${data.status || ''})`, 'error');
    }
  });
}

function collectSettings() {
  return {
    token: document.getElementById('github-token').value.trim(),
    repo: document.getElementById('github-repo').value.trim(),
    branch: document.getElementById('github-branch').value.trim() || 'main',
    targetPath: document.getElementById('github-path').value.trim(),
    useSubfolder: document.getElementById('use-subfolder').checked,
  };
}

function showSettingsStatus(msg, type) {
  const el = document.getElementById('settings-status');
  el.textContent = msg;
  el.className = `status-msg ${type}`;
  el.classList.remove('hidden');
  setTimeout(() => el.classList.add('hidden'), 5000);
}

// ─── Push tab ─────────────────────────────────────────────────────────────────

let currentItems = [];

async function loadSelectedItems() {
  const noItems = document.getElementById('no-items');
  const container = document.getElementById('items-container');
  const list = document.getElementById('items-list');

  let selected = [];
  try {
    selected = await eagle.item.getSelected();
  } catch (err) {
    console.error('Failed to get selected items:', err);
  }

  if (!selected || selected.length === 0) {
    noItems.classList.remove('hidden');
    container.classList.add('hidden');
    currentItems = [];
    return;
  }

  currentItems = selected;
  noItems.classList.add('hidden');
  container.classList.remove('hidden');

  const countEl = document.getElementById('item-count');
  countEl.textContent = `已選取 ${selected.length} 個項目`;

  list.innerHTML = '';
  selected.forEach((item, i) => {
    const row = buildItemRow(item, i);
    list.appendChild(row);
  });
}

function buildItemRow(item, index) {
  const ext = item.ext || '';
  const size = item.fileSize || item.size || 0;

  const row = document.createElement('div');
  row.className = 'item-row';
  row.dataset.index = index;

  const thumbEl = item.thumbnailUrl
    ? `<img class="item-thumb" src="${escHtml(item.thumbnailUrl)}" alt="" onerror="this.style.display='none'">`
    : `<div class="item-thumb-placeholder">${escHtml(ext.toUpperCase())}</div>`;

  row.innerHTML = `
    ${thumbEl}
    <div class="item-info">
      <div class="item-name" title="${escHtml(item.name)}.${escHtml(ext)}">${escHtml(item.name)}.${escHtml(ext)}</div>
      <div class="item-meta">${size ? formatBytes(size) : ext}</div>
    </div>
    <span class="item-status" data-status></span>
    <button class="item-push-btn" data-index="${index}">推送</button>
  `;

  row.querySelector('.item-push-btn').addEventListener('click', () => pushSingle(index));
  return row;
}

function getItemRow(index) {
  return document.querySelector(`.item-row[data-index="${index}"]`);
}

function setItemState(index, state, msg) {
  const row = getItemRow(index);
  if (!row) return;
  row.className = `item-row ${state}`;
  const statusEl = row.querySelector('[data-status]');
  statusEl.className = `item-status ${state}`;

  const icons = {
    pushing: '⏳',
    done: '✓',
    failed: '✗',
    '': '',
  };
  statusEl.textContent = msg || icons[state] || '';

  const btn = row.querySelector('.item-push-btn');
  if (state === 'pushing') {
    btn.disabled = true;
    btn.textContent = '…';
  } else if (state === 'done') {
    btn.disabled = true;
    btn.textContent = '✓';
  } else {
    btn.disabled = false;
    btn.textContent = '推送';
  }
}

function buildTargetPath(settings, item) {
  const ext = item.ext || '';
  const fileName = `${item.name}.${ext}`;
  let base = settings.targetPath || '';

  if (base && !base.endsWith('/')) base += '/';

  if (settings.useSubfolder && item.folders && item.folders.length > 0) {
    // item.folders is an array of folder IDs; Eagle doesn't expose folder names directly here
    // so we fall back to not using subfolder names unless the item has a folderName property
    const folderName = item.folderName || '';
    if (folderName) base += `${folderName}/`;
  }

  return base + fileName;
}

function buildCommitMessage(items, custom) {
  if (custom && custom.trim()) return custom.trim();
  const names = items.map(i => `${i.name}.${i.ext}`).join(', ');
  const ts = new Date().toISOString().slice(0, 10);
  if (names.length > 80) return `Add ${items.length} images via Eagle [${ts}]`;
  return `Add ${names} via Eagle [${ts}]`;
}

async function pushSingle(index) {
  const item = currentItems[index];
  if (!item) return;
  await doPush([{ item, index }]);
}

async function pushAll() {
  const jobs = currentItems.map((item, index) => ({ item, index }));
  await doPush(jobs);
}

async function doPush(jobs) {
  const settings = loadSettings();

  if (!settings.token) {
    addLog('尚未設定 GitHub Token，請到「設定」頁面填寫', 'error');
    switchTab('settings');
    return;
  }
  if (!settings.repo || !settings.repo.includes('/')) {
    addLog('尚未設定 Repository，請到「設定」頁面填寫', 'error');
    switchTab('settings');
    return;
  }

  const [owner, repo] = settings.repo.split('/');
  const branch = settings.branch || 'main';
  const customMsg = document.getElementById('commit-message').value.trim();

  const pushAllBtn = document.getElementById('btn-push-all');
  pushAllBtn.disabled = true;

  for (const { item, index } of jobs) {
    setItemState(index, 'pushing');

    const filePath = item.filePath || item.path;
    if (!filePath || !fs.existsSync(filePath)) {
      setItemState(index, 'failed', '找不到檔案');
      addLog(`✗ ${item.name}: 找不到本地檔案 (${filePath || '無路徑'})`, 'error');
      continue;
    }

    let content;
    try {
      content = fileToBase64(filePath);
    } catch (err) {
      setItemState(index, 'failed', '讀取失敗');
      addLog(`✗ ${item.name}: 讀取失敗 – ${err.message}`, 'error');
      continue;
    }

    const targetPath = buildTargetPath(settings, item);
    const message = buildCommitMessage([item], customMsg);

    addLog(`⟳ 推送 ${item.name}.${item.ext} → ${targetPath}`, 'info');

    try {
      const { ok, status, data } = await pushFile(settings.token, owner, repo, branch, targetPath, content, message);

      if (ok) {
        setItemState(index, 'done');
        const url = data.content && data.content.html_url ? data.content.html_url : '';
        addLog(`✓ ${item.name}.${item.ext} 推送成功${url ? ` → ${url}` : ''}`, 'success');
      } else {
        setItemState(index, 'failed');
        addLog(`✗ ${item.name}.${item.ext}: ${data.message || '未知錯誤'} (HTTP ${status})`, 'error');
      }
    } catch (err) {
      setItemState(index, 'failed');
      addLog(`✗ ${item.name}.${item.ext}: 網路錯誤 – ${err.message}`, 'error');
    }
  }

  pushAllBtn.disabled = false;
}

// ─── Tab switching ────────────────────────────────────────────────────────────

function switchTab(name) {
  document.querySelectorAll('.tab').forEach(t => t.classList.toggle('active', t.dataset.tab === name));
  document.querySelectorAll('.tab-content').forEach(c => {
    const match = c.id === `tab-${name}`;
    c.classList.toggle('active', match);
    c.classList.toggle('hidden', !match);
  });
}

// ─── Init ─────────────────────────────────────────────────────────────────────

function init() {
  // Tab clicks
  document.querySelectorAll('.tab').forEach(tab => {
    tab.addEventListener('click', () => switchTab(tab.dataset.tab));
  });

  // Refresh buttons
  document.getElementById('btn-refresh').addEventListener('click', loadSelectedItems);
  document.getElementById('btn-refresh-list').addEventListener('click', loadSelectedItems);

  // Push all button
  document.getElementById('btn-push-all').addEventListener('click', pushAll);

  // Clear log
  document.getElementById('btn-clear-log').addEventListener('click', () => {
    document.getElementById('log-entries').innerHTML = '';
    document.getElementById('push-log').classList.add('hidden');
  });

  initSettingsTab();
}

// ─── Eagle lifecycle ──────────────────────────────────────────────────────────

eagle.onPluginCreate(() => {
  init();
  loadSelectedItems();
});

eagle.onPluginShow(() => {
  // Refresh selected items each time the plugin window comes into focus
  loadSelectedItems();
});
