using System;
using System.Collections.Generic;
using System.Linq;
using static MorseCode;


/// <summary>
/// Morse code signal enum.
/// </summary>
public enum EMorseSignal
{
    None, // Must be the first enum value, to allow default initialisation of MorseChar.
    Dot,
    Dash,
    CharBreak,
    WordBreak,
}


public struct MorseChar : IEquatable<MorseChar>
{
    /// <summary>
    /// Returns a morse char with all sigs set to None.
    /// </summary>
    public static MorseChar Empty => new();

    public EMorseSignal Sig1;
    public EMorseSignal Sig2;
    public EMorseSignal Sig3;
    public EMorseSignal Sig4;
    public EMorseSignal Sig5;


    public MorseChar(EMorseSignal c1, EMorseSignal c2 = EMorseSignal.None, EMorseSignal c3 =
        EMorseSignal.None, EMorseSignal c4 = EMorseSignal.None, EMorseSignal c5 = EMorseSignal.None)
    {
        Sig1 = c1;
        Sig2 = c2;
        Sig3 = c3;
        Sig4 = c4;
        Sig5 = c5;
    }


    public bool Equals(MorseChar other)
    {
        if (Sig1 == other.Sig1 &&
            Sig2 == other.Sig2 &&
            Sig3 == other.Sig3 &&
            Sig4 == other.Sig4 &&
            Sig5 == other.Sig5) return true;
        return false;
    }


    /// <summary>
    /// Adds a morse signal onto the end of the character (the first empty signal is overwritten).
    /// </summary>
    /// <returns>`true` if the operation was successful, `false` if the char was full.</returns>
    public bool AddSig(EMorseSignal sig)
    {
        if (Sig1 == EMorseSignal.None)
            Sig1 = sig;
        else if (Sig2 == EMorseSignal.None)
            Sig2 = sig;
        else if (Sig3 == EMorseSignal.None)
            Sig3 = sig;
        else if (Sig4 == EMorseSignal.None)
            Sig4 = sig;
        else if (Sig5 == EMorseSignal.None)
            Sig5 = sig;
        else
            return false;
        return true;
    }


    /// <summary>
    /// Translates a morse code character into a string of '-' and '.' characters.
    /// </summary>
    /// <returns>The translated - and . character string.</returns>
    public override string ToString()
    {
        string morseCharVisualised =
            VisualiseMorseSignal(Sig1)
            + VisualiseMorseSignal(Sig2)
            + VisualiseMorseSignal(Sig3)
            + VisualiseMorseSignal(Sig4)
            + VisualiseMorseSignal(Sig5);

        return new(morseCharVisualised);
    }
}


public class MorseWord : IEquatable<MorseWord>
{
    public List<MorseChar> Items;


    public MorseWord()
    {
        Items = new();
    }


    public MorseWord(MorseWord other)
    {
        Items = new(other.Items);
    }


    public void AddChar(MorseChar ch)
    {
        Items.Add(ch);
    }


    public bool Equals(MorseWord other)
    {
        if (other == null) return false;
        if (Items.SequenceEqual(other.Items)) return true;
        return false;
    }


    /// <summary>
    /// Translates a morse code string into a string of '-', '.' and ' ' characters.
    /// </summary>
    /// <returns>The translated - . and space character string.</returns>
    public override string ToString()
    {
        string ret = "";
        foreach (MorseChar ch in Items)
        {
            ret += ch.ToString() + '/';
        }

        // Remove trailing '/'
        if (ret.Length > 0) return ret.Remove(ret.Length - 1, 1);
        return ret;
    }
}


public class MorsePhrase : IEquatable<MorsePhrase>
{
    public List<MorseWord> Items;


    public MorsePhrase()
    {
        Items = new();
    }


    public MorsePhrase(MorsePhrase other)
    {
        Items = new(other.Items);
    }


    public void AddWord(MorseWord word)
    {
        Items.Add(word);
    }


    public bool Equals(MorsePhrase other)
    {
        if (other == null) return false;
        if (Items.SequenceEqual(other.Items)) return true;
        return false;
    }


    /// <summary>
    /// Translates a morse code string into a string of '-', '.' and ' ' characters.
    /// </summary>
    /// <returns>The translated - . and space character string.</returns>
    public override string ToString()
    {
        string ret = "";
        foreach (MorseWord ch in Items)
        {
            ret += ch.ToString() + "//";
        }

        // Remove trailing '//'
        if (ret.Length > 0) return ret.Remove(ret.Length - 2, 2);
        return ret;
    }
}


