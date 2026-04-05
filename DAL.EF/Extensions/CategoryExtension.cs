using System;
using System.Drawing;
using Utilities;

namespace DAL.EF
{
    public partial class Category
    {
        public Image PictureImage
        {
            get => Utils.ByteArrayToImage(Picture);
        }

        public string RowVersionStr
        {
            get
            {
                if (RowVersion == null || RowVersion.Length < 8)
                    return string.Empty;

                return BitConverter.ToInt64(RowVersion, 0).ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    RowVersion = null;
                    return;
                }

                RowVersion = BitConverter.GetBytes(long.Parse(value));
            }
        }
    }
}
