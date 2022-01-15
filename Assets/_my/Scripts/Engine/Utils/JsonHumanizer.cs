using System.Linq;
using System.Text;
using Smr.Extensions;

namespace Smr.Utils {
    public static class JsonHumanizer {

        public static string AddIndentsToJson(string json) {
            var sb = new StringBuilder();

            int indent = 0;
            bool quoted = false;
            const string indentStr = "    ";

            for (var i = 0; i < json.Length; i++) {
                var ch = json[i];
                switch (ch) {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted) {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(indentStr));
                        }

                        break;
                    case '}':
                    case ']':
                        if (!quoted) {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(indentStr));
                        }
                        sb.Append(ch);

                        break;
                    case '"':
                        sb.Append(ch);
                        bool escaped = false;
                        var index = i;
                        while (index > 0 && json[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;

                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted) {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(item => sb.Append(indentStr));
                        }

                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");

                        break;
                    default:
                        sb.Append(ch);

                        break;
                }
            }

            return sb.ToString();
        }

    }
}