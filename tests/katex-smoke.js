const assert = require('node:assert/strict');
const path = require('node:path');
const katex = require(path.join(__dirname, '..', 'NppMarkdownPanel', 'katex', 'katex.min.js'));

const expressions = [
    { tex: String.raw`x^2`, displayMode: false },
    { tex: String.raw`\frac{a}{b}`, displayMode: true },
    { tex: String.raw`V_{\text{изменения}} \sim \frac{Intent \times Awareness}{Noise \times Scale}`, displayMode: true }
];

for (const expression of expressions) {
    const html = katex.renderToString(expression.tex, {
        displayMode: expression.displayMode,
        throwOnError: true,
        strict: 'warn'
    });
    assert.match(html, /class="katex"/);
    assert.match(html, /<math/);
}

console.log(`Local KaTeX rendering passed for ${expressions.length} formulas.`);
