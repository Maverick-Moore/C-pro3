using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Image_Info
{
    public partial class Form1 : Form
    {
        enum PropertyID
        {

            PropertyTagDateTime = 0x0132,
            PropertyTagExifDTOrig = 0x9003,
            PropertyTagEquipMake = 0x010F,
            PropertyTagEquipModel = 0x0110

        }
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            txtImageInfo.Clear();

            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ofd.Filter = "JPEGS|*.jpg|GIFs|*.gif|PNGs|*.png|TIF|*.tif";
            
            if (ofd.ShowDialog() != DialogResult.Cancel)
            {
                
                pbImage.Load(ofd.FileName);
                Image img = Image.FromFile(ofd.FileName);
                string imageType = "";

                if (ImageFormat.Jpeg.Equals(img.RawFormat))
                {

                    imageType = "JPEG Image";
                    GetPropItems(img, "Date and Time Taken: ", (int)PropertyID.PropertyTagDateTime);
                    GetPropItems(img, "\r\nCamera Maker: ", (int)PropertyID.PropertyTagEquipMake);
                    GetPropItems(img, "\r\nCamera Model: ", (int)PropertyID.PropertyTagEquipModel);

                }
                else if (ImageFormat.Gif.Equals(img.RawFormat))
                {

                    imageType = "GIF Image";

                }
                else if (ImageFormat.Tiff.Equals(img.RawFormat))
                {

                    imageType = "TIFF Image";

                }
                else if (ImageFormat.Png.Equals(img.RawFormat))
                {

                    imageType = "PNG Image";

                }

                string imageWidth = img.Width.ToString();
                string imageHeight = img.Height.ToString();
                string imageResolution = img.HorizontalResolution.ToString() + " dpi";
                string imagePixelDepth = Image.GetPixelFormatSize(img.PixelFormat).ToString();


                txtImageInfo.Text += imageType + "\r\n";
                txtImageInfo.Text += "Width = " + imageWidth + "\r\n";
                txtImageInfo.Text += "Height = " + imageHeight + "\r\n";
                txtImageInfo.Text += "Resolution = " + imageResolution + "\r\n";
                txtImageInfo.Text += "Pixel Depth = " + imagePixelDepth + "\r\n";

                int colorPaletteLength = img.Palette.Entries.Length;
                string[] colorsArray = new string[colorPaletteLength];
                
                if (img.Palette != null && (colorPaletteLength > 0 && colorPaletteLength < 257))
                {
                    for (int i = 0; i < colorPaletteLength; i++)
                    {
                        string htmlColor = ColorTranslator.ToHtml(img.Palette.Entries[i]);
                        colorsArray[i] = htmlColor;
                    }
                    
                    List<string> removeDuplicateColors = new List<string>();

                    foreach (var item in colorsArray)
                    {
                        if (removeDuplicateColors.Contains(item) == false)
                        {
                            removeDuplicateColors.Add(item);
                        }
                    }

                    int hexCount = 0;

                    foreach (var hexVal in removeDuplicateColors)
                    {
                        ColorLabel("color" + hexCount.ToString(), hexVal);
                        hexCount++;
                    }
                }
            }

           
        }

        private void ColorLabel(string labelName, string labelColor)
        {
            var findLabel = Controls.Find(labelName, false).FirstOrDefault();
            
            if (findLabel != null)
            {
                findLabel.BackColor = ColorTranslator.FromHtml(labelColor);
            }
        }
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
        }

        private void txtImageInfo_TextChanged(object sender, EventArgs e)
        {

        }

        private void GetPropItems(Image img, string message, int ID)
        {
            try
            {
                PropertyItem propItem = img.GetPropertyItem(ID);
                if (propItem != null)
                {
                    ASCIIEncoding encod = new ASCIIEncoding();
                    string asciiInfo = encod.GetString(propItem.Value, 0, propItem.Len);
                    txtImageInfo.Text += message + asciiInfo;
                }
            }
            catch (Exception)
            {
                txtImageInfo.Text += message + "Not Available";
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int offsetX = 0;
            int offsetY = 0;
            int labelWidth = 30;
            int labelHeight = 30;
            
            for (int i = 0; i < 256; i++)
            {
                if (offsetX > 700)
                {
                    offsetX = 0;
                    offsetY += labelHeight;
                }

                Controls.Add(new Label
                {
                    Name = "color" + (i).ToString(),
                    Size = new Size(labelWidth, labelHeight),
                    Location = new Point(labelWidth + offsetX, 400 + offsetY),
                    BackColor = SystemColors.ControlDark,
                    Text = (i).ToString()
                });
                offsetX += 30;
            }
        }
    }

}
