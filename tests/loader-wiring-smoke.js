const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

const controller = fs.readFileSync(
    path.join(__dirname, '..', 'NppMarkdownPanel', 'MarkdownPanelController.cs'),
    'utf8');

const resolver = controller.match(
    /private Assembly CurrentDomain_AssemblyResolve\([\s\S]*?\n        }/);

assert.ok(resolver, 'Assembly resolver was not found');
assert.match(resolver[0], /Assembly\.UnsafeLoadFrom\(module\.FullName\)/);
assert.doesNotMatch(resolver[0], /Assembly\.LoadFrom\(/);

console.log('Mark-of-the-Web-safe assembly resolver wiring passed.');
