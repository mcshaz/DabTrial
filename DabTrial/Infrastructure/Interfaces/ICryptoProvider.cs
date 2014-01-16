using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Infrastructure.Interfaces
{
    public interface ICryptoProvider
    {
        string Encrypt(string unencrypted);
        string Decrypt(string encrypted);
        string[] PossibleEncryptionValues(string unencrypted);
        double SaltingCombinations();
    }
}