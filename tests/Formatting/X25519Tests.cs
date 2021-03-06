using System;
using System.Text;
using NSec.Cryptography;
using NSec.Cryptography.Formatting;
using Xunit;

namespace NSec.Tests.Formatting
{
    public static class X25519Tests
    {
        private static readonly byte[] s_oid = { 0x2B, 0x65, 0x6E };

        [Fact]
        public static void PkixPrivateKey()
        {
            var a = new X25519();
            var b = Utilities.RandomBytes.Slice(0, a.PrivateKeySize);

            using (var k = Key.Import(a, b, KeyBlobFormat.RawPrivateKey, KeyExportPolicies.AllowPlaintextExport))
            {
                var blob = k.Export(KeyBlobFormat.PkixPrivateKey);

                var reader = new Asn1Reader(blob);
                reader.BeginSequence();
                Assert.Equal(0, reader.Integer32());
                reader.BeginSequence();
                Assert.Equal(s_oid, reader.ObjectIdentifier().ToArray());
                reader.End();
                var curvePrivateKey = reader.OctetString();
                reader.End();
                Assert.True(reader.SuccessComplete);

                reader = new Asn1Reader(curvePrivateKey);
                Assert.Equal(b.ToArray(), reader.OctetString().ToArray());
                Assert.True(reader.SuccessComplete);
            }
        }

        [Fact]
        public static void PkixPrivateKeyText()
        {
            var a = new X25519();
            var b = Utilities.RandomBytes.Slice(0, a.PrivateKeySize);

            using (var k = Key.Import(a, b, KeyBlobFormat.RawPrivateKey, KeyExportPolicies.AllowPlaintextExport))
            {
                var expected = Encoding.UTF8.GetBytes(
                    "-----BEGIN PRIVATE KEY-----\r\n" +
                    Convert.ToBase64String(k.Export(KeyBlobFormat.PkixPrivateKey)) + "\r\n" +
                    "-----END PRIVATE KEY-----\r\n");

                var actual = k.Export(KeyBlobFormat.PkixPrivateKeyText);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public static void PkixPublicKey()
        {
            var a = new X25519();
            var b = Utilities.RandomBytes.Slice(0, a.PrivateKeySize);

            using (var k = Key.Import(a, b, KeyBlobFormat.RawPrivateKey))
            {
                var publicKeyBytes = k.Export(KeyBlobFormat.RawPublicKey);
                var blob = k.Export(KeyBlobFormat.PkixPublicKey);

                var reader = new Asn1Reader(blob);
                reader.BeginSequence();
                reader.BeginSequence();
                Assert.Equal(s_oid, reader.ObjectIdentifier().ToArray());
                reader.End();
                Assert.Equal(publicKeyBytes, reader.BitString().ToArray());
                reader.End();
                Assert.True(reader.SuccessComplete);
            }
        }

        [Fact]
        public static void PkixPublicKeyText()
        {
            var a = new X25519();
            var b = Utilities.RandomBytes.Slice(0, a.PrivateKeySize);

            using (var k = Key.Import(a, b, KeyBlobFormat.RawPrivateKey))
            {
                var expected = Encoding.UTF8.GetBytes(
                    "-----BEGIN PUBLIC KEY-----\r\n" +
                    Convert.ToBase64String(k.Export(KeyBlobFormat.PkixPublicKey)) + "\r\n" +
                    "-----END PUBLIC KEY-----\r\n");

                var actual = k.Export(KeyBlobFormat.PkixPublicKeyText);

                Assert.Equal(expected, actual);
            }
        }
    }
}
