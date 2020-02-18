using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using Horseshoe.NET.Collections;

namespace Horseshoe.NET.Cryptography
{
    internal static class CryptoUtil
    {
        internal static SymmetricAlgorithm BuildSymmetricAlgorithmForEncryptionEmbeddedKIV(CryptoOptions options)
        {
            var algorithm = BuildSymmetricAlgorithm(options);

            algorithm.GenerateKey();
            algorithm.GenerateIV();

            return algorithm;
        }

        internal static SymmetricAlgorithm BuildSymmetricAlgorithmForDecryptionEmbeddedKIV(CryptoOptions options, ref byte[] cipherBytes)
        {
            var algorithm = BuildSymmetricAlgorithm(options);
            var keyLength = algorithm.KeySize / 8;
            var ivLength = algorithm.BlockSize / 8;

            algorithm.IV = CollectionUtil.ScoopOffTheEnd(ref cipherBytes, ivLength);
            algorithm.Key = CollectionUtil.ScoopOffTheEnd(ref cipherBytes, keyLength);

            return algorithm;
        }

        internal static SymmetricAlgorithm BuildSymmetricAlgorithm(CryptoOptions options)
        {
            options = options ?? new CryptoOptions();

            if (options.UseEmbeddedKIV && options.Key != null)
            {
                throw new UtilityException("Please leave the symmetric key null when using embedded mode");
            }

            // priority 1 - user-supplied (via 'options')
            // priority 2 - settings (app|web.config / default) - any single default setting can be substituted for a user-supplied setting
            var algorithm = BuildSymmetricAlgorithm
            (
                options.Algorithm ?? CryptoSettings.DefaultSymmetricAlgorithm,
                options.Key,
                options.AutoPadKey,
                options.IV,
                options.AutoPopulateIVFromKey,
                options.BlockSize,
                options.Mode,
                options.Padding
            );

            return algorithm;
        }

        public static SymmetricAlgorithm BuildSymmetricAlgorithm(SymmetricAlgorithm algorithm, byte[] key, bool autoPadKey, byte[] iv, bool autoPopulateIVFromKey, int? blockSize, CipherMode? mode, PaddingMode? padding)
        {
            if (algorithm == null) return null;
            var validKeyLengths = algorithm.GetValidKeyLengths();
            var validBlockLengths = algorithm.GetValidBlockLengths();
            if (key != null)
            {
                if (autoPadKey)
                {
                    if (key.Length > validKeyLengths.Max())
                    {
                        throw new ValidationException("Key exceeds max allowable size.  Detected size: " + key.Length + ".  Valid sizes: " + string.Join(", ", validKeyLengths));
                    }
                    var targetLength = validKeyLengths.First(ln => ln >= key.Length);
                    key = key.PadLeft((byte)0, targetLength).ToArray();
                }
                else if (!key.Length.In(validKeyLengths))
                {
                    throw new ValidationException("Invalid key size: " + key.Length + ".  Valid sizes: " + string.Join(", ", validKeyLengths));
                }
                algorithm.Key = key;
            }
            if (iv != null) 
            {
                if (!iv.Length.In(validBlockLengths))
                {
                    throw new ValidationException("Invalid IV size: " + iv.Length + ".  Valid sizes: " + string.Join(", ", validBlockLengths));
                }
                algorithm.IV = iv;
            }
            else if (key != null && autoPopulateIVFromKey)
            {
                algorithm.IV = key.PadLeft((byte)0, algorithm.BlockSize / 8, TruncatePolicy.Simple).ToArray();
            }
            if (blockSize.HasValue)
            {
                if (!blockSize.Value.In(validBlockLengths))
                {
                    throw new ValidationException("Invalid block size: " + blockSize.Value + ".  Valid sizes: " + string.Join(", ", validBlockLengths));
                }
                algorithm.BlockSize = blockSize.Value;
            }
            if (mode.HasValue) algorithm.Mode = mode.Value;
            if (padding.HasValue) algorithm.Padding = padding.Value;
            return algorithm;
        }
    }
}
