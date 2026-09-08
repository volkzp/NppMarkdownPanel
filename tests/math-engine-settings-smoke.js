const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');

function read(relativePath) {
    return fs.readFileSync(path.join(__dirname, '..', relativePath), 'utf8');
}

const settings = read('NppMarkdownPanel/Entities/Settings.cs');
const controller = read('NppMarkdownPanel/MarkdownPanelController.cs');
const form = read('NppMarkdownPanel/Forms/SettingsForm.cs');
const preview = read('NppMarkdownPanel/Forms/MarkdownPreviewForm.cs');
const webview = read('Webview2Viewer/Webview2WebbrowserControl.cs');

assert.match(settings, /MathRenderingEngine = MATH_RENDERING_ENGINE_KATEX/);
assert.match(controller, /"MathRenderingEngine"[\s\S]*MATH_RENDERING_ENGINE_KATEX/);
assert.match(form, /comboMathRenderingEngine\.SelectedIndex == 1[\s\S]*MATHJAX[\s\S]*KATEX/);
assert.match(preview, /IsMathRenderingEngineMathJax\(\)/);
assert.match(preview, /KaTeXAssets\.Current/);
assert.match(webview, /window\.clearTypesetMath/);
assert.match(webview, /currentMathRenderer != pageMathRenderer/);

console.log('Formula renderer selection wiring passed.');
