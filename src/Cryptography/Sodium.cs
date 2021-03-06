using System;
using static Interop.Libsodium;

namespace NSec.Cryptography
{
    internal static class Sodium
    {
        private static readonly Lazy<bool> s_initialized = new Lazy<bool>(new Func<bool>(InitializeCore));

        public static void Initialize()
        {
            if (!s_initialized.Value)
            {
                throw Error.Cryptographic_InitializationFailed();
            }
        }

        public static bool TryInitialize()
        {
            return s_initialized.Value;
        }

        private static bool InitializeCore()
        {
            // sodium_init() returns 0 on success, -1 on failure, and 1 if the
            // library had already been initialized. We call sodium_init() only
            // once, but if another library p/invokes into libsodium it might
            // have already been initialized.
            return sodium_init() >= 0
                && sodium_library_version_major() == SODIUM_LIBRARY_VERSION_MAJOR
                && sodium_library_version_minor() == SODIUM_LIBRARY_VERSION_MINOR;
        }
    }
}
