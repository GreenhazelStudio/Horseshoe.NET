using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Cryptography
{
    public static class CryptoUtil
    {
        internal static SymmetricAlgorithm PrepareSymmetricAlgorithmForEncryption(CryptoOptions options)
        {
            options = options ?? new CryptoOptions();
            var algorithm = _PrepareSymmetricAlgorithm(options);

            if (options.UseEmbeddedKIV)
            {
                algorithm.GenerateKey();
                algorithm.GenerateIV();
            }
            return algorithm;
        }

        internal static SymmetricAlgorithm PrepareSymmetricAlgorithmForDecryption(CryptoOptions options, ref byte[] cipherBytes)
        {
            options = options ?? new CryptoOptions();
            var algorithm = _PrepareSymmetricAlgorithm(options);

            var keyLength = algorithm.KeySize / 8;
            var ivLength = algorithm.BlockSize / 8;

            if (options.UseEmbeddedKIV)
            {
                algorithm.IV = CollectionUtil.ScoopOffTheEnd(ref cipherBytes, ivLength);
                algorithm.Key = CollectionUtil.ScoopOffTheEnd(ref cipherBytes, keyLength);
            }
            return algorithm;
        }

        private static SymmetricAlgorithm _PrepareSymmetricAlgorithm(CryptoOptions options)
        {
            if (options.UseEmbeddedKIV && options.Key != null)
            {
                throw new UtilityException("Please leave the symmetric key null when using embedded mode");
            }

            // priority 1 - user-supplied algorithm incl. specifics
            if (options.Algorithm != null)
            {
                return ValidateAndBuildAlgorithm(options.Algorithm, options.Key, options.KeySize, options.BlockSize, options.PaddingMode, options);
            }

            // priority 2 - built-in algorithm incl. specifics (options.Key overrides, if applicable)
            return ValidateAndBuildAlgorithm(Settings.DefaultSymmetricAlgorithm, options.Key ?? Settings.DefaultSymmetricKey, Settings.DefaultSymmetricKeySize, Settings.DefaultSymmetricBlockSize, Settings.DefaultSymmetricPaddingMode, options);
        }

        private static SymmetricAlgorithm ValidateAndBuildAlgorithm (SymmetricAlgorithm algorithm, byte[] key, int? keySize, int? blockSize, PaddingMode? padding, CryptoOptions options)
        {
            if (keySize.HasValue)
            {
                if (algorithm.ValidKeySize(keySize.Value))
                {
                    algorithm.KeySize = keySize.Value;
                }
                else
                {
                    throw new UtilityException("Invalid key size: " + keySize + " -- Valid key sizes for " + algorithm.GetType().Name + ": " + GetDisplayKeySizes(algorithm.LegalKeySizes) + ".");
                }
            }
            if (blockSize.HasValue)
            {
                algorithm.BlockSize = blockSize.Value;
            }
            if (key != null)
            {
                var keyLength = algorithm.KeySize / 8;
                var ivLength = algorithm.BlockSize / 8;
                algorithm.Key = key.PadLeft((byte)0, keyLength, TruncatePolicy.Exception).ToArray();
                algorithm.IV = key.PadLeft((byte)0, ivLength, TruncatePolicy.Simple).ToArray();
            }
            else if (!options.UseEmbeddedKIV)
            {
                throw new UtilityException("A symmetric key is required");
            }
            if (padding.HasValue)
            {
                algorithm.Padding = padding.Value;
            }
            return algorithm;
        }

        private static string GetDisplayKeySizes(KeySizes[] keySizeses)
        {
            var set = new SortedSet<int>();
            foreach(var keySizes in keySizeses)
            {
                for(int i = keySizes.MinSize; i <= keySizes.MaxSize; i += keySizes.SkipSize)
                {
                    set.Add(i);
                }
            }
            return string.Join(", ", set);
        }
    }
}
