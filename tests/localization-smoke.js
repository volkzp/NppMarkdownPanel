const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

function read(relativePath) {
    return fs.readFileSync(path.join(__dirname, '..', relativePath), 'utf8');
}

function keys(xml) {
    return [...xml.matchAll(/<text key="([^"]+)">/g)].map(match => match[1]).sort();
}

const localization = read('NppMarkdownPanel/PluginLocalization.cs');
const english = read('NppMarkdownPanel/localization/english.xml');
const russian = read('NppMarkdownPanel/localization/russian.xml');
const sourceFiles = [
    'NppMarkdownPanel/Forms/AboutForm.cs',
    'NppMarkdownPanel/Forms/MarkdownPreviewForm.cs',
    'NppMarkdownPanel/Forms/SettingsForm.cs',
    'NppMarkdownPanel/MarkdownPanelController.cs',
    'NppMarkdownPanel/Utils.cs'
].map(read);

assert.deepEqual(keys(russian), keys(english), 'English and Russian key sets differ');
assert.match(localization, /Path\.Combine\(assemblyDirectory, "localization", language \+ "\.xml"\)/);
assert.match(localization, /NPPM_GETNATIVELANGFILENAME/);
assert.match(localization, /NPPM_GETCURRENTNATIVELANGENCODING/);
assert.match(russian, /Предпросмотр печати/);
assert.match(russian, /Движок формул/);

const definedKeys = new Set(keys(english));
const referencedKeys = new Set(sourceFiles.flatMap(source =>
    [...source.matchAll(/"((?:menu|settings|preview|filter|error|about|panel)\.[a-z_]+)"/g)]
        .map(match => match[1])));

for (const key of referencedKeys) {
    assert.ok(definedKeys.has(key), `Missing localization key: ${key}`);
}

assert.ok(sourceFiles.every(source => !/[А-Яа-яЁё]/.test(source)), 'C# UI source still contains inline Russian text');
assert.match(sourceFiles[1], /item\.AutoToolTip = false/);
assert.match(sourceFiles[1], /tbPreview\.Invalidate\(true\)/);

console.log(`External English/Russian localization passed for ${definedKeys.size} keys.`);