public static class MorseCode
{
    // ------------------------------------ STATIC VARS -------------------------------------------


    private static readonly Dictionary<char, MorseChar> s_charToMorseChar = new Dictionary<char, MorseChar>
    {
        { 'A', new(EMorseSignal.Dot, EMorseSignal.Dash) },
        { 'B', new(EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot) },
        { 'C', new(EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dot) },
        { 'D', new(EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dot) },
        { 'E', new(EMorseSignal.Dot) },
        { 'F', new(EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dot) },
        { 'G', new(EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dot) },
        { 'H', new(EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot) },
        { 'I', new(EMorseSignal.Dot, EMorseSignal.Dot) },
        { 'J', new(EMorseSignal.Dot, EMorseSignal.Dash,EMorseSignal.Dash, EMorseSignal.Dash) },
        { 'K', new(EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dash) },
        { 'L', new(EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dot) },
        { 'M', new(EMorseSignal.Dash, EMorseSignal.Dash) },
        { 'N', new(EMorseSignal.Dash, EMorseSignal.Dot) },
        { 'O', new(EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash) },
        { 'P', new(EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dot) },
        { 'Q', new(EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dash) },
        { 'R', new(EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dot) },
        { 'S', new(EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot) },
        { 'T', new(EMorseSignal.Dash) },
        { 'U', new(EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dash) },
        { 'V', new(EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dash) },
        { 'W', new(EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dash) },
        { 'X', new(EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dash) },
        { 'Y', new(EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dash) },
        { 'Z', new(EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dot) },
        { '1', new(EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash) },
        { '2', new(EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash) },
        { '3', new(EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dash, EMorseSignal.Dash) },
        { '4', new(EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dash) },
        { '5', new(EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot) },
        { '6', new(EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot) },
        { '7', new(EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dot, EMorseSignal.Dot) },
        { '8', new(EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dot, EMorseSignal.Dot) },
        { '9', new(EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dot) },
        { '0', new(EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash, EMorseSignal.Dash) },
    };


    // ------------------------------------ TRANSLATION -------------------------------------------

    /// <summary>
    /// Translates an English character into a morse code string.
    /// </summary>
    /// <returns>The morse code string, or null if none were found.</returns>
    public static MorseChar EnglishCharToMorseChar(char englishChar)
    {
        if (s_charToMorseChar.ContainsKey(englishChar))
            return s_charToMorseChar[englishChar];
        else
            return MorseChar.Empty;
    }


    /// <summary>
    /// Translates a morse code string into an English character, by a linear search.
    /// Morse code strings have a 1:1 mapping to English characters.
    /// </summary>
    /// <returns>The English character, if one was found, otherwise null character.</returns>
    public static string MorseCharToEnglishChar(MorseChar morseChar)
    {
        foreach (var kvp in s_charToMorseChar)
        {
            if (kvp.Value.Equals(morseChar))
            {
                return $"{kvp.Key}";
            }
        }

        return "";
    }


    public static MorseWord EnglishWordToMorseWord(string englishWord)
    {
        MorseWord ret = new();
        foreach (char englishChar in englishWord)
        {
            ret.AddChar(EnglishCharToMorseChar(englishChar));
        }
        return ret;
    }


    public static string MorseWordToEnglishWord(MorseWord morseWord)
    {
        string ret = "";
        foreach (MorseChar morseChar in morseWord.Items)
        {
            ret += MorseCharToEnglishChar(morseChar);
        }
        return ret;
    }


    public static MorsePhrase EnglishPhraseToMorsePhrase(string englishPhrase)
    {
        MorsePhrase ret = new();

        // Split the phrase into words by spaces.
        foreach (string englishWord in englishPhrase.Split(' '))
        {
            ret.AddWord(EnglishWordToMorseWord(englishWord));
        }
        return ret;
    }


    public static string MorsePhraseToEnglishPhrase(MorsePhrase morsePhrase)
    {
        string ret = "";
        foreach (MorseWord morseWord in morsePhrase.Items)
        {
            ret += MorseWordToEnglishWord(morseWord);
        }
        return ret;
    }


    // ------------------------------------ VISIBILITY -------------------------------------------

    /// <summary>
    /// Translates a morse code character into a "-" or "." if possible, otherwise "".
    /// </summary>
    /// <returns>The "-", ".", or "".</returns>
    public static string VisualiseMorseSignal(EMorseSignal morseSignal)
    {
        return morseSignal switch
        {
            EMorseSignal.Dot => ".",
            EMorseSignal.Dash => "-",
            _ => "",
        };
    }
}