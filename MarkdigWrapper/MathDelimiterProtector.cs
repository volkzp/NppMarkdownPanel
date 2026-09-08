using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace MarkdigWrapper
{
    internal static class MathDelimiterProtector
    {
        private static readonly Regex MathExpression = new Regex(
            @"(?<!\\)(\$\$(?<displayDollar>[\s\S]+?)(?<!\\)\$\$|\\\[(?<displayBracket>[\s\S]+?)\\\]|\\\((?<inlineParen>[^\r\n]+?)\\\)|\$(?<inlineDollar>[^\r\n$]+?)(?<!\\)\$)",
            RegexOptions.Compiled);

        internal sealed class ProtectedMarkdown
        {
            private readonly IDictionary<string, string> expressions;

            internal ProtectedMarkdown(string markdown, IDictionary<string, string> expressions)
            {
                Markdown = markdown;
                this.expressions = expressions;
            }

            internal string Markdown { get; }

            internal string RestoreIntoHtml(string html)
            {
                foreach (var expression in expressions)
                    html = html.Replace(expression.Key, WebUtility.HtmlEncode(expression.Value));

                return html;
            }
        }

        internal static ProtectedMarkdown Protect(string markdown)
        {
            var expressions = new Dictionary<string, string>();
            var documentToken = Guid.NewGuid().ToString("N");
            var index = 0;

            var protectedText = MathExpression.Replace(markdown ?? String.Empty, match =>
            {
                var token = "NPPMATH" + documentToken + "X" + index++ + "Z";
                expressions[token] = match.Value;
                return token;
            });

            return new ProtectedMarkdown(protectedText, expressions);
        }
    }
}
