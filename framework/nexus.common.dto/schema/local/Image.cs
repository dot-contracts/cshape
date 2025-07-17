using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;

using nexus.common.dal;
using nexus.common.dto;
using nexus.common.core;
using nexus.common.cache;

namespace nexus.common.dal
{

    public class dlImage : IDisposable
    {

        public enum ImageTypes { Primary, Secondary }

        private string     mConnectionString = "";
        private string     mDataError        = "";
        private bool       mChanged;                              public bool   Changed        { get { return mChanged; }                          set { mChanged = value; } }
        private bool       mValidImage = false;                   public bool   ValidImage     { get { return (mImageData != null);} }
        private string     mImageURL = "C:\\nexus\\image.jpg";    public string ImageURL       { get { return mImageURL; } }


        private dlEntity   mEntity;              public dlEntity Entity         { get { return mEntity; }                 set { mEntity = value; } }
                                                 public string   Description    { get { return mEntity.Description; }     set { mEntity.Description = value; } }   
                                                 public bool     NewEntry       { get { return mEntity.NewEntry; }        set { mEntity.NewEntry = value; } }

        private string     mImagePK = "";        public string   ImagePK        { get { return mImagePK; }                set { mImagePK = value; } }
        private string     mEntityId = "";       public string   EntityId       { get { return mEntityId; }               set { mEntityId = value; } }

        private nxProperty mImageType;           public string   ImageType       { get { return mImageType.Value; }       set { mImageType.Value = value;  mImageTypeID        = EnumCache.Instance.getTypeFromDesc   ("Image", mImageType.Value); } }
        private string     mImageTypeID;         public string   ImageTypeID     { get { return mImageTypeID; }           set { mImageTypeID = value;      mImageType.Value    = EnumCache.Instance.getDescFromId     (         mImageTypeID); } }

        private nxProperty mImageState;          public string   ImageState      { get { return mImageState.Value; }      set { mImageState.Value = value; mImageStateID       = EnumCache.Instance.getStateFromDesc  ("Image", mImageState.Value); } }
        private string     mImageStateID;        public string   ImageStateID    { get { return mImageStateID; }          set { mImageStateID = value;     mImageState.Value   = EnumCache.Instance.getDescFromId     (         mImageStateID); } }

        private nxProperty mImageFormat;         public string   ImageFormat      { get { return mImageFormat.Value; }    set { mImageFormat.Value = value; mImageFormatID     = EnumCache.Instance.getFormatFromDesc ("Image", mImageState.Value); } }
        private string     mImageFormatID;       public string   ImageFormatID    { get { return mImageFormatID; }        set { mImageFormatID = value;     mImageFormat.Value = EnumCache.Instance.getDescFromId     (         mImageStateID); } }

        private byte[]      mImageData;          public byte[]   ImageData       { get { return mImageData; }             set { mImageData = value; } }
        //private BitmapImage mBitmapImage;        public BitmapImage BitmapImage  { get { return mBitmapImage; }           set { mBitmapImage = value; } }

        public event         OnEntryStatusChangeEventHandler OnEntryStatusChange;
        public delegate void OnEntryStatusChangeEventHandler(bool Status);



        public dlImage()
        {
            mEntity = new dlEntity("Image");
            if (shell.Instance != null)
                Create(setting.ConnectionString);
        }
        public dlImage(string ConnectionString)
        {
            mEntity = new dlEntity("Image");
            Create(ConnectionString);
        }

        public void Create(string ConnectionString)
        {
            //mBitmapImage = new BitmapImage();

            mConnectionString = ConnectionString;
            mEntity.Create(mConnectionString);

            mImageType     = new nxProperty("Image", "ImageType");
            mImageState    = new nxProperty("Image", "ImageState");
            mImageFormat   = new nxProperty("Image", "ImageFormat");

            Reset();
        }

        public bool Open(int    ImagePK) { return Open(ImagePK.ToString()); }
        public bool Open(string ImagePK)
        {
            Reset();

            SQLServer DB = new SQLServer(mConnectionString);
            DataTable DT = DB.GetDataTable("loc.lst_Image", "I~S~ImagePK~" + ImagePK);
            if (DT.Rows.Count > 0)
                LoadFromRow(DT.Rows[0]);

            mDataError = DB.DataError;
            DB.Dispose();
            DB = null;

            mChanged = false;

            return String.IsNullOrEmpty(mDataError);
        }

        private void LoadFromRow(DataRow Row)
        {
            if (Row != null)
            {
                mImagePK = Row["ImagePK"].ToString();
                mEntity.Open(Row["ImagePK"].ToString());
                mEntityId = Row["EntityId"].ToString();

                ImageTypeID = Row["ImageTypeID"].ToString();
                ImageStateID       = Row["ImageStateID"].ToString();
                ImageFormatID      = Row["ImageFormatID"].ToString();
            }

            mEntity.Open(Row["ImagePK"].ToString());


            //try
            //{
            //    mImageData = (byte[])Row["ImageData"];

            //    mBitmapImage = new BitmapImage();

            //    using (MemoryStream stream = new MemoryStream(mImageData))
            //    {
            //        stream.Seek(0, SeekOrigin.Begin);
            //        mBitmapImage.BeginInit();
            //        mBitmapImage.StreamSource = stream;
            //        mBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //        mBitmapImage.EndInit();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    //throw;
            //}

        }

