using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nut.Core.Caching {

    public interface ISignals : IVolatileProvider {
        void Trigger<T>(T signal);
        IVolatileToken When<T>(T signal);
    }

    public class Signals : ISignals {
        readonly IDictionary<object, Token> _tokens = new Dictionary<object, Token>();

        public void Trigger<T>(T signal) {
            lock (_tokens) {
                Token token;
                if (_tokens.TryGetValue(signal, out token)) {
                    _tokens.Remove(signal);
                    token.Trigger();
                }

                if (signal is string) {
                    string pattern = signal as String;
                    var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    var keysToRemove = new List<String>();

                    foreach (var item in _tokens.Keys)
                        if (regex.IsMatch(item.ToString()))
                            keysToRemove.Add((String)item);

                    foreach (string key in keysToRemove) {
                        if (_tokens.TryGetValue(signal, out token)) {
                            _tokens.Remove(signal);
                            token.Trigger();
                        }
                    }
                }
            }
        }

        public IVolatileToken When<T>(T signal) {
            lock (_tokens) {
                Token token;
                if (!_tokens.TryGetValue(signal, out token)) {
                    token = new Token();
                    _tokens[signal] = token;
                }
                return token;
            }
        }


        class Token : IVolatileToken {
            public Token() {
                IsCurrent = true;
            }
            public bool IsCurrent { get; private set; }
            public void Trigger() { IsCurrent = false; }
        }
    }
}
