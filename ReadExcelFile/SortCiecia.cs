using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ReadXmlFile
{
  public class MultiValueDictionary<TKey, TValue> : SortedDictionary<TKey, List<TValue>>
  {
    public static readonly List<TValue> EmptySet = new List<TValue>();
    public void Add(TKey key, TValue value)
    {
      List<TValue> list;
      if (!TryGetValue(key, out list))
        base.Add(key, list = new List<TValue>());

      list.Add(value);
    }

    public new List<TValue> this[TKey key]
    {
      get
      {
        List<TValue> list;
        if (!TryGetValue(key, out list))
          list = new List<TValue>();

        return list;
      }

      set { base[key] = value; }
    }
  }

  public class SortCiecia
  {
    public static void Analyze(string source_path, string output_path, string nr_opt)
    {
           
      if (!Directory.Exists(output_path))
        Directory.CreateDirectory(output_path);

      foreach (var filepath in Directory.EnumerateFiles(source_path, nr_opt + "*.xml")) // pętla po wszystkich paczkach
      {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filepath);
        if (fileNameWithoutExtension.EndsWith("z")) continue; // pominięcie pliku od zgrzewarki

        MultiValueDictionary<string, XElement> dict = new MultiValueDictionary<string, XElement>();

        XElement src_root = XElement.Load(filepath);
        foreach (var ciecie in src_root.Descendants("Cięcie"))
        {
          string p = ciecie.Attribute("Przegroda").Value;
          if (string.IsNullOrEmpty(p)) p = "0";

          dict.Add (ciecie.Attribute("Stojak").Value + "|" + int.Parse(p).ToString("00"), ciecie);
        }

        XDocument xdoc = new XDocument();
        XElement out_root = new XElement("Root");
        foreach (var list in dict)
        {
          XElement przegroda = new XElement("Przegroda");
          przegroda.Add(new XAttribute("Nazwa", list.Key));
          przegroda.Add(new XAttribute("Ile", list.Value.Count));
          foreach (var ciecie in list.Value)
          {
            przegroda.Add(ciecie);
          }
          out_root.Add(przegroda);
        }
        xdoc.Add(out_root);
        xdoc.Save(Path.Combine (output_path, fileNameWithoutExtension + "sorted.xml"));
      }
    }
  }
}