        //public bool UpdateImage(byte[] FaceImage, string ImageId)
        //{
        //    bool ret = true;

        //    using (SqlConnection CN = new SqlConnection("")) //dwr setting.ConnectionString))
        //    {
        //        using (System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand())
        //        {
        //            try
        //            {
        //                CN.Open();
        //                cmd.Connection = CN;
        //                cmd.CommandText = "Update [loc].[Image] set ImageData = isnull(@ImageData, ImageData) where ImagePK = @ImagePK";
        //                cmd.Parameters.AddWithValue("@ImagePK", ImageId);

        //                IDataParameter par = cmd.CreateParameter();
        //                par.ParameterName = "ImageData";
        //                par.DbType = DbType.Binary;
        //                par.Value = FaceImage;

        //                cmd.Parameters.Add(par);
        //                cmd.ExecuteNonQuery();

        //            }
        //            catch (Exception ex) { ret = false; }
        //        }
        //    }


        //    return ret;

        //}



        public bool Update()
        {
            string Params = "";
            if (!mEntity.NewEntry) Params = "I~S~ImagePK~" + mImagePK + ";";
            Params += "I~S~EntityId~"   + mEntityId + ";";
            Params += "I~S~ImageType~"  + mImageTypeID + ";";
            Params += "I~S~ImageState~" + mImageStateID + ";";

            using (SQLServer DB = new SQLServer(setting.ConnectionString))
            {
                mEntity.EntityPK = DB.ExecLookup("loc.mod_Image", Params).ToString();
                mDataError = DB.DataError;

                DB.UpdateImage(mImageData, mEntity.EntityPK);
            }

            if (String.IsNullOrEmpty(mDataError))
            {
                if (!mEntity.NewEntry)
                {
                    mImageType.Update();
                    mImageState.Update();
                    mImageFormat.Update();
                }
                Open(mEntity.EntityPK);
            }
            else { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Image", "UpdateError"), mDataError); }

            return String.IsNullOrEmpty(mDataError);
        }

       public string getParams()
        {
            string Params = " ";


            // Params += "I~S~Captured~" + mCaptured + ";";
            //Params += "I~S~CropX~"        + mCropX.ToString() + ";";
            //Params += "I~S~CropY~"        + mCropX.ToString() + ";";
            //Params += "I~S~OffsetX~"      + mCropX.ToString() + ";";
            //Params += "I~S~OffsetY~"      + mCropX.ToString() + ";";

            // if (!String.IsNullOrEmpty(mCompanyNo.Value))              Params += "I~S~ImageFormat~"  + mImageFormatID + ";";

            Params += mEntity.getParams();
            return Params;
        }




        public void Reset()
        {
            // event is not right
            try { if (File.Exists(mImageURL)) File.Delete(mImageURL); }
            catch (Exception ex) { EventCache.Instance.InsertEvent("Warning", ActionCache.Instance.strId("Action", "Image", "Reset"),  ex.Message);  }

            mImageData    = null;
            mChanged      = false;
            //mBitmapImage  = new BitmapImage();

            ImageTypeID   = EnumCache.Instance.getTypeId  ("Image",   "Primary");
            ImageStateID  = EnumCache.Instance.getStateId ("Entity",  "Active");

        }



        //public BitmapImage GetBitmap(ImageTypes ImageType, string EntityID)
        //{
        //    mBitmapImage = new BitmapImage();

        //    if (!String.IsNullOrWhiteSpace(EntityID))
        //    {

        //        try
        //        {
        //            if (File.Exists(mImageURL)) File.Delete(mImageURL);
        //        }
        //        catch (Exception ex) { }

        //        SQLServer DB = new SQLServer(setting.ConnectionString);
        //        DataTable DT = DB.GetDataTable("Select ImageData from loc.Image where ImageState=cmn.getStatePK('Image','Active') and EntityID=" + EntityID);

        //        if (DT.Rows.Count > 0)
        //        {
        //            FileStream FS1 = new FileStream(mImageURL, FileMode.Create);
        //            byte[] blob = (byte[])DT.Rows[0][0];
        //            FS1.Write(blob, 0, blob.Length);
        //            FS1.Close();
        //            FS1 = null;

        //            if (blob.Length > 0)
        //            {
        //                mBitmapImage.BeginInit();
        //                mBitmapImage.StreamSource = new MemoryStream(System.IO.File.ReadAllBytes(mImageURL));
        //                mBitmapImage.EndInit();
        //                mBitmapImage.Freeze();
        //            }
        //        }

        //        DB.Dispose();
        //        DB = null;
        //    }

        //    return mBitmapImage;
        //}


        //public BitmapImage EntityImage
        //{
        //    get { return mBitmapImage; }
        //    set
        //    {
        //        MemoryStream memStream = new MemoryStream();
        //        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
        //        mImageData = memStream.GetBuffer();
        //        try
        //        {
        //            encoder.Frames.Add(BitmapFrame.Create(value));
        //            encoder.Save(memStream);
        //            mImageData = memStream.GetBuffer();
        //        }
        //        catch (Exception ex)
        //        {
        //            mImageData = null;
        //        }
        //        mChanged = true;
        //    }
        //}

        #region IDisposable Implementation
        // To detect redundant calls
        private bool disposedValue = false;
        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: free managed resources when explicitly called
                }

                // TODO: free shared unmanaged resources
            }
            this.disposedValue = true;
        }
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
