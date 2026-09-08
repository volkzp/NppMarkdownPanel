const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const { JSDOM } = require('jsdom');

async function main() {
    const dom = new JSDOM('<!doctype html><html><body><main id="preview">\\(x^2\\)</main></body></html>', {
        pretendToBeVisual: true,
        runScripts: 'outside-only',
        url: 'https://markdownpanel.local/'
    });
    const { window } = dom;
    window.MathJax = {
        tex: {
            inlineMath: [['\\(', '\\)'], ['$', '$']],
            displayMath: [['\\[', '\\]'], ['$$', '$$']],
            processEscapes: true
        },
        svg: { fontCache: 'local' },
        startup: { typeset: false }
    };

    const bundle = fs.readFileSync(
        path.join(__dirname, '..', 'NppMarkdownPanel', 'mathjax', 'tex-svg-full.js'),
        'utf8'
    );
    window.eval(bundle);
    await window.MathJax.startup.promise;

    const preview = window.document.getElementById('preview');
    await window.MathJax.typesetPromise([preview]);
    assert.equal(preview.querySelectorAll('mjx-container svg').length, 1);

    window.MathJax.typesetClear([preview]);
    preview.innerHTML = '$$\\frac{a}{b}$$';
    await window.MathJax.typesetPromise([preview]);
    assert.equal(preview.querySelectorAll('mjx-container[display="true"] svg').length, 1);

    console.log('Local MathJax initial and incremental rendering passed.');
}

main().catch(error => {
    console.error(error);
    process.exitCode = 1;
});
