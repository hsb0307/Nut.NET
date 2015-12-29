using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Nut.Core.License {
    public class DefaultCryptoProvider : ICryptoProvider {


        public string Encrypt(string clearText) {
            using (var sha1 = SHA1.Create())
            using (var mstm = new MemoryStream())
            using (var crystm = new CryptoStream(mstm, sha1, CryptoStreamMode.Write)) {
                var data = Encoding.Unicode.GetBytes(clearText);

                crystm.Write(data, 0, data.Length);
                crystm.FlushFinalBlock();

               return BitConverter.ToString(sha1.Hash);
            }
        }
    }
}
