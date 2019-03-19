using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace KCS.Common
{
    public static class Encryptor
    {
        static public String Decrypt(string str)
        {
            var IVString = "45287112549354892144548565456541";
            var KeyString = "$$kcs$1234$kcs$$";
            var sRet = "";

            try
            {
                var encoding = new UTF8Encoding();
                var Key = encoding.GetBytes(KeyString);
                var IV = encoding.GetBytes(IVString);


                byte[] cypher = Convert.FromBase64String(str);
                using (var rj = new RijndaelManaged())
                {
                    try
                    {
                        rj.Padding = PaddingMode.PKCS7;
                        rj.Mode = CipherMode.CBC;
                        rj.KeySize = 256;
                        rj.BlockSize = 256;
                        rj.Key = Key;
                        rj.IV = IV;
                        var ms = new MemoryStream(cypher);

                        using (var cs = new CryptoStream(ms, rj.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                sRet = sr.ReadLine();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        rj.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return sRet;
        }

        static public String Encrypt(string str)
        {
            var IVString = "45287112549354892144548565456541";
            var KeyString = "$$kcs$1234$kcs$$";
            var cypher = str;
            // var sRet = "";

            // var encoding = new UTF8Encoding();
            var Key = Encoding.ASCII.GetBytes(KeyString);
            var IV = Encoding.ASCII.GetBytes(IVString);



            using (var rj = new RijndaelManaged())
            {
                try
                {
                    rj.Padding = PaddingMode.PKCS7;
                    rj.Mode = CipherMode.CBC;
                    rj.KeySize = 256;
                    rj.BlockSize = 256;
                    rj.Key = Key;
                    rj.IV = IV;
                    var ms = new MemoryStream();

                    using (var cs = new CryptoStream(ms, rj.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                    {
                        //var csEncrypt = new CryptoStream new CryptoStream(ms, cs, CryptoStreamMode.Write);

                        var toEncrypt = Encoding.ASCII.GetBytes(cypher);

                        cs.Write(toEncrypt, 0, toEncrypt.Length);
                        cs.FlushFinalBlock();

                        var encrypted = ms.ToArray();

                        return (Convert.ToBase64String(encrypted));
                    }
                }
                catch (Exception ex)
                {
                    return "";
                }
                finally
                {
                    rj.Clear();
                }
            }


        }
    }
}