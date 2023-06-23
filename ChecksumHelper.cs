using System.Security.Cryptography;
using Spectre.Console;

namespace VmChamp;

public class ChecksumHelper
{
    private string ConvertHashToString(byte[] hash)
    {
        return BitConverter.ToString(hash).Replace("-", "");
    }

    public bool CompareHash(byte[] firstHash, byte[] secondHash)
    {
        if (ConvertHashToString(firstHash) == ConvertHashToString(secondHash)) return true;
        else return false;
    }
    
    public string GetSHA1(string inputFile)
    {
        using (var sha1 = SHA1.Create())
        using (var stream = File.OpenRead(inputFile))
            return ConvertHashToString(sha1.ComputeHash(stream));
    }

    public string GetSHA256(string inputFile)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(inputFile))
            return ConvertHashToString(sha256.ComputeHash(stream));
    }

    public string GetSHA512(string inputFile)
    {
        using (var sha512 = SHA512.Create())
        using (var stream = File.OpenRead(inputFile))
            return ConvertHashToString(sha512.ComputeHash(stream));
            
            
    }
    
    public bool ReadChecksum(string checksumInputFile, string hash)
    {
        ;
        if (!File.Exists(checksumInputFile))
        {
            return false;
        }
        foreach (string line in File.ReadLines(checksumInputFile))
        {
            string[] compare;
            compare = line.Split(" ");
            
            
            for(int i = 0; i < compare.Length; i++)
            {
                if ((compare[i].ToLower()) == (hash.ToLower()))
                {
                    return true;
                }
            }
            

        }

        return false;
    }

    public int ChecksumCheck(DistroInfo distroInfo, FileInfo targetFile, FileInfo checksumFile)
    {
       switch (distroInfo.ChecksumType)
        {
            case "sha1":
                if (ReadChecksum(checksumFile.ToString(), GetSHA1(targetFile.FullName)))
                {
                    return 1;
                }

                break;


            case "sha256":
                if (ReadChecksum(checksumFile.ToString(), GetSHA256(targetFile.FullName)))
                {
                    return 1;
                }
                break;

            case "sha512":
                if (ReadChecksum(checksumFile.ToString(), GetSHA512(targetFile.FullName)))
                {
                    return 1;
                }

                break;


            default:
                return 2;
        }

        return 0;

    }

}