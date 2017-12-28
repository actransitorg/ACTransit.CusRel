using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace ACTransit.Framework.Imaging
{
    public static class ImageHelper
    {
        public const string DefaultThumbnailSuffix = "_thumb";

        private static string thumbnailSuffix;
        public static string ThumbnailSuffix 
        {
            get
            {
                return thumbnailSuffix ?? (thumbnailSuffix = DefaultThumbnailSuffix);
            }
        }

        public static bool IsValidImage(string base64String)
        {
            bool isValidImage;

            try
            {
                // Convert base 64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);

                // Convert byte[] to Image
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    var format = Image.FromStream(ms, true).RawFormat;
                    var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);
                    isValidImage = codec != null;
                }

            }
            catch
            {
                isValidImage = false;
            }

            return isValidImage;
        }


        public static void MakeThumbnail(string sourceFile, string targetFile)
        {
            var image = Image.FromFile(sourceFile);
            using (var thumb = image.GetThumbnailImage(32, 32, () => false, IntPtr.Zero))
                thumb.Save(targetFile);
            
            using (var thumb = image.GetThumbnailImage(160, 160, () => false, IntPtr.Zero))
                thumb.Save(targetFile.Replace("thumb", "thumb_big"));
                    
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return new Bitmap(image);
            }
        }

        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to base 64 string
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        #region Image Metadata

        public static ImageMetaData GetMetadata(this Image image)
        {
            var result = new ImageMetaData();
            var items = image.PropertyItems;
            foreach (var item in items)
            {
                if (Enum.IsDefined(typeof(ImagePropertyIdEnum), item.Id))
                {
                    object value = null;
                    object[] calculateValue = null;
                    switch (item.Type)
                    {
                        case (int) ImagePropertyTypeEnum.ByteArray:
                            value = item.Value;
                            break;
                        case (int) ImagePropertyTypeEnum.String:
                            value = System.Text.Encoding.ASCII.GetString(item.Value);
                            if (((string) value).EndsWith("\0"))
                                value = ((string) value).Substring(0, ((string) value).Length - 1);
                            break;
                        case (int) ImagePropertyTypeEnum.ShortArray:
                            try
                            {
                                var loopCount = (int) Math.Ceiling((double) item.Len / 2);
                                var res = new List<short>();
                                for (var loopNumber = 0; loopNumber < loopCount; loopNumber++)
                                    res.Add(BitConverter.ToInt16(item.Value, (loopNumber * 2)));
                                value = res;
                            }
                            catch // (Exception ex)
                            {
                                //Console.Write(Enum.GetName(typeof(ImagePropertyIdEnum), item.Id) + ", type:" +
                                //              (ImagePropertyTypeEnum) item.Type + ", len:" + item.Len + ", Error:" +
                                //              ex.Message);
                            }
                            break;
                        case (int) ImagePropertyTypeEnum.ObjectArray:
                            value = item.Value;
                            break;
                        case (int) ImagePropertyTypeEnum.IntegerArray:
                            try
                            {
                                var loopCount = (int) Math.Ceiling((double) item.Len / 4);
                                var res = new List<int>();
                                for (var loopNumber = 0; loopNumber < loopCount; loopNumber++)
                                    res.Add(BitConverter.ToInt32(item.Value, (loopNumber * 4)));
                                value = res;
                            }
                            catch //(Exception ex)
                            {
                                //Console.Write(Enum.GetName(typeof(ImagePropertyIdEnum), item.Id) + ", type:" +
                                //              (ImagePropertyTypeEnum) item.Type + ", len:" + item.Len + ", Error:" +
                                //              ex.Message);
                            }
                            break;
                        case (int) ImagePropertyTypeEnum.PairOfLongArray:
                        case (int) ImagePropertyTypeEnum.PairOfLongArray1:
                        {
                            value = "";
                            var loopCount = (int) Math.Ceiling((double) item.Len / 8);
                            calculateValue = new object[loopCount];
                            //BitConverter.ToUInt32(item.Value)
                            for (var loopNumber = 0; loopNumber < loopCount; loopNumber++)
                            {
                                var numeratorStr = BitConverter.ToUInt32(item.Value, (loopNumber * 8));
                                var denominatorStr = BitConverter.ToUInt32(item.Value, (loopNumber * 8) + 4);
                                value += numeratorStr + "/" + denominatorStr + ";";
                                calculateValue[loopNumber] = (double) numeratorStr / denominatorStr;
                            }
                            break;
                        }
                    }
                    result.Items.Add(new ImageMetaDataItem
                    {
                        Name = (ImagePropertyIdEnum) item.Id,
                        NameStr = Enum.GetName(typeof(ImagePropertyIdEnum), item.Id),
                        Type = (ImagePropertyTypeEnum) item.Type,
                        Value = value,
                        CalculatedValues = calculateValue,
                        OriginalValue = item.Value
                    });
                }
            }
            return result;
        }

    }

    public class ImageMetaData
    {
        public ImageMetaData() { Items = new List<ImageMetaDataItem>(); }

        public double? Direction
        {
            get
            {
                // at this point, we just calculate Direction for True north. Magnetic north, m, to be added later id needed.
                var dirItem = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagGpsImgDirRef);
                string dirRef = dirItem != null && dirItem.Value != null ? dirItem.Value as string : null;
                if (!string.IsNullOrWhiteSpace(dirRef) && string.Equals(dirRef, "t", StringComparison.OrdinalIgnoreCase))
                {
                    var item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagGpsImgDir);
                    if (item != null && item.CalculatedValues != null && item.CalculatedValues.Length == 1)
                        return (double)item.CalculatedValues[0];
                }

                return null;
            }
        }

        public string SoftwareUsed
        {
            get
            {
                var item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagSoftwareUsed);
                return (item != null ? item.ToString() : null);
            }
        }

        public string DateTaken
        {
            get
            {
                var item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagDateTime);
                return (item != null ? item.ToString() : null);
            }
        }

        public double? Brightness
        {
            get
            {
                var item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagExifBrightness);
                if ((item != null ? item.CalculatedValues : null) != null && item.CalculatedValues.Length == 1)
                    return (double)item.CalculatedValues[0];
                return null;
            }
        }

        public string Model
        {
            get
            {
                var item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagEquipModel);
                return (item != null ? item.ToString() : null);
            }
        }

        public string Maker
        {
            get
            {
                var item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagEquipMake);
                return (item != null ? item.ToString() : null);
            }
        }
        public double? Latitude
        {
            get
            {
                var coef = 1;
                var item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagGpsLatitudeRef);
                if ((item != null ? item.Value : null) != null && string.Equals((string)item.Value, "s", StringComparison.InvariantCultureIgnoreCase))
                    coef = -1;
                item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagGpsLatitude);
                if (item != null && item.CalculatedValues != null && item.CalculatedValues.Length == 3)
                {
                    var res = (double)item.CalculatedValues[0] + (((double)item.CalculatedValues[1]) / 60d) +
                           (((double)item.CalculatedValues[2]) / 3600d);
                    return coef * res;

                }
                return null;
            }
        }

        public double? Longitude
        {
            get
            {
                var coef = 1;
                var item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagGpsLongitudeRef);
                if ((item != null ? item.Value : null) != null && string.Equals((string)item.Value, "w", StringComparison.InvariantCultureIgnoreCase))
                    coef = -1;
                item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagGpsLongitude);
                if ((item != null ? item.CalculatedValues : null) != null && item.CalculatedValues.Length == 3)
                {
                    var res = (double)item.CalculatedValues[0] + (((double)item.CalculatedValues[1]) / 60d) +
                           (((double)item.CalculatedValues[2]) / 3600d);
                    return coef * res;

                }
                return null;
            }
        }

        public double? Altitude
        {
            get
            {
                var item = Items.FirstOrDefault(m => m.Name == ImagePropertyIdEnum.PropertyTagGpsAltitude);
                if ((item != null ? item.CalculatedValues : null) != null && item.CalculatedValues.Length == 1)
                    return (double)item.CalculatedValues[0];
                return null;
            }
        }

        public List<ImageMetaDataItem> Items { get; set; }
    }

    public class ImageMetaDataItem
    {
        public ImagePropertyIdEnum Name { get; set; }
        public string NameStr { get; set; }
        public object Value { get; set; }
        public object[] CalculatedValues { get; set; }
        public ImagePropertyTypeEnum Type { get; set; }
        public byte[] OriginalValue { get; set; }
    }

    public enum ImagePropertyIdEnum
    {
        PropertyTagGpsVer = 0x0000,
        PropertyTagGpsLatitudeRef = 0x0001,
        PropertyTagGpsLatitude = 0x0002,
        PropertyTagGpsLongitudeRef = 0x0003,
        PropertyTagGpsLongitude = 0x0004,
        PropertyTagGpsAltitudeRef = 0x0005,
        PropertyTagGpsAltitude = 0x0006,
        PropertyTagGpsGpsTime = 0x0007,
        PropertyTagGpsGpsSatellites = 0x0008,
        PropertyTagGpsGpsStatus = 0x0009,
        PropertyTagGpsGpsMeasureMode = 0x000A,
        PropertyTagGpsGpsDop = 0x000B,
        PropertyTagGpsSpeedRef = 0x000C,
        PropertyTagGpsSpeed = 0x000D,
        PropertyTagGpsTrackRef = 0x000E,
        PropertyTagGpsTrack = 0x000F,
        PropertyTagGpsImgDirRef = 0x0010,
        PropertyTagGpsImgDir = 0x0011,
        PropertyTagGpsMapDatum = 0x0012,
        PropertyTagGpsDestLatRef = 0x0013,
        PropertyTagGpsDestLat = 0x0014,
        PropertyTagGpsDestLongRef = 0x0015,
        PropertyTagGpsDestLong = 0x0016,
        PropertyTagGpsDestBearRef = 0x0017,
        PropertyTagGpsDestBear = 0x0018,
        PropertyTagGpsDestDistRef = 0x0019,
        PropertyTagGpsDestDist = 0x001A,
        PropertyTagNewSubfileType = 0x00FE,
        PropertyTagSubfileType = 0x00FF,
        PropertyTagImageWidth = 0x0100,
        PropertyTagImageHeight = 0x0101,
        PropertyTagBitsPerSample = 0x0102,
        PropertyTagCompression = 0x0103,
        PropertyTagPhotometricInterp = 0x0106,
        PropertyTagThreshHolding = 0x0107,
        PropertyTagCellWidth = 0x0108,
        PropertyTagCellHeight = 0x0109,
        PropertyTagFillOrder = 0x010A,
        PropertyTagDocumentName = 0x010D,
        PropertyTagImageDescription = 0x010E,
        PropertyTagEquipMake = 0x010F,
        PropertyTagEquipModel = 0x0110,
        PropertyTagStripOffsets = 0x0111,
        PropertyTagOrientation = 0x0112,
        PropertyTagSamplesPerPixel = 0x0115,
        PropertyTagRowsPerStrip = 0x0116,
        PropertyTagStripBytesCount = 0x0117,
        PropertyTagMinSampleValue = 0x0118,
        PropertyTagMaxSampleValue = 0x0119,
        PropertyTagXResolution = 0x011A,
        PropertyTagYResolution = 0x011B,
        PropertyTagPlanarConfig = 0x011C,
        PropertyTagPageName = 0x011D,
        PropertyTagXPosition = 0x011E,
        PropertyTagYPosition = 0x011F,
        PropertyTagFreeOffset = 0x0120,
        PropertyTagFreeByteCounts = 0x0121,
        PropertyTagGrayResponseUnit = 0x0122,
        PropertyTagGrayResponseCurve = 0x0123,
        PropertyTagT4Option = 0x0124,
        PropertyTagT6Option = 0x0125,
        PropertyTagResolutionUnit = 0x0128,
        PropertyTagPageNumber = 0x0129,
        PropertyTagTransferFunction = 0x012D,
        PropertyTagSoftwareUsed = 0x0131,
        PropertyTagDateTime = 0x0132,
        PropertyTagArtist = 0x013B,
        PropertyTagHostComputer = 0x013C,
        PropertyTagPredictor = 0x013D,
        PropertyTagWhitePoint = 0x013E,
        PropertyTagPrimaryChromaticities = 0x013F,
        PropertyTagColorMap = 0x0140,
        PropertyTagHalftoneHints = 0x0141,
        PropertyTagTileWidth = 0x0142,
        PropertyTagTileLength = 0x0143,
        PropertyTagTileOffset = 0x0144,
        PropertyTagTileByteCounts = 0x0145,
        PropertyTagInkSet = 0x014C,
        PropertyTagInkNames = 0x014D,
        PropertyTagNumberOfInks = 0x014E,
        PropertyTagDotRange = 0x0150,
        PropertyTagTargetPrinter = 0x0151,
        PropertyTagExtraSamples = 0x0152,
        PropertyTagSampleFormat = 0x0153,
        PropertyTagSMinSampleValue = 0x0154,
        PropertyTagSMaxSampleValue = 0x0155,
        PropertyTagTransferRange = 0x0156,
        PropertyTagJPEGProc = 0x0200,
        PropertyTagJPEGInterFormat = 0x0201,
        PropertyTagJPEGInterLength = 0x0202,
        PropertyTagJPEGRestartInterval = 0x0203,
        PropertyTagJPEGLosslessPredictors = 0x0205,
        PropertyTagJPEGPointTransforms = 0x0206,
        PropertyTagJPEGQTables = 0x0207,
        PropertyTagJPEGDCTables = 0x0208,
        PropertyTagJPEGACTables = 0x0209,
        PropertyTagYCbCrCoefficients = 0x0211,
        PropertyTagYCbCrSubsampling = 0x0212,
        PropertyTagYCbCrPositioning = 0x0213,
        PropertyTagREFBlackWhite = 0x0214,
        PropertyTagGamma = 0x0301,
        PropertyTagICCProfileDescriptor = 0x0302,
        PropertyTagSRGBRenderingIntent = 0x0303,
        PropertyTagImageTitle = 0x0320,
        PropertyTagResolutionXUnit = 0x5001,
        PropertyTagResolutionYUnit = 0x5002,
        PropertyTagResolutionXLengthUnit = 0x5003,
        PropertyTagResolutionYLengthUnit = 0x5004,
        PropertyTagPrintFlags = 0x5005,
        PropertyTagPrintFlagsVersion = 0x5006,
        PropertyTagPrintFlagsCrop = 0x5007,
        PropertyTagPrintFlagsBleedWidth = 0x5008,
        PropertyTagPrintFlagsBleedWidthScale = 0x5009,
        PropertyTagHalftoneLPI = 0x500A,
        PropertyTagHalftoneLPIUnit = 0x500B,
        PropertyTagHalftoneDegree = 0x500C,
        PropertyTagHalftoneShape = 0x500D,
        PropertyTagHalftoneMisc = 0x500E,
        PropertyTagHalftoneScreen = 0x500F,
        PropertyTagJPEGQuality = 0x5010,
        PropertyTagGridSize = 0x5011,
        PropertyTagThumbnailFormat = 0x5012,
        PropertyTagThumbnailWidth = 0x5013,
        PropertyTagThumbnailHeight = 0x5014,
        PropertyTagThumbnailColorDepth = 0x5015,
        PropertyTagThumbnailPlanes = 0x5016,
        PropertyTagThumbnailRawBytes = 0x5017,
        PropertyTagThumbnailSize = 0x5018,
        PropertyTagThumbnailCompressedSize = 0x5019,
        PropertyTagColorTransferFunction = 0x501A,
        PropertyTagThumbnailData = 0x501B,
        PropertyTagThumbnailImageWidth = 0x5020,
        PropertyTagThumbnailImageHeight = 0x5021,
        PropertyTagThumbnailBitsPerSample = 0x5022,
        PropertyTagThumbnailCompression = 0x5023,
        PropertyTagThumbnailPhotometricInterp = 0x5024,
        PropertyTagThumbnailImageDescription = 0x5025,
        PropertyTagThumbnailEquipMake = 0x5026,
        PropertyTagThumbnailEquipModel = 0x5027,
        PropertyTagThumbnailStripOffsets = 0x5028,
        PropertyTagThumbnailOrientation = 0x5029,
        PropertyTagThumbnailSamplesPerPixel = 0x502A,
        PropertyTagThumbnailRowsPerStrip = 0x502B,
        PropertyTagThumbnailStripBytesCount = 0x502C,
        PropertyTagThumbnailResolutionX = 0x502D,
        PropertyTagThumbnailResolutionY = 0x502E,
        PropertyTagThumbnailPlanarConfig = 0x502F,
        PropertyTagThumbnailResolutionUnit = 0x5030,
        PropertyTagThumbnailTransferFunction = 0x5031,
        PropertyTagThumbnailSoftwareUsed = 0x5032,
        PropertyTagThumbnailDateTime = 0x5033,
        PropertyTagThumbnailArtist = 0x5034,
        PropertyTagThumbnailWhitePoint = 0x5035,
        PropertyTagThumbnailPrimaryChromaticities = 0x5036,
        PropertyTagThumbnailYCbCrCoefficients = 0x5037,
        PropertyTagThumbnailYCbCrSubsampling = 0x5038,
        PropertyTagThumbnailYCbCrPositioning = 0x5039,
        PropertyTagThumbnailRefBlackWhite = 0x503A,
        PropertyTagThumbnailCopyRight = 0x503B,
        PropertyTagLuminanceTable = 0x5090,
        PropertyTagChrominanceTable = 0x5091,
        PropertyTagFrameDelay = 0x5100,
        PropertyTagLoopCount = 0x5101,
        PropertyTagGlobalPalette = 0x5102,
        PropertyTagIndexBackground = 0x5103,
        PropertyTagIndexTransparent = 0x5104,
        PropertyTagPixelUnit = 0x5110,
        PropertyTagPixelPerUnitX = 0x5111,
        PropertyTagPixelPerUnitY = 0x5112,
        PropertyTagPaletteHistogram = 0x5113,
        PropertyTagCopyright = 0x8298,
        PropertyTagExifExposureTime = 0x829A,
        PropertyTagExifFNumber = 0x829D,
        PropertyTagExifIFD = 0x8769,
        PropertyTagICCProfile = 0x8773,
        PropertyTagExifExposureProg = 0x8822,
        PropertyTagExifSpectralSense = 0x8824,
        PropertyTagGpsIFD = 0x8825,
        PropertyTagExifISOSpeed = 0x8827,
        PropertyTagExifOECF = 0x8828,
        PropertyTagExifVer = 0x9000,
        PropertyTagExifDTOrig = 0x9003,
        PropertyTagExifDTDigitized = 0x9004,
        PropertyTagExifCompConfig = 0x9101,
        PropertyTagExifCompBPP = 0x9102,
        PropertyTagExifShutterSpeed = 0x9201,
        PropertyTagExifAperture = 0x9202,
        PropertyTagExifBrightness = 0x9203,
        PropertyTagExifExposureBias = 0x9204,
        PropertyTagExifMaxAperture = 0x9205,
        PropertyTagExifSubjectDist = 0x9206,
        PropertyTagExifMeteringMode = 0x9207,
        PropertyTagExifLightSource = 0x9208,
        PropertyTagExifFlash = 0x9209,
        PropertyTagExifFocalLength = 0x920A,
        PropertyTagExifMakerNote = 0x927C,
        PropertyTagExifUserComment = 0x9286,
        PropertyTagExifDTSubsec = 0x9290,
        PropertyTagExifDTOrigSS = 0x9291,
        PropertyTagExifDTDigSS = 0x9292,
        PropertyTagExifFPXVer = 0xA000,
        PropertyTagExifColorSpace = 0xA001,
        PropertyTagExifPixXDim = 0xA002,
        PropertyTagExifPixYDim = 0xA003,
        PropertyTagExifRelatedWav = 0xA004,
        PropertyTagExifInterop = 0xA005,
        PropertyTagExifFlashEnergy = 0xA20B,
        PropertyTagExifSpatialFR = 0xA20C,
        PropertyTagExifFocalXRes = 0xA20E,
        PropertyTagExifFocalYRes = 0xA20F,
        PropertyTagExifFocalResUnit = 0xA210,
        PropertyTagExifSubjectLoc = 0xA214,
        PropertyTagExifExposureIndex = 0xA215,
        PropertyTagExifSensingMethod = 0xA217,
        PropertyTagExifFileSource = 0xA300,
        PropertyTagExifSceneType = 0xA301,
        PropertyTagExifCfaPattern = 0xA302
    }

    public enum ImagePropertyTypeEnum
    {
        //Specifies that Value is an array of bytes.
        ByteArray = 1,
        //Specifies that Value is a null-terminated ASCII string. If you set the type data member to ASCII type, you should set the Len property to the length of the string including the null terminator. For example, the string "Hello" would have a length of 6.
        String = 2,
        //Specifies that Value is an array of unsigned short (16-bit) integers.
        ShortArray = 3,
        //Specifies that Value is an array of unsigned long (32-bit) integers.
        LongArray = 4,
        //Specifies that Value data member is an array of pairs of unsigned long integers. Each pair represents a fraction; the first integer is the numerator and the second integer is the denominator.
        PairOfLongArray = 5,
        //Specifies that Value is an array of bytes that can hold values of any data type.
        ObjectArray = 6,
        //Specifies that Value is an array of signed long (32-bit) integers.
        IntegerArray = 7,
        //Specifies that Value is an array of pairs of signed long integers. Each pair represents a fraction; the first integer is the numerator and the second integer is the denominator.
        PairOfLongArray1 = 10
    }

    #endregion
}
