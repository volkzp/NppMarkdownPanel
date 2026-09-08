const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

function read(relativePath) {
    return fs.readFileSync(path.join(__dirname, '..', relativePath), 'utf8');
}

const localization = read('NppMarkdownPanel/PluginLocalization.cs');
const preview = read('NppMarkdownPanel/Forms/MarkdownPreviewForm.cs');
const settings = read('NppMarkdownPanel/Forms/SettingsForm.cs');
const controller = read('NppMarkdownPanel/MarkdownPanelController.cs');

assert.match(localization, /NPPM_GETNATIVELANGFILENAME/);
assert.match(localization, /NPPM_GETCURRENTNATIVELANGENCODING/);
assert.match(preview, /Просмотр Markdown/);
assert.match(preview, /Предпросмотр печати/);
assert.match(preview, /Оглавление/);
assert.match(settings, /Настройки панели Markdown/);
assert.match(settings, /Автоматически открывать панель/);
assert.match(controller, /NPPN_NATIVELANGCHANGED/);
assert.match(controller, /Показать\/скрыть &панель/);

console.log('Russian interface localization wiring passed.');
