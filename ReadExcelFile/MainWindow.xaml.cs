

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Data;

using System.Xml;
using System.Xml.Linq;
using System.IO;
using Path = System.IO.Path;

namespace ReadXmlFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        public MainWindow()
        {
            InitializeComponent();

        }


        public DataTable dt = new DataTable();
        public void makeDataTable()
        {
            dt.Columns.Add("Paczka", typeof(string));
            dt.Columns.Add("Stojak", typeof(string));
            dt.Columns.Add("Sztuk w przegrodzie", typeof(string));
            dt.Columns.Add("Rama", typeof(string));
            dt.Columns.Add("Skrzydło", typeof(string));
            dt.Columns.Add("Inne", typeof(string));


            dtGrid.ItemsSource = dt.DefaultView;

        }
              private void btnOpen_Click(object sender, RoutedEventArgs e)
              {
                  OpenFileDialog openfile = new OpenFileDialog();
                  openfile.DefaultExt = ".xml";
                  openfile.Filter = "(.xml)|*.xml";
                  //openfile.ShowDialog();

                  var browsefile = openfile.ShowDialog();

                  if (browsefile == true)
                  {

                      //MessageBox.Show(System.IO.Path.GetDirectoryName(openfile.FileName));
                      txtFilePath.Text = openfile.FileName;
                      XmlDocument xmlDoc = new XmlDocument();
                      xmlDoc.Load(txtFilePath.Text);

                      XElement xmlToSort = XElement.Load(txtFilePath.Text);

                      //MessageBox.Show(xmlToSort.Attribute("Numer").Value.ToString());
                      /*----------------------------------------------------------------------------*/

                      string source_path = System.IO.Path.GetDirectoryName(openfile.FileName); //@"c:\teren\karol\elwiz\";
                      string output_path = System.IO.Path.Combine(source_path, "out");
                      string nr_opt = xmlToSort.Attribute("Numer").Value.ToString(); //"2843";


                      SortCiecia.Analyze(source_path, output_path, nr_opt); //grupujemy paczki opt

                      /*----------------------------------------------------------------------------*/




                      makeDataTable();

                      XmlDocument xmlDoc2 = new XmlDocument();
                      xmlDoc2.Load(System.IO.Path.Combine(output_path, nr_opt + "sorted.xml"));
                      XmlNodeList nodeList = xmlDoc2.DocumentElement.SelectNodes("Root/Przegroda/Cięcie");

                      XElement xmlToDataTable = XElement.Load(System.IO.Path.Combine(output_path, nr_opt + "sorted.xml"));
                      // MessageBox.Show(System.IO.Path.Combine(output_path, nr_opt + "sorted.xml"));


                      string[] strData = { "", "", "", "", "", "" };


                      //MessageBox.Show(level0Element.Attribute("TypProfili").Value.ToString());
                      foreach (var filepath in Directory.EnumerateFiles(output_path, nr_opt + "*sorted.xml")) // pętla po wszystkich paczkach
                      {
                          foreach (XElement level2Element in xmlToDataTable.Elements("Przegroda"))


                          {

                              int oscCount = 0;
                              int skrCount = 0;
                              int othersCount = 0;

                              strData[1] = level2Element.Attribute("Nazwa").Value.ToString();
                              strData[2] = level2Element.Attribute("Ile").Value.ToString();



                              foreach (XElement level3Element in level2Element.Elements("Cięcie"))
                              {


                                  try
                                  {
                                      switch (level3Element.Attribute("TypProfEn").Value.ToString())
                                      {

                                          case "2":
                                              oscCount += 1; ;
                                              break;
                                          case "3":
                                              skrCount += 1;
                                              break;
                                          default:
                                              othersCount += 1;
                                              break;

                                      }
                                  }
                                  catch (Exception ex)
                                  {

                                  }


                              }
                              strData[0] = Path.GetFileNameWithoutExtension(filepath).Replace("sorted", "");
                              strData[3] = (oscCount.ToString());
                              strData[4] = (skrCount.ToString());
                              strData[5] = (othersCount.ToString());


                              dt.Rows.Add(strData);

                          }


                      }
                  }


              }
    }






}



